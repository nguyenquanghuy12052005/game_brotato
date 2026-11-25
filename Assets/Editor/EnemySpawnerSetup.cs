using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Nối 4 prefab quái vào EnemySpawner trong GameScene rồi lưu scene.
/// Dùng menu Tools > Brotato > Nối 4 loại quái vào Spawner.
/// Chỉ chạy ở edit mode.
/// </summary>
public static class EnemySpawnerSetup
{
    private const string ScenePath = "Assets/Scenes/GameScene.unity";

    private static readonly (string path, float weight)[] Prefabs =
    {
        ("Assets/Prefabs/Enemy/Normal/Brown/Enemy_Normal_Brown.prefab", 4f),
        ("Assets/Prefabs/Enemy/Normal/Green/Enemy_Normal_Green.prefab", 2f),
        ("Assets/Prefabs/Enemy/Normal/Red/Enemy_Normal_Red.prefab", 2f),
        ("Assets/Prefabs/Enemy/Normal/Snow/Enemy_Normal_Snow.prefab", 2f)
    };

    [MenuItem("Tools/Brotato/Nối 4 loại quái vào Spawner")]
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

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner == null)
        {
            Debug.LogError("Không tìm thấy EnemySpawner trong GameScene.");
            return;
        }

        EnemyHealth template = null;
        SerializedObject serialized = new SerializedObject(spawner);
        SerializedProperty list = serialized.FindProperty("enemies");
        if (list == null)
        {
            Debug.LogError("EnemySpawner chưa biên dịch xong bản mới (không có field 'enemies'). " +
                           "Đợi Unity compile rồi chạy lại.");
            return;
        }

        list.arraySize = Prefabs.Length;
        for (int i = 0; i < Prefabs.Length; i++)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs[i].path);
            if (prefab == null)
            {
                Debug.LogError("Thiếu prefab: " + Prefabs[i].path);
                return;
            }
            if (template == null) template = prefab.GetComponent<EnemyHealth>();
            if (template == null)
            {
                Debug.LogError("Prefab không có EnemyHealth: " + Prefabs[i].path);
                return;
            }

            SerializedProperty element = list.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("prefab").objectReferenceValue = prefab;
            element.FindPropertyRelative("weight").floatValue = Prefabs[i].weight;
            Debug.Log($"[Setup] Ô {i}: {prefab.name} weight {Prefabs[i].weight}");
        }

        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(spawner);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log($"[Setup] Đã nối {Prefabs.Length} loại quái và lưu {ScenePath}.");
    }
}
