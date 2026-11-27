using UnityEngine;
using UnityEngine.InputSystem;

internal enum PracticeTargetMode { Dummies, BossDummy, AttackingBoss }

public sealed class GunSkillTestArena : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Material spriteMaterial;
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite echoGunSprite;
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private Texture2D pistolShots;
    [SerializeField] private Texture2D shotgunMuzzle;
    [SerializeField] private Texture2D smgMuzzle;
    [SerializeField] private Texture2D plasmaShot;
    [SerializeField] private Texture2D echoLaser;
    [SerializeField] private Texture2D iceLance;
    [SerializeField] private Texture2D frostBreath;
    [SerializeField] private Texture2D gunTracer;
    [SerializeField] private Texture2D cannonballMuzzle;
    [SerializeField] private Sprite piercingSprite;
    [SerializeField] private Texture2D[] coreTwoSheets;
    [SerializeField] private Texture2D[] ultimateSheets;
    [SerializeField] private Texture2D grenadeAtlas;
    [SerializeField] private Texture2D meteorAtlas;
    [SerializeField] private Texture2D[] updatedSkillSheets;
    [SerializeField] private Texture2D[] pixelSkillSheets;
    [SerializeField] private Material skillNoGlowMaterial;

    private const float TargetRespawnDelay = 0.75f;
    private const float PlayerRetryDelay = 1.5f;
    private static readonly Vector2 PlayerStart = new Vector2(-5f, 0f);
    private static readonly Vector2[] TargetPositions =
    {
        new Vector2(0f, 0f), new Vector2(2f, 0f), new Vector2(4f, 0f), new Vector2(6f, 0f),
        new Vector2(0.65f, 1.1f), new Vector2(0.8f, -1.35f),
        new Vector2(3.6f, 2.3f), new Vector2(3.6f, -2.3f)
    };

    private static readonly string[] SkillNames = { "", "Đạn nảy", "Xuyên thấu", "Đạn bung toả" };
    private static readonly string[] BranchNames = { "Cơ bản", "Shotgun", "SMG", "Rocket" };
    private static readonly string[,] UpgradeNames =
    {
        { "", "", "" },
        { "Thêm viên", "Đạn hẹp", "Đẩy lùi" },
        { "Đạn băng", "Ném lựu đạn", "Thêm đạn" },
        { "Bùng nổ", "Đẩy lùi", "Thêm đạn" }
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
        { "Tắt", "Lưỡi đạn càn quét", "Áp lực công phá", "Phun lửa" },
        { "Tắt", "Kíp nổ liên hoàn", "Băng đạn cộng hưởng", "Hồi máu" },
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
        "", "Lốc lửa", "Mưa thiên thạch", "Pháo đài cơ động",
        "Lồng đạn Hexcore"
    };
    private GunHexcoreUltimate ultimate;
    private HexcoreUltimate selectedUltimate;
    private Vector2 lastAimPoint;
    private bool bossMode;
    private bool hardBoss;
    private int practiceHealth;
    private float? playerRetryAt;
    private PlayerHealth playerHealth;

    internal bool PlayerStanding => playerHealth != null ? playerHealth.CurrentHealth > 0 : practiceHealth > 0;
    internal int IntegratedHealth => playerHealth != null ? playerHealth.CurrentHealth : practiceHealth;
    internal int IntegratedMaximumHealth => playerHealth != null ? playerHealth.MaxHealth : 100;
    internal EnemyHealth[] IntegratedTargets => targets;
    internal Bounds IntegratedPlayerBounds
    {
        get
        {
            Collider2D playerCollider = player != null ? player.GetComponent<Collider2D>() : null;
            return playerCollider != null
                ? playerCollider.bounds
                : new Bounds(player != null ? (Vector3)player.position : Vector3.zero, Vector3.one * 0.5f);
        }
    }
    internal PlayerHealth IntegratedPlayerHealth => playerHealth;

    internal void SetIntegratedCoreStats(CharacterProgressionTest stats)
    {
        if (!Integrated) return;
        player.GetComponent<PlayerController>().LevelUpSpeedMultiplier = stats.MoveSpeedMultiplier;
        weapon.SetIntegratedCoreStats(stats);
        ultimate.SetDamageOwner(weapon);
    }

    internal void AddIntegratedBonusHealth(int amount)
    {
        if (!Integrated || !PlayerStanding || amount <= 0) return;
        if (playerHealth != null) playerHealth.IncreaseMaximumHealthAndHeal(amount);
        else practiceHealth += amount;
    }

    internal void RestorePracticeHealth(int amount)
    {
        if (!PlayerStanding || amount <= 0) return;
        if (playerHealth != null) playerHealth.Heal(amount);
        else practiceHealth = Mathf.Min(100, practiceHealth + amount);
    }

    internal void ReceiveBossAttack(Vector2 source, int damage)
    {
        if (!hardBoss || !PlayerStanding) return;
        if (coreTwo.Block(source)) return;
        if (playerHealth != null) playerHealth.TakeDamage(damage);
        else practiceHealth = Mathf.Max(0, practiceHealth - Mathf.Max(0, damage));
        weapon.ShowImpact(player.position, new Color(1f, 0.32f, 0.22f));
        if (!PlayerStanding)
        {
            weapon.ResetShots();
            ultimate.ResetEffects();
            if (!Integrated) playerRetryAt = Time.time + PlayerRetryDelay;
        }
    }
    internal bool Integrated { get; set; }

    internal void SetIntegratedBranch(int value) => branch = (GunBranch)(value + 1);
    internal void SetIntegratedPassive(int value) => skill = (GunSkill)(value + 1);
    internal static string BranchChoice(int i) => BranchNames[i + 1];
    internal static string PassiveChoice(int i) => SkillNames[i + 1];
    internal static string FirstCoreChoice(int b, int i) => Level11Names[b + 1, i + 1];
    internal static string SecondCoreChoice(int b, int i) => CoreTwoNames[b + 1, i];
    internal static string UltimateChoice(int i) => UltimateNames[i + 1];

    internal bool UseIntegratedSkill(int slot, int choice)
    {
        if (!PlayerStanding || branch == GunBranch.Basic) return false;
        if (slot == 2) return ultimate.Cast((HexcoreUltimate)(choice + 1), lastAimPoint);
        EnemyHealth target = null;
        float best = 5f * 5f;
        foreach (EnemyHealth enemy in targets)
        {
            if (enemy == null || enemy.CurrentHealth <= 0) continue;
            float distance = ((Vector2)enemy.transform.position - lastAimPoint).sqrMagnitude;
            if (distance > best) continue;
            target = enemy;
            best = distance;
        }
        return slot == 0
            ? level11.TryIntegratedCast(branch, choice, target, weapon.MuzzlePosition, weaponAim.transform.right)
            : coreTwo.TryIntegratedCast(branch, choice, target, weaponAim.transform.right);
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
        if (Integrated)
        {
            cameraObject.transform.SetParent(transform, true);
            playerObject.transform.SetParent(transform, true);
        }
        player = playerObject.GetComponent<Rigidbody2D>();
        playerHealth = playerObject.GetComponent<PlayerHealth>();
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
        if (Integrated)
        {
            targetRoot.SetParent(transform, true);
            effectsRoot.SetParent(transform, true);
        }
        weapon = gun.AddComponent<GunSkillWeapon>();
        weapon.Setup(muzzle, effectsRoot, spriteMaterial, bulletSprite, piercingSprite);
        weapon.SetPistolShots(pistolShots);
        weapon.SetBranchArt(shotgunMuzzle, smgMuzzle, gunTracer);
        weapon.SetEchoArt(echoLaser);
        weapon.SetPlasmaArt(plasmaShot);
        var importedArt = gameObject.AddComponent<GunNewSkillArt>();
        importedArt.Setup(updatedSkillSheets, pixelSkillSheets, skillNoGlowMaterial);
        weapon.SetRocketArt(cannonballMuzzle, importedArt);
        level11 = gameObject.AddComponent<GunLevel11>();
        level11.Setup(weapon, player.transform, effectsRoot, targets, importedArt, echoGunSprite);
        weapon.SetLevel11(level11);
        coreTwo = gameObject.AddComponent<GunCoreTwo>();
        coreTwo.Setup(weapon, player.transform, muzzle, spriteMaterial, coreTwoSheets, importedArt);
        weapon.SetCoreTwo(coreTwo);
        ultimate = gameObject.AddComponent<GunHexcoreUltimate>();
        ultimate.Setup(muzzle, spriteMaterial, targets, ultimateSheets, grenadeAtlas, meteorAtlas, importedArt);
        weapon.SetSmgUpgradeArt(iceLance, frostBreath, ultimate);
        ResetTargets();
    }

    private void LateUpdate()
    {
        if (Integrated && Time.timeScale == 0f) return;
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
        if (!Integrated)
        {
            level11.Select(branch, level11ByBranch[(int)branch], ranksByBranch[(int)branch], bossMode);
            coreTwo.Select(branch, coreTwoByBranch[(int)branch], ranksByBranch[(int)branch], bossMode ? targets[0] : null);
            if (PlayerStanding && Keyboard.current?.rKey.wasPressedThisFrame == true)
                ultimate.Cast(selectedUltimate, lastAimPoint);
        }
        weapon.Tick(autoFire && (!hardBoss || PlayerStanding), lastAimPoint);
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
        float playWidth = Integrated ? Screen.width : ControlPanelRect().x;
        labCamera.rect = Integrated ? new Rect(0f, 0f, 1f, 1f) : new Rect(0f, 0f, playWidth / Mathf.Max(1f, Screen.width), 1f);
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
        if (playerHealth != null) playerHealth.RestoreFullHealth();
        else practiceHealth = 100;
        playerRetryAt = null;
        weapon.ResetShots();
        respawnAt = null;
        SpawnTargets();
    }

    internal void ConfigureIntegratedTargets(PracticeTargetMode mode, bool moving, bool sturdy)
    {
        bossMode = mode != PracticeTargetMode.Dummies;
        hardBoss = mode == PracticeTargetMode.AttackingBoss;
        movingTargets = moving;
        sturdyTargets = sturdy;
        ResetTargets();
    }

    private void SpawnTargets()
    {
        var mode = hardBoss ? PracticeTargetMode.AttackingBoss : bossMode ? PracticeTargetMode.BossDummy : PracticeTargetMode.Dummies;
        SpawnPracticeEnemies(targets, targetRoot, player.transform, Vector2.zero, mode,
            movingTargets, sturdyTargets, false, () => PlayerStanding, ReceiveBossAttack);
    }

    internal void SpawnPracticeEnemies(EnemyHealth[] population, Transform root, Transform target, Vector2 offset,
        PracticeTargetMode mode, bool moving, bool sturdy, bool useMageMovement,
        System.Func<bool> targetAlive, System.Action<Vector2, int> receiveAttack)
    {
        System.Array.Clear(population, 0, population.Length);
        bool isBoss = mode != PracticeTargetMode.Dummies;
        bool attacks = mode == PracticeTargetMode.AttackingBoss;
        int count = isBoss ? 1 : Mathf.Min(population.Length, TargetPositions.Length);
        for (int i = 0; i < count; i++)
        {
            var enemy = Instantiate(enemyPrefab, TargetPositions[i] + offset, Quaternion.identity, root);
            enemy.name = attacks ? "Core two boss" : isBoss ? "Boss dummy" : "Target " + (i + 1);
            if (isBoss) enemy.transform.localScale *= 1.45f;
            var movement = enemy.GetComponent<EnemyController>();
            movement.SetTarget(target);
            movement.enabled = !attacks && (moving || useMageMovement);
            if (mode == PracticeTargetMode.Dummies && !moving) movement.SetTrainingDummy(true);
            if (attacks) movement.SetMoveSpeed(2.7f);
            enemy.AddComponent<GunSkillTargetEffects>().IgnorePush = isBoss;
            if (isBoss && useMageMovement) enemy.AddComponent<MageBossMarker>();
            if (enemy.TryGetComponent<EnemyMeleeAttack>(out var melee))
            {
                Destroy(melee);
                // EnemyMeleeAttack sets a 1.2-unit stopping distance in Awake.
                // Practice enemies have no melee attack, so let movers reach contact range.
                if (!attacks && (moving || useMageMovement)) movement.SetStopDistance(0f);
            }
            foreach (var renderer in enemy.GetComponentsInChildren<SpriteRenderer>())
                renderer.sharedMaterial = spriteMaterial;
            population[i] = enemy.GetComponent<EnemyHealth>();
            population[i].InitializeHealth(attacks ? 2500 : isBoss ? 1500 : sturdy ? 100 : i == 0 ? 50 : 100);
            if (attacks)
                enemy.AddComponent<GunLabBoss>().Setup(target, bulletSprite, spriteMaterial, targetAlive, receiveAttack, offset);
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
        if (Integrated) return;
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
                : $"Người chơi · {IntegratedHealth}/100 HP";
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
        Vector2 aim = ultimate.GroundAim(lastAimPoint);
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
        else if (selectedUltimate == HexcoreUltimate.Tornado && ultimate.TornadoCharges == 0)
            status = "Hết lượt Lốc lửa";
        else if (ultimate.CooldownLeft > 0f)
            status = $"Hồi chiêu · {ultimate.CooldownLeft:0.0}s";
        else
            status = "R · Tung chiêu";

        if (selectedUltimate == HexcoreUltimate.Turret)
            status += $" ({ultimate.TurretCount}/{GunHexcoreUltimate.TurretLimit} trụ)";
        if (selectedUltimate == HexcoreUltimate.Tornado)
        {
            status += $" · Lốc {ultimate.TornadoCharges}/{GunHexcoreUltimate.TornadoCapacity}";
            if (ultimate.TornadoRechargeLeft > 0f)
                status += $" · Hồi 1 lượt sau {ultimate.TornadoRechargeLeft:0.0}s";
        }
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
