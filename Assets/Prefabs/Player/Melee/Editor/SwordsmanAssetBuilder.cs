using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class SwordsmanAssetBuilder
{
    private const string Root = "Assets/Prefabs/Player/Melee/Swordsman";

    [InitializeOnLoadMethod]
    private static void ImportWhenReady()
    {
        if (File.Exists(Root + "/Frames/Death/Up/Death_Up_03.png")
            && !File.Exists(Root + "/Swordsman.controller"))
            EditorApplication.delayCall += Build;
    }

    [MenuItem("Tools/Brotato/Build Swordsman Assets")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) return;
        string[] directions = { "Down", "Left", "Right", "Up" };
        string[] actions = { "Idle", "Run", "Attack", "Death" };
        float[][] timing = {
            new[] { .16f, .17f, .17f, .17f },
            new[] { .10f, .10f, .10f, .10f, .10f },
            new[] { .12f, .14f, .05f, .07f, .10f, .26f },
            new[] { .12f, .12f, .14f, .20f }
        };
        string[] files = Directory.GetFiles(Root + "/Frames", "*.png", SearchOption.AllDirectories);
        if (files.Length != 76) throw new System.InvalidOperationException("Expected 76 swordsman frames.");
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
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "/Swordsman.controller");
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(Root + "/Swordsman.controller");
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
        var preview = new GameObject("SwordsmanVisual");
        try
        {
            var renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Frames/Idle/Down/Idle_Down_00.png");
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
            preview.AddComponent<Animator>().runtimeAnimatorController = controller;
            PrefabUtility.SaveAsPrefabAsset(preview, Root + "/SwordsmanVisual.prefab");
        }
        finally { Object.DestroyImmediate(preview); }
        AssetDatabase.SaveAssets();
        Debug.Log("SWORDSMAN READY: 76 sprites, 16 clips, controller and visual prefab.");
    }
}
