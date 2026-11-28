using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MainMenuSceneSetup
{
    public const string ScenePath = "Assets/Scenes/MainMenu.unity";
    private const string ArtRoot = "Assets/Resources/BrotatoMenu";
    private const string InputPath = "Assets/Settings/InputSystem_Actions.inputactions";

    [MenuItem("Tools/Brotato/Mở hoặc tạo MainMenu")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
        {
            EditorSceneManager.OpenScene(ScenePath);
            return;
        }
        ConfigureTextures();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = new Color(.045f, .03f, .08f);
        camera.transform.position = new Vector3(0, 0, -10f);
        camera.cullingMask = 0;

        var canvasObject = new GameObject("MainCanvas", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = .5f;

        var eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        var input = eventObject.GetComponent<InputSystemUIInputModule>();
        SetInput(input);
        var controller = new GameObject("MainMenuController").AddComponent<MainMenuController>();
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("canvas").objectReferenceValue = canvas;
        serialized.FindProperty("eventSystem").objectReferenceValue = eventObject.GetComponent<EventSystem>();
        serialized.ApplyModifiedPropertiesWithoutUndo();

        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new System.InvalidOperationException("Không lưu được " + ScenePath);
        Debug.Log("BROTATO MAIN MENU SCENE SAVED: " + ScenePath);
    }

    private static void ConfigureTextures()
    {
        foreach (string id in AssetDatabase.FindAssets("t:Texture2D", new[] { ArtRoot }))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            if (AssetImporter.GetAtPath(path) is not TextureImporter importer) continue;
            importer.alphaIsTransparency = path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase);
            importer.filterMode = path.Contains("/Portraits/") ? FilterMode.Point : FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static void SetInput(InputSystemUIInputModule input)
    {
        var references = AssetDatabase.LoadAllAssetsAtPath(InputPath).OfType<InputActionReference>()
            .Where(reference => reference.action?.actionMap?.name == "UI")
            .GroupBy(reference => reference.action.name)
            .ToDictionary(group => group.Key, group => group.First());
        if (new[] { "Navigate", "Submit", "Cancel", "Point", "Click", "ScrollWheel" }
            .Any(name => !references.ContainsKey(name)))
        {
            input.AssignDefaultActions();
            return;
        }
        input.actionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        input.move = references["Navigate"];
        input.submit = references["Submit"];
        input.cancel = references["Cancel"];
        input.point = references["Point"];
        input.leftClick = references["Click"];
        input.scrollWheel = references["ScrollWheel"];
    }
}
