using UnityEngine;
using UnityEngine.InputSystem;

public sealed class GunSkillTestArena : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Material spriteMaterial;
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private Sprite piercingSprite;
    [SerializeField] private Sprite shockwaveSprite;
    [SerializeField] private Sprite[] flameFrames;
    [SerializeField] private Texture2D[] level11Sheets;
    [SerializeField] private Texture2D[] coreTwoSheets;
    [SerializeField] private Texture2D[] ultimateSheets;
    [SerializeField] private Texture2D turretAtlas;

    private const float TargetRespawnDelay = 0.75f;
    private const float PlayerRetryDelay = 1.5f;
    private static readonly Vector2 PlayerStart = new Vector2(-5f, 0f);
    private static readonly Vector2[] TargetPositions =
    {
        new Vector2(0f, 0f), new Vector2(2f, 0f), new Vector2(4f, 0f), new Vector2(6f, 0f),
        new Vector2(0.65f, 1.1f), new Vector2(0.8f, -1.35f),
        new Vector2(3.6f, 2.3f), new Vector2(3.6f, -2.3f)
    };

    private static readonly string[] SkillNames = { "", "Đạn nảy", "Ngắm điểm yếu", "Đạn bung toả" };
    private static readonly string[] BranchNames = { "Cơ bản", "Đạn toả", "Liên thanh", "Xuyên thấu" };
    private static readonly string[,] UpgradeNames =
    {
        { "", "", "" },
        { "Thêm viên", "Đạn hẹp", "Đẩy lùi" },
        { "Thêm viên", "Đạn nảy", "Bung toả" },
        { "Tăng sát thương xuyên", "Dấu xuyên", "Dư chấn" }
    };
    private readonly EnemyHealth[] targets = new EnemyHealth[TargetPositions.Length];
    private Camera labCamera;
    private WeaponAim weaponAim;
    private Vector2 panelScroll;
    private GUIStyle buttonStyle;
    private bool restartRequested;
    private Rigidbody2D player;
    private Transform targetRoot;
    private Transform effectsRoot;
    private GunSkillWeapon weapon;
    private GunSkill skill;
    private GunBranch branch;
    private readonly Vector3Int[] ranksByBranch = new Vector3Int[4];
    private static readonly string[,] Level11Names =
    {
        { "", "", "", "" },
        { "Tắt", "Lưỡi đạn càn quét", "Áp lực công phá", "Phát bắn dồn lực" },
        { "Tắt", "Kíp nổ liên hoàn", "Băng đạn cộng hưởng", "Bão đạn quỹ đạo" },
        { "Tắt", "Vết rách không gian", "Xuyên kích hồi quy", "Thiên thương" }
    };
    private readonly int[] level11ByBranch = new int[4];
    private GunLevel11 level11;
    private static readonly string[,] CoreTwoNames =
    {
        { "", "" },
        { "Giáp phản pháo", "Lưỡi cưa bám đuổi" },
        { "Hoả lực leo thang", "Đạn phân rã" },
        { "Cổng xuyên kích", "Mỏ neo xuyên tâm" }
    };
    private readonly int[] coreTwoByBranch = new int[4];
    private GunCoreTwo coreTwo;
    private static readonly string[] UltimateNames =
    {
        "", "Ngũ bom liên hoàn", "Trực thăng oanh kích", "Pháo đài cơ động",
        "Pháo ray Hexcore", "Lồng đạn Hexcore"
    };
    private GunHexcoreUltimate ultimate;
    private HexcoreUltimate selectedUltimate;
    private Vector2 lastAimPoint;
    private bool bossMode;
    private bool hardBoss;
    private int practiceHealth;
    private float nextBossDamage;
    private float? playerRetryAt;

    internal bool PlayerStanding => practiceHealth > 0;

    internal void ReceiveBossAttack(Vector2 source, int damage)
    {
        if (!hardBoss || !PlayerStanding || Time.time < nextBossDamage) return;
        nextBossDamage = Time.time + 0.58f;
        if (coreTwo.Block(source)) return;
        practiceHealth = Mathf.Max(0, practiceHealth - damage);
        weapon.ShowImpact(player.position, new Color(1f, 0.32f, 0.22f));
        if (!PlayerStanding)
        {
            weapon.ResetShots();
            ultimate.ResetEffects();
            playerRetryAt = Time.time + PlayerRetryDelay;
        }
    }
    private bool movingTargets;
    private bool autoFire = true;
    private bool sturdyTargets;
    private float? respawnAt;

    private void Awake()
    {
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        labCamera = cameraObject.AddComponent<Camera>();
        labCamera.orthographic = true;
        labCamera.clearFlags = CameraClearFlags.SolidColor;
        labCamera.backgroundColor = new Color(0.09f, 0.11f, 0.16f);
        var playerObject = Instantiate(playerPrefab, PlayerStart, Quaternion.identity);
        playerObject.name = "Player";
        player = playerObject.GetComponent<Rigidbody2D>();
        FrameCamera();

        var gun = new GameObject("Gun");
        gun.transform.SetParent(player.transform, false);
        gun.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        var visual = new GameObject("Gun art");
        visual.transform.SetParent(gun.transform, false);
        var renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = gunSprite;
        renderer.sharedMaterial = spriteMaterial;
        renderer.sortingOrder = 3;
        visual.transform.localPosition = new Vector3(0.24f, 0f, 0f);
        float scale = 0.55f / renderer.sprite.bounds.size.x;
        visual.transform.localScale = new Vector3(scale, scale, 1f);
        var muzzle = new GameObject("FirePoint").transform;
        muzzle.SetParent(gun.transform, false);
        muzzle.localPosition = new Vector3(0.56f, 0f, 0f);
        weaponAim = gun.AddComponent<WeaponAim>();
        weaponAim.AimFromArena = true;

        targetRoot = new GameObject("Practice targets").transform;
        effectsRoot = new GameObject("Shots and effects").transform;
        weapon = gun.AddComponent<GunSkillWeapon>();
        weapon.Setup(muzzle, effectsRoot, spriteMaterial, bulletSprite, piercingSprite, shockwaveSprite, flameFrames);
        level11 = gameObject.AddComponent<GunLevel11>();
        level11.Setup(weapon, player.transform, effectsRoot, targets, level11Sheets);
        weapon.SetLevel11(level11);
        coreTwo = gameObject.AddComponent<GunCoreTwo>();
        coreTwo.Setup(weapon, player.transform, muzzle, spriteMaterial, coreTwoSheets);
        weapon.SetCoreTwo(coreTwo);
        ultimate = gameObject.AddComponent<GunHexcoreUltimate>();
        ultimate.Setup(weapon, player.transform, muzzle, spriteMaterial, targets, ultimateSheets, turretAtlas);
        ResetTargets();
    }

    private void LateUpdate()
    {
        if (restartRequested || (playerRetryAt.HasValue && Time.time >= playerRetryAt.Value))
        {
            restartRequested = false;
            RestartPractice();
        }

        FrameCamera();
        if (weaponAim.isActiveAndEnabled && Mouse.current != null)
        {
            Vector2 pointer = Mouse.current.position.ReadValue();
            if (labCamera.pixelRect.Contains(pointer))
            {
                weaponAim.RotateTowardsMouse();
                lastAimPoint = labCamera.ScreenToWorldPoint(new Vector3(pointer.x, pointer.y, -labCamera.transform.position.z));
            }
        }

        if (playerRetryAt.HasValue) return;
        RespawnWhenCleared();
        weapon.Select(skill, branch, ranksByBranch[(int)branch]);
        level11.Select(branch, level11ByBranch[(int)branch], ranksByBranch[(int)branch], bossMode);
        coreTwo.Select(branch, coreTwoByBranch[(int)branch], ranksByBranch[(int)branch], bossMode ? targets[0] : null);
        if (PlayerStanding && Keyboard.current?.rKey.wasPressedThisFrame == true)
            ultimate.Cast(selectedUltimate, lastAimPoint, bossMode);
        weapon.Tick(autoFire && (!hardBoss || PlayerStanding));
    }

    private void RespawnWhenCleared()
    {
        foreach (EnemyHealth target in targets)
        {
            if (target != null && target.CurrentHealth > 0) return;
        }

        respawnAt ??= Time.time + TargetRespawnDelay;
        if (Time.time < respawnAt.Value) return;

        respawnAt = null;
        level11.ResetEffects(false);
        SpawnTargets();
    }

    private void SelectTargets(bool useBoss, bool useHardBoss)
    {
        if (bossMode == useBoss && hardBoss == useHardBoss) return;
        bossMode = useBoss;
        hardBoss = useHardBoss;
        restartRequested = true;
    }

    private void RestartPractice()
    {
        player.linearVelocity = Vector2.zero;
        player.position = PlayerStart;
        ResetTargets();
    }

    private static Rect ControlPanelRect()
    {
        float width = Mathf.Min(330f, Screen.width * 0.40f);
        return new Rect(Screen.width - width, 0f, width, Screen.height);
    }

    private void FrameCamera()
    {
        Vector3 center = player.transform.position;
        center.z = -10f;
        labCamera.transform.position = center;
        float playWidth = ControlPanelRect().x;
        labCamera.rect = new Rect(0f, 0f, playWidth / Mathf.Max(1f, Screen.width), 1f);
        float playAspect = playWidth / Mathf.Max(1f, Screen.height);
        labCamera.orthographicSize = Mathf.Max(5f, 6.5f / Mathf.Max(0.1f, playAspect));
    }

    private void ResetTargets()
    {
        ClearChildren(targetRoot);
        ClearChildren(effectsRoot);
        level11.ResetEffects();
        coreTwo.ResetEffects();
        ultimate.ResetEffects();
        lastAimPoint = (Vector2)player.position + Vector2.right * 5f;
        practiceHealth = 100;
        playerRetryAt = null;
        nextBossDamage = 0f;
        weapon.ResetShots();
        respawnAt = null;
        SpawnTargets();
    }

    private void SpawnTargets()
    {
        System.Array.Clear(targets, 0, targets.Length);
        int count = bossMode ? 1 : targets.Length;
        for (int i = 0; i < count; i++)
        {
            var enemy = Instantiate(enemyPrefab, TargetPositions[i], Quaternion.identity, targetRoot);
            enemy.name = hardBoss ? "Core two boss" : bossMode ? "Boss dummy" : "Target " + (i + 1);
            if (bossMode) enemy.transform.localScale *= 1.45f;
            var movement = enemy.GetComponent<EnemyController>();
            movement.SetTarget(player.transform);
            movement.enabled = movingTargets && !hardBoss;
            if (hardBoss) movement.SetMoveSpeed(2.7f);
            enemy.AddComponent<GunSkillTargetEffects>().IgnorePush = bossMode;
            enemy.GetComponent<EnemyContactDamage>().enabled = false;
            // Disabled MonoBehaviours still receive trigger callbacks in Unity.
            Destroy(enemy.GetComponent<EnemyContactDamage>());
            foreach (var renderer in enemy.GetComponentsInChildren<SpriteRenderer>())
                renderer.sharedMaterial = spriteMaterial;
            targets[i] = enemy.GetComponent<EnemyHealth>();
            targets[i].InitializeHealth(hardBoss ? 2500 : bossMode ? 1500 : sturdyTargets ? 100 : i == 0 ? 10 : 30);
            if (hardBoss) enemy.AddComponent<GunLabBoss>().Setup(this, player, bulletSprite, spriteMaterial);
        }
    }

    private static void ClearChildren(Transform root)
    {
        foreach (Transform child in root)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    private void OnGUI()
    {
        DrawTargetHealth();
        DrawUltimateAim();

        Rect panel = ControlPanelRect();
        Color original = GUI.color;
        GUI.color = new Color(0.095f, 0.12f, 0.18f);
        GUI.DrawTexture(panel, Texture2D.whiteTexture);
        GUI.color = original;
        buttonStyle ??= new GUIStyle(GUI.skin.button) { wordWrap = true, alignment = TextAnchor.MiddleLeft };
        GUILayout.BeginArea(new Rect(panel.x + 8f, 8f, panel.width - 16f, panel.height - 16f));
        GUILayout.Label("Sân thử súng", GUI.skin.box);
        if (hardBoss)
        {
            string status = playerRetryAt.HasValue
                ? $"Thử lại sau {Mathf.Max(0f, playerRetryAt.Value - Time.time):0.0}s"
                : $"Người chơi · {practiceHealth}/100 HP";
            GUILayout.Label(status);
        }

        panelScroll = GUILayout.BeginScrollView(panelScroll, false, true);
        DrawUltimateControls();
        DrawWeaponControls();
        DrawPracticeControls();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawTargetHealth()
    {
        foreach (EnemyHealth target in targets)
        {
            if (target == null || target.CurrentHealth <= 0) continue;
            Vector3 screen = labCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 0.55f);
            if (!labCamera.pixelRect.Contains(new Vector2(screen.x, screen.y))) continue;
            GUI.Label(new Rect(screen.x - 25f, Screen.height - screen.y, 100f, 22f), $"{target.CurrentHealth} HP");
        }
    }

    private void DrawUltimateAim()
    {
        if (selectedUltimate == HexcoreUltimate.None || selectedUltimate == HexcoreUltimate.Turret) return;
        Vector2 aim = selectedUltimate == HexcoreUltimate.Rail ? lastAimPoint : ultimate.GroundAim(lastAimPoint);
        Vector3 point = labCamera.WorldToScreenPoint(aim);
        if (!labCamera.pixelRect.Contains(new Vector2(point.x, point.y))) return;

        float x = point.x;
        float y = Screen.height - point.y;
        Color old = GUI.color;
        GUI.color = new Color(0.43f, 0.92f, 1f, 0.7f);
        GUI.DrawTexture(new Rect(x - 9f, y, 6f, 1f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(x + 4f, y, 6f, 1f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(x, y - 9f, 1f, 6f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(x, y + 4f, 1f, 6f), Texture2D.whiteTexture);
        GUI.color = old;
    }

    private void DrawUltimateControls()
    {
        GUILayout.Label("Lõi Hexcore · chọn một", GUI.skin.box);
        for (int i = 1; i < UltimateNames.Length; i++)
        {
            var choice = (HexcoreUltimate)i;
            if (StateButton(UltimateNames[i], selectedUltimate == choice, true))
                selectedUltimate = selectedUltimate == choice ? HexcoreUltimate.None : choice;
        }

        string status;
        if (selectedUltimate == HexcoreUltimate.None)
            status = "Chọn chiêu cuối để dùng phím R";
        else if (!PlayerStanding)
            status = "Chờ sân đặt lại";
        else if (selectedUltimate == HexcoreUltimate.Turret && ultimate.TurretCount >= GunHexcoreUltimate.TurretLimit)
            status = $"Đủ {GunHexcoreUltimate.TurretLimit} trụ";
        else if (ultimate.CooldownLeft > 0f)
            status = $"Hồi chiêu · {ultimate.CooldownLeft:0.0}s";
        else
            status = "R · Tung chiêu";

        if (selectedUltimate == HexcoreUltimate.Turret)
            status += $" ({ultimate.TurretCount}/{GunHexcoreUltimate.TurretLimit} trụ)";
        GUILayout.Label(status, GUI.skin.box);
    }

    private void DrawWeaponControls()
    {
        // Keep the same control layout for the whole IMGUI event when a button changes the branch.
        GunBranch shownBranch = branch;
        GUILayout.Label("Nhánh súng", GUI.skin.box);
        for (int i = 0; i < BranchNames.Length; i++)
            if (StateButton(BranchNames[i], shownBranch == (GunBranch)i, true))
                branch = (GunBranch)i;

        GUILayout.Label("Kỹ năng cơ bản", GUI.skin.box);
        for (int i = 1; i < SkillNames.Length; i++)
            if (StateButton(SkillNames[i], skill == (GunSkill)i))
                skill = skill == (GunSkill)i ? GunSkill.None : (GunSkill)i;

        if (shownBranch == GunBranch.Basic) return;

        int index = (int)shownBranch;
        GUILayout.Label("Nâng cấp nhánh", GUI.skin.box);
        var ranks = ranksByBranch[index];
        for (int i = 0; i < 3; i++)
            if (PanelButton($"{UpgradeNames[index, i]} · {ranks[i]}/3", ranks[i] > 0))
                ranks[i] = (ranks[i] + 1) % 4;
        ranksByBranch[index] = ranks;

        GUILayout.Label("Lõi đầu · chọn một", GUI.skin.box);
        for (int i = 1; i <= 3; i++)
            if (StateButton(Level11Names[index, i], level11ByBranch[index] == i))
                level11ByBranch[index] = level11ByBranch[index] == i ? 0 : i;

        GUILayout.Label("Lõi hai · bật độc lập", GUI.skin.box);
        for (int i = 0; i < 2; i++)
        {
            int bit = 1 << i;
            if (StateButton(CoreTwoNames[index, i], (coreTwoByBranch[index] & bit) != 0))
                coreTwoByBranch[index] ^= bit;
        }
    }

    private void DrawPracticeControls()
    {
        GUILayout.Label("Mục tiêu và sân thử", GUI.skin.box);
        if (StateButton("Bia thường", !bossMode, true)) SelectTargets(false, false);
        if (StateButton("Bia boss · 1.500 HP", bossMode && !hardBoss, true)) SelectTargets(true, false);
        if (StateButton("Boss tấn công · 2.500 HP", hardBoss, true)) SelectTargets(true, true);

        if (StateButton("Tự bắn", autoFire))
        {
            autoFire = !autoFire;
            if (!autoFire) weapon.ResetShots();
        }

        bool previousEnabled = GUI.enabled;
        GUI.enabled = previousEnabled && !hardBoss;
        if (StateButton("Bia di chuyển", movingTargets && !hardBoss))
        {
            movingTargets = !movingTargets;
            foreach (EnemyHealth target in targets)
                if (target != null) target.GetComponent<EnemyController>().enabled = movingTargets;
        }

        GUI.enabled = previousEnabled && !bossMode;
        if (StateButton("Bia 100 HP · lượt sau", sturdyTargets && !bossMode))
            sturdyTargets = !sturdyTargets;
        GUI.enabled = previousEnabled;
        if (PanelButton("Đặt lại sân", false)) restartRequested = true;
    }

    private bool StateButton(string name, bool active, bool exclusive = false)
    {
        string state = exclusive ? (active ? "Đã chọn" : "Chọn") : (active ? "Bật" : "Tắt");
        return PanelButton($"{state} · {name}", active);
    }

    private bool PanelButton(string label, bool active)
    {
        Color previous = GUI.backgroundColor;
        GUI.backgroundColor = active ? new Color(0.49f, 0.82f, 0.72f) : Color.white;
        bool clicked = GUILayout.Button(label, buttonStyle, GUILayout.MinHeight(29f));
        GUI.backgroundColor = previous;
        return clicked;
    }
}
