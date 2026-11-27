using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Copies the three existing character-test roots into Gameplay while letting Unity remap
/// serialized references inside each copied hierarchy.
/// </summary>
public static class GameplayCharacterSelectionSetup
{
    private const string GameplayPath = "Assets/Scenes/Gameplay.unity";
    private const string ReferencePath = "Assets/Scenes/CharacterProgressionTest.unity";

    [MenuItem("Tools/Brotato/Setup Gameplay Character Selection")]
    private static void SetupGameplayCharacterSelection()
    {
        Scene gameplay = SceneManager.GetSceneByPath(GameplayPath);
        if (!gameplay.IsValid() || !gameplay.isLoaded)
        {
            Debug.LogError("Hãy mở Assets/Scenes/Gameplay.unity trước khi chạy công cụ này.");
            return;
        }

        if (FindRoot<CharacterProgressionTest>(gameplay) != null)
        {
            Debug.LogError("Gameplay đã có CharacterProgressionTest; dừng để tránh nhân đôi hệ thống.");
            return;
        }

        Scene reference = EditorSceneManager.OpenScene(ReferencePath, OpenSceneMode.Additive);
        CharacterProgressionTest sourceManager = FindRoot<CharacterProgressionTest>(reference);
        if (sourceManager == null)
        {
            EditorSceneManager.CloseScene(reference, true);
            Debug.LogError("Không tìm thấy CharacterProgressionTest trong scene tham chiếu.");
            return;
        }

        SerializedObject sourceSerialized = new SerializedObject(sourceManager);
        GameObject sourceGun = sourceSerialized.FindProperty("gunRoot").objectReferenceValue as GameObject;
        GameObject sourceMelee = sourceSerialized.FindProperty("meleeRoot").objectReferenceValue as GameObject;
        GameObject sourceMage = sourceSerialized.FindProperty("mageRoot").objectReferenceValue as GameObject;
        if (sourceGun == null || sourceMelee == null || sourceMage == null)
        {
            EditorSceneManager.CloseScene(reference, true);
            Debug.LogError("Scene tham chiếu thiếu một trong ba root Gun, Melee hoặc Mage.");
            return;
        }

        GameObject gunRoot = CopyRoot(sourceGun, gameplay);
        GameObject meleeRoot = CopyRoot(sourceMelee, gameplay);
        GameObject mageRoot = CopyRoot(sourceMage, gameplay);
        GameObject managerRoot = CopyRoot(sourceManager.gameObject, gameplay);
        CharacterProgressionTest manager = managerRoot.GetComponent<CharacterProgressionTest>();
        SerializedObject managerSerialized = new SerializedObject(manager);
        managerSerialized.FindProperty("gunRoot").objectReferenceValue = gunRoot;
        managerSerialized.FindProperty("meleeRoot").objectReferenceValue = meleeRoot;
        managerSerialized.FindProperty("mageRoot").objectReferenceValue = mageRoot;
        managerSerialized.ApplyModifiedPropertiesWithoutUndo();

        SetRootActive(gameplay, "Main Camera", false);
        SetRootActive(gameplay, "Player", false);
        SetRootActive(gameplay, "Enemy", false);

        EditorSceneManager.CloseScene(reference, true);
        EditorSceneManager.MarkSceneDirty(gameplay);
        EditorSceneManager.SaveScene(gameplay);
        Debug.Log("Gameplay đã có chọn hệ Gun/Melee/Mage. Các object cũ được giữ trong scene nhưng tắt để tránh trùng camera/điều khiển.");
    }

    private static GameObject CopyRoot(GameObject source, Scene destination)
    {
        GameObject copy = Object.Instantiate(source);
        copy.name = source.name;
        SceneManager.MoveGameObjectToScene(copy, destination);
        return copy;
    }

    private static T FindRoot<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponent<T>();
            if (component != null) return component;
        }
        return null;
    }

    private static void SetRootActive(Scene scene, string objectName, bool active)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            if (root.name == objectName)
                root.SetActive(active);
    }
}
