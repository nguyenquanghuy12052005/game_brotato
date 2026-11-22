using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Nối MapManager + 4 bảng màu quái vào GameScene rồi lưu.
/// Menu Tools > Brotato > Nối 4 map vào Scene.
/// </summary>
public static class MapSetupTool
{
    private const string ScenePath = "Assets/Scenes/GameScene.unity";
    private const string MapManagerName = "MapManager";

    private static readonly (string biome, float centerX, string[] prefabs)[] Maps =
    {
        ("Forest", 0f, new[] { "Enemy" }),
        ("Moon", 35f, new[] { "Enemy_Goblin_Green" }),
        ("Inferno", 70f, new[] { "Enemy_Goblin_Red" }),
        ("Tundra", 105f, new[] { "Enemy_Goblin_Snow" })
    };

    [MenuItem("Tools/Brotato/Nối 4 map vào Scene")]
    public static void Apply()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Hãy thoát Play Mode trước.");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        WaveManager wave = Object.FindFirstObjectByType<WaveManager>();
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        PlayerHealth health = Object.FindFirstObjectByType<PlayerHealth>();
        if (wave == null || spawner == null || health == null)
        {
            Debug.LogError("Thiếu WaveManager / EnemySpawner / PlayerHealth trong scene.");
            return;
        }

        // Tạo MapManager nếu chưa có.
        MapManager manager = Object.FindFirstObjectByType<MapManager>();
        if (manager == null)
        {
            GameObject holder = new GameObject(MapManagerName);
            manager = holder.AddComponent<MapManager>();
            Debug.Log("[Setup] Đã tạo object MapManager.");
        }

        SerializedObject map = new SerializedObject(manager);
        map.FindProperty("player").objectReferenceValue = health.transform;
        map.FindProperty("playerHealth").objectReferenceValue = health;
        map.FindProperty("enemySpawner").objectReferenceValue = spawner;
        map.FindProperty("playerController").objectReferenceValue =
            health.GetComponent<PlayerController>();

        SerializedProperty maps = map.FindProperty("maps");
        maps.arraySize = Maps.Length;
        for (int i = 0; i < Maps.Length; i++)
        {
            SerializedProperty element = maps.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("biomeName").stringValue = Maps[i].biome;
            element.FindPropertyRelative("centerX").floatValue = Maps[i].centerX;

            SerializedProperty enemies = element.FindPropertyRelative("enemies");
            enemies.arraySize = Maps[i].prefabs.Length;
            for (int j = 0; j < Maps[i].prefabs.Length; j++)
            {
                string path = $"Assets/Prefabs/Enemy/{Maps[i].prefabs[j]}.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogError("Thiếu prefab: " + path);
                    return;
                }
                SerializedProperty entry = enemies.GetArrayElementAtIndex(j);
                entry.FindPropertyRelative("prefab").objectReferenceValue = prefab;
                entry.FindPropertyRelative("weight").floatValue = 1f;
            }
            Debug.Log($"[Setup] Map {i + 1} {Maps[i].biome}: tâm X {Maps[i].centerX}, " +
                      $"{Maps[i].prefabs.Length} loại quái");
        }
        map.ApplyModifiedProperties();

        // WaveManager cần biết MapManager.
        SerializedObject waveObject = new SerializedObject(wave);
        waveObject.FindProperty("mapManager").objectReferenceValue = manager;
        waveObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(wave);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[Setup] Đã nối 4 map và lưu scene.");
    }
}
