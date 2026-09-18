using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CombatSetup
{
    [MenuItem("Tools/Brotato/Apply Combat Setup")]
    public static void Apply()
    {
        if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Stop Play Mode first.");
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/GameScene.unity")
            throw new System.InvalidOperationException("Open GameScene first.");
        var player = Object.FindFirstObjectByType<PlayerHealth>();
        var waves = Object.FindFirstObjectByType<WaveManager>();
        var spawner = Object.FindFirstObjectByType<EnemySpawner>();
        var game = Object.FindFirstObjectByType<GameManager>();
        var camera = Camera.main;
        if (!player || !waves || !spawner || !game || !camera)
            throw new System.InvalidOperationException("Missing gameplay objects.");

        SetReference(waves, "enemySpawner", spawner);
        SetReference(waves, "gameManager", game);
        SetReference(game, "playerHealth", player);
        SetReference(game, "enemySpawner", spawner);
        SetReference(game, "waveManager", waves);
        SetReference(spawner, "player", player.transform);
        SetReference(spawner, "gameplayCamera", camera);
        var follow = camera.GetComponent<CameraFollow>();
        if (!follow) follow = Undo.AddComponent<CameraFollow>(camera.gameObject);
        SetReference(follow, "target", player.transform);
        Undo.RecordObject(camera.transform, "Center gameplay camera");
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
            camera.transform.position.z);

        var serialized = new SerializedObject(waves);
        serialized.FindProperty("intermissionDuration").floatValue = 2f;
        var entries = serialized.FindProperty("waves");
        entries.arraySize = 3;
        float[] durations = { 30f, 30f, 45f };
        int[] caps = { 10, 15, 30 };
        int[] hp = { 10, 20, 30 };
        float[] intervals = { 1.5f, 1f, 0.6f };
        float[] speeds = { 2f, 2.5f, 3f };
        for (int i = 0; i < 3; i++)
        {
            var entry = entries.GetArrayElementAtIndex(i);
            entry.FindPropertyRelative("duration").floatValue = durations[i];
            entry.FindPropertyRelative("maxEnemiesAlive").intValue = caps[i];
            entry.FindPropertyRelative("enemyHealth").intValue = hp[i];
            entry.FindPropertyRelative("spawnInterval").floatValue = intervals[i];
            entry.FindPropertyRelative("enemySpeed").floatValue = speeds[i];
        }
        serialized.ApplyModifiedProperties();

        string[] bulletGuids = AssetDatabase.FindAssets("Bullet t:Prefab", new[] { "Assets/Prefabs" });
        if (bulletGuids.Length != 1) throw new System.InvalidOperationException("Expected exactly one Bullet prefab.");
        string bulletPath = AssetDatabase.GUIDToAssetPath(bulletGuids[0]);
        var prefab = PrefabUtility.LoadPrefabContents(bulletPath);
        try
        {
            if (!prefab.GetComponent<CircleCollider2D>())
                throw new System.InvalidOperationException("Bullet needs its circle collider.");
            // Keep the redundant collider for easy recovery, but disable it.
            foreach (var box in prefab.GetComponents<BoxCollider2D>()) box.enabled = false;
            PrefabUtility.SaveAsPrefabAsset(prefab, bulletPath);
        }
        finally { PrefabUtility.UnloadPrefabContents(prefab); }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Combat setup saved: 10/15/30 enemies, 10/20/30 HP, 30/30/45 seconds, camera follow.");
    }

    private static void SetReference(Object owner, string field, Object value)
    {
        var serialized = new SerializedObject(owner);
        serialized.FindProperty(field).objectReferenceValue = value;
        serialized.ApplyModifiedProperties();
    }
}
