using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class MeleeSkillAssetBuilder
{
    private const string Root = "Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills";

    [MenuItem("Tools/Brotato/Build Melee Stage Skills")]
    public static void Build()
    {
        BuildAssets(new[] { "Assassin/TamLienThu", "Assassin/AnTuHuyet", "DaoWarrior/DiaLietTram", "DaoWarrior/HoanhTao", "DaoWarrior/TrongDao", "QiSwordsman/HoiNguyet", "QiSwordsman/PhanQuang", "QiSwordsman/KiemAn" });
    }

    private static void BuildAssets(string[] names)
    {
        if (EditorApplication.isPlaying) return;
        AssetDatabase.Refresh();
        foreach (string entry in names)
        {
            string folder = Root + "/" + entry;
            string name = Path.GetFileName(entry);
            if (Directory.GetFiles(folder + "/Frames", "*.png").Length != 8)
                throw new System.InvalidOperationException("Expected 8 frames: " + name);
            var keys = new ObjectReferenceKeyframe[9];
            for (int i = 0; i < 8; i++)
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
            bool orbitLoop = name == "LuoiDaoHoThe";
            keys[8] = new ObjectReferenceKeyframe { time = .64f, value = orbitLoop ? keys[0].value : null };
            string clipPath = folder + "/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
            clip.frameRate = 25;
            AnimationUtility.SetObjectReferenceCurve(clip,
                EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = orbitLoop;
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

