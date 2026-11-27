using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class CharacterProgressionTest : MonoBehaviour
{
    private enum Path { Gun, Melee, Mage }
    private enum Stage { Base, Branch, Passive, FirstCore, SecondCore, Ultimate, Complete }

    private struct Card
    {
        internal readonly int Choice;
        internal readonly string Name, Detail;
        internal readonly int Icon;

        internal Card(int choice, string name, string detail = "", int icon = -1)
        {
            Choice = choice;
            Name = name;
            Detail = detail;
            Icon = icon;
        }
    }

    [SerializeField] private GameObject gunRoot, meleeRoot, mageRoot;
    private GunSkillTestArena gun;
    private MeleeController melee;
    private MageCombat mage;
    private MageLoadout mageLoadout;
    private MageUltimate mageUltimate;
    private PlayerHealth mageHealth;
    private PracticeTargetMode targetMode;
    private bool targetMenuOpen, targetsMove, sturdyTargets;
    private readonly EnemyHealth[] liveTargets = new EnemyHealth[8];
    private Transform liveTargetRoot, meleeTarget;
    private float? resetTargetsAt;
    private Texture2D icons;
    private readonly List<Card> cards = new List<Card>(4);
    private readonly int[] skills = { -1, -1, -1 };
    private readonly string[] skillNames = { "Chưa mở", "Chưa mở", "Chưa mở" };
    private readonly float[] nextCast = new float[3];
    private static readonly string[] PathNames = { "Súng", "Cận chiến", "Pháp sư" };
    private static readonly string[] MeleeBranches = { "Sát thủ", "Đao sĩ", "Kiếm khí" };
    private int level = 1, selectedBranch = -1;
    private Stage stage;
    private Path? chosenPath;
    private bool choosing;
    private float priorScale = 1f, openedAt;
    private string passiveName = "Chưa mở";
    private string branchName = "Súng cơ bản";
    private GUIStyle title, label, button, small;


    private void Awake()
    {
        gun = gunRoot.GetComponentInChildren<GunSkillTestArena>(true);
        melee = meleeRoot.GetComponentInChildren<MeleeController>(true);
        mage = mageRoot.GetComponentInChildren<MageCombat>(true);
        mageLoadout = mage.GetComponent<MageLoadout>();
        mageUltimate = mage.GetComponent<MageUltimate>();
        mageHealth = mage.GetComponent<PlayerHealth>();
        gun.Integrated = true;
        melee.Integrated = true;
        mage.Integrated = true;
        icons = Resources.Load<Texture2D>("Magical/MageLevelUpIconsClean");
        gunRoot.SetActive(true);
    }

    private void OnDisable()
    {
        if (!choosing) return;
        Time.timeScale = priorScale;
        choosing = false;
    }

    private void Update()
    {
        if (choosing) return;
        UpdatePracticeTargets();
        if (chosenPath == null || selectedBranch < 0 || Keyboard.current == null) return;
        var keyboard = Keyboard.current;
        if (keyboard.qKey.wasPressedThisFrame) TryCast(0);
        if (keyboard.eKey.wasPressedThisFrame) TryCast(1);
        if (keyboard.rKey.wasPressedThisFrame) TryCast(2);
    }

    private bool UsesGunTargets => selectedBranch < 0 || chosenPath == Path.Gun;
    private int PracticeHealth => chosenPath == Path.Mage ? mageHealth.CurrentHealth : melee.IntegratedHealth;

    private void UpdatePracticeTargets()
    {
        if (UsesGunTargets || liveTargetRoot == null) return;
        if (chosenPath == Path.Melee) meleeTarget.position = melee.IntegratedGround;
        if (PracticeHealth > 0)
        {
            foreach (EnemyHealth target in liveTargets)
                if (target != null && target.CurrentHealth > 0) return;
        }
        resetTargetsAt ??= Time.time + .75f;
        if (Time.time >= resetTargetsAt.Value) ResetPracticeTargets();
    }

    private void ResetPracticeTargets()
    {
        resetTargetsAt = null;
        if (UsesGunTargets)
        {
            gun.ConfigureIntegratedTargets(targetMode, targetsMove, sturdyTargets);
            return;
        }

        if (liveTargetRoot == null)
            liveTargetRoot = new GameObject("Quái tập chung").transform;
        liveTargetRoot.SetParent(chosenPath == Path.Melee ? meleeRoot.transform : mageRoot.transform, false);
        foreach (Transform child in liveTargetRoot)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        Transform target;
        if (chosenPath == Path.Melee)
        {
            if (meleeTarget == null)
            {
                meleeTarget = new GameObject("Điểm va chạm Cận chiến").transform;
                meleeTarget.SetParent(meleeRoot.transform, false);
            }
            meleeTarget.position = melee.IntegratedGround;
            target = meleeTarget;
            if (PracticeHealth <= 0) melee.RestoreIntegratedHealth();
        }
        else
        {
            target = mage.transform;
            if (PracticeHealth <= 0) mageHealth.RestoreFullHealth();
        }
        gun.SpawnPracticeEnemies(liveTargets, liveTargetRoot, target, (Vector2)target.position + Vector2.right * 2.65f,
            targetMode, targetsMove, sturdyTargets, chosenPath == Path.Mage,
            () => PracticeHealth > 0, ReceiveIntegratedAttack);
        if (chosenPath == Path.Melee) melee.SetIntegratedTargets(liveTargets);
    }

    private void ReceiveIntegratedAttack(Vector2 source, int damage)
    {
        if (chosenPath == Path.Melee) melee.ReceiveIntegratedHit(source, damage);
        else mageHealth.TakeDamage(damage);
    }

    private bool TryCast(int slot)
    {
        if (choosing || skills[slot] < 0 || Time.time < nextCast[slot]) return false;
        int choice = skills[slot];
        bool fired;
        switch (chosenPath)
        {
            case Path.Gun:
                fired = gun.UseIntegratedSkill(slot, choice);
                if (fired && slot != 2) nextCast[slot] = Time.time + (slot == 0 ? 4f : 6f);
                return fired;
            case Path.Melee:
                fired = melee.UseIntegratedSkill(slot, choice);
                if (fired) nextCast[slot] = Time.time + (slot == 2 ? 12f : 5f);
                return fired;
            case Path.Mage:
                return slot == 2 ? mageUltimate.TryCast() : mage.UseIntegratedStarter(choice);
            default:
                return false;
        }
    }

    private void OpenNextStage()
    {
        if (choosing || stage == Stage.Complete) return;
        targetMenuOpen = false;
        level++;
        stage++;
        priorScale = Time.timeScale;
        Time.timeScale = 0f;
        choosing = true;
        FillCards();
    }

    private void Choose(Card card)
    {
        if (!choosing) return;
        int choice = card.Choice;
        switch (stage)
        {
            case Stage.Branch:
                if (chosenPath == null)
                {
                    chosenPath = (Path)choice;
                    FillCards();
                    return;
                }
                selectedBranch = choice;
                branchName = card.Name;
                ActivateBranch(choice);
                break;
            case Stage.Passive:
                passiveName = card.Name;
                if (chosenPath == Path.Gun) gun.SetIntegratedPassive(choice);
                else if (chosenPath == Path.Melee) melee.SetIntegratedPassive(choice);
                else mageLoadout.SetCore(choice, true);
                break;
            case Stage.FirstCore:
            case Stage.SecondCore:
            case Stage.Ultimate:
                int slot = (int)stage - (int)Stage.FirstCore;
                skills[slot] = choice;
                skillNames[slot] = card.Name;
                if (stage == Stage.Ultimate)
                {
                    if (chosenPath == Path.Mage) mageLoadout.SelectUltimate((MageUltimateKind)choice);
                    stage = Stage.Complete;
                }
                break;
        }
        choosing = false;
        cards.Clear();
        Time.timeScale = priorScale;
    }

    private void ActivateBranch(int choice)
    {
        switch (chosenPath)
        {
            case Path.Gun:
                gun.SetIntegratedBranch(choice);
                break;
            case Path.Melee:
                melee.ConfigureIntegratedBranch(choice);
                gunRoot.SetActive(false);
                meleeRoot.SetActive(true);
                ResetPracticeTargets();
                break;
            case Path.Mage:
                mageLoadout.SelectElement((MageElement)choice);
                for (int i = 0; i < 4; i++) mageLoadout.SetCore(i, false);
                for (int i = 0; i < 3; i++) mageLoadout.SetTier(i, 0);
                gunRoot.SetActive(false);
                mageRoot.SetActive(true);
                ResetPracticeTargets();
                break;
        }
    }

    private void FillCards()
    {
        openedAt = Time.unscaledTime;
        cards.Clear();
        if (chosenPath == null)
        {
            for (int i = 0; i < PathNames.Length; i++) cards.Add(new Card(i, PathNames[i]));
            return;
        }
        switch (chosenPath.Value)
        {
            case Path.Gun: FillGunCards(); break;
            case Path.Melee: FillMeleeCards(); break;
            case Path.Mage: FillMageCards(); break;
        }
    }

    private void FillGunCards()
    {
        switch (stage)
        {
            case Stage.Branch: AddCards(3, GunSkillTestArena.BranchChoice); break;
            case Stage.Passive: AddCards(3, GunSkillTestArena.PassiveChoice); break;
            case Stage.FirstCore: AddCards(3, i => GunSkillTestArena.FirstCoreChoice(selectedBranch, i)); break;
            case Stage.SecondCore: AddCards(2, i => GunSkillTestArena.SecondCoreChoice(selectedBranch, i)); break;
            case Stage.Ultimate: AddCards(4, GunSkillTestArena.UltimateChoice); break;
        }
    }

    private void FillMeleeCards()
    {
        if (stage == Stage.Branch) { AddCards(MeleeBranches.Length, i => MeleeBranches[i]); return; }
        int group = stage == Stage.Passive ? 0 : stage == Stage.FirstCore ? 1 : stage == Stage.SecondCore ? 3 : 2;
        AddCards(3, i =>
        {
            string namedSlot = MeleeController.PreviewV2DisplayName(group, selectedBranch + 1, i);
            return namedSlot[(namedSlot.IndexOf('·') + 1)..].Trim();
        });
    }

    private void FillMageCards()
    {
        if (stage == Stage.Branch)
        {
            AddCards(4, i => MageLevelProgression.ElementNames[i], 4);
            return;
        }
        if (stage == Stage.Passive)
        {
            for (int i = 0; i < 4; i++)
            {
                // The movement-speed core stays outside this animation-only test.
                if (selectedBranch == (int)MageElement.Light && i == 2) continue;
                cards.Add(new Card(i, MageLevelProgression.CoreNames[selectedBranch][i],
                    MageLevelProgression.CoreDescriptions[selectedBranch][i], 12 + selectedBranch * 4 + i));
            }
            return;
        }
        if (stage == Stage.Ultimate)
        {
            AddCards(3, i => MageLevelProgression.UltimateNames[i], 28);
            return;
        }
        for (int i = 0; i < 3; i++)
        {
            if (stage == Stage.SecondCore && skills[0] == i) continue;
            cards.Add(new Card(i, MageLevelProgression.StarterNames[i], icon: i));
        }
    }

    private void AddCards(int count, System.Func<int, string> name, int firstIcon = -1)
    {
        for (int i = 0; i < count; i++) cards.Add(new Card(i, name(i), icon: firstIcon < 0 ? -1 : firstIcon + i));
    }

    private void OnGUI()
    {
        title ??= Style(GUI.skin.label, 24, FontStyle.Bold);
        label ??= Style(GUI.skin.label, 15, FontStyle.Normal);
        button ??= Style(GUI.skin.button, 14, FontStyle.Bold);
        small ??= new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 13 };
        if (choosing) { DrawCards(); return; }
        string health = UsesGunTargets ? "" : $" · HP {PracticeHealth}";
        GUI.Box(new Rect(10, 10, Mathf.Min(Screen.width - 20, 360), 108),
            $"Cấp {level} · {branchName}{health}\nBị động: {passiveName}\nQ: {skillNames[0]}\nE: {skillNames[1]}\nR: {skillNames[2]}", small);
        if (chosenPath == Path.Melee)
            GUI.Label(new Rect(10, 120, 340, 24), "WASD di chuyển · Tự đánh trong tầm · Chuột ngắm kỹ năng");
        if (stage != Stage.Complete && GUI.Button(new Rect(Screen.width - 158, 10, 148, 34), "Thử lên cấp"))
        {
            OpenNextStage();
            return;
        }
        if (GUI.Button(new Rect(Screen.width - 158, 49, 148, 34), "Chơi lại"))
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                gameObject.scene.path, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(gameObject.scene.path);
#endif
            return;
        }
        DrawTargetMenu();
    }

    private void DrawTargetMenu()
    {
        const float width = 222f;
        float left = Screen.width - width - 10f;
        if (GUI.Button(new Rect(left, 88, width, 34), targetMenuOpen ? "Đóng menu quái" : "Chọn quái tập"))
            targetMenuOpen = !targetMenuOpen;
        if (!targetMenuOpen) return;
        Fill(new Rect(left, 129f, width, 206f), new Color(.065f, .09f, .15f, .96f));
        PickTargetMode(new Rect(left + 8, 136, width - 16, 29), PracticeTargetMode.Dummies, "Bia thường");
        PickTargetMode(new Rect(left + 8, 168, width - 16, 29), PracticeTargetMode.BossDummy, "Bia boss · 1.500 HP");
        PickTargetMode(new Rect(left + 8, 200, width - 16, 29), PracticeTargetMode.AttackingBoss, "Boss tấn công · 2.500 HP");
        if (targetMode == PracticeTargetMode.Dummies)
        {
            bool moving = GUI.Toggle(new Rect(left + 8, 234, width - 16, 23), targetsMove, "Bia di chuyển");
            bool sturdy = GUI.Toggle(new Rect(left + 8, 260, width - 16, 23), sturdyTargets, "Tất cả bia 100 HP");
            if (moving != targetsMove || sturdy != sturdyTargets)
            {
                targetsMove = moving;
                sturdyTargets = sturdy;
                ResetPracticeTargets();
            }
        }
        if (GUI.Button(new Rect(left + 8, 296, width - 16, 29), "Đặt lại quái")) ResetPracticeTargets();
    }

    private void PickTargetMode(Rect rect, PracticeTargetMode mode, string caption)
    {
        if (!GUI.Button(rect, mode == targetMode ? "✓ " + caption : caption) || mode == targetMode) return;
        targetMode = mode;
        ResetPracticeTargets();
    }

    private void DrawCards()
    {
        float appear = Mathf.SmoothStep(0f, 1f, (Time.unscaledTime - openedAt) / .23f);
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(.01f, .02f, .06f, .86f));
        int columns = Mathf.Max(1, Screen.width < 700 ? Mathf.Min(2, cards.Count) : cards.Count);
        int rows = (cards.Count + columns - 1) / columns;
        float width = Mathf.Min(1120, Screen.width - 30f);
        float cardHeight = Mathf.Clamp((Screen.height - 130f) / rows - 12f, 125f, 244f);
        const float gap = 10f, margin = 16f, header = 78f;
        float height = header + margin + rows * cardHeight + (rows - 1) * gap;
        float left = (Screen.width - width) / 2f;
        float top = (Screen.height - height) / 2f + (1f - appear) * 22f;
        Fill(new Rect(left, top, width, height), new Color(.06f, .08f, .14f));
        GUI.Label(new Rect(left, top + 11f, width, 34f), $"LÊN CẤP {level}", title);
        string instruction = stage switch
        {
            Stage.Branch => chosenPath == null ? "Chọn hệ" : "Chọn chuyên nhánh",
            Stage.Passive => "Chọn kỹ năng bị động",
            Stage.FirstCore => "Chọn Lõi 1 · Q",
            Stage.SecondCore => "Chọn Lõi 2 · E",
            _ => "Chọn chiêu cuối · R"
        };
        GUI.Label(new Rect(left, top + 45f, width, 27f), instruction, label);
        float cardWidth = (width - 2f * margin - (columns - 1) * gap) / columns;
        for (int i = 0; i < cards.Count; i++)
        {
            Rect rect = new Rect(left + margin + (cardWidth + gap) * (i % columns),
                top + header + (cardHeight + gap) * (i / columns), cardWidth, cardHeight);
            Card card = cards[i];
            Fill(rect, new Color(.10f, .14f, .23f));
            Fill(new Rect(rect.x, rect.y, rect.width, 3), new Color(.20f, .67f, .80f));
            float iconSize = Mathf.Min(62f, cardHeight * .32f);
            if (icons != null && card.Icon >= 0)
            {
                var icon = new Rect(rect.center.x - iconSize * .5f, rect.y + 11f, iconSize, iconSize);
                var uv = new Rect((card.Icon % 4) / 4f, 1f - (card.Icon / 4 + 1) / 8f, .25f, .125f);
                GUI.DrawTextureWithTexCoords(icon, icons, uv);
            }
            float textTop = card.Icon >= 0 ? rect.y + iconSize + 9f : rect.y + 12f;
            float textHeight = rect.yMax - textTop - 49f;
            string content = string.IsNullOrEmpty(card.Detail) ? card.Name : card.Name + "\n" + card.Detail;
            GUI.Label(new Rect(rect.x + 8, textTop, rect.width - 16, textHeight), content, label);
            if (GUI.Button(new Rect(rect.x + 10, rect.yMax - 43, rect.width - 20, 32), "CHỌN", button))
            {
                Choose(card);
                return;
            }
        }
    }

    private static GUIStyle Style(GUIStyle skin, int size, FontStyle font)
    {
        var style = new GUIStyle(skin)
        { alignment = TextAnchor.MiddleCenter, fontSize = size, fontStyle = font, wordWrap = true };
        style.normal.textColor = Color.white;
        return style;
    }

    private static void Fill(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;
    }
}
