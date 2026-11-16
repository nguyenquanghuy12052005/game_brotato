using System;
using UnityEngine;

public sealed class MeleeSkillDemo : MonoBehaviour
{
    [Serializable]
    private sealed class Skill
    {
        [SerializeField] internal string title;
        [SerializeField] internal string group;
        [SerializeField] internal string description;
        [SerializeField] internal GameObject prefab;
    }

    [SerializeField] private Camera demoCamera;
    [SerializeField] private Skill[] skills;
    [SerializeField] private MeleeDemoCombat combat;
    private int selected = -1;
    private Vector2 scroll;
    private int ultimateBranch;
    private float zoom = 1f;

    public int SkillCount => skills == null ? 0 : skills.Length;
    public int SelectedIndex => selected;

    private void Start()
    {
        if (SkillCount > 0) SelectSkill(0);
    }

    public void SelectSkill(int index)
    {
        if (index < 0 || index >= SkillCount) return;
        selected = index;
        combat.Show(index, skills[index].prefab, ultimateBranch);
    }

    private void Update()
    {
        if (demoCamera == null) return;
        float scale = Mathf.Clamp(Screen.height / 720f, .45f, 1.2f);
        float sidebar = Mathf.Min(300f * scale, Screen.width * .34f);
        float viewHeight = Mathf.Max(40f, Screen.height - 220f * scale);
        demoCamera.rect = new Rect(sidebar / Screen.width, 100f * scale / Screen.height,
            1f - sidebar / Screen.width, viewHeight / Screen.height);
        demoCamera.orthographicSize = Mathf.Max(3.7f, 3.7f / demoCamera.aspect) / zoom;
    }

    private void OnGUI()
    {
        float scale = Mathf.Clamp(Screen.height / 720f, .45f, 1.2f);
        float width = Screen.width / scale;
        float height = Screen.height / scale;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
        float sidebar = Mathf.Min(300f, width * .34f);
        // Camera chỉ xóa vùng sân. Xóa riêng vùng UI mỗi Repaint để chữ không lưu vệt.
        Color oldColor = GUI.color;
        GUI.color = new Color(.11f, .15f, .13f);
        GUI.DrawTexture(new Rect(0, 0, width, 120), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(0, height - 100, width, 100), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(0, 120, sidebar, Mathf.Max(0, height - 220)), Texture2D.whiteTexture);
        GUI.color = oldColor;
        GUI.Box(new Rect(0, 0, sidebar, height), "");
        GUILayout.BeginArea(new Rect(12, 14, sidebar - 24, height - 28));
        GUILayout.Label("DEMO CẬN CHIẾN");
        GUILayout.Space(10);
        scroll = GUILayout.BeginScrollView(scroll);
        string lastGroup = null;
        for (int i = 0; i < SkillCount; i++)
        {
            if (lastGroup != skills[i].group)
            {
                GUILayout.Space(14);
                GUILayout.Label(skills[i].group);
                lastGroup = skills[i].group;
            }
            Color previous = GUI.backgroundColor;
            if (i == selected) GUI.backgroundColor = new Color(.65f, .85f, .65f);
            if (GUILayout.Button(skills[i].title, GUILayout.MinHeight(36))) SelectSkill(i);
            GUI.backgroundColor = previous;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        if (selected < 0) return;
        GUILayout.BeginArea(new Rect(sidebar + 20, 18, width - sidebar - 40, 100));
        GUILayout.Label(skills[selected].title, new GUIStyle(GUI.skin.label) { fontSize = 24 });
        GUILayout.Label(skills[selected].description, new GUIStyle(GUI.skin.label) { wordWrap = true });
        if (selected >= 12)
        {
            int branch = GUILayout.Toolbar(ultimateBranch, new[] { "Sát thủ", "Đao khách", "Kiếm khí" });
            if (branch != ultimateBranch) { ultimateBranch = branch; SelectSkill(selected); }
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(sidebar + 20, height - 100, width - sidebar - 40, 90));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Phát lại", GUILayout.Height(32))) combat.Replay();
        if (GUILayout.Button(combat.Paused ? "Tiếp tục" : "Tạm dừng", GUILayout.Height(32)))
        {
            combat.Paused = !combat.Paused;
        }
        combat.Repeat = GUILayout.Toggle(combat.Repeat, "Tự phát lại");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Thu phóng", GUILayout.Width(90));
        zoom = GUILayout.HorizontalSlider(zoom, .7f, 1.1f);
        GUILayout.EndHorizontal();
        GUILayout.Label("WASD để di chuyển • Kỹ năng tự minh họa • Bấm vào sân Demo để nhận bàn phím");
        GUILayout.EndArea();
    }
}

