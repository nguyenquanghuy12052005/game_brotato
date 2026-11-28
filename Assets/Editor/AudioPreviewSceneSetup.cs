using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class AudioPreviewSceneSetup
{
    public const string PathToScene = "Assets/Scenes/AudioPreviewScene.unity";
    private const string MeleeScene = "Assets/Scenes/HexcoreSkillPreview.unity";
    private const string GunScene = "Assets/Scenes/GunSkillTest.unity";
    private const string CatalogPath = "Assets/Resources/AudioPreview/Catalog.json";
    private const string Frames = "Assets/Prefabs/Player/CharacterArt/";
    private static readonly string[] BossArt =
    {
        "Skeleton/SkillAssets/Skeleton_DashTrail", "Skeleton/SkillAssets/Skeleton_SpinSlash",
        "Skeleton/SkillAssets/Skeleton_RoarShockwave", "LavaGolem/SkillAssets/LavaGolem_ConeFire",
        "LavaGolem/SkillAssets/LavaGolem_HealAura", "LavaGolem/SkillAssets/LavaGolem_Fireball",
        "Yeti/SkillAssets/Yeti_Snowball", "Yeti/SkillAssets/Yeti_IcePrison", "Yeti/SkillAssets/Yeti_Snowflake"
    };

    [MenuItem("Tools/Brotato/Audio Preview/Xây lại scene và catalog")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string script = File.ReadAllText("demos/brotato-audio/catalog.js").Trim();
        const string prefix = "window.audioCatalog = ";
        if (!script.StartsWith(prefix) || !script.EndsWith(";")) throw new InvalidDataException("Catalog web không hợp lệ.");
        var catalog = JsonUtility.FromJson<AudioPreviewCatalog>("{\"entries\":" + script[prefix.Length..^1] + "}");
        if (catalog?.entries == null || catalog.entries.Length != 146) throw new InvalidDataException("Catalog phải có đủ 146 mục.");
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(CatalogPath));
        File.WriteAllText(CatalogPath, JsonUtility.ToJson(catalog));
        AssetDatabase.ImportAsset(CatalogPath, ImportAssetOptions.ForceSynchronousImport);

        Scene preview = EditorSceneManager.OpenScene(MeleeScene, OpenSceneMode.Single);
        MeleeController melee = FindInScene<MeleeController>(preview);
        var meleeData = new SerializedObject(melee);
        var camera = (Camera)meleeData.FindProperty("previewCamera").objectReferenceValue;
        var dummy = (SpriteRenderer)meleeData.FindProperty("dummy").objectReferenceValue;
        var enemy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Normal/Brown/Enemy_Normal_Brown.prefab");
        Sprite enemySprite = enemy.GetComponentInChildren<SpriteRenderer>().sprite;
        if (enemySprite != null) dummy.sprite = enemySprite;
        dummy.gameObject.name = "Bia · lính Nâu";

        var display = new GameObject("Studio Display Camera").AddComponent<Camera>();
        display.clearFlags=CameraClearFlags.SolidColor;
        display.backgroundColor=new Color(.047f,.078f,.10f);
        display.cullingMask=0;
        display.depth=-10;

        GameObject root = new("Audio Preview Studio");
        var stage = root.AddComponent<AudioPreviewCharacterDriver>();
        var audio = root.AddComponent<AudioPreviewAudioDriver>();
        var controller = root.AddComponent<AudioPreviewController>();
        var mage = root.AddComponent<MageEffectLibrary>();
        var stageData = new SerializedObject(stage);
        Set(stageData,"cameraView",camera);
        Set(stageData,"melee",melee);
        Set(stageData,"dummySprite",enemySprite);
        Set(stageData,"idle",AssetDatabase.LoadAssetAtPath<AnimationClip>(Frames+"Clips/Idle_Right.anim"));
        Set(stageData,"run",AssetDatabase.LoadAssetAtPath<AnimationClip>(Frames+"Clips/Run_Right.anim"));
        Set(stageData,"death",AssetDatabase.LoadAssetAtPath<AnimationClip>(Frames+"Clips/Death_Right.anim"));
        Set(stageData,"playerSprite",AssetDatabase.LoadAssetAtPath<Sprite>(Frames+"Frames/Idle/Right/Idle_Right_00.png"));
        var magePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Mage.prefab");
        var mageSource = magePrefab.GetComponent<MageEffectLibrary>();
        var mageData = new SerializedObject(mage);
        ArrayFrom(new SerializedObject(mageSource).FindProperty("atlases"),mageData.FindProperty("atlases"));
        mageData.ApplyModifiedPropertiesWithoutUndo();
        Set(stageData,"mageEffects",mage);
        Set(stageData,"mageSprite",magePrefab.transform.Find("Visual").GetComponent<SpriteRenderer>().sprite);
        Set(stageData,"staff",magePrefab.transform.Find("Staff").GetComponent<SpriteRenderer>().sprite);
        SerializedProperty bossArt=stageData.FindProperty("bossEffects");
        bossArt.arraySize=BossArt.Length;
        for(int i=0;i<BossArt.Length;i++)
            bossArt.GetArrayElementAtIndex(i).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Enemy/Boss/"+BossArt[i]+".png");

        var gunScene = EditorSceneManager.OpenScene(GunScene, OpenSceneMode.Additive);
        var guns = new SerializedObject(FindInScene<GunSkillTestArena>(gunScene));
        ArrayFrom(guns.FindProperty("updatedSkillSheets"),stageData.FindProperty("gunSheets"));
        ArrayFrom(guns.FindProperty("pixelSkillSheets"),stageData.FindProperty("gunPixelSheets"));
        Set(stageData,"gunMaterial",guns.FindProperty("skillNoGlowMaterial").objectReferenceValue);
        Set(stageData,"pistol",guns.FindProperty("gunSprite").objectReferenceValue);
        Set(stageData,"bullet",guns.FindProperty("bulletSprite").objectReferenceValue);
        stageData.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.CloseScene(gunScene,true);

        var controls = new SerializedObject(controller);
        Set(controls,"sound",audio);
        Set(controls,"stage",stage);
        controls.ApplyModifiedPropertiesWithoutUndo();

        if (!EditorSceneManager.SaveScene(preview,PathToScene,true))
            throw new IOException("Không lưu được Audio Preview Scene.");
        EditorSceneManager.OpenScene(PathToScene,OpenSceneMode.Single);
        AssetDatabase.SaveAssets();
        Debug.Log("AUDIO STUDIO BUILT · 146 ENTRIES");
    }

    private static T FindInScene<T>(Scene scene) where T : Component =>
        scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<T>(true)).First();

    private static void Set(SerializedObject obj,string name,Object reference) =>
        obj.FindProperty(name).objectReferenceValue = reference;

    private static void ArrayFrom(SerializedProperty source,SerializedProperty dest)
    {
        dest.arraySize=source.arraySize;
        for (int i=0;i<source.arraySize;i++)
            dest.GetArrayElementAtIndex(i).objectReferenceValue=source.GetArrayElementAtIndex(i).objectReferenceValue;
    }
}
