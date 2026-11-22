using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Đặt lại danh sách wave cho GameScene: mỗi wave là một loại quái mới xuất hiện thêm.
/// Menu Tools > Brotato > Đặt lại Wave theo loại quái.
/// </summary>
public static class WaveSetupTool
{
    private const string ScenePath = "Assets/Scenes/GameScene.unity";

    [MenuItem("Tools/Brotato/Đặt lại Wave theo loại quái")]
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
        if (wave == null)
        {
            Debug.LogError("Không tìm thấy WaveManager trong GameScene.");
            return;
        }

        SerializedObject serialized = new SerializedObject(wave);
        SerializedProperty waves = serialized.FindProperty("waves");

        // Hiện chỉ có lính thường -> 1 wave. Thêm cung thủ/tinh nhuệ/trùm thì thêm dòng ở đây.
        waves.arraySize = 1;
        SetWave(waves.GetArrayElementAtIndex(0), 30f, 10, 10, 1.5f, 2f);

        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(wave);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[Setup] Đã đặt 1 wave (chỉ lính thường) cho cả 4 map và lưu scene.");
    }

    private static void SetWave(SerializedProperty element, float duration, int alive,
        int health, float interval, float speed)
    {
        element.FindPropertyRelative("duration").floatValue = duration;
        element.FindPropertyRelative("maxEnemiesAlive").intValue = alive;
        element.FindPropertyRelative("enemyHealth").intValue = health;
        element.FindPropertyRelative("spawnInterval").floatValue = interval;
        element.FindPropertyRelative("enemySpeed").floatValue = speed;
    }
}
