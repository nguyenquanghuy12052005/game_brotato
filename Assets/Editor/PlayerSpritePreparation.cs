using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

// Prepare chi tao asset; Apply la lenh rieng de gan vao prefab va scene.
public static class PlayerSpritePreparation
{
    private const string Root = "Assets/Prefabs/Player/CharacterArt";
    private const string Source = Root + "/PlayerSheet_Transparent_v1.png";
    private const int FrameWidth = 192;
    private const int FrameHeight = 224;
    private const int GroundY = 32;
    private static readonly string[] Directions = { "Down", "Left", "Right", "Up" };
    private static readonly string[] Actions = { "Idle", "Run", "Death" };
    private static readonly int[] Counts = { 4, 5, 4 };
    private static readonly int[] Rates = { 6, 10, 8 };

    private struct CutFrame
    {
        public RectInt Bounds;
        public int Component;
    }

    [MenuItem("Tools/Brotato/Apply Player Artwork")]
    public static void ApplyArtwork()
    {
        if (EditorApplication.isPlaying)
            throw new InvalidOperationException("Hay dung Play Mode truoc.");
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/GameScene.unity")
            throw new InvalidOperationException("Hay mo GameScene truoc.");

        var player = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root + "/PlayerCharacter.controller");
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Frames/Idle/Down/Idle_Down_00.png");
        if (player == null || controller == null || sprite == null)
            throw new InvalidOperationException("Thieu Player hoac bo animation.");

        const string prefabPath = "Assets/Prefabs/Player/Player.prefab";
        if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(player.gameObject) != prefabPath)
            throw new InvalidOperationException("Player trong scene khong phai prefab can thay.");

        GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            ConfigurePlayer(prefab.GetComponent<PlayerController>(), controller, sprite);
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        }
        finally { PrefabUtility.UnloadPrefabContents(prefab); }

        ConfigurePlayer(player, controller, sprite);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = player.transform.Find("PlayerVisual").gameObject;
        Debug.Log("PLAYER ARTWORK APPLIED: prefab and GameScene saved.");
    }

    private static void ConfigurePlayer(PlayerController player,
        RuntimeAnimatorController controller, Sprite sprite)
    {
        Transform visual = player.transform.Find("PlayerVisual");
        if (visual == null || visual.GetComponent<SpriteRenderer>() == null)
            throw new InvalidOperationException("Thieu PlayerVisual/SpriteRenderer.");

        Undo.RecordObject(player, "Set directional player artwork");
        var serialized = new SerializedObject(player);
        serialized.FindProperty("playerVisual").objectReferenceValue = null;
        serialized.ApplyModifiedProperties();

        Undo.RecordObject(visual, "Align player artwork");
        visual.localPosition = Vector3.zero;
        visual.localRotation = Quaternion.identity;
        visual.localScale = Vector3.one;
        var renderer = visual.GetComponent<SpriteRenderer>();
        Undo.RecordObject(renderer, "Set player sprite");
        renderer.sprite = sprite;
        renderer.flipX = false;
        renderer.flipY = false;
        var animator = visual.GetComponent<Animator>();
        if (animator == null) animator = Undo.AddComponent<Animator>(visual.gameObject);
        Undo.RecordObject(animator, "Set player animator");
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.enabled = true;
        if (visual.GetComponent<PlayerSpriteAnimator>() == null)
            Undo.AddComponent<PlayerSpriteAnimator>(visual.gameObject);

        if (PrefabUtility.IsPartOfPrefabInstance(player))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(player);
            PrefabUtility.RecordPrefabInstancePropertyModifications(visual);
            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            PrefabUtility.RecordPrefabInstancePropertyModifications(animator);
        }
    }

    [MenuItem("Tools/Brotato/Prepare Player Sprites")]
    public static void Prepare()
    {
        if (EditorApplication.isPlaying)
            throw new InvalidOperationException("Hay dung Play Mode truoc.");

        var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!source.LoadImage(File.ReadAllBytes(Source)))
            throw new InvalidOperationException("Khong doc duoc spritesheet.");

        try
        {
            Color32[] pixels = source.GetPixels32();
            List<CutFrame>[] rows = FindFrames(pixels, source.width, source.height, out int[] labels);
            Directory.CreateDirectory(Root + "/Frames");
            Directory.CreateDirectory(Root + "/Clips");
            var preview = new Texture2D(FrameWidth * 13, FrameHeight * 4,
                TextureFormat.RGBA32, false);
            preview.SetPixels32(new Color32[preview.width * preview.height]);
            var report = new List<string>
            {
                "Direction,Action,Frame,SourceX,SourceY,Width,Height"
            };

            try
            {
                for (int row = 0; row < 4; row++)
                {
                    int column = 0;
                    for (int action = 0; action < Actions.Length; action++)
                    {
                        string directory = Root + "/Frames/" + Actions[action] + "/" + Directions[row];
                        Directory.CreateDirectory(directory);
                        for (int frame = 0; frame < Counts[action]; frame++, column++)
                        {
                            CutFrame cut = rows[row][column];
                            RectInt bounds = cut.Bounds;
                            var output = MakeFrame(pixels, source.width, source.height, cut, labels);
                            try
                            {
                                string path = directory + "/" + Actions[action] + "_"
                                    + Directions[row] + "_" + frame.ToString("00") + ".png";
                                File.WriteAllBytes(path, output.EncodeToPNG());
                                preview.SetPixels(column * FrameWidth, (3 - row) * FrameHeight,
                                    FrameWidth, FrameHeight, output.GetPixels());
                                report.Add($"{Directions[row]},{Actions[action]},{frame},{bounds.x},{bounds.y},{bounds.width},{bounds.height}");
                            }
                            finally { UnityEngine.Object.DestroyImmediate(output); }
                        }
                    }
                }

                preview.Apply();
                Directory.CreateDirectory("docs/player-art");
                File.WriteAllBytes("docs/player-art/PlayerFramesPreview.png", preview.EncodeToPNG());
                File.WriteAllLines("docs/player-art/frame-bounds.csv", report);
            }
            finally { UnityEngine.Object.DestroyImmediate(preview); }

            AssetDatabase.Refresh();
            foreach (string file in Directory.GetFiles(Root + "/Frames", "*.png", SearchOption.AllDirectories))
                ConfigureSprite(file.Replace('\\', '/'));

            CreateAnimations();
            ConfigureSprite(Source);
            AssetDatabase.SaveAssets();
            Validate();
            Debug.Log("PLAYER SPRITES READY: 52 frames, 12 clips, 1 controller. Prefab/scene unchanged.");
        }
        finally { UnityEngine.Object.DestroyImmediate(source); }
    }

    private static List<CutFrame>[] FindFrames(Color32[] pixels, int width, int height, out int[] labels)
    {
        var rows = new List<CutFrame>[4];
        for (int i = 0; i < rows.Length; i++) rows[i] = new List<CutFrame>();
        var visited = new bool[pixels.Length];
        labels = new int[pixels.Length];
        var queue = new Queue<int>();
        int component = 0;

        for (int start = 0; start < pixels.Length; start++)
        {
            if (visited[start] || pixels[start].a <= 128) continue;
            component++;
            visited[start] = true;
            queue.Enqueue(start);
            int count = 0, minX = width, minY = height, maxX = 0, maxY = 0;
            while (queue.Count > 0)
            {
                int position = queue.Dequeue();
                labels[position] = component;
                int x = position % width, y = position / width;
                count++;
                minX = Math.Min(minX, x); minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                    int next = ny * width + nx;
                    if (visited[next] || pixels[next].a <= 128) continue;
                    visited[next] = true;
                    queue.Enqueue(next);
                }
            }

            if (count < 500) continue; // Bo cac dom alpha roi rat.
            int bottomFromTop = height - 1 - minY;
            int row = Mathf.Clamp(Mathf.RoundToInt((bottomFromTop - 278f) / 240f), 0, 3);
            int left = Math.Max(0, minX - 2), bottom = Math.Max(0, minY - 2);
            rows[row].Add(new CutFrame
            {
                Component = component,
                Bounds = new RectInt(left, bottom,
                    Math.Min(width - 1, maxX + 2) - left + 1,
                    Math.Min(height - 1, maxY + 2) - bottom + 1)
            });
        }

        for (int row = 0; row < rows.Length; row++)
        {
            rows[row].Sort((a, b) => a.Bounds.x.CompareTo(b.Bounds.x));
            if (rows[row].Count != 13)
                throw new InvalidOperationException($"Hang {row}: can 13 hinh tach roi, tim thay {rows[row].Count}. Khong tu cat doan.");
        }
        return rows;
    }

    private static Texture2D MakeFrame(Color32[] source, int sourceWidth, int sourceHeight,
        CutFrame cut, int[] labels)
    {
        RectInt bounds = cut.Bounds;
        if (bounds.width > FrameWidth || bounds.height + GroundY > FrameHeight)
            throw new InvalidOperationException("Khung qua lon; khong tu thu nho tung pose.");

        var result = new Texture2D(FrameWidth, FrameHeight, TextureFormat.RGBA32, false);
        var target = new Color32[FrameWidth * FrameHeight];
        int left = (FrameWidth - bounds.width) / 2;
        for (int y = 0; y < bounds.height; y++)
        for (int x = 0; x < bounds.width; x++)
        {
            int sx = bounds.x + x, sy = bounds.y + y;
            int position = sy * sourceWidth + sx;
            Color32 color = source[position];
            // Hai pose co the chong bounding box; chi lay pixel cua dung nhan vat.
            if (labels[position] != cut.Component)
            {
                if (color.a > 128 || color.a == 0) continue;
                bool nearOwnOutline = false;
                for (int dy = -2; dy <= 2 && !nearOwnOutline; dy++)
                for (int dx = -2; dx <= 2; dx++)
                {
                    int nx = sx + dx, ny = sy + dy;
                    if (nx >= 0 && ny >= 0 && nx < sourceWidth && ny < sourceHeight
                        && labels[ny * sourceWidth + nx] == cut.Component)
                    { nearOwnOutline = true; break; }
                }
                if (!nearOwnOutline) continue;
            }
            target[(GroundY + y) * FrameWidth + left + x] = color;
        }
        result.SetPixels32(target);
        result.Apply();
        return result;
    }

    private static void ConfigureSprite(string path)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 120f;
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteAlignment = (int)SpriteAlignment.Center;
        settings.spritePivot = new Vector2(0.5f, 0.5f);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.SaveAndReimport();
    }

    private static void CreateAnimations()
    {
        string controllerPath = Root + "/PlayerCharacter.controller";
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        AnimatorStateMachine machine = controller.layers[0].stateMachine;

        foreach (string direction in Directions)
        for (int action = 0; action < Actions.Length; action++)
        {
            string name = Actions[action] + "_" + direction;
            string path = Root + "/Clips/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, path); }
            clip.name = name;
            clip.frameRate = Rates[action];
            var keys = new ObjectReferenceKeyframe[Counts[action] + 1];
            for (int frame = 0; frame < Counts[action]; frame++)
            {
                string framePath = Root + "/Frames/" + Actions[action] + "/" + direction
                    + "/" + name + "_" + frame.ToString("00") + ".png";
                keys[frame] = new ObjectReferenceKeyframe
                { time = frame / (float)Rates[action], value = AssetDatabase.LoadAssetAtPath<Sprite>(framePath) };
            }
            keys[Counts[action]] = new ObjectReferenceKeyframe
            { time = Counts[action] / (float)Rates[action], value = keys[Counts[action] - 1].value };
            AnimationUtility.SetObjectReferenceCurve(clip,
                EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
            var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = action != 2;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
            AnimatorState state = null;
            foreach (ChildAnimatorState child in machine.states)
                if (child.state.name == name) state = child.state;
            if (state == null) state = machine.AddState(name);
            state.motion = clip;
            if (name == "Idle_Down") machine.defaultState = state;
            EditorUtility.SetDirty(clip);
        }
        EditorUtility.SetDirty(controller);
    }

    public static void Validate()
    {
        int count = AssetDatabase.FindAssets("t:Sprite", new[] { Root + "/Frames" }).Length;
        if (count != 52) throw new InvalidOperationException("Expected 52 sprites, found " + count);
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "/PlayerCharacter.controller");
        if (controller == null || controller.layers[0].stateMachine.states.Length != 12)
            throw new InvalidOperationException("Expected 12 animation states.");
        foreach (ChildAnimatorState state in controller.layers[0].stateMachine.states)
        {
            var clip = (AnimationClip)state.state.motion;
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            if (bindings.Length != 1) throw new InvalidOperationException("Missing sprite curve: " + clip.name);
            foreach (ObjectReferenceKeyframe key in AnimationUtility.GetObjectReferenceCurve(clip, bindings[0]))
                if (key.value == null) throw new InvalidOperationException("Missing frame: " + clip.name);
        }
        Debug.Log("PLAYER SPRITE ASSET VALIDATION PASSED");
    }
}
