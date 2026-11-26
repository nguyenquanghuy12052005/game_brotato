using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class QiSwordsmanAssetBuilder
{
    private const string Root = "Assets/Prefabs/Player/Melee/QiSwordsman";
    private const string WaveRoot = "Assets/Prefabs/Player/Melee/Shared/VFX/QiWave";

    [MenuItem("Tools/Brotato/Build QiSwordsman Assets")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) return;
        string[] directions = { "Down", "Left", "Right", "Up" };
        string[] actions = { "Idle", "Run", "Attack", "Death" };
        float[][] timing = {
            new[] { .16f, .17f, .17f, .17f },
            new[] { .10f, .10f, .10f, .10f, .10f },
            new[] { .12f, .12f, .07f, .06f, .08f, .12f, .21f },
            new[] { .12f, .12f, .14f, .20f }
        };
        string[] files = Directory.GetFiles(Root + "/Frames", "*.png", SearchOption.AllDirectories);
        if (files.Length != 80) throw new System.InvalidOperationException("Expected 80 qi swordsman frames.");
        AssetDatabase.Refresh();
        var importFiles = new List<string>(files);
        string[] waveFiles = Directory.GetFiles(WaveRoot + "/WaveFrames", "*.png");
        if (waveFiles.Length != 4) throw new System.InvalidOperationException("Expected 4 qi wave frames.");
        importFiles.AddRange(waveFiles);
        foreach (string file in importFiles)
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
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "/QiSwordsman.controller");
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(Root + "/QiSwordsman.controller");
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
        var preview = new GameObject("QiSwordsmanVisual");
        try
        {
            var renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Frames/Idle/Down/Idle_Down_00.png");
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
            preview.AddComponent<Animator>().runtimeAnimatorController = controller;
            PrefabUtility.SaveAsPrefabAsset(preview, Root + "/QiSwordsmanVisual.prefab");
        }
        finally { Object.DestroyImmediate(preview); }
        CreateWave();
        AssetDatabase.SaveAssets();
        Debug.Log("QI SWORDSMAN READY: 80 character sprites, 4 wave sprites, 17 clips and 2 visual prefabs.");
    }

    private static void CreateWave()
    {
        string path = WaveRoot + "/QiWave.anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, path); }
        clip.name = "QiWave";
        clip.frameRate = 100;
        float[] duration = { .06f, .08f, .10f, .12f };
        var keys = new ObjectReferenceKeyframe[5];
        float time = 0f;
        for (int frame = 0; frame < 4; frame++)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(WaveRoot + "/WaveFrames/QiWave_" + frame.ToString("00") + ".png");
            if (sprite == null) throw new System.InvalidOperationException("Missing wave sprite " + frame);
            keys[frame] = new ObjectReferenceKeyframe { time = time, value = sprite };
            time += duration[frame];
        }
        keys[4] = new ObjectReferenceKeyframe { time = time, value = null };
        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        string controllerPath = WaveRoot + "/QiWave.controller";
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var machine = controller.layers[0].stateMachine;
        AnimatorState state = machine.defaultState;
        if (state == null) state = machine.AddState("QiWave");
        state.motion = clip;
        machine.defaultState = state;
        EditorUtility.SetDirty(controller);
        var preview = new GameObject("QiWaveVisual");
        try
        {
            var renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = (Sprite)keys[0].value;
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
            renderer.sortingOrder = 2;
            preview.AddComponent<Animator>().runtimeAnimatorController = controller;
            PrefabUtility.SaveAsPrefabAsset(preview, WaveRoot + "/QiWaveVisual.prefab");
        }
        finally { Object.DestroyImmediate(preview); }
    }
}
