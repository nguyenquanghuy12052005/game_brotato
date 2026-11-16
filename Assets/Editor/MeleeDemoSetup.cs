using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MeleeDemoSetup
{
    [MenuItem("Tools/Brotato/Rebuild and Verify Melee Demo %&F8")]
    public static void BuildAndVerify()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        Build();
        MeleeDemoVerification.Run();
    }

    [MenuItem("Tools/Brotato/Create Melee Demo Scene")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) return;
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cameraObject = new GameObject("Demo Camera", typeof(Camera));
        var camera = cameraObject.GetComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 0, -10);
        camera.orthographic = true;
        camera.orthographicSize = 3.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.11f, .15f, .13f);
        var demo = new GameObject("Melee Skill Demo").AddComponent<MeleeSkillDemo>();
        var combat = demo.gameObject.AddComponent<MeleeDemoCombat>();
        var combatData = new SerializedObject(combat);
        var characters = combatData.FindProperty("characterPrefabs");
        characters.arraySize = 4;
        string[] characterNames = { "Swordsman", "Assassin", "DaoWarrior", "QiSwordsman" };
        for (int i = 0; i < characterNames.Length; i++)
        {
            string character = characterNames[i];
            characters.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Player/" + character + "/" + character + "Visual.prefab");
        }
        var enemy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Enemy.prefab").GetComponentInChildren<SpriteRenderer>();
        combatData.FindProperty("enemySprite").objectReferenceValue = enemy.sprite;
        combatData.FindProperty("spriteMaterial").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
        const string goldPath = "Assets/Prefabs/Player/MeleeUltimates/VanAnh/GoldVfxOnly.mat";
        var goldMaterial = AssetDatabase.LoadAssetAtPath<Material>(goldPath);
        if (goldMaterial == null)
        {
            goldMaterial = new Material(Shader.Find("Brotato/Gold VFX Only"));
            AssetDatabase.CreateAsset(goldMaterial, goldPath);
        }
        combatData.FindProperty("goldVfxMaterial").objectReferenceValue = goldMaterial;
        const string barrierPath = "Assets/Prefabs/Player/MeleeSkills/Stage1/KhienChan/BarrierShield.mat";
        var barrier = AssetDatabase.LoadAssetAtPath<Material>(barrierPath);
        if (barrier == null)
        {
            barrier = new Material(Shader.Find("Brotato/Barrier Shield"));
            AssetDatabase.CreateAsset(barrier, barrierPath);
        }
        combatData.FindProperty("barrierMaterial").objectReferenceValue = barrier;
        const string cleanPath = "Assets/Prefabs/Player/MeleeSkills/CleanVfx.mat";
        var cleanMaterial = AssetDatabase.LoadAssetAtPath<Material>(cleanPath);
        if (cleanMaterial == null)
        {
            cleanMaterial = new Material(Shader.Find("Brotato/Clean VFX"));
            AssetDatabase.CreateAsset(cleanMaterial, cleanPath);
        }
        combatData.FindProperty("cleanVfxMaterial").objectReferenceValue = cleanMaterial;
        combatData.FindProperty("sweepSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Player/MeleeSkills/DaoWarrior/HoanhTao/Frames/HoanhTao_03.png");
        combatData.FindProperty("returningWaveSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Player/MeleeSkills/QiSwordsman/HoiNguyet/Frames/HoiNguyet_02.png");
        combatData.FindProperty("fanSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Player/MeleeSkills/QiSwordsman/PhanQuang/Frames/PhanQuang_02.png");
        combatData.ApplyModifiedPropertiesWithoutUndo();
        string[] paths = {
            "MeleeSkills/Stage1/KhienChan", "MeleeSkills/Stage1/AnhBo", "MeleeSkills/Stage1/LuoiDaoHoThe",
            "MeleeSkills/Assassin/TamLienThu", "MeleeSkills/Assassin/AnhHoiKich", "MeleeSkills/Assassin/AnTuHuyet",
            "MeleeSkills/DaoWarrior/DiaLietTram", "MeleeSkills/DaoWarrior/HoanhTao", "MeleeSkills/DaoWarrior/TrongDao",
            "MeleeSkills/QiSwordsman/HoiNguyet", "MeleeSkills/QiSwordsman/PhanQuang", "MeleeSkills/QiSwordsman/KiemAn",
            "MeleeUltimates/ThienVan", "MeleeUltimates/TamThien", "MeleeUltimates/VanAnh"
        };
        string[] titles = {
            "Khiên Chắn", "Ảnh Bộ", "Lưỡi Đao Hộ Thể", "Tam Liên Thứ", "Ảnh Hồi Kích", "Ấn Tử Huyệt",
            "Địa Liệt Trảm", "Hoành Tảo", "Trọng Đao", "Hồi Nguyệt", "Phân Quang", "Kiếm Ấn",
            "Thiên Vẫn Trảm", "Tam Thiên Kiếm Quyết", "Vạn Ảnh Tuyệt Sát"
        };
        string[] groups = { "Giai đoạn 1", "Sát thủ · Nội tại", "Đao khách · Nội tại", "Kiếm khí · Nội tại", "Lõi cuối" };
        string[] descriptions = {
            "Màn chắn trong suốt bao quanh nhân vật và di chuyển cùng người. Dùng WASD để thử.",
            "Bóng mờ nối tiếp trên đường lướt.",
            "Hai lưỡi đao xoay quanh nhân vật.",
            "Chuỗi đâm nhanh sau hai đòn trúng cùng mục tiêu.",
            "Dư ảnh đâm bồi sau khi rời tầm cận chiến.",
            "Dấu tử huyệt và hiệu ứng phá ấn.",
            "Nhát bổ mở đường nứt chạy về phía trước.",
            "Cú quét vòng quanh thân khi đánh trúng nhiều địch.",
            "Ánh đồng tụ trên vũ khí trước cú bổ mạnh.",
            "Sóng kiếm quay đầu và trở về.",
            "Ba sóng kiếm tỏa hình quạt.",
            "Hai kiếm ấn bị phá bằng nhát chém trực tiếp.",
            "Giáng xuống rồi quạt lửa quanh tâm.",
            "Tụ ba đường kiếm, phóng kiếm khí theo hình nón.",
            "Các vệt chém ảo ảnh nối theo ngôi sao trên sân xanh nhạt."
        };
        var serialized = new SerializedObject(demo);
        serialized.FindProperty("demoCamera").objectReferenceValue = camera;
        serialized.FindProperty("combat").objectReferenceValue = combat;
        var entries = serialized.FindProperty("skills");
        entries.arraySize = paths.Length;
        for (int i = 0; i < paths.Length; i++)
        {
            string name = System.IO.Path.GetFileName(paths[i]);
            string path = "Assets/Prefabs/Player/" + paths[i] + "/" + name + "Visual.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) throw new System.InvalidOperationException("Missing skill prefab: " + path);
            if (prefab.GetComponent<Animator>() == null || prefab.GetComponent<SpriteRenderer>() == null)
                throw new System.InvalidOperationException("Invalid preview prefab: " + path);
            var entry = entries.GetArrayElementAtIndex(i);
            entry.FindPropertyRelative("title").stringValue = titles[i];
            entry.FindPropertyRelative("group").stringValue = groups[i / 3];
            entry.FindPropertyRelative("description").stringValue = descriptions[i];
            entry.FindPropertyRelative("prefab").objectReferenceValue = prefab;
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MeleeDemo.unity");
        Debug.Log("MELEE DEMO READY: 15 existing skill prefabs wired.");
    }
}
