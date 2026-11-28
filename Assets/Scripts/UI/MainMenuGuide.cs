using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class MainMenuController
{
    private const int GuideWidth = 1530;
    private const int GuideHeight = 900;
    private static readonly string[] GuideTabs = { "Tổng quan", "Điều khiển", "Nhân vật", "Quái vật" };
    private static readonly string[] EnemyKeys = { "all", "normal", "bow", "elite", "boss" };
    private static readonly string[] EnemyLabels = { "Tất cả", "Lính thường", "Cung thủ", "Tinh nhuệ", "Boss" };
    private static readonly HashSet<string> PreviewBranches = new() { "shotgun", "dao", "fire" };
    private static readonly HashSet<string> PreviewEnemies = new() { "normal-brown", "normal-green", "bow-snow" };

    [Serializable] private class Catalog
    {
        public Entry starter;
        public Family[] families;
        public Entry[] enemies;
        public Biome[] maps;
    }
    [Serializable] private class Family { public string name, tone; public Entry[] branches; }
    [Serializable] private class Biome { public string name, lead, story, tone; }
    [Serializable] private class Entry
    {
        public string id, name, sprite, family, type, tone, intro, weapon, style, signature, behavior, counter;
        public string[] stats;
    }

    private Catalog catalog;
    private RectTransform guideBody, detail;
    private RawImage idlePortrait;
    private Text pauseStatus;
    private Button pauseButton, resumeButton;
    private int enemyFilter;
    private bool pausePreview;

    private void OpenGuide()
    {
        pausePreview = false;
        enemyFilter = 0;
        ShowGuide(0);
    }

    private void ShowGuide(int tab)
    {
        OpenWindow("Hướng dẫn", GuideWidth, GuideHeight);
        Box(window, "Dải tab", 2, 80, GuideWidth - 4, 76, Plate);
        for (int i = 0; i < GuideTabs.Length; i++)
        {
            int next = i;
            var button = Control(window, GuideTabs[i], 31 + 369 * i, 91, 360, 52, () => ShowGuide(next), i == tab);
            if (i == tab) Focus(button);
        }
        guideBody = Rect("Nội dung " + GuideTabs[tab], window, 30, 171, 1470, 650);
        switch (tab)
        {
            case 0: BuildOverview(); break;
            case 1: BuildControls(); break;
            case 2: BuildCharacters(); break;
            case 3: BuildBestiary(); break;
        }
    }

    private void BuildOverview()
    {
        var page = ScrollPage(guideBody, 880);
        Picture(page, "Banner chiến trường", "Home/Battlefield", 0, 0, 1460, 228).uvRect =
            new UnityEngine.Rect(0, .36f, 1, .277f);
        Box(page, "Nền chữ banner", 0, 111, 1460, 117, new Color(.12f, .07f, .14f, .76f));
        Label(page, "BROTATO 2D", 56, 20, 113, 720, 66, Gold, true);
        Label(page, "Hành động sinh tồn góc nhìn từ trên xuống. Điều khiển chiến binh khăn đỏ qua bốn vùng bị goblin bao vây.",
            22, 24, 179, 1160, 40, Ink);
        OverviewCard(page, 0, 242, "◈", "Bối cảnh hành trình",
            "Mở đường từ Forest qua Moon và Inferno tới Tundra.", Gold);
        OverviewCard(page, 736, 242, "◎", "Mục tiêu", "Di chuyển, né đòn và sống sót 30 giây ở mỗi vùng.", Gold);
        OverviewCard(page, 0, 324, "✓", "Chiến thắng", "Sống sót hết đợt cuối ở Tundra.", ColorOf("#BFF183"));
        OverviewCard(page, 736, 324, "!", "Thất bại", "HP về 0 thì Game Over. Dùng Chơi lại để bắt đầu lượt mới.", ColorOf("#FFAB9C"));
        Label(page, "Bốn vùng trên đường sinh tồn", 28, 1, 412, 1400, 42, Gold, true);
        for (int i = 0; i < catalog.maps.Length; i++)
        {
            var biome = catalog.maps[i];
            float x = 736 * (i % 2);
            float y = 462 + 209 * (i / 2);
            Color accent = ColorOf(biome.tone);
            Box(page, biome.name, x, y, 720, 195, Plate);
            Box(page, "Viền " + biome.name, x, y, 4, 195, accent);
            Picture(page, "Toàn cảnh " + biome.name, "Maps/" + biome.name.ToLowerInvariant(), x + 11, y + 10, 280, 168);
            Label(page, biome.name, 28, x + 306, y + 10, 398, 36, accent, true);
            Label(page, biome.lead, 21, x + 306, y + 51, 400, 53, Ink, true);
            Label(page, biome.story, 18, x + 306, y + 101, 398, 87, Muted);
        }
    }

    private void OverviewCard(Transform parent, float x, float y, string icon, string label, string body, Color tone)
    {
        Box(parent, label, x, y, 720, 70, Plate);
        Box(parent, "Điểm nhấn", x + 12, y + 13, 38, 38, tone);
        Label(parent, icon, 22, x + 12, y + 13, 38, 38, Deep, true, TextAnchor.MiddleCenter);
        Label(parent, label, 22, x + 65, y + 7, 642, 26, tone, true);
        Label(parent, body, 18, x + 65, y + 35, 642, 27, Muted);
    }

    private void BuildControls()
    {
        const int right = 748;
        ControlCard("Di chuyển", 0, 0, 720, 272);
        Keycap("W", "Lên", 313, 72, Gold);
        Keycap("A", "Trái", 195, 169, Gold);
        Keycap("S", "Xuống", 313, 169, Gold);
        Keycap("D", "Phải", 431, 169, Gold);
        ControlCard("Kỹ năng", right, 0, 720, 272);
        var violet = ColorOf("#CBB5FF");
        Keycap("Q", "Lõi 1", right + 97, 110, violet);
        Keycap("E", "Lõi 2", right + 314, 110, violet);
        Keycap("R", "Chiêu cuối", right + 531, 110, violet);

        ControlCard("Tương tác trong trận", 0, 291, 720, 355);
        InteractionRow(364, "Chuột", "Ngắm và tự bắn theo hướng ngắm.");
        InteractionRow(445, "Chọn", "Bấm thẻ để chọn nâng cấp.");
        InteractionRow(526, "Chơi lại", "Bắt đầu lượt mới ở màn hình kết quả.");

        ControlCard("Tạm dừng trận", right, 291, 720, 355);
        Picture(guideBody, "Forest minh họa", "Maps/forest", right + 23, 363, 674, 180).uvRect =
            new UnityEngine.Rect(0, .25f, 1, .445f);
        Box(guideBody, "Nền trạng thái", right + 236, 420, 248, 57, Deep);
        pauseStatus = Label(guideBody, "", 23, right + 236, 420, 248, 57, Gold, true, TextAnchor.MiddleCenter);
        pauseButton = Control(guideBody, "Ⅱ   Tạm dừng", right + 23, 565, 326, 55, () => UpdatePausePreview(true), true);
        resumeButton = Control(guideBody, "▶   Tiếp tục", right + 371, 565, 326, 55, () => UpdatePausePreview(false));
        RefreshPausePreview();
    }

    private void ControlCard(string name, float x, float y, float w, float h)
    {
        Box(guideBody, name, x, y, w, h, Edge);
        Box(guideBody, "Mặt thẻ", x + 1, y + 1, w - 2, h - 2, Plate);
        Label(guideBody, name, 32, x + 21, y + 15, w - 42, 49, Gold, true);
    }

    private void Keycap(string key, string label, float x, float y, Color color)
    {
        Box(guideBody, "Viền phím " + key, x, y + 5, 88, 63, ColorOf("#85573E"));
        Box(guideBody, "Phím " + key, x, y, 88, 62, color);
        Label(guideBody, key, 36, x, y, 88, 62, Deep, true, TextAnchor.MiddleCenter);
        Label(guideBody, label, 19, x - 30, y + 69, 148, 27, Ink, true, TextAnchor.MiddleCenter);
    }

    private void InteractionRow(float y, string key, string copy)
    {
        Box(guideBody, key, 24, y, 670, 64, Deep);
        Label(guideBody, key, 21, 37, y + 13, 125, 38, Gold, true);
        Label(guideBody, copy, 21, 171, y + 11, 505, 46, Ink);
    }

    private void UpdatePausePreview(bool pause)
    {
        pausePreview = pause;
        RefreshPausePreview();
        Focus(pause ? resumeButton : pauseButton);
    }

    private void RefreshPausePreview()
    {
        pauseStatus.text = pausePreview ? "Đang tạm dừng" : "Đang chơi";
        pauseButton.interactable = !pausePreview;
        resumeButton.interactable = pausePreview;
    }

    private RectTransform ScrollPage(Transform parent, float length)
    {
        var scrollRect = Rect("Vùng cuộn", parent, 0, 0, 1470, 650);
        var receiver = scrollRect.gameObject.AddComponent<Image>();
        receiver.color = Color.clear;
        var scroll = scrollRect.gameObject.AddComponent<ScrollRect>();
        var viewport = Rect("Viewport", scrollRect, 0, 0, 1460, 650);
        viewport.gameObject.AddComponent<RectMask2D>();
        var page = Rect("Trang", viewport, 0, 0, 1460, length);
        var rail = Box(scrollRect, "Thanh cuộn", 1462, 0, 8, 650, Plate);
        var grip = Box(rail.transform, "Tay cuộn", 0, 0, 8, 70, Gold);
        var bar = rail.gameObject.AddComponent<Scrollbar>();
        bar.handleRect = grip.rectTransform;
        bar.targetGraphic = grip;
        bar.direction = Scrollbar.Direction.BottomToTop;
        rail.raycastTarget = true;
        grip.raycastTarget = true;
        scroll.viewport = viewport;
        scroll.content = page;
        scroll.horizontal = false;
        scroll.verticalScrollbar = bar;
        scroll.scrollSensitivity = 43f;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.verticalNormalizedPosition = 1f;
        return page;
    }

    private void BuildCharacters()
    {
        CollectionBase();
        Progress("Hình thái đã khám phá", PreviewBranches.Count, 10);
        EntryCard(catalog.starter, false, false, 14, 89, 840, 75);
        for (int row = 0; row < catalog.families.Length; row++)
        {
            var family = catalog.families[row];
            float y = 177 + row * 150;
            var tone = EntryTone(family.tone);
            Label(guideBody, family.name, 22, 14, y, 820, 36, tone, true);
            float width = (844 - 10 * (family.branches.Length - 1)) / family.branches.Length;
            for (int i = 0; i < family.branches.Length; i++)
            {
                var entry = family.branches[i];
                EntryCard(entry, false, !PreviewBranches.Contains(entry.id), 14 + i * (width + 10), y + 38, width, 100);
            }
        }
        ShowEntry(catalog.starter, false, false);
    }

    private void BuildBestiary()
    {
        CollectionBase();
        Progress("Quái đã gặp", PreviewEnemies.Count, catalog.enemies.Length);
        for (int i = 0; i < EnemyLabels.Length; i++)
        {
            int next = i;
            Control(guideBody, EnemyLabels[i], 14 + i * 172, 91, 160, 43, () =>
            {
                enemyFilter = next;
                ShowGuide(3);
            }, i == enemyFilter);
        }
        Entry first = null;
        int card = 0;
        foreach (var enemy in catalog.enemies)
        {
            if (enemyFilter != 0 && enemy.type != EnemyKeys[enemyFilter]) continue;
            first ??= enemy;
            EntryCard(enemy, true, !PreviewEnemies.Contains(enemy.id),
                14 + (card % 4) * 211, 149 + (card / 4) * 119, 200, 108);
            card++;
        }
        if (first != null) ShowEntry(first, true, !PreviewEnemies.Contains(first.id));
    }

    private void CollectionBase()
    {
        Box(guideBody, "Khay bộ sưu tập", 0, 0, 872, 650, Plate);
        detail = Rect("Chi tiết", guideBody, 888, 0, 582, 650);
    }

    private void Progress(string name, int current, int total)
    {
        Box(guideBody, "Tiến độ", 14, 13, 844, 62, Deep);
        Label(guideBody, name, 22, 26, 18, 710, 35, Gold, true);
        Label(guideBody, current + " / " + total, 21, 730, 18, 110, 35, Gold, true, TextAnchor.UpperRight);
        Box(guideBody, "Mốc tối đa", 26, 55, 816, 9, ColorOf("#151023"));
        Box(guideBody, "Đã thấy", 26, 55, 816f * current / total, 9, Gold);
    }

    private void EntryCard(Entry entry, bool monster, bool locked, float x, float y, float w, float h)
    {
        string name = monster && locked ? "???" : entry.name;
        var background = Box(guideBody, name, x, y, w, h, locked ? ColorOf("#453A51") : Deep);
        background.raycastTarget = true;
        var rect = background.rectTransform;
        float size = entry == catalog.starter ? 52 : 67;
        Portrait(rect, entry, (w - size) / 2, 3, size, locked);
        Label(rect, name, 18, 3, h - 32, w - 6, 29, Ink, true, TextAnchor.MiddleCenter);
        if (locked) LockIcon(rect, w - 26, 10);
        AttachButton(background, () => ShowEntry(entry, monster, locked), Color.white);
    }

    private RawImage Portrait(Transform parent, Entry entry, float x, float y, float size, bool locked)
    {
        var image = Picture(parent, "Sprite " + entry.sprite, "Portraits/" + entry.sprite, x, y, size, size);
        image.uvRect = new UnityEngine.Rect(0, 0, .25f, 1);
        if (locked) image.color = Color.black;
        return image;
    }

    private void ShowEntry(Entry entry, bool monster, bool locked)
    {
        foreach (Transform child in detail)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        Color tone = EntryTone(monster ? entry.type : entry.tone);
        Box(detail, "Nền chi tiết", 0, 0, 582, 650, Plate);
        Box(detail, "Khung hình thái", 14, 14, 554, 216, ColorOf("#2D243D"));
        Box(detail, "Viền hình thái", 14, 228, 554, 3, tone);
        var picture = Portrait(detail, entry, 192, 22, 198, locked);
        idlePortrait = locked ? null : picture;
        if (locked) LockIcon(detail, 534, 31);
        if (!locked && (entry.sprite == "mage" || entry.sprite == "player"))
        {
            bool mage = entry.sprite == "mage";
            var weapon = Picture(detail, "Vũ khí", "Portraits/" + (mage ? "staff" : "pistol"),
                306, 119, mage ? 92 : 63, mage ? 25 : 44);
            if (mage) weapon.rectTransform.localRotation = Quaternion.Euler(0, 0, 34);
        }
        string name = monster && locked ? "???" : entry.name;
        Label(detail, name, 33, 20, 246, 545, 43, tone, true);
        if (locked)
        {
            Label(detail, "Đang khóa", 22, 20, 303, 545, 35, Gold, true);
            Label(detail, monster ? "Gặp quái này trong trận để mở mục ghi chép."
                : "Chọn và tiến hóa thành " + entry.name + " trong trận để khám phá hình thái.",
                22, 20, 349, 535, 116, Muted);
            return;
        }
        Label(detail, entry.intro, 21, 20, 297, 540, 68, Ink);
        float y = 375;
        if (entry.stats != null && entry.stats.Length > 0)
        {
            Label(detail, string.Join("  ·  ", entry.stats), 18, 20, y, 540, 42, Gold, true);
            y += 52;
        }
        string category = monster ? "Tấn công" : "Vũ khí";
        DetailRow(category, monster ? entry.behavior : entry.weapon, y);
        DetailRow(monster ? "Đặc biệt" : "Phong cách", monster ? entry.signature : entry.style, y + 70);
        DetailRow(monster ? "Đối phó" : "Đặc trưng", monster ? entry.counter : entry.signature, y + 140);
    }

    private void DetailRow(string label, string value, float y)
    {
        Label(detail, label, 19, 20, y, 116, 60, Gold, true);
        Label(detail, value, 18, 142, y, 423, Mathf.Min(69, 650 - y), Muted);
    }

    private static void LockIcon(Transform root, float x, float y)
    {
        Box(root, "Quai khóa", x + 3, y, 13, 16, Gold);
        Box(root, "Lỗ quai", x + 6, y + 3, 7, 10, Deep);
        Box(root, "Ổ khóa", x, y + 9, 19, 13, Gold);
    }

    private void AnimatePortrait()
    {
        if (idlePortrait == null) return;
        int column = motionEnabled ? Mathf.FloorToInt(Time.unscaledTime * 4) % 4 : 0;
        var rect = new UnityEngine.Rect(column * .25f, 0, .25f, 1);
        if (idlePortrait.uvRect != rect) idlePortrait.uvRect = rect;
    }

    private static Color EntryTone(string type) => type switch
    {
        "melee" or "bow" => ColorOf("#A9EA8C"),
        "mage" or "elite" => ColorOf("#C9B3FF"),
        "boss" => ColorOf("#FFAAA0"),
        _ => Gold
    };
}
