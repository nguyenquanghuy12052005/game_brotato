using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class MeleeUltimateAssetBuilder
{
    private const string Root = "Assets/Prefabs/Player/MeleeUltimates";

    [MenuItem("Tools/Brotato/Build Melee Ultimate Assets")]
    public static void Build()
    {
        BuildAssets(new[] { "ThienVan", "TamThien", "VanAnh" });
    }

    [MenuItem("Tools/Brotato/Build TamThien Assets")]
    public static void BuildTamThien()
    {
        BuildAssets(new[] { "TamThien" });
    }

    [MenuItem("Tools/Brotato/Build VanAnh Assets")]
    public static void BuildVanAnh()
    {
        BuildAssets(new[] { "VanAnh" });
    }

    private static void BuildAssets(string[] names)
    {
        if (EditorApplication.isPlaying) return;
        AssetDatabase.Refresh();
        foreach (string name in names)
        {
            string folder = Root + "/" + name;
            if (Directory.GetFiles(folder + "/Frames", "*.png").Length != 16)
                throw new System.InvalidOperationException("Expected 16 frames: " + name);
            var keys = new ObjectReferenceKeyframe[17];
            for (int i = 0; i < 16; i++)
            {
                string file = folder + "/Frames/" + name + "_" + i.ToString("00") + ".png";
                var importer = (TextureImporter)AssetImporter.GetAtPath(file);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 48;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                if (sprite == null) throw new System.InvalidOperationException(file);
                keys[i] = new ObjectReferenceKeyframe { time = i * .08f, value = sprite };
            }
            keys[16] = new ObjectReferenceKeyframe { time = 1.28f, value = null };
            string clipPath = folder + "/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
            clip.frameRate = 25;
            AnimationUtility.SetObjectReferenceCurve(clip,
                EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            string controllerPath = folder + "/" + name + ".controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            var machine = controller.layers[0].stateMachine;
            var state = machine.defaultState;
            if (state == null) state = machine.AddState(name);
            state.motion = clip;
            machine.defaultState = state;
            EditorUtility.SetDirty(clip);
            EditorUtility.SetDirty(controller);
            var preview = new GameObject(name + "Visual");
            try
            {
                var renderer = preview.AddComponent<SpriteRenderer>();
                renderer.sprite = (Sprite)keys[0].value;
                renderer.sortingOrder = 4;
                renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
                preview.AddComponent<Animator>().runtimeAnimatorController = controller;
                PrefabUtility.SaveAsPrefabAsset(preview, folder + "/" + name + "Visual.prefab");
            }
            finally { Object.DestroyImmediate(preview); }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("MELEE ULTIMATES READY: " + names.Length + " visual sets built successfully.");
    }
}
