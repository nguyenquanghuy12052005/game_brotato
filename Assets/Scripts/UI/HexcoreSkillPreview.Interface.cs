using UnityEngine;

public sealed partial class HexcoreSkillPreview
{
    private static readonly string[] Groups = { "Kiếm sĩ", "Sát thủ", "Đao sĩ", "Kiếm khí", "Lõi HEXCORE" };
    private static readonly string[] PreviewModes = { "Đứng", "Chạy", "Quay", "Đánh thường", "Kỹ năng", "Chuỗi đòn" };
    private static readonly Color PanelColor = new(.075f, .11f, .101f);
    private static readonly Color HeaderColor = new(.053f, .084f, .075f);
    private static readonly Color AccentColor = new(.52f, .79f, .66f);
    private static readonly Color MutedColor = new(.73f, .81f, .77f);
    private Vector2 panelScroll;
    private int skillGroup;
    private float previewCameraSize;
    private GUIStyle skillButton, titleLabel, sectionLabel, hintLabel, smallButton;

    private float DockWidth => Mathf.Min(Screen.width * .46f, Mathf.Clamp(Screen.width * .31f, 230, 350));
    private float HudHeight => Screen.width < 660 ? 105 : 88;

    private void UpdatePreviewViewport()
    {
        float dock = DockWidth / Mathf.Max(1, Screen.width);
        float top = Mathf.Min(.45f, HudHeight / Mathf.Max(1, Screen.height));
        previewCamera.rect = new Rect(dock, 0, 1 - dock, 1 - top);
        float aspect = Mathf.Max(.01f, Screen.width * (1 - dock) / Mathf.Max(1, Screen.height * (1 - top)));
        previewCamera.orthographicSize = previewCameraSize * Mathf.Max(1, 1.08f / aspect);
    }

    public static string VisibleSkillName(int index)
    {
        string catalogName = SkillName(index);
        return catalogName[(catalogName.IndexOf('·') + 1)..].Trim();
    }

    private void OnGUI()
    {
        if (actor == null) return;
        BuildStyles();
        float dock = DockWidth;
        Fill(new Rect(0, 0, dock, Screen.height), PanelColor);
        Fill(new Rect(dock, 0, Screen.width - dock, HudHeight), HeaderColor);
        Fill(new Rect(dock - 1, 0, 1, Screen.height), new Color(.19f, .31f, .26f));

        GUILayout.BeginArea(new Rect(10, 10, dock - 20, Screen.height - 18));
        GUILayout.Label("HEXCORE", sectionLabel, GUILayout.Height(23));
        int mode = GUILayout.SelectionGrid(freeMode ? 0 : 1, new[] { "Tự do", "Trình diễn" }, 2, smallButton);
        if ((mode == 0) != freeMode)
        {
            SetFreeMode(mode == 0);
            RestartPlayback();
            GUIUtility.ExitGUI();
        }

        panelScroll = GUILayout.BeginScrollView(panelScroll, false, false);
        GUILayout.Space(8);
        GUILayout.Label("Nhóm kỹ năng", sectionLabel);
        int group = GUILayout.SelectionGrid(skillGroup, Groups, 2, smallButton);
        if (group != skillGroup)
        {
            skillGroup = group;
            int firstSkill = System.Array.FindIndex(HexcorePreviewCatalog.Skills,
                item => item.Form == (group == 4 ? -1 : group));
            PickSkill(firstSkill);
            GUIUtility.ExitGUI();
        }
        GUILayout.Space(5);
        for (int i = 0; i < SkillCount; i++)
        {
            if (PrimaryForm(i) != (skillGroup == 4 ? -1 : skillGroup)) continue;
            if (PanelButton(VisibleSkillName(i), i == skillIndex, skillButton))
            {
                PickSkill(i);
                GUIUtility.ExitGUI();
            }
        }

        if (skillGroup == 4)
        {
            GUILayout.Space(6);
            GUILayout.Label("Tạo hình", sectionLabel);
            int form = GUILayout.SelectionGrid(formIndex, Names, 2, smallButton);
            if (form != formIndex) PickSkill(skillIndex, form);
        }
        GUILayout.Space(8);
        DrawPreviewControls();
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(dock + 14, 9, Mathf.Max(10, Screen.width - dock - 27), HudHeight - 11));
        GUILayout.Label(Names[formIndex] + "  /  " + (freeMode ? "Tự do" : "Trình diễn") + "  /  " + DirectionLabels[aim], hintLabel);
        GUILayout.Label(VisibleSkillName(skillIndex), titleLabel);
        GUILayout.Label(freeMode ? "WASD / Mũi tên  ·  Space thi triển  ·  R về điểm đầu" : phase ?? "", hintLabel);
        GUILayout.EndArea();
    }

    private void PickSkill(int index, int coreForm = -1)
    {
        int primary = PrimaryForm(index);
        int nextForm = primary >= 0 ? primary : coreForm >= 0 ? coreForm : formIndex;
        if (freeCasting) CommitCastPosition();
        SelectForm(nextForm);
        skillIndex = index;
        if (!freeMode && selection != Preview.Chain) selection = Preview.Q1;
        ConfigureEffect();
        RestartPlayback();
    }

    private void DrawPreviewControls()
    {
        if (!freeMode)
        {
            GUILayout.Label("Chế độ trình diễn", sectionLabel);
            int mode = GUILayout.SelectionGrid((int)selection, PreviewModes, 3, smallButton);
            if (mode != (int)selection)
            {
                selection = (Preview)mode;
                RestartPlayback();
            }
            DrawDirectionGrid("Hướng di chuyển", true);
        }
        DrawDirectionGrid("Hướng đánh", false);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(freeMode ? "Thử tại chỗ" : "Phát lại", smallButton)) Replay();
        if (freeMode && GUILayout.Button("Về điểm đầu", smallButton)) ResetFreePlay();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(paused ? "Tiếp tục" : "Tạm dừng", smallButton)) paused = !paused;
        if (GUILayout.Button(halfSpeed ? "0,5×" : "1×", smallButton)) halfSpeed = !halfSpeed;
        GUILayout.EndHorizontal();
        if (!freeMode)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Khung trước", smallButton)) ReviewAt(Mathf.Max(0, clock - FrameTime), showEffects);
            if (GUILayout.Button("Khung sau", smallButton)) ReviewAt(clock + FrameTime, showEffects);
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        showEffects = GUILayout.Toggle(showEffects, "VFX");
        bool sound = GUILayout.Toggle(hear, "SFX");
        if (hear && !sound) audioSource.Stop();
        hear = sound;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        shake = GUILayout.Toggle(shake, "Rung");
        hitStop = GUILayout.Toggle(hitStop, "Dừng va chạm");
        GUILayout.EndHorizontal();
        if (!hitStop) hold = 0;
        if (skillIndex == 2)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = !freeCasting;
            bool incoming = GUILayout.Toggle(parry, "Có đòn giả");
            GUI.enabled = enabled;
            if (incoming != parry)
            {
                parry = incoming;
                if (!freeMode) RestartPlayback();
            }
        }
        if (paused && !freeMode)
        {
            scrubbing = true;
            Render();
            scrubbing = false;
        }
    }

    private void DrawDirectionGrid(string label, bool movement)
    {
        GUILayout.Label(label, sectionLabel);
        bool enabled = GUI.enabled;
        GUI.enabled = !freeCasting;
        int current = movement ? locomotionDirection : aim;
        int changed = GUILayout.SelectionGrid(current, DirectionLabels, 4, smallButton);
        GUI.enabled = enabled;
        if (changed == current) return;
        if (movement) locomotionDirection = changed;
        else if (freeMode) FaceFreeDirection(changed);
        else aim = changed;
        if (!freeMode) RestartPlayback();
    }

    private bool PanelButton(string caption, bool active, GUIStyle style)
    {
        Color first = GUI.backgroundColor;
        GUI.backgroundColor = active ? AccentColor : new Color(.66f, .74f, .70f);
        bool pressed = GUILayout.Button(caption, style, GUILayout.MinHeight(31));
        GUI.backgroundColor = first;
        return pressed;
    }

    private void BuildStyles()
    {
        if (skillButton != null)
        {
            titleLabel.fontSize = Screen.width < 660 ? 17 : 21;
            return;
        }
        skillButton = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, wordWrap = true, fontSize = 13 };
        smallButton = new GUIStyle(GUI.skin.button) { wordWrap = true, fontSize = 12, stretchWidth = true, fixedHeight = 25 };
        sectionLabel = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 12 };
        sectionLabel.normal.textColor = AccentColor;
        titleLabel = new GUIStyle(GUI.skin.label) { fontSize = Screen.width < 660 ? 17 : 21, fontStyle = FontStyle.Bold, wordWrap = true };
        titleLabel.normal.textColor = Color.white;
        hintLabel = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };
        hintLabel.normal.textColor = MutedColor;
    }

    private static void Fill(Rect rect, Color tint)
    {
        Color first = GUI.color;
        GUI.color = tint;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = first;
    }
}
