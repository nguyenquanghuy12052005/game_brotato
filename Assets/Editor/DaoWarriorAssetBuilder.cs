using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class DaoWarriorAssetBuilder
{
    private const string Root = "Assets/Prefabs/Player/DaoWarrior";

    [MenuItem("Tools/Brotato/Build DaoWarrior Assets")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) return;
        string[] directions = { "Down", "Left", "Right", "Up" };
        string[] actions = { "Idle", "Run", "Attack", "Death" };
        float[][] timing = {
            new[] { .16f, .17f, .17f, .17f },
            new[] { .12f, .12f, .12f, .12f, .12f },
            new[] { .12f, .14f, .14f, .16f, .05f, .08f, .18f, .23f },
            new[] { .12f, .12f, .14f, .20f }
        };
        string[] files = Directory.GetFiles(Root + "/Frames", "*.png", SearchOption.AllDirectories);
        if (files.Length != 84) throw new System.InvalidOperationException("Expected 84 dao warrior frames.");
        AssetDatabase.Refresh();
        foreach (string file in files)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(file.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 120;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(.5f, .5f);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
        Directory.CreateDirectory(Root + "/Clips");
        AssetDatabase.Refresh();
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "/DaoWarrior.controller");
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(Root + "/DaoWarrior.controller");
        var machine = controller.layers[0].stateMachine;
        foreach (string direction in directions)
        for (int action = 0; action < actions.Length; action++)
        {
            string name = actions[action] + "_" + direction;
            string path = Root + "/Clips/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, path); }
            clip.name = name;
            clip.frameRate = 100;
            var keys = new ObjectReferenceKeyframe[timing[action].Length + 1];
            float time = 0;
            for (int frame = 0; frame < timing[action].Length; frame++)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Frames/" + actions[action]
                    + "/" + direction + "/" + name + "_" + frame.ToString("00") + ".png");
                if (sprite == null) throw new System.InvalidOperationException("Missing " + name + frame);
                keys[frame] = new ObjectReferenceKeyframe { time = time, value = sprite };
                time += timing[action][frame];
            }
            keys[keys.Length - 1] = new ObjectReferenceKeyframe { time = time, value = keys[keys.Length - 2].value };
            AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
            var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = action < 2;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
            AnimatorState state = null;
            foreach (var existing in machine.states) if (existing.state.name == name) state = existing.state;
            if (state == null) state = machine.AddState(name);
            state.motion = clip;
            if (name == "Idle_Down") machine.defaultState = state;
            EditorUtility.SetDirty(clip);
        }
        EditorUtility.SetDirty(controller);
        var preview = new GameObject("DaoWarriorVisual");
        try
        {
            var renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Frames/Idle/Down/Idle_Down_00.png");
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
            preview.AddComponent<Animator>().runtimeAnimatorController = controller;
            PrefabUtility.SaveAsPrefabAsset(preview, Root + "/DaoWarriorVisual.prefab");
        }
        finally { Object.DestroyImmediate(preview); }
        AssetDatabase.SaveAssets();
        Debug.Log("DAO WARRIOR READY: 84 sprites, 16 clips, controller and visual prefab.");
    }
}
