using System.Collections.Generic;
using UnityEngine;

public sealed class MageLevelProgression : MonoBehaviour
{
    private enum ChoiceKind { Stat, Starter, Element, Tier, Core, Ultimate }
    private enum StatKind { Vitality, Agility, Focus }

    private struct Choice
    {
        public ChoiceKind kind;
        public int value;
        public int icon;
        public string title;
        public string description;

        public Choice(ChoiceKind kind, int value, int icon, string title, string description)
        {
            this.kind = kind;
            this.value = value;
            this.icon = icon;
            this.title = title;
            this.description = description;
        }
    }

    private static readonly string[] ElementNames = { "Hỏa", "Băng", "Quang", "Ám" };
    private static readonly string[] StarterNames = { "Hồi phục", "Sóng xung kích", "Vùng trọng lực" };
    private static readonly string[] UltimateNames =
        { "Ngưng Đọng Thời Gian", "Thiên Tinh Giáng Lâm", "Thiên Cầu Ma Lực" };
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
    private static readonly string[][] TierDescriptions =
    {
        new[] { "+15% sát thương nổ mỗi bậc", "+25% sát thương thiêu đốt mỗi bậc", "+20% bán kính nổ mỗi bậc" },
        new[] { "+10% hiệu quả làm chậm mỗi bậc", "+0,5 giây đóng băng mỗi bậc", "+15% sát thương lên mục tiêu bị lạnh mỗi bậc" },
        new[] { "+1 nhịp cột sáng mỗi bậc", "+20% bán kính mỗi bậc", "+15% sát thương mỗi nhịp" },
        new[] { "+1 mục tiêu nguyền mỗi bậc", "+25% sát thương nguyền mỗi bậc", "+6% suy yếu mỗi bậc" }
    };
    private static readonly string[][] CoreDescriptions =
    {
        new[] { "Thiêu đốt kéo dài 5 giây.", "Mỗi loạt thứ ba bắn thêm hai cầu lửa.", "Gây thêm sát thương lên quái đang cháy.", "Quái đang cháy phát nổ khi bị hạ." },
        new[] { "Tăng hiệu quả làm chậm lên 45%.", "Tích tụ tối đa hai tầng sát thương lạnh.", "Gây thêm sát thương lên quái bị đóng băng.", "Băng có thể lan dấu lạnh sang quái gần đó." },
        new[] { "Thêm một nhịp sát thương sau cột sáng.", "Mỗi ba nhịp gây thêm sát thương phán quyết.", "Cột sáng tăng tốc độ di chuyển cho Pháp sư.", "Làm giảm sát thương của quái trúng sáng." },
        new[] { "Tăng giới hạn nguyền lên năm mục tiêu.", "Mỗi ba lần trúng gây thêm sát thương dồn.", "Tăng làm chậm và giảm sát thương quái.", "Chuyển lời nguyền sang mục tiêu gần khi quái chết." }
    };
    private static readonly int[][] TierIcons =
    {
        new[] { 12, 13, 14 }, new[] { 16, 17, 18 },
        new[] { 20, 21, 22 }, new[] { 24, 25, 26 }
    };
    private static readonly int[][] CoreIcons =
    {
        new[] { 12, 13, 14, 15 }, new[] { 16, 17, 18, 19 },
        new[] { 20, 21, 22, 23 }, new[] { 24, 25, 26, 27 }
    };

    private MageCombat combat;
    private MageLoadout loadout;
    private MageExperience experience;
    private PlayerController movement;
    private PlayerHealth health;
    private MageUltimate ultimate;
    private Texture2D iconAtlas;
    private readonly Queue<int> pendingChoiceLevels = new Queue<int>();
    private readonly int[] statRanks = new int[3];
    private List<Choice> activeChoices;
    private int activeChoiceLevel;
    private int selectedElement = -1;
    private int selectedStarter = -1;
    private int selectedUltimate = -1;
    private float attackSpeedMultiplier = 1f;
    private float previousTimeScale = 1f;
    private bool ownsPause;
    private bool previewUnlockAll;
    private GUIStyle panelStyle;
    private GUIStyle headingStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle cardStyle;
    private GUIStyle cardTitleStyle;
    private GUIStyle cardBodyStyle;
    private GUIStyle buttonStyle;
    private GUIStyle choiceButtonStyle;
    private GUIStyle hudStyle;
    private GUIStyle progressBackgroundStyle;
    private GUIStyle progressFillStyle;
    private Texture2D choiceButtonNormalTexture;
    private Texture2D choiceButtonHoverTexture;
    private Texture2D choiceButtonActiveTexture;

    public bool HasStarter => selectedStarter >= 0;
    public bool HasElement => selectedElement >= 0;
    public bool HasUltimate => selectedUltimate >= 0;
    public bool CanUseStarter => HasStarter || previewUnlockAll;
    public bool CanUseUltimate => HasUltimate || previewUnlockAll;
    public bool IsChoosing => activeChoices != null;
    public float AttackSpeedMultiplier => attackSpeedMultiplier;

    public void Initialize(MageCombat mageCombat, MageLoadout mageLoadout,
        MageExperience mageExperience, PlayerController playerMovement,
        PlayerHealth playerHealth, MageUltimate mageUltimate)
    {
        combat = mageCombat;
        loadout = mageLoadout;
        experience = mageExperience;
        movement = playerMovement;
        health = playerHealth;
        ultimate = mageUltimate;
        experience.LevelReached -= OnLevelReached;
        experience.LevelReached += OnLevelReached;
        iconAtlas = Resources.Load<Texture2D>("Magical/MageLevelUpIconsClean");
    }

    private void Update()
    {
        if (activeChoices == null && pendingChoiceLevels.Count > 0)
            ShowNextChoice();
    }

    private void OnGUI()
    {
        if (experience == null) return;
        EnsureStyles();
        DrawExperienceHud();
        if (activeChoices != null) DrawChoicePanel();
    }

    private void DrawExperienceHud()
    {
        float width = Mathf.Min(400f, Screen.width - 16f);
        Rect panel = new Rect(8f, 91f, width, 56f);
        Color old = GUI.color;
        GUI.color = new Color(0.035f, 0.055f, 0.105f, 0.96f);
        GUI.Box(panel, GUIContent.none, panelStyle);
        GUI.color = old;

        string bonus = experience.ExperienceMultiplier > 1f ? "   EXP ×1,3" : string.Empty;
        string progress = experience.Level >= 20
            ? "ĐÃ ĐẠT CẤP TỐI ĐA"
            : $"CẤP {experience.Level}     EXP {experience.CurrentExperience}/{experience.ExperienceToNextLevel}{bonus}";
        GUI.Label(new Rect(panel.x + 12f, panel.y + 6f, width - 24f, 20f), progress, hudStyle);
        Rect bar = new Rect(panel.x + 12f, panel.y + 33f, width - 24f, 10f);
        GUI.Box(bar, GUIContent.none, progressBackgroundStyle);
        if (experience.Level < 20 && experience.ExperienceToNextLevel > 0)
        {
            float ratio = Mathf.Clamp01((float)experience.CurrentExperience / experience.ExperienceToNextLevel);
            GUI.Box(new Rect(bar.x, bar.y, bar.width * ratio, bar.height), GUIContent.none, progressFillStyle);
        }

#if UNITY_EDITOR
        Rect xpButton = new Rect(panel.xMax - 88f, panel.y + 5f, 78f, 22f);
        if (GUI.Button(xpButton, "+10 EXP", buttonStyle)) experience.AddExperience(10);
        Rect unlockButton = new Rect(panel.xMax - 88f, panel.y + 29f, 78f, 21f);
        if (GUI.Button(unlockButton, previewUnlockAll ? "Thử skill: Bật" : "Thử skill: Tắt", buttonStyle))
            previewUnlockAll = !previewUnlockAll;
#endif
    }

    private void DrawChoicePanel()
    {
        Color old = GUI.color;
        GUI.color = new Color(0f, 0.015f, 0.06f, 0.92f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = old;

        float width = Mathf.Min(Screen.width * 0.92f, 1240f);
        float height = Mathf.Min(Screen.height * 0.78f, 560f);
        Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f,
            width, height);
        GUI.color = new Color(0.055f, 0.075f, 0.14f, 1f);
        GUI.Box(panel, GUIContent.none, panelStyle);
        GUI.color = old;

        GUI.Label(new Rect(panel.x + 24f, panel.y + 20f, width - 48f, 42f),
            $"LÊN CẤP {activeChoiceLevel}", headingStyle);
        GUI.DrawTexture(new Rect(panel.center.x - 48f, panel.y + 63f, 96f, 2f), Texture2D.whiteTexture);
        GUI.Label(new Rect(panel.x + 24f, panel.y + 70f, width - 48f, 26f),
            "Chọn một nâng cấp để tiếp tục", subtitleStyle);

        int count = activeChoices.Count;
        float padding = Mathf.Clamp(width * 0.022f, 18f, 28f);
        float gap = Mathf.Clamp(width * 0.014f, 12f, 20f);
        float cardWidth = (width - padding * 2f - gap * (count - 1)) / count;
        float cardY = panel.y + 112f;
        float cardHeight = panel.yMax - padding - cardY;
        float startX = panel.x + padding;
        int selectedIndex = -1;
        for (int i = 0; i < count; i++)
        {
            Rect card = new Rect(startX + i * (cardWidth + gap), cardY, cardWidth, cardHeight);
            if (DrawChoiceCard(card, activeChoices[i]))
                selectedIndex = i;
        }
        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp ||
            Event.current.type == EventType.KeyDown)
            Event.current.Use();
        if (selectedIndex >= 0) SelectChoice(selectedIndex);
    }

    private bool DrawChoiceCard(Rect rect, Choice choice)
    {
        Color old = GUI.color;
        bool hovered = rect.Contains(Event.current.mousePosition);
        GUI.color = hovered
            ? new Color(0.16f, 0.22f, 0.34f, 1f)
            : new Color(0.115f, 0.15f, 0.24f, 1f);
        GUI.Box(rect, GUIContent.none, cardStyle);
        GUI.color = new Color(0.28f, 0.72f, 0.84f, 1f);
        GUI.DrawTexture(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, 3f), Texture2D.whiteTexture);
        GUI.color = old;

        float iconHeight = Mathf.Clamp(Mathf.Min(rect.width * 0.4f, rect.height * 0.3f), 64f, 112f);
        Rect iconRect = new Rect(rect.center.x - iconHeight * 0.5f, rect.y + 18f, iconHeight, iconHeight);
        DrawIcon(iconRect, choice.icon);
        float titleHeight = Mathf.Clamp(rect.height * 0.16f, 34f, 48f);
        GUI.Label(new Rect(rect.x + 14f, iconRect.yMax + 8f, rect.width - 28f, titleHeight),
            choice.title, cardTitleStyle);
        float bodyY = iconRect.yMax + titleHeight + 20f;
        Rect button = new Rect(rect.x + 14f, rect.yMax - 54f, rect.width - 28f, 38f);
        GUI.Label(new Rect(rect.x + 18f, bodyY, rect.width - 36f,
            Mathf.Max(36f, button.y - bodyY - 14f)), choice.description, cardBodyStyle);
        string buttonLabel = rect.width < 210f ? "CHỌN" : "CHỌN NÂNG CẤP";
        bool selected = GUI.Button(button, buttonLabel, choiceButtonStyle);
        return selected;
    }

    private void DrawIcon(Rect rect, int iconIndex)
    {
        if (iconAtlas == null)
        {
            GUI.Label(rect, "✦", headingStyle);
            return;
        }
        if (iconIndex < 0 || iconIndex >= 32)
        {
            GUI.Label(rect, "✦", headingStyle);
            return;
        }
        int column = iconIndex % 4;
        int row = iconIndex / 4;
        Rect uv = new Rect(column / 4f, 1f - (row + 1) / 8f, 0.25f, 1f / 8f);
        GUI.DrawTextureWithTexCoords(rect, iconAtlas, uv, true);
    }

    private void EnsureStyles()
    {
        if (panelStyle != null) return;
        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = Texture2D.whiteTexture;
        panelStyle.border = new RectOffset(8, 8, 8, 8);
        headingStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.Clamp(Screen.height / 32, 22, 34),
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.84f, 0.91f, 1f) }
        };
        subtitleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.Clamp(Screen.height / 65, 14, 20),
            normal = { textColor = new Color(0.68f, 0.77f, 0.92f) }
        };
        cardStyle = new GUIStyle(GUI.skin.box);
        cardStyle.normal.background = Texture2D.whiteTexture;
        cardStyle.border = new RectOffset(8, 8, 8, 8);
        cardTitleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontSize = Mathf.Clamp(Screen.height / 58, 15, 21),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        cardBodyStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperCenter,
            wordWrap = true,
            fontSize = Mathf.Clamp(Screen.height / 82, 13, 17),
            normal = { textColor = new Color(0.75f, 0.81f, 0.91f) }
        };
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.Clamp(Screen.height / 75, 14, 18),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            border = new RectOffset(6, 6, 6, 6)
        };
        choiceButtonNormalTexture = CreateColorTexture(new Color(0.06f, 0.42f, 0.52f));
        choiceButtonHoverTexture = CreateColorTexture(new Color(0.08f, 0.56f, 0.66f));
        choiceButtonActiveTexture = CreateColorTexture(new Color(0.04f, 0.32f, 0.41f));
        choiceButtonStyle = new GUIStyle(buttonStyle);
        choiceButtonStyle.normal.background = choiceButtonNormalTexture;
        choiceButtonStyle.hover.background = choiceButtonHoverTexture;
        choiceButtonStyle.active.background = choiceButtonActiveTexture;
        choiceButtonStyle.focused.background = choiceButtonHoverTexture;
        choiceButtonStyle.normal.textColor = Color.white;
        choiceButtonStyle.hover.textColor = Color.white;
        choiceButtonStyle.active.textColor = Color.white;
        choiceButtonStyle.focused.textColor = Color.white;
        hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Clamp(Screen.height / 85, 13, 18),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        progressBackgroundStyle = new GUIStyle(GUI.skin.box);
        progressBackgroundStyle.normal.background = Texture2D.whiteTexture;
        progressFillStyle = new GUIStyle(GUI.skin.box);
        progressFillStyle.normal.background = Texture2D.whiteTexture;
    }

    private static Texture2D CreateColorTexture(Color color)
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private void OnLevelReached(int level)
    {
        if (level == 2)
        {
            experience.SetExperienceMultiplier(1.3f);
            return;
        }
        if (HasChoicesAtLevel(level)) pendingChoiceLevels.Enqueue(level);
    }

    private bool HasChoicesAtLevel(int level)
    {
        return IsStatLevel(level) || level == 5 || level == 9 ||
            (level >= 10 && level <= 14) || level == 15 || (level >= 19 && level <= 20);
    }

    private static bool IsStatLevel(int level)
    {
        return level == 3 || level == 4 || (level >= 6 && level <= 8) ||
            (level >= 16 && level <= 18);
    }

    private void ShowNextChoice()
    {
        if (pendingChoiceLevels.Count == 0) return;
        activeChoiceLevel = pendingChoiceLevels.Peek();
        activeChoices = BuildChoices(activeChoiceLevel);
        if (activeChoices.Count == 0)
        {
            pendingChoiceLevels.Dequeue();
            activeChoices = null;
            return;
        }
        if (!ownsPause)
        {
            previousTimeScale = Time.timeScale;
            ownsPause = true;
        }
        Time.timeScale = 0f;
    }

    private List<Choice> BuildChoices(int level)
    {
        var choices = new List<Choice>(4);
        if (IsStatLevel(level))
        {
            AddStatChoice(choices, StatKind.Vitality, 8, "Sinh lực", "+15 máu tối đa và hồi 15 máu.");
            AddStatChoice(choices, StatKind.Agility, 10, "Nhanh nhẹn", "+7% tốc độ di chuyển.");
            AddStatChoice(choices, StatKind.Focus, 9, "Tập trung", "+7% tốc độ đánh tự động.");
        }
        else if (level == 5)
        {
            choices.Add(new Choice(ChoiceKind.Starter, 0, 0, StarterNames[0],
                "Q: hồi 25 máu. Hồi chiêu 25 giây."));
            choices.Add(new Choice(ChoiceKind.Starter, 1, 1, StarterNames[1],
                "Q: gây 15 sát thương và đẩy lùi trong bán kính 2,5 m."));
            choices.Add(new Choice(ChoiceKind.Starter, 2, 2, StarterNames[2],
                "Q: tạo vùng trọng lực tại vị trí chuột trong 4 giây."));
        }
        else if (level == 9)
        {
            for (int i = 0; i < 4; i++)
                choices.Add(new Choice(ChoiceKind.Element, i, 4 + i, ElementNames[i],
                    GetElementDescription((MageElement)i)));
        }
        else if (level >= 10 && level <= 12)
        {
            int branch = (int)loadout.Element;
            for (int i = 0; i < 3; i++)
                if (loadout.GetTier(i) < 3)
                    choices.Add(new Choice(ChoiceKind.Tier, i, TierIcons[branch][i],
                        TierNames[branch][i] + " " + (loadout.GetTier(i) + 1),
                        TierDescriptions[branch][i]));
        }
        else if (level == 13 || level == 14 || level == 19 || level == 20)
        {
            int branch = (int)loadout.Element;
            for (int i = 0; i < 4; i++)
            {
                if (loadout.GetCore(i)) continue;
                choices.Add(new Choice(ChoiceKind.Core, i, CoreIcons[branch][i],
                    CoreNames[branch][i], CoreDescriptions[branch][i]));
            }
            if (choices.Count > 3)
            {
                for (int i = choices.Count - 1; i > 0; i--)
                {
                    int swap = Random.Range(0, i + 1);
                    Choice temporary = choices[i];
                    choices[i] = choices[swap];
                    choices[swap] = temporary;
                }
                choices.RemoveRange(3, choices.Count - 3);
            }
        }
        else if (level == 15)
        {
            choices.Add(new Choice(ChoiceKind.Ultimate, 0, 28, UltimateNames[0],
                "E: dừng quái thường trong 3 giây."));
            choices.Add(new Choice(ChoiceKind.Ultimate, 1, 29, UltimateNames[1],
                "E: gọi 5 đợt sao xuống vùng được chọn."));
            choices.Add(new Choice(ChoiceKind.Ultimate, 2, 30, UltimateNames[2],
                "E: tích năng lượng rồi phóng 2–8 phi đạn."));
        }
        return choices;
    }

    private void AddStatChoice(List<Choice> choices, StatKind stat, int icon,
        string title, string description)
    {
        if (statRanks[(int)stat] < 5)
            choices.Add(new Choice(ChoiceKind.Stat, (int)stat, icon, title, description));
    }

    private static string GetElementDescription(MageElement element)
    {
        switch (element)
        {
            case MageElement.Fire: return "Cầu lửa gây sát thương và thiêu đốt mục tiêu.";
            case MageElement.Ice: return "Hơi thở băng làm chậm và tích dấu đóng băng.";
            case MageElement.Light: return "Cột sáng đánh nhiều nhịp tại vị trí chuột.";
            default: return "Phi đạn bóng tối nguyền và làm chậm mục tiêu.";
        }
    }

    private void SelectChoice(int index)
    {
        if (activeChoices == null || index < 0 || index >= activeChoices.Count) return;
        Choice choice = activeChoices[index];
        switch (choice.kind)
        {
            case ChoiceKind.Stat: ApplyStat((StatKind)choice.value); break;
            case ChoiceKind.Starter:
                loadout.SelectStarter((MageStarterSkill)choice.value);
                combat.ResetPreviewStarter();
                selectedStarter = choice.value;
                break;
            case ChoiceKind.Element:
                loadout.SelectElement((MageElement)choice.value);
                combat.ResetPreviewElement();
                selectedElement = choice.value;
                break;
            case ChoiceKind.Tier:
                loadout.SetTier(choice.value, loadout.GetTier(choice.value) + 1);
                break;
            case ChoiceKind.Core:
                loadout.SetCore(choice.value, true);
                break;
            case ChoiceKind.Ultimate:
                ultimate.ResetPreviewUltimate();
                loadout.SelectUltimate((MageUltimateKind)choice.value);
                selectedUltimate = choice.value;
                break;
        }

        activeChoices = null;
        if (pendingChoiceLevels.Count > 0) pendingChoiceLevels.Dequeue();
        if (pendingChoiceLevels.Count > 0) ShowNextChoice();
        else
        {
            if (ownsPause) Time.timeScale = previousTimeScale;
            ownsPause = false;
        }
    }

    private void ApplyStat(StatKind stat)
    {
        statRanks[(int)stat]++;
        switch (stat)
        {
            case StatKind.Vitality:
                health.IncreaseMaximumHealthAndHeal(15);
                break;
            case StatKind.Agility:
                movement.LevelUpSpeedMultiplier = 1f + 0.07f * statRanks[(int)stat];
                break;
            case StatKind.Focus:
                attackSpeedMultiplier = 1f + 0.07f * statRanks[(int)stat];
                break;
        }
    }

    private void OnDestroy()
    {
        if (experience != null) experience.LevelReached -= OnLevelReached;
        if (ownsPause) Time.timeScale = previousTimeScale;
        DestroyChoiceButtonTextures();
    }

    private void DestroyChoiceButtonTextures()
    {
        if (choiceButtonNormalTexture != null) Destroy(choiceButtonNormalTexture);
        if (choiceButtonHoverTexture != null) Destroy(choiceButtonHoverTexture);
        if (choiceButtonActiveTexture != null) Destroy(choiceButtonActiveTexture);
    }
}
