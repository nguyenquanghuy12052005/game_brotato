using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class MeleeController : MonoBehaviour
{
    #region Scene and preview playback

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

    internal bool Integrated { get; set; }
    private int integratedPassive = -1;
    private int integratedAimDirection = -1;
    private Vector2 integratedAim;
    private float nextIntegratedDamage;

    internal void ConfigureIntegratedBranch(int choice)
    {
        branch = choice + 1;
        startInFreeMode = true;
    }

    internal void SetIntegratedPassive(int choice) => integratedPassive = choice;

    private Vector3 FreeCharacterPosition => freeCasting ? castOrigin + SkillDisplacement(clock) : freePosition;
    internal Vector3 IntegratedGround => (actor != null ? FreeCharacterPosition : FreeSpawn) + TrainingFloor;
    internal int IntegratedHealth => Mathf.CeilToInt(playerDummyHp);

    internal void SetIntegratedTargets(EnemyHealth[] enemies)
    {
        for (int i = 0; i < trainingTargets.Length; i++)
        {
            EnemyHealth enemy = i < enemies.Length ? enemies[i] : null;
            trainingTargets[i] = enemy != null ? new PreviewTarget
            {
                LiveEnemy = enemy,
                Renderer = enemy.GetComponentInChildren<SpriteRenderer>(),
                ResistsPull = enemy.GetComponent<GunSkillTargetEffects>().IgnorePush
            } : null;
        }
        ResetTrainingCast();
    }

    internal void RestoreIntegratedHealth()
    {
        playerDummyHp = 100f;
        nextIntegratedDamage = 0f;
    }

    internal bool UseIntegratedSkill(int slot, int choice)
    {
        if (!freeMode || freeCasting || IntegratedHealth == 0) return false;
        int group = slot == 0 ? 1 : slot == 1 ? 3 : 2;
        int index = PreviewV2Source(group, branch, choice);
        UpdateIntegratedAim();
        if (DirectionCoverage(index, aim) == "MissingArt") UseClosestCardinalPose();
        skillIndex = index;
        skillGroup = group;
        BeginFreeCast();
        return freeCasting;
    }

    private void UpdateIntegratedAim()
    {
        if (Mouse.current == null) return;
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector3 screen = new Vector3(mouse.x, mouse.y, Mathf.Abs(previewCamera.transform.position.z - IntegratedGround.z));
        Vector2 delta = previewCamera.ScreenToWorldPoint(screen) - IntegratedGround;
        if (delta.sqrMagnitude <= .01f) return;
        SetIntegratedAim(delta);
        practiceAimDistance = Mathf.Clamp(delta.magnitude, 1.2f, 5.8f);
    }

    private void SetIntegratedAim(Vector2 delta)
    {
        if (delta.sqrMagnitude < .0001f) delta = DirectionVector(aim);
        aim = (Mathf.RoundToInt(Mathf.Atan2(delta.x, delta.y) / (Mathf.PI / 4f)) + 8) % 8;
        integratedAim = delta.normalized;
        integratedAimDirection = aim;
    }

    private void TryAutoBasic()
    {
        if (IntegratedHealth == 0) return;
        Vector3 origin = IntegratedGround;
        PreviewTarget target = NearestTarget(origin, Vector3.zero, false);
        if (target == null) return;
        Vector2 delta = target.Ground - origin;
        float reach = formIndex == 3 && StyleReady ? .42f + BasicWaveTravel : 1.82f;
        if (delta.sqrMagnitude > reach * reach) return;

        SetIntegratedAim(delta);
        if (!HasCombo(aim) && (aim & 1) != 0) UseClosestCardinalPose();
        BeginBasic();
    }

    private void UseClosestCardinalPose()
    {
        bool mainlyHorizontal = Mathf.Abs(integratedAim.x) >= Mathf.Abs(integratedAim.y);
        aim = mainlyHorizontal ? (integratedAim.x >= 0 ? 2 : 6) : (integratedAim.y >= 0 ? 0 : 4);
        integratedAimDirection = aim;
    }

    private void Start()
    {
        actor = swordsman.GetComponent<SpriteRenderer>();
        cameraRest = previewCamera.transform.position;
        previewCameraSize = previewCamera.orthographicSize;
        previewBackground = previewCamera.backgroundColor;
        swordsman.speed = 0;
        InitRoster();
        if (Integrated) SelectForm(branch);
        skillGroup = 2;
        skillIndex = PreviewV2Source(2, branch, 0);
        ConfigureEffect();
        freeMode = startInFreeMode;
        freePosition = FreeSpawn;
        if (Integrated) UpdatePreviewViewport();
        RestartPlayback();
    }

    public void Replay()
    {
        if (freeMode)
        {
            if (previewV2 && skillIndex < 3) BeginBasic();
            else BeginFreeCast();
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
        ResetTrainingCast();
        if (previewV2 && !freeMode) PlaceTraining(FreeSpawn, AimVector);
        parryCuePlayed = false;
        audioSource.Stop();
        paused = false;
        Render();
    }

    public bool Select(Preview mode, int direction = 2, bool slow = false)
    {
        if ((mode == Preview.Idle || mode == Preview.Run) && !HasLocomotion(direction)) return false;
        previewV2 = false;
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
        if (Integrated && Time.timeScale == 0f) return;
        UpdatePreviewViewport();
        if (!paused) practiceTime += Mathf.Min(Time.unscaledDeltaTime, .05f) * (halfSpeed ? .5f : 1);
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
        BeginTrainingDraw();
        if (previewV2)
        {
            RenderOther();
            FinishTrainingDraw();
            return;
        }
        FinishTrainingDraw();
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
        SampleBasicHit(time, frame, 12);
        PlayBasicFrame(frame);
        if (StyleReady) return;
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

    #endregion

    #region Free play and input

    [SerializeField] private bool startInFreeMode = true;
    [SerializeField, Min(.1f)] private float freeMoveSpeed = 3.6f;

    private static readonly Vector3 FreeSpawn = new(-1.4f, .667f);
    private const float FreeTurnDuration = .10f;
    private enum FreeAction { None, Basic, Skill }
    private FreeAction afterTurn;
    private bool freeMode, freeCasting;
    private Vector3 freePosition, castOrigin, freeVelocity, cameraFollow, cameraFollowVelocity;
    private float locomotionTime, freeTurn;
    private int turnFrom;
    public bool InFreePlay => freeMode;
    public bool CastingInFreePlay => freeMode && freeCasting;
    public Vector3 ActorPosition => swordsman.transform.position;

    public void EnableFreePlay()
    {
        SetFreeMode(true);
        RestartPlayback();
    }

    private float CurrentSkillDuration => skillIndex == 2 && !parry ? Chosen.Starts[2] + .25f : SkillDuration(skillIndex);

    private Vector3 SkillDisplacement(float time) =>
        skillIndex == 0 ? AimVector * DashOffset(time) : Travel(time);

    private void SetFreeMode(bool active)
    {
        if (freeMode == active) return;
        if (freeCasting) CommitCastPosition();
        basicCasting = queuedCast = queuedBasic = false;
        afterTurn = FreeAction.None;
        trainingPlaced = false;
        freeMode = active;
        freeVelocity = Vector3.zero;
        freeTurn = 0;
        if (active) cameraFollow = freePosition - FreeSpawn;
        cameraFollowVelocity = Vector3.zero;
    }

    private void BeginFreeCast()
    {
        if (previewV2 && skillIndex < 3) return;
        if (DirectionCoverage(skillIndex, aim) == "MissingArt") return;
        if (freeCasting) CommitCastPosition();
        castOrigin = freePosition;
        basicCasting = queuedCast = queuedBasic = false;
        freeVelocity = Vector3.zero;
        freeTurn = 0;
        freeCasting = true;
        ConfigureEffect();
        RestartPlayback();
    }

    private void CommitCastPosition()
    {
        freePosition = castOrigin + SkillDisplacement(Mathf.Min(clock, CurrentSkillDuration));
        if (previewV2 && skillIndex == 3 && ambushFacing >= 0) aim = ambushFacing;
        freeCasting = false;
    }

    private void ResetFreePlay()
    {
        freeCasting = false;
        freePosition = FreeSpawn;
        afterTurn = FreeAction.None;
        freeVelocity = cameraFollow = cameraFollowVelocity = Vector3.zero;
        locomotionTime = freeTurn = 0;
        aim = locomotionDirection = 2;
        ResetPractice();
        RestartPlayback();
    }

    private void FaceFreeDirection(int direction)
    {
        if (freeCasting || basicCasting || aim == direction) return;
        turnFrom = aim;
        aim = direction;
        freeTurn = FreeTurnDuration;
    }

    private void UpdateFreePlay(float delta)
    {
        var keyboard = Keyboard.current;
        if (!Integrated && keyboard?.rKey.wasPressedThisFrame == true)
        {
            ResetFreePlay();
            return;
        }

        var move = keyboard == null ? Vector2.zero : new Vector2(
            Axis(keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed,
                keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed),
            Axis(keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed,
                keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)).normalized;

        if (!paused)
        {
            bool wasCasting = freeCasting;
            bool wasBasic = basicCasting;
            if (previewV2 && keyboard?.hKey.wasPressedThisFrame == true) ReceivePracticeHit();
            if (!freeCasting && !basicCasting)
            {
                if (move != Vector2.zero)
                {
                    locomotionDirection = (Mathf.RoundToInt(Mathf.Atan2(move.x, move.y) / (Mathf.PI / 4)) + 8) % 8;
                    if (!previewV2 || aimFollowsMovement) FaceFreeDirection(locomotionDirection);
                }
                if (Integrated) TryAutoBasic();
                else if (keyboard?.spaceKey.wasPressedThisFrame == true) afterTurn = FreeAction.Skill;
                else if (previewV2 && keyboard?.jKey.wasPressedThisFrame == true) afterTurn = FreeAction.Basic;
                bool finishTurn = StyleReady && freeTurn > 0 &&
                    (afterTurn == FreeAction.Basic || skillIndex < 3 || PolishedBranch);
                if (afterTurn != FreeAction.None && !finishTurn)
                {
                    FreeAction action = afterTurn;
                    afterTurn = FreeAction.None;
                    if (action == FreeAction.Basic) BeginBasic();
                    else Replay();
                }
            }
            else if (!Integrated && previewV2 && basicCasting && skillIndex >= 3 && keyboard?.spaceKey.wasPressedThisFrame == true)
                queuedCast = true;
            if (!Integrated && previewV2 && freeCasting && keyboard?.jKey.wasPressedThisFrame == true) queuedBasic = true;

            if (basicCasting)
            {
                if (wasBasic)
                {
                    if (hold > 0) hold -= delta;
                    else clock += delta * (halfSpeed ? .5f : 1);
                }
                if (clock >= AttackDuration)
                {
                    basicCasting = false;
                    if (queuedCast) BeginFreeCast();
                    queuedCast = false;
                }
            }
            if (freeCasting)
            {
                if (wasCasting)
                {
                    if (previewV2 && skillIndex == 23 && projectileLocked)
                    {
                        freeVelocity = Vector3.MoveTowards(freeVelocity, (Vector3)move * freeMoveSpeed,
                            delta * freeMoveSpeed * 12);
                        castOrigin += freeVelocity * delta;
                    }
                    if (hold > 0) hold -= delta;
                    else clock = Mathf.Min(CurrentSkillDuration, clock + delta * (halfSpeed ? .5f : 1f));
                    if (clock >= CurrentSkillDuration)
                    {
                        CommitCastPosition();
                        if (queuedBasic) BeginBasic();
                        queuedBasic = false;
                    }
                }
            }
            else if (Integrated || !basicCasting)
            {
                bool wasMoving = freeVelocity.sqrMagnitude > .025f;
                freeVelocity = Vector3.MoveTowards(freeVelocity, (Vector3)move * freeMoveSpeed, delta * freeMoveSpeed * 12);
                freePosition += freeVelocity * delta;
                if (previewV2 && wasMoving != (freeVelocity.sqrMagnitude > .025f)) locomotionTime = 0;
                else locomotionTime += delta;
                freeTurn = Mathf.Max(0, freeTurn - delta);
            }
        }

        cameraFollow = Vector3.SmoothDamp(cameraFollow, FreeCharacterPosition - FreeSpawn,
            ref cameraFollowVelocity, .12f, Mathf.Infinity, delta);
        RenderFreePlay();
    }

    private static int Axis(bool positive, bool negative) => (positive ? 1 : 0) - (negative ? 1 : 0);

    private void RenderFreePlay()
    {
        BeginTrainingDraw();
        if (!Integrated && previewV2 && !trainingPlaced) PlaceTraining(freePosition, AimVector);
        if (freeCasting) RenderOther();
        else
        {
            ClearFeedback();
            ResetLines();
            swordsman.transform.position = freePosition;
            dummy.enabled = false;
            bool running = freeVelocity.sqrMagnitude > .025f;
            if (basicCasting) ShowBasic(clock);
            else if (freeTurn > 0) ShowTurn(turnFrom, aim, 1 - freeTurn / FreeTurnDuration);
            else ShowLocomotion(running, locomotionTime, previewV2 && running ? locomotionDirection : aim);
        }
        if (Integrated)
        {
            Vector3 center = FreeSpawn + cameraFollow;
            previewCamera.transform.position = new Vector3(center.x, center.y, cameraRest.z);
        }
        else previewCamera.transform.position += cameraFollow + PracticeFramingOffset;
        FinishTrainingDraw();
    }

    #endregion

    #region Preview controls

    private static readonly string[] Groups = { "Bị động", "Chủ động AOE", "Lõi HEXCORE", "Bổ trợ" };
    private static readonly string[] Branches = { "Sát thủ", "Đao sĩ", "Kiếm khí" };
    private static readonly string[] PreviewModes = { "Đứng", "Chạy", "Quay", "Đánh thường", "Kỹ năng", "Chuỗi đòn" };
    private static readonly Color PanelColor = new(.075f, .11f, .101f);
    private static readonly Color HeaderColor = new(.053f, .084f, .075f);
    private static readonly Color AccentColor = new(.52f, .79f, .66f);
    private static readonly Color MutedColor = new(.73f, .81f, .77f);
    private Vector2 panelScroll;
    private int skillGroup;
    private float previewCameraSize;
    private Color previewBackground;
    private bool lightBackground;
    private GUIStyle skillButton, titleLabel, sectionLabel, hintLabel, smallButton;

    private float DockWidth => Mathf.Min(Screen.width * .46f, Mathf.Clamp(Screen.width * .31f, 230, 350));
    private float HudHeight => Screen.width < 660 ? 105 : 88;

    private void UpdatePreviewViewport()
    {
        if (Integrated)
        {
            previewCamera.rect = new Rect(0f, 0f, 1f, 1f);
            previewCamera.orthographicSize = 5f;
            return;
        }
        float dock = DockWidth / Mathf.Max(1, Screen.width);
        float top = Mathf.Min(.45f, HudHeight / Mathf.Max(1, Screen.height));
        previewCamera.rect = new Rect(dock, 0, 1 - dock, 1 - top);
        float aspect = Mathf.Max(.01f, Screen.width * (1 - dock) / Mathf.Max(1, Screen.height * (1 - top)));
        float room = HasLongPreviewRange ? 2.04f : previewV2 && IsBranchSkill ? 1.48f : 1;
        previewCamera.orthographicSize = previewCameraSize * Mathf.Max(1, 1.08f / aspect) * room;
    }

    public static string VisibleSkillName(int index)
    {
        string catalogName = SkillName(index);
        return catalogName[(catalogName.IndexOf('·') + 1)..].Trim();
    }

    private void OnGUI()
    {
        if (Integrated) return;
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
        int form = GUILayout.SelectionGrid(formIndex, Names, 2, smallButton);
        if (form != formIndex)
        {
            if (freeCasting) CommitCastPosition();
            basicCasting = queuedCast = queuedBasic = false;
            SelectForm(form);
            if (IsBranchSkill && !Compatible(skillIndex, form)) skillGroup = 2;
            if (form > 0) branch = form;
            PickSkill(PreviewV2Source(skillGroup, branch, previewLoadout[skillGroup]));
            GUIUtility.ExitGUI();
        }
        GUILayout.Label("Chuyên nhánh E / F", sectionLabel);
        int nextBranch = GUILayout.SelectionGrid(branch - 1, Branches, 3, smallButton) + 1;
        if (nextBranch != branch)
        {
            branch = nextBranch;
            if (skillGroup is 1 or 3) PickSkill(PreviewV2Source(skillGroup, branch, previewLoadout[skillGroup]));
            GUIUtility.ExitGUI();
        }
        GUILayout.Label("Nhóm kỹ năng", sectionLabel);
        int group = GUILayout.SelectionGrid(skillGroup, Groups, 2, smallButton);
        if (group != skillGroup)
        {
            skillGroup = group;
            PickSkill(PreviewV2Source(group, branch, previewLoadout[group]));
            GUIUtility.ExitGUI();
        }
        GUILayout.Space(5);
        for (int slot = 0; slot < 3; slot++)
        {
            int index = PreviewV2Source(skillGroup, branch, slot);
            if (!PanelButton(PreviewV2DisplayName(skillGroup, branch, slot), index == skillIndex, skillButton)) continue;
            if (skillGroup == 0 && SelectedPassive != slot)
            {
                passiveHits = 0;
                passiveStart = -10;
            }
            previewLoadout[skillGroup] = slot;
            PickSkill(index);
            GUIUtility.ExitGUI();
        }
        GUILayout.Label(V2Descriptions[skillIndex], hintLabel);
        GUILayout.Label("Pha: " + (phase ?? ""), hintLabel);
        GUILayout.Space(8);
        DrawPreviewControls();
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(dock + 14, 9, Mathf.Max(10, Screen.width - dock - 27), HudHeight - 11));
        GUILayout.Label(Names[formIndex] + "  /  " + (freeMode ? "Tự do" : "Trình diễn") + "  /  " + DirectionLabels[aim], hintLabel);
        GUILayout.Label(previewV2 ? PreviewV2DisplayName(skillGroup, branch, previewLoadout[skillGroup]) : VisibleSkillName(skillIndex), titleLabel);
        GUILayout.Label(freeMode ? "WASD / Mũi tên · J đánh thường · Space thử chiêu · H nhận đòn · R reset" : phase ?? "", hintLabel);
        GUILayout.EndArea();
    }

    public static string PreviewV2DisplayName(int group, int branch, int slot)
    {
        int index = PreviewV2Source(group, branch, slot);
        string name = index switch
        {
            0 => "Phong Ảnh Trảm",
            8 => "Bá Vương Đao Vực",
            11 => "Thiên Kiếm Xuyên Tâm",
            _ => VisibleSkillName(index)
        };
        return $"{"PERF"[group]}{slot + 1} · {name}";
    }

    private void PickSkill(int index)
    {
        if (freeCasting) CommitCastPosition();
        previewV2 = true;
        afterTurn = FreeAction.None;
        basicCasting = queuedCast = queuedBasic = false;
        int primary = PrimaryForm(index);
        if (index >= 3 && primary >= 0) SelectForm(primary);
        skillIndex = index;
        if (!freeMode)
        {
            if (index < 3) selection = Preview.Basic;
            else if (selection != Preview.Chain) selection = Preview.Q1;
        }
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
        if (freeMode) aimFollowsMovement = GUILayout.Toggle(aimFollowsMovement, "Ngắm theo hướng chạy");
        GUILayout.Label("Bia mô phỏng và vùng tác động", sectionLabel);
        bool controlsEnabled = GUI.enabled;
        GUI.enabled = controlsEnabled && !freeCasting && !basicCasting;
        int count = GUILayout.SelectionGrid(targetCount == 0 ? 0 : targetCount == 1 ? 1 : targetCount == 5 ? 2 : 3,
            new[] { "0", "1", "5", "9" }, 4, smallButton);
        int requestedCount = count == 0 ? 0 : count == 1 ? 1 : count == 2 ? 5 : 9;
        if (targetCount != requestedCount)
        {
            targetCount = requestedCount;
            PlaceTraining(trainingPlaced ? formationOrigin : ActorPosition, trainingPlaced ? formationForward : AimVector);
        }
        bool nextWeak = GUILayout.Toggle(weakTargets, "Cụm bia yếu");
        if (weakTargets != nextWeak)
        {
            weakTargets = nextWeak;
            PlaceTraining(trainingPlaced ? formationOrigin : ActorPosition, trainingPlaced ? formationForward : AimVector);
        }
        bool nextBoss = GUILayout.Toggle(bossFixture, "Bia đầu là boss mô phỏng");
        if (nextBoss != bossFixture)
        {
            bossFixture = nextBoss;
            PlaceTraining(trainingPlaced ? formationOrigin : ActorPosition, trainingPlaced ? formationForward : AimVector);
        }
        GUILayout.Label($"Điểm ngắm  {practiceAimDistance:0.0} u", hintLabel);
        practiceAimDistance = GUILayout.HorizontalSlider(practiceAimDistance, 1.2f, 5.8f);
        if (GUILayout.Button("Đặt lại bia theo hướng ngắm", smallButton)) PlaceTraining(freeMode ? freePosition : FreeSpawn, AimVector);
        if (freeMode && GUILayout.Button("Đánh thường (J)", smallButton)) BeginBasic();
        GUI.enabled = controlsEnabled;
        if (GUILayout.Button("Ẩn bia gần nhất", smallButton))
        {
            var removed = NearestTarget(ActorPosition, AimVector, false);
            if (removed != null) removed.Hp = 0;
        }
        showAreas = GUILayout.Toggle(showAreas, "Hiện vùng tác động");
        bool nextBackground = GUILayout.Toggle(lightBackground, "Nền sáng kiểm tra viền");
        if (lightBackground != nextBackground)
        {
            lightBackground = nextBackground;
            previewCamera.backgroundColor = lightBackground ? new Color(.80f, .83f, .85f) : previewBackground;
        }
        if (GUILayout.Button("Giả lập nhận đòn (H)", smallButton)) ReceivePracticeHit();
        string passiveState = SelectedPassive < 2
            ? $"{passiveHits}/{(SelectedPassive == 0 ? 5 : 4)} đòn trúng"
            : practiceTime >= parryReadyAt ? "Sẵn sàng" : $"{parryReadyAt - practiceTime:0.0}s";
        GUILayout.Label($"P{SelectedPassive + 1}: {passiveState} · Kích hoạt {passiveProcs} · Trúng bia {trainingHits}", hintLabel);
        GUILayout.Label($"HP mô phỏng {playerDummyHp:0}   {(practiceTime < empowerUntil ? "· Cường hóa" : "")}", hintLabel);
        if (!freeMode && GUILayout.Button("Reset mô phỏng", smallButton))
        {
            ResetPractice();
            RestartPlayback();
        }
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
        GUI.enabled = enabled && !freeCasting && !basicCasting;
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

    #endregion
}
