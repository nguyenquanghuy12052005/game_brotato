using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public sealed partial class MainMenuController : MonoBehaviour
{
    private const string Root = "BrotatoMenu/";
    private const string GameplayScene = "Assets/Scenes/GameScene.unity";
    private const string Preferences = "Brotato.MainMenu.";
    private static readonly Color Gold = ColorOf("#FFD575");
    private static readonly Color Ink = ColorOf("#FFF2DF");
    private static readonly Color Muted = ColorOf("#D5C8DA");
    private static readonly Color Plate = ColorOf("#392A44");
    private static readonly Color Edge = ColorOf("#705471");
    private static readonly Color Deep = ColorOf("#241B31");

    [SerializeField] private Canvas canvas;
    [SerializeField] private EventSystem eventSystem;

    private RectTransform frame, art, modal, window;
    private RawImage[] streaks;
    private RectTransform[] streakRects;
    private Button[] homeButtons;
    private Font font;
    private AudioSource music;
    private AudioClip confirmSound;
    private GameObject previousFocus;
    private bool loading, motionEnabled;
    private Vector2 lastScreen;

    [Serializable] private class HomeLayout { public HomeItem[] items; }
    [Serializable] private class HomeItem { public string name; public int x, y, width, height; }

    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        music = GetComponent<AudioSource>();
        music.clip = Resources.Load<AudioClip>(Root + "Home/Music");
        confirmSound = Resources.Load<AudioClip>(Root + "Home/Confirm");
        music.loop = true;
        music.playOnAwake = false;
        music.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(Preferences + "Volume", .58f));
        music.mute = PlayerPrefs.GetInt(Preferences + "Music", 1) == 0;
        motionEnabled = PlayerPrefs.GetInt(Preferences + "Motion", 1) == 1;
        catalog = Read<Catalog>("Guide");

        frame = Rect("Khung 16:9", canvas.transform, 0, 0, 1920, 1080);
        frame.anchorMin = frame.anchorMax = new Vector2(.5f, .5f);
        frame.pivot = new Vector2(.5f, .5f);
        frame.anchoredPosition = Vector2.zero;
        frame.gameObject.AddComponent<RectMask2D>();
        BuildHome();
        ScaleFrame();
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        ScaleFrame();
        if (music.clip != null) music.Play();
        Focus(homeButtons[0]);
    }

    private void Update()
    {
        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height) ScaleFrame();
        UpdateBackdrop();
        AnimatePortrait();
        if (!loading && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
        {
            if (modal != null) CloseWindow();
            else OpenExit();
        }
    }

    private void ScaleFrame()
    {
        lastScreen = new Vector2(Screen.width, Screen.height);
        var size = ((RectTransform)canvas.transform).rect.size;
        frame.localScale = Vector3.one * Mathf.Min(size.x / 1920f, size.y / 1080f);
    }

    private void BuildHome()
    {
        art = Picture(frame, "Chiến trường", "Home/Battlefield", -24, -14, 1968, 1108).rectTransform;
        Picture(frame, "Lớp tương phản", "Home/Scrim", 0, 0, 1920, 1080);
        streaks = new RawImage[6];
        streakRects = new RectTransform[streaks.Length];
        for (int i = 0; i < streaks.Length; i++)
        {
            var glow = Picture(frame, "Tia lửa", null, 0, 0, 60 - i * 5, 2);
            glow.texture = Texture2D.whiteTexture;
            glow.rectTransform.localRotation = Quaternion.Euler(0, 0, i % 2 == 0 ? -7 : 12);
            streaks[i] = glow;
            streakRects[i] = glow.rectTransform;
        }

        var home = Read<HomeLayout>("Home/Layout");
        var labels = new[] { "Bắt đầu", "Chơi tiếp", "Chọn màn", "Hướng dẫn", "Cài đặt", "Thoát" };
        var actions = new Action[] { OpenStart, null, () => OpenStages(0), OpenGuide, OpenSettings, OpenExit };
        homeButtons = new Button[labels.Length];
        for (int i = 0; i < home.items.Length; i++)
        {
            var item = home.items[i];
            var image = Picture(frame, i == 0 ? "Logo BROTATO 2D" : labels[i - 1],
                "Home/" + item.name, item.x, item.y, item.width, item.height);
            if (i == 0) continue;
            image.raycastTarget = true;
            var button = AttachButton(image, actions[i - 1], Color.white);
            if (i == 2) button.interactable = false;
            homeButtons[i - 1] = button;
        }
        var order = new[] { homeButtons[0], homeButtons[2], homeButtons[3], homeButtons[4], homeButtons[5] };
        for (int i = 0; i < order.Length; i++)
        {
            var previous = order[(i + order.Length - 1) % order.Length];
            var next = order[(i + 1) % order.Length];
            order[i].navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit, selectOnLeft = previous, selectOnRight = next,
                selectOnUp = previous, selectOnDown = next
            };
        }
    }

    private void UpdateBackdrop()
    {
        bool moving = motionEnabled && modal == null;
        Vector2 offset = Vector2.zero;
        if (moving && Mouse.current != null)
        {
            Vector2 cursor = Mouse.current.position.ReadValue();
            offset = new Vector2(cursor.x / Mathf.Max(1, Screen.width) - .5f,
                cursor.y / Mathf.Max(1, Screen.height) - .5f) * 15f;
        }
        art.anchoredPosition = Vector2.Lerp(art.anchoredPosition, new Vector2(-24, 14) + offset,
            Mathf.Min(1f, Time.unscaledDeltaTime * 8f));

        float seconds = Time.unscaledTime;
        for (int i = 0; i < streaks.Length; i++)
        {
            streaks[i].enabled = moving;
            if (!moving) continue;
            float phase = Mathf.Repeat(seconds * (.39f + i * .024f) + i * .18f, 1f);
            streakRects[i].anchoredPosition = new Vector2(650 + phase * 420, -518 - i * 14 + phase * 64);
            streaks[i].color = new Color(1f, i % 2 == 0 ? .84f : .58f, .35f, Mathf.Sin(phase * Mathf.PI) * .63f);
        }
    }

    private void OpenStart()
    {
        OpenWindow("Bắt đầu", 680, 285);
        Label(window, "Forest · Hành trình mới", 30, 70, 114, 540, 50, Ink);
        WindowAction("Vào Forest", 422, 205, 215, LoadGame);
    }

    private void OpenStages(int index)
    {
        OpenWindow("Chọn màn", 1030, 665);
        var biome = catalog.maps[index];
        for (int i = 0; i < catalog.maps.Length; i++)
        {
            int next = i;
            Control(window, catalog.maps[i].name, 40 + i * 239, 104, 231, 56, () => OpenStages(next), i == index);
        }
        Picture(window, biome.name, "Maps/" + biome.name.ToLowerInvariant(), 40, 186, 520, 312);
        Label(window, biome.name, 37, 588, 205, 390, 56, Gold, true);
        Label(window, biome.lead, 24, 588, 275, 386, 68, Ink, true);
        Label(window, biome.story, 22, 588, 352, 386, 125, Muted);
        Label(window, index == 0 ? "Có thể vào màn" : "Đang khóa", 21, 40, 526, 505, 40, Muted, true);
        WindowAction("Vào màn", 770, 591, 213, index == 0 ? LoadGame : null);
    }

    private void OpenSettings()
    {
        OpenWindow("Cài đặt", 820, 495);
        Label(window, "Âm lượng menu", 24, 42, 110, 340, 40, Ink, true);
        var value = Label(window, Mathf.RoundToInt(music.volume * 100) + "%", 24, 657, 110, 112, 40, Gold, true);
        var sliderObject = Rect("Thanh âm lượng", window, 45, 164, 730, 45);
        var sliderHitArea = Box(sliderObject, "Vùng kéo âm lượng", 0, 0, 730, 45, Color.clear);
        sliderHitArea.raycastTarget = true;
        Box(sliderObject, "Rãnh", 0, 13, 730, 17, Edge);
        var handle = Box(sliderObject, "Tay kéo", 0, 1, 24, 41, Gold);
        handle.rectTransform.anchorMin = new Vector2(0, 1);
        handle.rectTransform.anchorMax = new Vector2(0, 1);
        var slider = sliderObject.gameObject.AddComponent<Slider>();
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.value = music.volume;
        slider.onValueChanged.AddListener(amount =>
        {
            music.volume = amount;
            value.text = Mathf.RoundToInt(amount * 100f) + "%";
            PlayerPrefs.SetFloat(Preferences + "Volume", amount);
        });
        Control(window, music.mute ? "Bật nhạc nền" : "Tắt nhạc nền", 45, 235, 730, 55, () =>
        {
            music.mute = !music.mute;
            PlayerPrefs.SetInt(Preferences + "Music", music.mute ? 0 : 1);
            OpenSettings();
        });
        Control(window, motionEnabled ? "Giảm chuyển động chiến trường" : "Bật chuyển động chiến trường",
            45, 306, 730, 55, () =>
            {
                motionEnabled = !motionEnabled;
                PlayerPrefs.SetInt(Preferences + "Motion", motionEnabled ? 1 : 0);
                OpenSettings();
            });
    }

    private void OpenExit()
    {
        OpenWindow("Thoát trò chơi?", 700, 300);
        Label(window, "Bạn muốn đóng BROTATO 2D?", 29, 45, 115, 570, 55, Ink);
        WindowAction("Thoát game", 432, 220, 224, Application.Quit);
    }

    private void LoadGame()
    {
        if (loading) return;
        if (!Application.CanStreamedLevelBeLoaded(GameplayScene))
        {
            OpenWindow("Chưa mở được Forest", 780, 305);
            Label(window, "GameScene cần có trong Scene List của Build Profiles.", 24, 45, 116, 685, 74, Ink);
            return;
        }
        OpenWindow("Đang vào Forest", 685, 250);
        Label(window, "Bắt đầu hành trình", 27, 45, 115, 590, 55, Ink);
        loading = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(GameplayScene);
    }

    private void OpenWindow(string heading, float width, float height)
    {
        if (modal == null) previousFocus = eventSystem.currentSelectedGameObject;
        foreach (var button in homeButtons) button.interactable = false;
        DiscardModal();
        modal = Box(frame, "Modal", 0, 0, 1920, 1080, new Color(.06f, .035f, .08f, .83f)).rectTransform;
        modal.GetComponent<Image>().raycastTarget = true;
        window = Box(modal, heading, (1920 - width) / 2, (1080 - height) / 2, width, height, Edge).rectTransform;
        Box(window, "Nền panel", 2, 2, width - 4, height - 4, Deep);
        Box(window, "Thanh tiêu đề", 2, 2, width - 4, 78, Plate);
        Label(window, heading.ToUpperInvariant(), 43, 31, 13, width - 120, 59, Gold, true);
        Control(window, "×", width - 79, 16, 53, 45, CloseWindow);
        Focus(Control(window, "Quay lại", 30, height - 66, 168, 46, CloseWindow));
    }

    private void CloseWindow()
    {
        DiscardModal();
        for (int i = 0; i < homeButtons.Length; i++) homeButtons[i].interactable = i != 1;
        PlayerPrefs.Save();
        eventSystem.SetSelectedGameObject(previousFocus != null ? previousFocus : homeButtons[0].gameObject);
        previousFocus = null;
    }

    private void DiscardModal()
    {
        idlePortrait = null;
        if (modal == null) return;
        modal.gameObject.SetActive(false);
        Destroy(modal.gameObject);
        modal = null;
    }

    private void WindowAction(string label, float x, float y, float w, Action command) =>
        Control(window, label, x, y, w, 54, command, true);

    private void Focus(Button button) => eventSystem.SetSelectedGameObject(button.gameObject);
    private static Color ColorOf(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;

    private static T Read<T>(string path) => JsonUtility.FromJson<T>(Resources.Load<TextAsset>(Root + path).text);

    private static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
        return rect;
    }

    private static Image Box(Transform parent, string name, float x, float y, float w, float h, Color color)
    {
        var image = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static RawImage Picture(Transform parent, string name, string path, float x, float y, float w, float h)
    {
        var image = Rect(name, parent, x, y, w, h).gameObject.AddComponent<RawImage>();
        if (path != null) image.texture = Resources.Load<Texture2D>(Root + path);
        image.raycastTarget = false;
        return image;
    }

    private Text Label(Transform parent, string text, int size, float x, float y, float w, float h,
        Color color, bool bold = false, TextAnchor anchor = TextAnchor.UpperLeft)
    {
        var view = Rect(text, parent, x, y, w, h).gameObject.AddComponent<Text>();
        view.font = font;
        view.color = color;
        view.fontSize = size;
        view.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        view.alignment = anchor;
        view.text = text;
        view.raycastTarget = false;
        return view;
    }

    private Button Control(Transform root, string label, float x, float y, float width, float height, Action click, bool primary = false)
    {
        var color = primary ? Gold : ColorOf("#5C435D");
        var image = Box(root, label, x, y, width, height, color);
        image.raycastTarget = true;
        Label(image.transform, label, 22, 4, 1, width - 8, height - 2, primary ? Deep : Ink, true, TextAnchor.MiddleCenter);
        return AttachButton(image, click, Color.white);
    }

    private Button AttachButton(Graphic graphic, Action click, Color normal)
    {
        var button = graphic.gameObject.AddComponent<Button>();
        button.targetGraphic = graphic;
        button.interactable = click != null;
        button.colors = new ColorBlock
        {
            normalColor = normal, highlightedColor = new Color(.91f, .96f, 1f),
            selectedColor = new Color(.81f, .91f, 1f), pressedColor = new Color(.67f, .70f, .76f),
            disabledColor = new Color(.88f, .88f, .88f), colorMultiplier = 1, fadeDuration = .09f
        };
        if (click != null) button.onClick.AddListener(() =>
        {
            if (!music.mute && confirmSound != null) music.PlayOneShot(confirmSound, .8f);
            click();
        });
        return button;
    }
}
