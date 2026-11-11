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

    private const float TargetRespawnDelay = 0.75f;
    private static readonly Vector2 PlayerStart = new Vector2(-5f, 0f);
    private static readonly Vector2[] TargetPositions =
    {
        new Vector2(0f, 0f), new Vector2(2f, 0f), new Vector2(4f, 0f), new Vector2(6f, 0f),
        new Vector2(0.65f, 1.1f), new Vector2(0.8f, -1.35f),
        new Vector2(3.6f, 2.3f), new Vector2(3.6f, -2.3f)
    };

    private static readonly string[] SkillNames = { "None", "Ricochet", "Weak point", "Death scatter" };
    private readonly EnemyHealth[] targets = new EnemyHealth[TargetPositions.Length];
    private Camera labCamera;
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
    private bool bossMode;
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
        FrameCamera();

        var playerObject = Instantiate(playerPrefab, PlayerStart, Quaternion.identity);
        playerObject.name = "Player";
        player = playerObject.GetComponent<Rigidbody2D>();

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
        // WeaponAim still handles mouse aim. This gun has no legacy bullet assigned.
        gun.AddComponent<WeaponAim>();

        targetRoot = new GameObject("Practice targets").transform;
        effectsRoot = new GameObject("Shots and effects").transform;
        weapon = gun.AddComponent<GunSkillWeapon>();
        weapon.Setup(muzzle, effectsRoot, spriteMaterial, bulletSprite, piercingSprite, shockwaveSprite, flameFrames);
        level11 = gameObject.AddComponent<GunLevel11>();
        level11.Setup(weapon, player.transform, effectsRoot, targets, level11Sheets);
        weapon.SetLevel11(level11);
        ResetTargets();
    }

    private void LateUpdate()
    {
        FrameCamera();
        var keyboard = Keyboard.current;
        if (keyboard != null) ReadNumberKeys(keyboard);

        RespawnWhenCleared();
        weapon.Select(skill, branch, ranksByBranch[(int)branch]);
        level11.Select(branch, level11ByBranch[(int)branch], ranksByBranch[(int)branch], bossMode);
        weapon.Tick(autoFire);
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

    private void ReadNumberKeys(Keyboard keyboard)
    {
        bool shift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        int selected = Pressed(keyboard, Key.Digit1, Key.Numpad1) ? 1 :
            Pressed(keyboard, Key.Digit2, Key.Numpad2) ? 2 :
            Pressed(keyboard, Key.Digit3, Key.Numpad3) ? 3 : 0;
        if (!shift && selected != 0)
            skill = skill == (GunSkill)selected ? GunSkill.None : (GunSkill)selected;

        if (Pressed(keyboard, Key.Digit4, Key.Numpad4))
            branch = branch == GunBranch.Spread ? GunBranch.Basic : GunBranch.Spread;
        else if (Pressed(keyboard, Key.Digit5, Key.Numpad5))
            branch = branch == GunBranch.Burst ? GunBranch.Basic : GunBranch.Burst;
        else if (Pressed(keyboard, Key.Digit6, Key.Numpad6))
            branch = branch == GunBranch.Piercing ? GunBranch.Basic : GunBranch.Piercing;

        bool zero = Pressed(keyboard, Key.Digit0, Key.Numpad0);
        bool eight = Pressed(keyboard, Key.Digit8, Key.Numpad8);
        bool nine = Pressed(keyboard, Key.Digit9, Key.Numpad9);
        if (shift)
        {
            if (branch != GunBranch.Basic && selected != 0)
            {
                int active = level11ByBranch[(int)branch];
                level11ByBranch[(int)branch] = active == selected ? 0 : selected;
            }
            if (Pressed(keyboard, Key.Digit7, Key.Numpad7))
            {
                bossMode = !bossMode;
                player.position = PlayerStart;
                ResetTargets();
            }
            if (eight) autoFire = !autoFire;
            if (nine) sturdyTargets = !sturdyTargets;
            if (zero)
            {
                movingTargets = !movingTargets;
                foreach (EnemyHealth target in targets)
                {
                    if (target != null) target.GetComponent<EnemyController>().enabled = movingTargets;
                }
            }
        }
        else if (branch != GunBranch.Basic)
        {
            var ranks = ranksByBranch[(int)branch];
            if (Pressed(keyboard, Key.Digit7, Key.Numpad7)) ranks.x = (ranks.x + 1) % 4;
            if (eight) ranks.y = (ranks.y + 1) % 4;
            if (nine) ranks.z = (ranks.z + 1) % 4;
            ranksByBranch[(int)branch] = ranks;
        }

        if ((zero && !shift) || keyboard.rKey.wasPressedThisFrame)
        {
            player.position = PlayerStart;
            ResetTargets();
        }
    }

    private static bool Pressed(Keyboard keyboard, Key digit, Key numpadDigit)
    {
        return keyboard[digit].wasPressedThisFrame || keyboard[numpadDigit].wasPressedThisFrame;
    }

    private void FrameCamera()
    {
        labCamera.orthographicSize = Mathf.Max(5f, 8f / labCamera.aspect);
    }

    private void ResetTargets()
    {
        ClearChildren(targetRoot);
        ClearChildren(effectsRoot);
        level11.ResetEffects();
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
            enemy.name = bossMode ? "Boss dummy" : "Target " + (i + 1);
            if (bossMode) enemy.transform.localScale *= 1.45f;
            var movement = enemy.GetComponent<EnemyController>();
            movement.SetTarget(player.transform);
            movement.enabled = movingTargets;
            enemy.AddComponent<GunSkillTargetEffects>().IgnorePush = bossMode;
            enemy.GetComponent<EnemyContactDamage>().enabled = false;
            // Disabled MonoBehaviours still receive trigger callbacks in Unity.
            Destroy(enemy.GetComponent<EnemyContactDamage>());
            foreach (var renderer in enemy.GetComponentsInChildren<SpriteRenderer>())
                renderer.sharedMaterial = spriteMaterial;
            targets[i] = enemy.GetComponent<EnemyHealth>();
            targets[i].InitializeHealth(bossMode ? 1500 : sturdyTargets ? 100 : i == 0 ? 10 : 30);
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
        var panel = new Rect(12, 12, Mathf.Min(720, Screen.width - 24), 220);
        GUILayout.BeginArea(panel, GUI.skin.box);
        GUILayout.Label("GUN LAB  |  WASD move   Mouse aim   Targets respawn together");
        GUILayout.Label("Skills   [1] Ricochet   [2] Weak point   [3] Death scatter");
        GUILayout.Label("Branches   [4] Spread   [5] Burst   [6] Piercing");
        GUILayout.Label(UpgradeSummary());
        GUILayout.Label(branch == GunBranch.Basic ? "Cấp 11: chọn nhánh bằng [4] [5] [6]." :
            $"Cấp 11   [Shift+1] {Level11Names[(int)branch, 1]}   [Shift+2] {Level11Names[(int)branch, 2]}   [Shift+3] {Level11Names[(int)branch, 3]}");
        GUILayout.Label($"Cấp 11 đang dùng: {(branch == GunBranch.Basic ? "Tắt" : Level11Names[(int)branch, level11ByBranch[(int)branch]])}   |   [Shift+7] Bia boss: {(bossMode ? "1500 HP" : "Tắt")}");
        GUILayout.Label("[Shift+8] Auto fire  [Shift+9] Next target HP  [Shift+0] Move targets  [0] Reset");
        GUILayout.Label($"Skill: {SkillNames[(int)skill]}   |   {BranchSummary()}");
        GUILayout.Label($"Auto fire: {(autoFire ? "On" : "Off")}   |   Next targets: {(sturdyTargets ? "100 HP" : "10 / 30 HP")}   |   Movement: {(movingTargets ? "On" : "Off")}");
        GUILayout.EndArea();

        foreach (EnemyHealth target in targets)
        {
            if (target == null || target.CurrentHealth <= 0) continue;
            Vector3 screen = labCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 0.55f);
            GUI.Label(new Rect(screen.x - 20f, Screen.height - screen.y, 100f, 22f), target.CurrentHealth + " HP");
        }
    }

    private string UpgradeSummary()
    {
        var ranks = ranksByBranch[(int)branch];
        switch (branch)
        {
            case GunBranch.Spread: return $"[7] Pellets {ranks.x}/3   [8] Narrow spread {ranks.y}/3   [9] Knockback {ranks.z}/3";
            case GunBranch.Burst: return $"[7] Rounds {ranks.x}/3   [8] Ricochet {ranks.y}/3   [9] Death scatter {ranks.z}/3";
            case GunBranch.Piercing: return $"[7] Pierce damage {ranks.x}/3   [8] Slow {ranks.y}/3   [9] Shockwave {ranks.z}/3";
            default: return "Select a branch to test its three upgrades with [7] [8] [9].";
        }
    }

    private string BranchSummary()
    {
        var ranks = ranksByBranch[(int)branch];
        switch (branch)
        {
            case GunBranch.Spread: return $"Spread: {3 + ranks.x} pellets / {60 - 15 * ranks.y} degrees";
            case GunBranch.Burst: return $"Burst: {5 + 2 * ranks.x} rounds / burst";
            case GunBranch.Piercing: return $"Piercing: 10 damage, +{3 * ranks.x} per enemy passed";
            default: return "Basic: 10 damage / shot";
        }
    }
}
