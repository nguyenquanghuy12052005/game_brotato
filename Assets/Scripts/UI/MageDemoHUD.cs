using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

// Play Mode controls for trying a Mage build without the future level-choice UI.
public sealed class MageDemoHUD : MonoBehaviour
{
    [SerializeField] private MageLoadout loadout;
    [SerializeField] private MageCombat combat;
    [SerializeField] private MageUltimate ultimate;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private bool invulnerableForTesting = true;
    [SerializeField] private EnemyHealth dummyPrefab;
    [SerializeField] private Transform dummyRoot;
    [SerializeField] private Transform mage;

    private string dummyCount = "7";
    private string dummyHealth = "100";
    private string dummyRadius = "2.2";

    private bool panelOpen;
    private bool restarting;
    private Vector2 scrollPosition;
    private GUIStyle statusStyle;

    private static readonly string[] StarterNames = { "Hồi máu", "Xung kích", "Trọng lực" };
    private static readonly string[] ElementNames = { "Hỏa", "Băng", "Quang", "Ám" };
    private static readonly string[] UltimateNames = { "Ngưng đọng", "Sao rơi", "Thiên cầu" };
    private static readonly string[][] TierNames =
    {
        new[] { "Sức nổ", "Sức cháy", "Tầm nổ" },
        new[] { "Làm chậm", "Đóng băng", "Sát thương lạnh" },
        new[] { "Thêm nhịp", "Tầm cột sáng", "Sức nhịp" },
        new[] { "Số mục tiêu", "Sức nguyền", "Suy yếu" }
    };
    private static readonly string[][] CoreNames =
    {
        new[] { "Cháy lâu", "Thêm cầu lửa", "Tập trung đốt", "Nổ khi chết" },
        new[] { "Lạnh sâu", "Tụ Hàn", "Giòn vỡ", "Lan băng" },
        new[] { "Dư quang", "Phán Quyết", "Dẫn sáng", "Chói mắt" },
        new[] { "Đa ấn", "Dày Vò", "Suy nhược", "Di chúc" }
    };

    private void Start()
    {
        if (health != null) health.SetDamageEnabled(!invulnerableForTesting);
        ApplyDummies();
    }

    private void OnGUI()
    {
        if (loadout == null || combat == null || ultimate == null || health == null) return;
        if (statusStyle == null)
        {
            statusStyle = new GUIStyle(GUI.skin.box);
            statusStyle.alignment = TextAnchor.UpperLeft;
            statusStyle.fontSize = 14;
            statusStyle.normal.textColor = Color.white;
        }

        float statusWidth = Mathf.Max(170f, Screen.width - 145f);
        string q = combat.StarterRemaining > 0f ? combat.StarterRemaining.ToString("0.0") + "s" : "sẵn sàng";
        string e = ultimate.Remaining > 0f ? ultimate.Remaining.ToString("0.0") + "s" : "sẵn sàng";
        GUI.Box(new Rect(8, 8, statusWidth, 75),
            $"HP {health.CurrentHealth}/{health.MaxHealth} | {ElementNames[(int)loadout.Element]}\n" +
            $"Q {StarterNames[(int)loadout.StarterSkill]}: {q}\n" +
            $"E {UltimateNames[(int)loadout.Ultimate]}: {e}  ({ultimate.OrbCharge}/6)", statusStyle);

        float toggleWidth = 125f;
        if (GUI.Button(new Rect(Screen.width - toggleWidth - 8, 8, toggleWidth, 30),
            panelOpen ? "Đóng bảng test" : "Bảng test skill"))
            panelOpen = !panelOpen;

        if (panelOpen) DrawPanel();
        if (health.CurrentHealth == 0)
        {
            GUI.Box(new Rect(Screen.width / 2f - 115f, Screen.height / 2f - 45f, 230f, 85f),
                "GAME OVER");
            if (GUI.Button(new Rect(Screen.width / 2f - 95f, Screen.height / 2f - 10f, 190f, 30f),
                "Chơi lại"))
                RestartPreview();
        }
    }

    private void DrawPanel()
    {
        float width = Mathf.Min(355f, Screen.width - 16f);
        float height = Mathf.Max(180f, Screen.height - 55f);
        Rect area = new Rect(Screen.width - width - 8f, 47f, width, height);
        Color originalColor = GUI.color;
        GUI.color = new Color(0.05f, 0.07f, 0.13f, 0.98f);
        GUI.DrawTexture(area, Texture2D.whiteTexture);
        GUI.color = originalColor;
        GUI.Box(area, "");
        GUILayout.BeginArea(new Rect(area.x + 8f, area.y + 6f, area.width - 16f, area.height - 12f));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();
        GUILayout.Label("THỬ SKILL");
        if (GUILayout.Button("Đặt lại cảnh", GUILayout.Width(90f))) RestartPreview();
        GUILayout.EndHorizontal();
        GUILayout.Label("WASD di chuyển | chuột ngắm | Q, E");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("−25 HP"))
        {
            health.SetDamageEnabled(true);
            health.TakeDamage(25);
            health.SetDamageEnabled(!invulnerableForTesting);
        }
        if (GUILayout.Button("Dùng Q")) combat.TryUseStarter();
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
        GUILayout.Label("PHÒNG TẬP - BÙ NHÌN ĐỨNG YÊN");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Số lượng", GUILayout.Width(72f));
        dummyCount = GUILayout.TextField(dummyCount, GUILayout.Width(50f));
        GUILayout.Label("HP", GUILayout.Width(24f));
        dummyHealth = GUILayout.TextField(dummyHealth, GUILayout.Width(58f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Bán kính", GUILayout.Width(72f));
        dummyRadius = GUILayout.TextField(dummyRadius, GUILayout.Width(58f));
        if (GUILayout.Button("Tạo lại bù nhìn")) ApplyDummies();
        GUILayout.EndHorizontal();

        GUILayout.Label("Skill Q");
        int starter = GUILayout.SelectionGrid((int)loadout.StarterSkill, StarterNames, 3);
        if (starter != (int)loadout.StarterSkill)
        {
            loadout.SelectStarter((MageStarterSkill)starter);
            combat.ResetPreviewStarter();
        }

        GUILayout.Label("Nhánh đánh tự động");
        int element = GUILayout.SelectionGrid((int)loadout.Element, ElementNames, 4);
        if (element != (int)loadout.Element)
        {
            loadout.SelectElement((MageElement)element);
            combat.ResetPreviewElement();
        }

        GUILayout.Label("Ultimate E");
        int selectedUltimate = GUILayout.SelectionGrid((int)loadout.Ultimate, UltimateNames, 3);
        if (selectedUltimate != (int)loadout.Ultimate)
        {
            ultimate.ResetPreviewUltimate();
            loadout.SelectUltimate((MageUltimateKind)selectedUltimate);
        }

        GUILayout.Space(5f);
        GUILayout.Label("Nâng cấp nhánh (0-3)");
        int branch = (int)loadout.Element;
        for (int i = 0; i < 3; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(TierNames[branch][i], GUILayout.Width(125f));
            if (GUILayout.Button("−", GUILayout.Width(28f))) loadout.SetTier(i, loadout.GetTier(i) - 1);
            GUILayout.Label(loadout.GetTier(i).ToString(), GUILayout.Width(18f));
            if (GUILayout.Button("+", GUILayout.Width(28f))) loadout.SetTier(i, loadout.GetTier(i) + 1);
            GUILayout.EndHorizontal();
        }

        GUILayout.Label("Bốn lõi của nhánh");
        for (int i = 0; i < 4; i++)
        {
            bool selected = GUILayout.Toggle(loadout.GetCore(i), CoreNames[branch][i]);
            if (selected != loadout.GetCore(i)) loadout.SetCore(i, selected);
        }

        bool safe = GUILayout.Toggle(invulnerableForTesting, "Bất tử khi thử skill");
        if (safe != invulnerableForTesting)
        {
            invulnerableForTesting = safe;
            health.SetDamageEnabled(!safe);
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void RestartPreview()
    {
        if (restarting) return;
        restarting = true;
        StartCoroutine(RestartAfterFrame());
    }

    private void ApplyDummies()
    {
        if (dummyPrefab == null || dummyRoot == null || mage == null) return;
        if (!int.TryParse(dummyCount, out int count) ||
            !int.TryParse(dummyHealth, out int hp) ||
            !float.TryParse(dummyRadius.Replace(',', '.'), NumberStyles.Float,
                CultureInfo.InvariantCulture, out float radius)) return;
        count = Mathf.Clamp(count, 1, 30);
        hp = Mathf.Clamp(hp, 1, 10000);
        radius = Mathf.Clamp(radius, 1f, 6f);
        dummyCount = count.ToString();
        dummyHealth = hp.ToString();
        dummyRadius = radius.ToString("0.0", CultureInfo.InvariantCulture);

        foreach (Transform child in dummyRoot) Destroy(child.gameObject);
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 position = mage.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            EnemyHealth dummy = Instantiate(dummyPrefab, position, Quaternion.identity, dummyRoot);
            dummy.name = "Bù nhìn " + (i + 1);
            dummy.InitializeHealth(hp);
            EnemyController controller = dummy.GetComponent<EnemyController>();
            if (controller != null) controller.SetTrainingDummy(true);
            EnemyContactDamage contact = dummy.GetComponent<EnemyContactDamage>();
            if (contact != null) contact.enabled = false;
        }
    }

    private IEnumerator RestartAfterFrame()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
            "Assets/Scenes/Magical.unity",
            new LoadSceneParameters(LoadSceneMode.Single));
#else
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
    }
}
