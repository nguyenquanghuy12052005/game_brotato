using UnityEngine;

public sealed partial class HexcoreSkillPreview : MonoBehaviour
{
    [SerializeField] private Animator swordsman;
    [SerializeField] private SpriteRenderer dummy;
    [SerializeField] private Camera previewCamera;
    [SerializeField] private LineRenderer impact;
    [SerializeField] private LineRenderer swordTrail;
    [SerializeField] private LineRenderer dust;
    [SerializeField] private Sprite[] basicEast;
    [SerializeField] private Sprite[] q1East;

    public enum Preview { Idle, Run, Turn, Basic, Q1, Chain }

    private static readonly string[] CardinalStates = { "Up", "Right", "Down", "Left" };
    private static readonly string[] DirectionLabels = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
    private static readonly string[] Q1Phases =
    {
        "Anticipation", "Anticipation", "Anticipation", "Foot Push", "Dash", "Dash",
        "Sword Swing", "Impact", "Braking", "Braking", "Recovery", "Recovery"
    };

    private const float FrameTime = .075f;
    private const float AttackDuration = 12 * FrameTime;
    private const float ApproachDuration = .4f;
    private const float ChainLead = ApproachDuration + AttackDuration;
    private const float ReturnRun = .45f;
    private const float DashDistance = 2.05f;
    private const float StartX = -DashDistance;

    private Preview selection = Preview.Chain;
    private int locomotionDirection = 2;
    private float clock;
    private bool paused;
    private bool halfSpeed;
    private bool showEffects = true;
    private bool shake = true;
    private Vector3 cameraRest;
    private SpriteRenderer actor;
    private string phase;

    public string Phase => phase;
    public float Elapsed => clock;
    public Sprite CurrentSprite => actor != null ? actor.sprite : null;

    private void Start()
    {
        actor = swordsman.GetComponent<SpriteRenderer>();
        cameraRest = previewCamera.transform.position;
        previewCameraSize = previewCamera.orthographicSize;
        swordsman.speed = 0;
        InitRoster();
        freeMode = startInFreeMode;
        freePosition = FreeSpawn;
        RestartPlayback();
    }

    public void Replay()
    {
        if (freeMode)
        {
            BeginFreeCast();
            return;
        }
        RestartPlayback();
    }

    private void RestartPlayback()
    {
        clock = 0;
        hold = 0;
        lastCue = -1;
        lastDashFrame = -1;
        lastBasicFrame = -1;
        playedCues = 0;
        parryCuePlayed = false;
        audioSource.Stop();
        paused = false;
        Render();
    }

    public bool Select(Preview mode, int direction = 2, bool slow = false)
    {
        if ((mode == Preview.Idle || mode == Preview.Run) && !HasLocomotion(direction)) return false;
        SetFreeMode(false);
        SelectForm(0);
        aim = 2;
        skillIndex = 0;
        skillGroup = 0;
        selection = mode;
        locomotionDirection = direction;
        halfSpeed = slow;
        Replay();
        return true;
    }

    private bool HasLocomotion(int direction)
    {
        return direction >= 0 && direction < DirectionLabels.Length &&
            HasDirection(0, direction);
    }

    private void LateUpdate()
    {
        UpdatePreviewViewport();
        if (freeMode)
        {
            UpdateFreePlay(Mathf.Min(Time.unscaledDeltaTime, .05f));
            return;
        }
        if (paused) return;
        if (hold > 0)
        {
            hold -= Time.unscaledDeltaTime;
            return;
        }
        clock += Mathf.Min(Time.unscaledDeltaTime, .05f) * (halfSpeed ? .5f : 1f);
        Render();
    }

    private void Render()
    {
        if (freeMode)
        {
            RenderFreePlay();
            return;
        }
        if (!OriginalEast || selection is Preview.Idle or Preview.Run or Preview.Turn)
        {
            RenderOther();
            return;
        }
        ClearFeedback();
        float skillTime = selection == Preview.Chain ? clock - ChainLead : clock;
        bool hasSkill = selection == Preview.Q1 || selection == Preview.Chain;
        float skillEnd = skillTime - AttackDuration;
        float runOut = skillEnd - AttackDuration;
        float position = StartX;
        float approach = Mathf.Clamp01(clock / (locomotionDirection == aim ? ApproachDuration : .27f));
        if (selection == Preview.Chain)
            position += Mathf.Lerp(-.8f, 0, approach)
                + Mathf.SmoothStep(0, .9f, ExitProgress(runOut));
        if (hasSkill) position += DashOffset(skillTime);

        swordsman.transform.position = new Vector3(position, .667f);
        if (selection == Preview.Chain)
        {
            Vector3 alternate = DirectionVector(locomotionDirection) - Vector3.right;
            swordsman.transform.position += alternate * (-.8f * (1 - approach)
                + .9f * Mathf.SmoothStep(0, 1, ExitProgress(runOut)));
        }
        dummy.transform.position = new Vector3(.16f, .575f);
        dummy.enabled = hasSkill;
        dummy.color = Color.white;
        impact.enabled = false;
        dust.enabled = false;
        swordTrail.enabled = false;
        previewCamera.transform.position = cameraRest;

        switch (selection)
        {
            case Preview.Basic:
                Basic(clock);
                break;
            case Preview.Q1:
                if (skillEnd < 0) Skill(skillTime);
                else ShowLocomotion(false, skillEnd, 2);
                break;
            case Preview.Chain:
                if (clock < ApproachDuration) ShowApproach(clock);
                else if (skillTime < 0) Basic(clock - ApproachDuration);
                else if (skillEnd < 0) Skill(skillTime);
                else if (runOut < 0) Basic(skillEnd);
                else if (runOut < ReturnRun) ShowExitRun(runOut);
                else ShowLocomotion(false, runOut - ReturnRun, locomotionDirection);
                break;
        }
    }

    private static float DashOffset(float time)
    {
        return Mathf.SmoothStep(0, DashDistance, Mathf.InverseLerp(3 * FrameTime, 9 * FrameTime, time));
    }

    private void Basic(float time)
    {
        if (time >= AttackDuration)
        {
            ShowLocomotion(false, time - AttackDuration, 2);
            return;
        }

        int frame = FrameAt(time);
        SpritePose(basicEast[frame], frame < 6 ? "Basic · Chém ngang" : "Basic · Chém ngược");
        PlayBasicFrame(frame);
        if (frame == 3) DrawTrail(new Vector2(171, 144), new Vector2(216, 144), new Vector2(157, 133));
        if (frame == 7) DrawTrail(new Vector2(158, 115), new Vector2(199, 103), new Vector2(148, 131));
    }

    private void Skill(float time)
    {
        int frame = FrameAt(time);
        SpritePose(q1East[frame], Q1Phases[frame]);
        PlayDashFrame(frame);
        if (frame == 3 || frame == 9)
        {
            Vector3 foot = Anchor(frame == 3 ? new Vector2(68, 205) : new Vector2(163, 206));
            foot.x -= DashOffset(time) - DashOffset(frame * FrameTime);
            Draw(dust, .025f, foot + new Vector3(-.22f, .09f), foot, foot + new Vector3(-.15f, .16f));
        }
        if (frame == 6) DrawTrail(new Vector2(141, 151), new Vector2(181, 137), new Vector2(148, 156));
        if (frame != 7) return;

        if (lastCue != 7)
        {
            lastCue = 7;
            if (!scrubbing && hitStop) hold = .045f;
        }

        var hilt = new Vector2(167, 139);
        var tip = new Vector2(211, 139);
        DrawTrail(hilt, tip, new Vector2(180, 132));
        Vector3 contact = dummy.transform.position;
        Draw(impact, .04f, contact + new Vector3(.15f, .12f), contact,
            contact + new Vector3(.19f, -.07f), contact, contact + new Vector3(.01f, .14f));
        dummy.color = new Color(1, .57f, .4f);
        if (shake) previewCamera.transform.position += .036f * Mathf.Sin(time * 130) * Vector3.right;
    }

    private int FrameAt(float time) => Mathf.Clamp(Mathf.FloorToInt(time / FrameTime), 0, 11);

    private Vector3 Anchor(Vector2 topLeftPixels)
    {
        return swordsman.transform.position + new Vector3(topLeftPixels.x - 128, 128 - topLeftPixels.y) / 120;
    }

    private void DrawTrail(Vector2 hilt, Vector2 tip, Vector2 previousTip)
    {
        Draw(swordTrail, .04f, Anchor(hilt), Anchor(tip), Anchor(previousTip));
    }

    private void Draw(LineRenderer line, float width, params Vector3[] points)
    {
        if (!showEffects) return;
        line.positionCount = points.Length;
        line.SetPositions(points);
        line.widthMultiplier = width;
        line.enabled = true;
    }

    private void SpritePose(Sprite sprite, string label)
    {
        phase = label;
        swordsman.enabled = false;
        actor.sprite = sprite;
    }

}
