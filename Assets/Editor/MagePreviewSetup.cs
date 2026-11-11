using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MagePreviewSetup
{
    private const string Art = "Assets/Art/Mage/";
    private const string ScenePath = "Assets/Scenes/Magical.unity";
    private const string PrefabPath = "Assets/Prefabs/Player/Mage.prefab";
    private static readonly string[] AtlasPaths =
    {
        "3 skill starter/hoi-mau.png", "3 skill starter/song-xung-kich.png",
        "3 skill starter/vung-trong-luc.png", "fire/fireball.png",
        "fire/small-explosion.png", "fire/burning-effect-only.png",
        "ice/frost-breath.png", "ice/freeze-v2-d9694c96.png",
        "light/magic-circle-cast-53df1a37.png", "light/smite.png",
        "dark/magic-missile.png", "dark/implosion.png",
        "dark/cursed-effect-only.png", "ulti/time-stop.png",
        "ulti/star-fall.png", "ulti/rune-circle.png",
        "ulti/arcane-orb.png", "ulti/arcane-missiles.png"
    };

    [MenuItem("Tools/Brotato/Build Mage Preview")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("Stop Play Mode before building Mage preview."); return; }
        ConfigureArt();
        MarkBossPrefabs();
        AssetDatabase.Refresh();
        GameObject player = BuildPrefab();
        if (player == null) return;

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject previous = GameObject.Find("Mage Preview");
        if (previous != null) Object.DestroyImmediate(previous);
        var root = new GameObject("Mage Preview");
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(player, scene);
        instance.transform.SetParent(root.transform);
        instance.transform.position = Vector3.zero;

        Camera camera = Camera.main;
        if (camera == null) { Debug.LogError("Magical.unity needs a Main Camera."); return; }
        camera.orthographic = true;
        camera.orthographicSize = 4.5f;
        camera.backgroundColor = new Color(0.1f, 0.12f, 0.19f);
        CameraFollow follow = camera.GetComponent<CameraFollow>();
        if (follow == null) follow = camera.gameObject.AddComponent<CameraFollow>();
        SetReference(follow, "target", instance.transform);

        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Enemy.prefab");
        if (enemyPrefab == null) { Debug.LogError("Enemy prefab missing."); return; }
        var enemies = new GameObject("Preview Enemies");
        enemies.transform.SetParent(root.transform);
        for (int i = 0; i < 7; i++)
        {
            GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab, scene);
            enemy.transform.SetParent(enemies.transform);
            float angle = i * Mathf.PI * 2f / 7f;
            enemy.transform.position = new Vector3(Mathf.Cos(angle) * 3.4f,
                Mathf.Sin(angle) * 3.1f, 0f);
            enemy.GetComponent<EnemyHealth>().InitializeHealth(100);
            enemy.GetComponent<EnemyController>().SetTarget(instance.transform);
        }
        MageDemoHUD hud = root.AddComponent<MageDemoHUD>();
        SetReference(hud, "loadout", instance.GetComponent<MageLoadout>());
        SetReference(hud, "combat", instance.GetComponent<MageCombat>());
        SetReference(hud, "ultimate", instance.GetComponent<MageUltimate>());
        SetReference(hud, "health", instance.GetComponent<PlayerHealth>());
        SetReference(hud, "dummyPrefab", enemyPrefab.GetComponent<EnemyHealth>());
        SetReference(hud, "dummyRoot", enemies.transform);
        SetReference(hud, "mage", instance.transform);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeObject = instance;
        Debug.Log("Mage preview built in Magical.unity. Choose loadout on the Mage prefab instance, then Play.");
    }

    [MenuItem("Tools/Brotato/Apply Mage Preview Scale")]
    public static void ApplyPresentation()
    {
        if (EditorApplication.isPlaying) { Debug.LogError("Stop Play Mode first."); return; }
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != ScenePath) { Debug.LogError("Open Magical.unity first."); return; }

        ConfigureStaffPivot();
        GameObject prefab = PrefabUtility.LoadPrefabContents(PrefabPath);
        Transform visual = prefab.transform.Find("Visual");
        Transform staff = prefab.transform.Find("Staff");
        if (visual == null || staff == null) { Debug.LogError("Mage prefab is missing Visual or Staff."); return; }
        visual.localScale = Vector3.one * 1.5f;
        staff.localScale = Vector3.one * 1.45f;
        Sprite staffSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "MageStaff.png");
        if (staffSprite == null) { Debug.LogError("Mage staff sprite did not import."); return; }
        staff.GetComponent<SpriteRenderer>().sprite = staffSprite;
        prefab.GetComponent<CircleCollider2D>().radius = 0.45f;
        SerializedObject library = new SerializedObject(prefab.GetComponent<MageEffectLibrary>());
        library.FindProperty("visualScale").floatValue = 2.2f;
        library.ApplyModifiedPropertiesWithoutUndo();
        SetAimPlacement(prefab.GetComponent<MageAim>());
        PrefabUtility.SaveAsPrefabAsset(prefab, PrefabPath);
        PrefabUtility.UnloadPrefabContents(prefab);

        Camera camera = Camera.main;
        GameObject mage = GameObject.FindWithTag("Player");
        if (camera == null || mage == null) { Debug.LogError("Magical scene needs Main Camera and Mage."); return; }
        camera.orthographic = true;
        camera.orthographicSize = 4.5f;
        CameraFollow follow = camera.GetComponent<CameraFollow>();
        if (follow == null) follow = camera.gameObject.AddComponent<CameraFollow>();
        SetReference(follow, "target", mage.transform);
        MageDemoHUD hud = Object.FindFirstObjectByType<MageDemoHUD>();
        if (hud != null)
        {
            SerializedObject hudSettings = new SerializedObject(hud);
            hudSettings.FindProperty("invulnerableForTesting").boolValue = true;
            hudSettings.ApplyModifiedPropertiesWithoutUndo();
            GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Enemy.prefab");
            Transform root = GameObject.Find("Preview Enemies")?.transform;
            if (enemyPrefab != null && root != null)
            {
                SetReference(hud, "dummyPrefab", enemyPrefab.GetComponent<EnemyHealth>());
                SetReference(hud, "dummyRoot", root);
                SetReference(hud, "mage", mage.transform);
            }
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Mage effects enlarged and staff grip aligned with hand. Existing loadout was kept.");
    }

    private static void ConfigureArt()
    {
        string[] paths = new string[AtlasPaths.Length + 2];
        paths[0] = Art + "Mage_Neutral_FullSheet_52frames.png";
        paths[1] = Art + "MageStaff.png";
        for (int i = 0; i < AtlasPaths.Length; i++) paths[i + 2] = Art + AtlasPaths[i];
        foreach (string path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) { Debug.LogError("Missing mage art: " + path); continue; }
            importer.textureType = path.EndsWith("MageStaff.png") ? TextureImporterType.Sprite : TextureImporterType.Default;
            if (path.EndsWith("MageStaff.png"))
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                TextureImporterSettings staffSettings = new TextureImporterSettings();
                importer.ReadTextureSettings(staffSettings);
                staffSettings.spriteAlignment = (int)SpriteAlignment.Custom;
                importer.SetTextureSettings(staffSettings);
                importer.spritePivot = new Vector2(0.38f, 0.5f);
            }
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
    }

    private static void ConfigureStaffPivot()
    {
        TextureImporter importer = AssetImporter.GetAtPath(Art + "MageStaff.png") as TextureImporter;
        if (importer == null) { Debug.LogError("Mage staff texture is missing."); return; }
        importer.spriteImportMode = SpriteImportMode.Single;
        TextureImporterSettings staffSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(staffSettings);
        staffSettings.spriteAlignment = (int)SpriteAlignment.Custom;
        importer.SetTextureSettings(staffSettings);
        importer.spritePivot = new Vector2(0.38f, 0.5f);
        importer.SaveAndReimport();
    }

    private static void SetAimPlacement(MageAim aim)
    {
        SerializedObject serialized = new SerializedObject(aim);
        serialized.FindProperty("handOffset").vector2Value = new Vector2(0.28f, 0.33f);
        serialized.FindProperty("staffTipOffset").floatValue = 0.64f;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject BuildPrefab()
    {
        Texture2D sheet = AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "Mage_Neutral_FullSheet_52frames.png");
        Sprite staffSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "MageStaff.png");
        if (sheet == null || staffSprite == null) { Debug.LogError("Mage character or staff art missing."); return null; }
        var player = new GameObject("Mage");
        player.tag = "Player";
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.radius = 0.45f;
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerHealth>();
        var visual = new GameObject("Visual");
        visual.transform.SetParent(player.transform);
        visual.transform.localScale = Vector3.one * 1.5f;
        SpriteRenderer characterRenderer = visual.AddComponent<SpriteRenderer>();
        characterRenderer.sortingOrder = 10;
        MageVisual animation = visual.AddComponent<MageVisual>();
        SetReference(animation, "characterSheet", sheet);
        SetReference(animation, "controller", player.GetComponent<PlayerController>());
        var staff = new GameObject("Staff");
        staff.transform.SetParent(player.transform);
        SpriteRenderer staffRenderer = staff.AddComponent<SpriteRenderer>();
        staffRenderer.sprite = staffSprite;
        staffRenderer.sortingOrder = 11;
        staff.transform.localScale = Vector3.one * 1.45f;
        MageAim aim = player.AddComponent<MageAim>();
        SetReference(aim, "staff", staff.transform);
        SetReference(aim, "staffRenderer", staffRenderer);
        SetAimPlacement(aim);
        player.AddComponent<MageLoadout>();
        MageEffectLibrary library = player.AddComponent<MageEffectLibrary>();
        var serialized = new SerializedObject(library);
        SerializedProperty atlases = serialized.FindProperty("atlases");
        atlases.arraySize = AtlasPaths.Length;
        for (int i = 0; i < AtlasPaths.Length; i++)
            atlases.GetArrayElementAtIndex(i).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Texture2D>(Art + AtlasPaths[i]);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        var scaleSettings = new SerializedObject(library);
        scaleSettings.FindProperty("visualScale").floatValue = 2.2f;
        scaleSettings.ApplyModifiedPropertiesWithoutUndo();
        player.AddComponent<MageCombat>();
        player.AddComponent<MageUltimate>();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, PrefabPath);
        Object.DestroyImmediate(player);
        return prefab;
    }

    private static void MarkBossPrefabs()
    {
        string[] paths =
        {
            "Assets/Prefabs/Enemy/LavaGolem/LavaGolemBoss.prefab",
            "Assets/Prefabs/Enemy/Skeleton/SkeletonBoss.prefab",
            "Assets/Prefabs/Enemy/Yeti/YetiBoss.prefab"
        };
        foreach (string path in paths)
        {
            GameObject boss = PrefabUtility.LoadPrefabContents(path);
            if (boss.GetComponent<MageBossMarker>() == null)
            {
                boss.AddComponent<MageBossMarker>();
                PrefabUtility.SaveAsPrefabAsset(boss, path);
            }
            PrefabUtility.UnloadPrefabContents(boss);
        }
    }

    private static void SetReference(Object component, string property, Object value)
    {
        var serialized = new SerializedObject(component);
        serialized.FindProperty(property).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
