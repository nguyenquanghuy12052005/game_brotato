using UnityEngine;
using static HexcorePreviewCatalog;

public sealed partial class HexcoreSkillPreview
{
    [System.Serializable]
    private sealed class FormArt
    {
        public GameObject prefab;
        public Sprite[] locomotion;
        public Sprite[] diagonalIdle;
        public Sprite[] turns;
        public Sprite[] basic;
        public Sprite[] basicWest;
        public Sprite[] basicNorth;
        public Sprite[] basicSouth;
        public Sprite[] basicDiagonal;
        public Sprite[] slam;
        public Sprite[] hero;
        public Sprite[] heroWest;
    }

    [SerializeField] private FormArt[] forms;
    [SerializeField] private GameObject[] effects;
    [SerializeField] private Sprite[] signatures;
    [SerializeField] private Sprite[] continuousEast;
    [SerializeField] private GameObject qiWavePrefab;
    [SerializeField] private Material cleanupMaterial, goldMaterial;
    [SerializeField] private Sprite[] q1West, q1South;
    [SerializeField] private Sprite[] q1Additional;
    private static readonly string[] Names = { "Kiếm sĩ", "Sát thủ", "Đao sĩ", "Kiếm khí" };
    private Animator[] roster;
    private Animator[] shadows;
    private GameObject effect;
    private int configuredSkill = -1;
    private SpriteRenderer effectRenderer;
    private AnimationClip effectClip;
    private GameObject basicWave;
    private AnimationClip basicWaveClip;
    private int formIndex;
    private int skillIndex;
    private int aim = 2;
    private bool hear = true, hitStop = true, parry = true;
    private bool scrubbing;
    private float hold;
    private int lastCue = -1;
    private static readonly Vector2[] LandingBlade = { new(183, 205), new(203, 205), new(233, 239), new(233, 238) };
    private static readonly Vector2[] WestLandingBlade = { new(75, 207), new(77, 205), new(62, 239), new(96, 240) };
    private static readonly Vector2[,] CoreReleaseBlade =
    {
        { new(235, 154), new(197, 35), new(235, 127) },
        { new(240, 160), new(194, 31), new(251, 137) },
        { new(270, 207), new(55, 122), new(281, 134) },
        { new(265, 228), new(230, 83), new(266, 186) }
    };
    private static readonly Vector2[,] WestCoreReleaseBlade =
    {
        { new(13, 115), new(19, 78), new(4, 122) },
        { new(16, 132), new(20, 122), new(4, 121) },
        { new(58, 153), new(80, 85), new(37, 155) },
        { new(47, 155), new(47, 175), new(35, 148) }
    };
    private static readonly Vector2[] QiBasicBlade =
    {
        new(92, 104), new(243, 89), new(232, 90), new(240, 115),
        new(218, 238), new(78, 111), new(85, 155), new(86, 91)
    };
    private static readonly int[][] ContinuousBeats =
    {
        new[] { 0, 2, 9, 10, 12 },              // Q2
        new[] { 0, 2, 5, 8, 10, 12 },           // Q3
        new[] { 0, 2, 5, 7, 8, 10, 12 },        // Huyết Ấn
        new[] { 0, 2, 4, 7, 9, 12 },           // Chấn Địa Trảm
        new[] { 0, 2, 7, 8, 10, 12 },          // Nguyệt Nha Trảm
        new[] { 0, 2, 5, 7, 9, 10, 12 }        // Hồi Nguyệt Kiếm
    };
    private Skill Chosen => Skills[skillIndex];
    private bool OriginalEast => formIndex == 0 && aim == 2 && skillIndex == 0;
    private Vector3 AimVector => DirectionVector(aim);
    private static Vector3 DirectionVector(int direction)
    {
        float angle = direction * Mathf.PI / 4;
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0);
    }

    public static int SkillCount => Skills.Length;
    public static bool Compatible(int skill, int form) => Skills[skill].Supports(form);
    public static int PrimaryForm(int skill) => Skills[skill].Form;
    public static string EffectPath(int skill) => Skills[skill].Effect;
    public static string SkillName(int skill) => Skills[skill].Name;
    public static int SkillBeatCount(int skill) => Skills[skill].Beats.Length;
    public static float SkillDuration(int skill) => skill == 0 ? AttackDuration : Skills[skill].Starts[^1];

    public void ReviewAt(float seconds, bool vfx)
    {
        showEffects = vfx;
        paused = true;
        clock = seconds;
        scrubbing = true;
        Render();
        scrubbing = false;
    }

    private void InitRoster()
    {
        LoadVisualAnchors();
        roster = new Animator[forms.Length];
        roster[0] = swordsman;
        for (int i = 1; i < forms.Length; i++)
        {
            roster[i] = Instantiate(forms[i].prefab).GetComponent<Animator>();
            roster[i].speed = 0;
            roster[i].gameObject.SetActive(false);
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = .38f;
        basicWave = Instantiate(qiWavePrefab);
        var waveAnimator = basicWave.GetComponent<Animator>();
        basicWaveClip = waveAnimator.runtimeAnimatorController.animationClips[0];
        waveAnimator.enabled = false;
        basicWave.SetActive(false);
    }

    public bool SelectSkill(int form, int skill, int move, int facing, bool chain, bool slow)
    {
        if (form < 0 || form >= forms.Length || skill < 0 || skill >= Skills.Length || !Skills[skill].Supports(form))
            return false;
        SetFreeMode(false);
        SelectForm(form);
        skillIndex = skill;
        skillGroup = PrimaryForm(skill) < 0 ? 4 : form;
        aim = Mathf.Clamp(facing, 0, 7);
        locomotionDirection = Mathf.Clamp(move, 0, 7);
        selection = chain ? Preview.Chain : Preview.Q1;
        halfSpeed = slow;
        ConfigureEffect();
        Replay();
        return true;
    }

    public bool SelectLocomotion(int form, int direction, bool run)
    {
        if (!HasDirection(form, direction)) return false;
        SetFreeMode(false);
        SelectForm(form);
        locomotionDirection = direction;
        selection = run ? Preview.Run : Preview.Idle;
        Replay();
        return true;
    }

    public bool SelectBasic(int form, int direction)
    {
        SetFreeMode(false);
        SelectForm(form);
        aim = direction;
        selection = Preview.Basic;
        Replay();
        return HasCombo(direction);
    }

    public bool SelectTurn(int form, int from, int to)
    {
        SetFreeMode(false);
        SelectForm(form);
        locomotionDirection = from;
        aim = to;
        selection = Preview.Turn;
        Replay();
        return forms[formIndex].turns is { Length: 16 };
    }

    private void SelectForm(int index)
    {
        if (formIndex == index) return;
        swordsman.gameObject.SetActive(false);
        DestroyShadows();
        formIndex = index;
        swordsman = roster[index];
        swordsman.gameObject.SetActive(true);
        actor = swordsman.GetComponent<SpriteRenderer>();
    }

    private void DestroyShadows()
    {
        if (shadows == null) return;
        foreach (var copy in shadows)
        {
            copy.gameObject.SetActive(false);
            Destroy(copy.gameObject);
        }
        shadows = null;
    }

    private void ConfigureEffect()
    {
        if (configuredSkill == skillIndex) return;
        configuredSkill = skillIndex;
        if (effect != null)
        {
            effect.SetActive(false);
            Destroy(effect);
            effect = null;
        }
        effectClip = null;
        effectRenderer = null;
        if (effects[skillIndex] == null) return;
        effect = Instantiate(effects[skillIndex]);
        var animator = effect.GetComponent<Animator>();
        effectRenderer = effect.GetComponent<SpriteRenderer>();
        if (skillIndex == 14) effectRenderer.sharedMaterial = goldMaterial;
        else if (Chosen.Effect != "QiSwordsman/QiWave")
            effectRenderer.sharedMaterial = cleanupMaterial;
        effectClip = animator.runtimeAnimatorController.animationClips[0];
        animator.enabled = false;
        effect.SetActive(false);
    }

    private void ClearFeedback()
    {
        if (effect != null) effect.SetActive(false);
        basicWave.SetActive(false);
        if (shadows != null)
            foreach (var copy in shadows) copy.gameObject.SetActive(false);
        actor.color = Color.white;
        actor.enabled = true;
    }

    public bool HasDirection(int form, int direction)
    {
        return direction >= 0 && direction < 8 &&
            (direction % 2 == 0 || forms[form].locomotion[direction / 2 * 5] != null);
    }

    private Sprite Q1Sprite(int direction, int frame)
    {
        if (direction == 2) return q1East[frame];
        if (direction == 4) return q1South.Length == 12 ? q1South[frame] : null;
        if (direction == 6) return q1West.Length == 12 ? q1West[frame] : null;
        int index = direction * 12 + frame;
        return index < q1Additional.Length ? q1Additional[index] : null;
    }

    private bool HasQ1Facing(int direction) => Q1Sprite(direction, 0) != null;
    private Sprite[] BasicFrames(int direction) => direction switch
    {
        0 => forms[formIndex].basicNorth,
        2 => forms[formIndex].basic,
        4 => forms[formIndex].basicSouth,
        6 => forms[formIndex].basicWest,
        _ => null
    };
    private bool HasCombo(int direction) => direction % 2 == 0
        ? BasicFrames(direction) is { Length: 12 } : forms[formIndex].basicDiagonal[direction / 2 * 8] != null;
    public static int BasicFrameCount(int direction) => direction % 2 == 0 ? 12 : 8;

    public string DirectionCoverage(int skill, int direction)
    {
        if (skill == 0)
            return !HasQ1Facing(direction) ? "MissingArt" : direction == 2 ? "InReview" : "AuthoredBlockout";
        if (HasDirectionalTake(skill, direction)) return "AuthoredBlockout";
        if (direction % 2 != 0) return "MissingArt";
        return direction == 2 || direction == 6 && skill >= 12 && skill <= 14 &&
            forms[formIndex].heroWest is { Length: 24 } ? "AuthoredBlockout" : "SourcePoseBlockout";
    }

    public bool HasEffect => impact.enabled || dust.enabled || swordTrail.enabled ||
        effect != null && effect.activeSelf || basicWave != null && basicWave.activeSelf;

    public bool HasContinuousSkill(int skill) => continuousEast[skill * 12] != null;
    private void RenderOther()
    {
        ClearFeedback();
        ResetLines();
        Vector3 start = freeMode ? castOrigin : new Vector3(-1.4f, .667f);
        Preview mode = freeMode ? Preview.Q1 : selection;
        swordsman.transform.position = start;
        dummy.enabled = false;
        if (mode == Preview.Idle || mode == Preview.Run)
        {
            ShowLocomotion(mode == Preview.Run, clock, locomotionDirection);
            return;
        }
        if (mode == Preview.Turn)
        {
            if (clock < .48f) ShowTurn(locomotionDirection, aim, clock / .48f);
            else ShowLocomotion(false, clock - .48f, aim);
            return;
        }
        if (aim % 2 != 0 && (mode == Preview.Basic ? !HasCombo(aim) : DirectionCoverage(skillIndex, aim) == "MissingArt"))
        {
            ShowLocomotion(false, clock, aim);
            phase = "Thiếu pose đánh hướng " + DirectionLabels[aim];
            return;
        }
        if (mode == Preview.Basic)
        {
            if (clock < .9f) ShowBasic(clock);
            else ShowLocomotion(false, clock, aim);
            return;
        }

        Vector3 approach = DirectionVector(locomotionDirection) * .72f;
        float lead = mode == Preview.Chain ? ChainLead : 0;
        if (clock < lead)
        {
            if (clock < .4f)
            {
                float stop = locomotionDirection == aim ? .4f : .27f;
                swordsman.transform.position -= approach * (1 - Mathf.Clamp01(clock / stop));
                ShowApproach(clock);
            }
            else ShowBasic(clock - .4f);
            return;
        }
        float time = clock - lead;
        if (skillIndex == 0 && !freeMode)
            previewCamera.transform.position += Vector3.up * (AimVector.y * DashOffset(time) * .34f);
        float duration = CurrentSkillDuration;
        if (skillIndex == 0 && !HasQ1Facing(aim))
        {
            phase = "Q1 thiếu chuỗi riêng cho hướng đánh này";
            ShowFallbackPose();
            return;
        }
        Vector3 displacement = SkillDisplacement(time);
        swordsman.transform.position += displacement;
        if (skillIndex != 0 && !freeMode)
        {
            float followY = Mathf.Sign(displacement.y) * Mathf.Max(0, Mathf.Abs(displacement.y) - .7f);
            previewCamera.transform.position += Vector3.up * (followY * .7f);
        }
        if (time >= duration)
        {
            float follow = time - duration;
            if (mode == Preview.Chain && follow < .9f) ShowBasic(follow);
            else if (mode == Preview.Chain && follow < 1.35f)
            {
                swordsman.transform.position += .675f * ExitProgress(follow - .9f) * DirectionVector(locomotionDirection);
                ShowExitRun(follow - .9f);
            }
            else
            {
                if (mode == Preview.Chain) swordsman.transform.position += .675f * DirectionVector(locomotionDirection);
                ShowLocomotion(false, follow, mode == Preview.Chain ? locomotionDirection : aim);
            }
            return;
        }
        int beat = 0;
        while (beat + 1 < Chosen.Beats.Length && time >= Chosen.Starts[beat + 1]) beat++;
        float u = Mathf.InverseLerp(Chosen.Starts[beat], Chosen.Starts[beat + 1], time);
        var action = skillIndex == 2 && !parry && beat == 2 ? Action.Recover : Chosen.Beats[beat];
        phase = action.ToString();
        dummy.enabled = true;
        dummy.transform.position = StageContactPoint(start, start + AimVector * 1.55f);
        if (skillIndex == 0)
        {
            DrawDirectionalDash(time, start);
            return;
        }
        DrawBeat(action, beat, u, time);
    }

    private void DrawDirectionalDash(float time, Vector3 start)
    {
        int frame = FrameAt(time);
        SpritePose(Q1Sprite(aim, frame), Q1Phases[frame]);
        PlayDashFrame(frame);
        (Vector2 hilt, Vector2 tip) = aim switch
        {
            0 => (new Vector2(157, 77), new Vector2(162, 49)),
            1 => (new Vector2(157, 79), new Vector2(170, 53)),
            2 => (new Vector2(167, 139), new Vector2(211, 139)),
            3 => (new Vector2(167, 154), new Vector2(203, 174)),
            5 => (new Vector2(89, 154), new Vector2(49, 167)),
            6 => (new Vector2(61, 144), new Vector2(28, 143)),
            7 => (new Vector2(102, 75), new Vector2(72, 55)),
            _ => (new Vector2(139, 145), new Vector2(169, 143))
        };
        Vector3 offset = Anchor(tip) - swordsman.transform.position;
        dummy.transform.position = start + AimVector * DashOffset(7 * FrameTime) + offset;
        if (frame == 3 || frame == 9)
        {
            Vector2 plant = (aim, frame) switch
            {
                (0, 3) => new(86, 202),
                (0, 9) => new(134, 206),
                (1, 3) => new(81, 202),
                (1, 9) => new(96, 205),
                (3, 3) => new(84, 199),
                (3, 9) => new(93, 204),
                (5, 3) => new(76, 205),
                (5, 9) => new(93, 206),
                (7, 3) => new(116, 204),
                (7, 9) => new(151, 204),
                _ => new(128, 208)
            };
            Vector3 foot = Anchor(plant);
            foot -= AimVector * (DashOffset(time) - DashOffset(frame * FrameTime));
            Draw(dust, .025f, foot - AimVector * .18f, foot, foot + Vector3.up * .13f);
        }
        if (frame != 7) return;
        DrawTrail(hilt, tip, Vector2.Lerp(hilt, tip, .75f) - new Vector2(0, 8));
        var target = dummy.transform.position;
        Draw(impact, .04f, target + new Vector3(.12f, .09f), target, target + new Vector3(-.14f, .1f));
        dummy.color = new Color(1, .57f, .4f);
        if (shake) previewCamera.transform.position += AimVector * .04f * Mathf.Sin(time * 130);
        if (lastCue == 7) return;
        lastCue = 7;
        if (!scrubbing && hitStop) hold = .045f;
    }

    private void ShowApproach(float time)
    {
        if (locomotionDirection == aim || time < .27f)
        {
            ShowLocomotion(true, time, locomotionDirection);
            return;
        }
        ShowTurn(locomotionDirection, aim, Mathf.InverseLerp(.27f, .4f, time));
    }

    private void ShowExitRun(float time)
    {
        if (locomotionDirection != aim && time < .13f)
        {
            ShowTurn(aim, locomotionDirection, time / .13f);
            return;
        }
        ShowLocomotion(true, time, locomotionDirection);
    }

    private float ExitProgress(float time) =>
        Mathf.InverseLerp(locomotionDirection == aim ? 0 : .13f, ReturnRun, time);

    private void ShowTurn(int from, int to, float turn)
    {
        int delta = (to - from + 12) % 8 - 4;
        var frames = forms[formIndex].turns;
        if (from != to && frames is { Length: 16 })
        {
            int index = (Mathf.RoundToInt((from + delta * turn) * 2) + 16) % 16;
            SpritePose(frames[index], "Turn · key pose theo hướng");
            return;
        }
        int step = Mathf.RoundToInt(delta * turn);
        int toward = (from + step + 8) % 8;
        ShowLocomotion(false, 0, HasDirection(formIndex, toward) ? toward : from);
        phase = "Turn · pose nguồn";
    }

    private void ResetLines()
    {
        impact.enabled = dust.enabled = swordTrail.enabled = false;
        impact.startColor = impact.endColor = new Color(1, .82f, .45f);
        dust.startColor = dust.endColor = new Color(.82f, .68f, .49f, .64f);
        swordTrail.startColor = swordTrail.endColor = new Color(.72f, .91f, 1, .78f);
        previewCamera.transform.position = cameraRest;
        dummy.color = Color.white;
    }

    private Vector3 Travel(float time)
    {
        if (skillIndex == 15) return AssassinSlashPath(time);
        if (skillIndex == 12)
            return AimVector * (.72f * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(Chosen.Starts[1], Chosen.Starts[4], time)));
        if (skillIndex == 14)
            return AimVector * (EaseBeat(1, time) * .62f + EaseBeat(4, time) * .37f + EaseBeat(5, time) * .29f);
        float speed = skillIndex == 3 || skillIndex == 5 || skillIndex == 16 ? 1.2f : .72f;
        if (skillIndex == 3) speed = 1.96f;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < Chosen.Beats.Length && time > Chosen.Starts[i]; i++)
        {
            Action beat = Chosen.Beats[i];
            if (beat != Action.Advance && beat != Action.Vanish) continue;
            result += AimVector * (speed * EaseBeat(i, time));
        }
        return result;
    }

    private float EaseBeat(int beat, float time) =>
        Mathf.SmoothStep(0, 1, Mathf.InverseLerp(Chosen.Starts[beat], Chosen.Starts[beat + 1], time));

    private Vector3 AssassinSlashPath(float time)
    {
        Vector3 across = Vector3.Cross(Vector3.forward, AimVector);
        Vector3 first = AimVector * 1.95f + across * .40f;
        Vector3 second = AimVector * 1.23f - across * .52f;
        Vector3 third = AimVector * 1.73f + across * .16f;
        return first * EaseBeat(1, time) + (second - first) * EaseBeat(4, time) + (third - second) * EaseBeat(7, time);
    }

    private void ShowFallbackPose()
    {
        if (HasDirection(formIndex, aim) && aim % 2 != 0)
        {
            string label = phase;
            ShowLocomotion(false, 0, aim);
            phase = label;
        }
        else if (aim % 2 == 0)
        {
            NativePose(swordsman, "Idle", aim, 0);
        }
        else actor.enabled = false;
    }

    private void ShowLocomotion(bool run, float time, int direction)
    {
        string label = (run ? "Run" : "Idle") + " · " + DirectionLabels[direction];
        if (!HasDirection(formIndex, direction))
        {
            actor.enabled = false;
            phase = "Thiếu sprite " + label;
            return;
        }
        if (direction % 2 == 0)
        {
            phase = label;
            NativePose(swordsman, run ? "Run" : "Idle", direction,
                Mathf.Repeat(time / (run ? .5f : .67f), 1));
            return;
        }
        int frame = direction / 2 * 5;
        int idleFrame = direction / 2 * 4;
        if (!run && forms[formIndex].diagonalIdle[idleFrame] != null)
        {
            idleFrame += Mathf.FloorToInt(Mathf.Repeat(time, .68f) / .17f);
            SpritePose(forms[formIndex].diagonalIdle[idleFrame], label);
            return;
        }
        if (run) frame += 1 + Mathf.FloorToInt(Mathf.Repeat(time / .5f, 1) * 4);
        SpritePose(forms[formIndex].locomotion[frame], label);
    }

    private static void NativePose(Animator target, string action, int direction, float normalized)
    {
        target.enabled = true;
        target.Play(action + "_" + CardinalStates[direction / 2], 0, Mathf.Clamp(normalized, 0, .99f));
        target.Update(0);
    }

    private void ShowBasic(float time)
    {
        if (aim == 2 && formIndex == 0)
        {
            Basic(time);
            return;
        }
        if (!HasCombo(aim))
        {
            if (aim % 2 != 0)
            {
                ShowLocomotion(false, 0, aim);
                phase = "Thiếu combo · " + DirectionLabels[aim];
                return;
            }
            phase = "Đòn nguồn · thiếu combo hướng " + DirectionLabels[aim];
            NativePose(swordsman, "Attack", aim, time / .9f);
            return;
        }
        int count = BasicFrameCount(aim);
        int frame = Mathf.Clamp(Mathf.FloorToInt(time / AttackDuration * count), 0, count - 1);
        Sprite sprite = count == 12 ? BasicFrames(aim)[frame] : forms[formIndex].basicDiagonal[aim / 2 * 8 + frame];
        SpritePose(sprite, "Basic Blockout · " + (frame * (formIndex == 1 ? 3 : 2) / count + 1));
        PlayBasicFrame(frame, count);
        float release = (count == 12 ? 9 : 5) * AttackDuration / count;
        if (formIndex == 3 && time >= release && showEffects)
        {
            float age = time - release;
            basicWave.SetActive(true);
            basicWave.transform.position = swordsman.transform.position + WorldSpriteOffset(QiBasicBlade[aim]) + AimVector * (4 * age);
            basicWave.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AimVector.y, AimVector.x) * Mathf.Rad2Deg);
            basicWaveClip.SampleAnimation(basicWave, age);
        }
    }

    private void DrawBeat(Action action, int beat, float u, float time)
    {
        bool contact = action == Action.Hit || action == Action.Land || action == Action.Burst || action == Action.Catch;
        var position = swordsman.transform.position;
        int bodyDirection = skillIndex == 3 && beat >= 2 ? (aim + 4) % 8 :
            skillIndex == 15 ? (aim + (beat >= 7 ? 7 : beat >= 4 ? 3 : 0)) % 8 : aim;
        var take = FindTake(formIndex, skillIndex, bodyDirection);
        if (take == null && bodyDirection != aim) take = FindTake(formIndex, skillIndex, aim);
        sampledTake = null;
        bool westCore = aim == 6 && forms[formIndex].heroWest is { Length: 24 };
        bool customSlam = skillIndex == 12 && (aim == 2 && forms[formIndex].slam.Length == 12 || westCore);
        if (skillIndex == 18 && action == Action.Swing && cyclone.Length == 16)
        {
            int start = beat == 2 ? 0 : 8;
            int turn = Mathf.Min(7, Mathf.FloorToInt(u * 8));
            SpritePose(cyclone[start + (aim + turn) % 8], action.ToString());
        }
        else if (take != null)
        {
            ShowDirectionalPose(take, action, beat, u);
            if (skillIndex == 12) DrawLeap(time, position);
        }
        else if (customSlam)
        {
            int frame = action switch
            {
                Action.Prepare => Mathf.Min(2, Mathf.FloorToInt(u * 3)),
                Action.Rise => 3 + Mathf.Min(2, Mathf.FloorToInt(u * 3)),
                Action.Dive or Action.Swing => 6,
                Action.Land => 7 + Mathf.Min(2, Mathf.FloorToInt(u * 3)),
                _ => 10 + Mathf.Min(1, Mathf.FloorToInt(u * 2))
            };
            if (westCore)
            {
                frame = action switch
                {
                    Action.Prepare => 0,
                    Action.Rise => u < .5f ? 1 : 2,
                    Action.Dive or Action.Swing => 3,
                    Action.Land => u < .5f ? 4 : 5,
                    _ => u < .5f ? 6 : 7
                };
            }
            SpritePose(westCore ? forms[formIndex].heroWest[frame] : forms[formIndex].slam[frame], action.ToString());
            DrawLeap(time, position);
        }
        else if ((aim == 2 && forms[formIndex].hero.Length == 16 || westCore) && (skillIndex == 13 || skillIndex == 14))
        {
            int frame = skillIndex == 13
                ? CoreComboPose(beat, u)
                : beat switch { 0 => u < .5f ? 8 : 9, 1 => 10, 2 or 3 => 10, 4 => 11, 5 => u < .5f ? 12 : 13, 6 => 14, _ => 15 };
            SpritePose(westCore ? forms[formIndex].heroWest[frame + 8] : forms[formIndex].hero[frame], action.ToString());
        }
        else if (aim == 2 && HasContinuousSkill(skillIndex))
        {
            int profile = skillIndex switch { 1 => 0, 2 => 1, 4 => 2, 6 => 3, 9 => 4, 23 => 5, _ => -1 };
            float pose = profile < 0 ? (beat + u) * 12 / Chosen.Beats.Length
                : Mathf.Lerp(ContinuousBeats[profile][beat], ContinuousBeats[profile][beat + 1], u);
            int frame = Mathf.Min(11, Mathf.FloorToInt(pose));
            if (skillIndex == 2 && !parry && action == Action.Recover) frame = 10 + Mathf.FloorToInt(u * 2);
            SpritePose(continuousEast[skillIndex * 12 + frame], action.ToString());
        }
        else if (aim == 2 && signatures[skillIndex * 4] != null &&
            !((skillIndex == 1 || skillIndex == 18) && action == Action.Swing))
        {
            int key = action switch
            {
                Action.Prepare => 0,
                Action.Recover => 3,
                Action.Guard or Action.Rise or Action.Advance or Action.Vanish or Action.Charge => 1,
                Action.Swing => u < .55f ? 1 : 2,
                _ => 2
            };
            SpritePose(signatures[skillIndex * 4 + key], action.ToString());
        }
        else
        {
            float pose = action switch
            {
                Action.Prepare or Action.Guard or Action.Charge => .08f + .18f * u,
                Action.Swing or Action.Clone => .27f + .47f * u,
                Action.Recover => .72f + .26f * u,
                _ => .44f + .23f * u
            };
            int poseDirection = aim;
            if ((skillIndex == 1 || skillIndex == 18) && action == Action.Swing)
                poseDirection = (aim + 2 * Mathf.FloorToInt(u * 4)) % 8;
            NativePose(swordsman, "Attack", poseDirection, pose);
        }

        FrameAnchors anchors = MeasuredAnchors();
        if (action == Action.Vanish) actor.color = new Color(1, 1, 1, 1 - u);
        if (action == Action.Return) actor.color = new Color(1, 1, 1, u);
        if (action == Action.Clone && skillIndex == 14) actor.enabled = false;
        if (action == Action.Clone) DrawShadows(u, beat);
        if (skillIndex == 13 && action == Action.Charge)
        {
            Vector3 center = AnchorLocation(position, anchors.hilt, position + AimVector * (aim == 2 ? .48f : .35f));
            DrawCharge(center, u, beat > 1 ? .31f : .21f);
        }
        if (skillIndex == 2 && action == Action.Guard)
        {
            Vector3 blade = aim == 2 && HasContinuousSkill(2)
                ? position + WorldSpriteOffset(new Vector2(182, 109)) : position + AimVector * .43f;
            blade = AnchorLocation(position, anchors.tip, blade);
            if (parry && u > .35f)
            {
                Draw(impact, .035f * (1 - u), blade + new Vector3(.13f, .16f), blade, blade + new Vector3(.18f, -.08f));
                if (!parryCuePlayed && !scrubbing)
                {
                    parryCuePlayed = true;
                    Play(Cue.Parry);
                }
            }
        }
        if (skillIndex == 4 && (action == Action.Mark || action == Action.Charge))
        {
            swordTrail.startColor = swordTrail.endColor = new Color(.91f, .19f, .3f, .76f);
            Vector3 mark = dummy.transform.position + Vector3.up * .09f;
            float radius = .14f + .04f * Mathf.Sin(u * Mathf.PI);
            Draw(swordTrail, .026f, mark + Vector3.up * radius, mark + Vector3.right * radius,
                mark - Vector3.up * radius, mark - Vector3.right * radius, mark + Vector3.up * radius);
        }
        if ((skillIndex == 3 || skillIndex == 5 || skillIndex == 15) &&
            (action == Action.Vanish || action == Action.Advance))
        {
            Vector3 center = AnchorLocation(position, anchors.foot, position - Vector3.up * .25f);
            Draw(dust, .065f * (1 - u), center - AimVector * .88f, center - AimVector * .42f, center);
        }
        if (action == Action.Rain)
        {
            Vector3 sideOfTarget = Vector3.Cross(Vector3.forward, AimVector) * (beat % 2 == 0 ? -.42f : .42f);
            Vector3 landing = dummy.transform.position + sideOfTarget;
            Vector3 pointOfBlade = landing + Vector3.up * ((1 - u) * 1.4f);
            Draw(swordTrail, .05f, pointOfBlade + new Vector3(-.08f, .24f), pointOfBlade,
                pointOfBlade + new Vector3(.08f, .24f), pointOfBlade + Vector3.up * .36f,
                pointOfBlade + new Vector3(-.08f, .24f));
        }

        if (contact)
        {
            dummy.color = Color.Lerp(new Color(1, .62f, .44f), Color.white, u);
            if (shake) previewCamera.transform.position += AimVector * (.055f * Mathf.Sin(u * 25) * (1 - u));
            Vector3 at = action == Action.Land
                ? customSlam ? position + LandOffset(westCore) : position - Vector3.up * .62f
                : dummy.transform.position;
            at = skillIndex == 12 && action == Action.Land ? LockedLanding(take, position, at) :
                AnchorLocation(position, action == Action.Land ? anchors.landing : anchors.contact, at);
            DrawImpactSignature(at, action, u);
        }
        if (lastCue != beat)
        {
            lastCue = beat;
            if (!scrubbing && hitStop) hold = ImpactPause(action, beat);
            PlaySkillCue(action);
        }
        if (skillIndex == 14 && action is Action.Vanish or Action.Return)
        {
            DrawVanishWisp(position, action == Action.Vanish ? 1 - u : u);
            return;
        }
        if (skillIndex == 7 && action == Action.Swing)
        {
            DrawHeavyCleave(AnchorLocation(position, anchors.hilt, position + AimVector * .30f), u);
            return;
        }
        if ((skillIndex == 1 || skillIndex == 7 || skillIndex == 18) && action == Action.Hit) return;
        if (!showEffects || effect == null) return;

        Vector3 point = AnchorLocation(position, anchors.tip, position + AimVector * .68f);
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        float rotation = Mathf.Atan2(AimVector.y, AimVector.x) * Mathf.Rad2Deg;
        if ((skillIndex == 1 || skillIndex == 18) && action == Action.Swing)
        {
            point = position - Vector3.up * .28f;
            rotation += 360 * u;
            if (skillIndex == 18)
            {
                DrawCyclone(point, u, beat);
                return;
            }
        }
        else if (action == Action.Wave)
        {
            if (skillIndex == 13 && aim == 2)
            {
                int release = CoreRelease(beat);
                point = position + WorldSpriteOffset(CoreReleaseBlade[formIndex, release]);
            }
            else if (skillIndex == 13 && westCore)
            {
                int release = CoreRelease(beat);
                point = position + WorldSpriteOffset(WestCoreReleaseBlade[formIndex, release]);
            }
            else if (aim == 2 && HasContinuousSkill(skillIndex) && skillIndex is 9 or 23)
                point = position + WorldSpriteOffset(skillIndex == 9 ? new Vector2(252, 161) : new Vector2(248, 163));
            if (skillIndex == 13)
            {
                DrawCoreWave(point, u, CoreRelease(beat));
                return;
            }
            point += AimVector * (u * 1.8f);
        }
        else if (action == Action.Recall)
        {
            if (skillIndex == 23 && aim == 2 && HasContinuousSkill(skillIndex))
                point = position + WorldSpriteOffset(new Vector2(188, 171));
            point += AimVector * (1 - u) * 1.8f;
            rotation += 180;
        }
        else if (action == Action.Chase)
        {
            Vector3 path = dummy.transform.position - point;
            point += path * u + side * Mathf.Sin(u * Mathf.PI) * .38f;
            Vector3 tangent = path + side * Mathf.Cos(u * Mathf.PI) * (Mathf.PI * .38f);
            rotation = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        }
        else if (action == Action.Mark || action == Action.Burst || action == Action.Hit) point = dummy.transform.position;
        else if (action == Action.Land)
            point = customSlam ? position + LandOffset(westCore)
                : skillIndex == 6 && aim == 2 && HasContinuousSkill(skillIndex)
                    ? position + WorldSpriteOffset(new Vector2(221, 239)) : position - Vector3.up * .62f;
        else if (action == Action.Aura || action == Action.Guard || action == Action.Vanish)
            point = AnchorLocation(position, anchors.aura, position);
        else return;
        if (action == Action.Land) point = skillIndex == 12 ? LockedLanding(take, position, point) :
            AnchorLocation(position, anchors.landing, point);

        if (skillIndex == 12 && action == Action.Land) u = Mathf.Lerp(.48f, .9f, u);
        if (skillIndex == 14 && action == Action.Hit) u = Mathf.Lerp(.55f, .94f, u);
        effect.transform.localScale = Vector3.one * (skillIndex == 13 && action == Action.Wave
            ? .55f + beat * .09f : skillIndex == 12 ? .72f : .57f);
        bool behindActor = AimVector.y > .5f || action == Action.Land || skillIndex == 14;
        effectRenderer.sortingOrder = behindActor ? -2 : 2;
        effect.transform.SetPositionAndRotation(point, Quaternion.Euler(0, 0, rotation));
        effect.SetActive(true);
        effectClip.SampleAnimation(effect, Mathf.Min(.99f, u) * effectClip.length);
        if (skillIndex == 12 || skillIndex == 14)
        {
            Color finisher = effectRenderer.color;
            finisher.a = skillIndex == 14 ? .53f : .68f;
            effectRenderer.color = finisher;
        }
    }

    private Vector3 WorldSpriteOffset(Vector2 pixel)
    {
        Sprite sprite = actor.sprite;
        return new Vector3(pixel.x - sprite.pivot.x, sprite.rect.height - pixel.y - sprite.pivot.y) / sprite.pixelsPerUnit;
    }

    private Vector3 LandOffset(bool westCore)
    {
        return WorldSpriteOffset(westCore ? WestLandingBlade[formIndex] : LandingBlade[formIndex]);
    }

    private void DrawCharge(Vector3 center, float time, float radius)
    {
        var points = new Vector3[13];
        for (int i = 0; i < points.Length; i++)
        {
            float angle = i * Mathf.PI / 6 + time * 4;
            points[i] = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * .65f) * (radius * (.65f + .35f * time));
        }
        Draw(swordTrail, .025f + .025f * time, points);
    }

    private void DrawShadows(float u, int beat)
    {
        if (shadows == null)
        {
            shadows = new Animator[2];
            for (int i = 0; i < 2; i++)
            {
                var copy = Instantiate(forms[formIndex].prefab);
                shadows[i] = copy.GetComponent<Animator>();
                shadows[i].speed = 0;
                copy.GetComponent<SpriteRenderer>().color = new Color(.64f, .68f, 1, .58f);
            }
        }
        Vector3 perpendicular = Vector3.Cross(Vector3.forward, AimVector);
        for (int i = 0; i < shadows.Length; i++)
        {
            var copy = shadows[i];
            copy.gameObject.SetActive(true);
            int direction = skillIndex == 14
                ? (aim + (beat % 2 == 0 ? i * 2 : i == 0 ? 7 : 5)) % 8
                : aim;
            var take = FindTake(formIndex, skillIndex, direction);
            if (take == null && direction % 2 != 0)
            {
                take = FindTake(formIndex, skillIndex, aim);
                direction = aim;
            }
            Vector3 travel = DirectionVector(direction);
            float lane = i == 0 ? .36f : -.36f;
            Vector3 offset = perpendicular * lane;
            float sweep = skillIndex == 14 ? Mathf.SmoothStep(0, 1, u) * 1.75f - .93f : u - .72f;
            copy.transform.position = dummy.transform.position + travel * sweep + offset;
            float swing = Mathf.Clamp01(u + (i == beat % 2 ? .1f : 0));
            int strikePose = Mathf.Min(2, Mathf.FloorToInt(swing * 3));
            var copyRenderer = copy.GetComponent<SpriteRenderer>();
            Sprite authored = take?.frames[4 + strikePose];
            if (authored == null && skillIndex == 14)
            {
                var form = forms[formIndex];
                if (direction == 2 && form.hero is { Length: 16 }) authored = form.hero[12 + strikePose];
                if (direction == 6 && form.heroWest is { Length: 24 }) authored = form.heroWest[20 + strikePose];
            }
            if (authored == null && skillIndex == 17 && direction == 2 && HasContinuousSkill(17))
                authored = continuousEast[17 * 12 + 5 + strikePose];
            if (authored != null)
            {
                copy.enabled = false;
                copyRenderer.sprite = authored;
            }
            else NativePose(copy, "Attack", direction, swing);
            copyRenderer.sortingOrder = i == 0 ? 1 : -2;
            if (skillIndex == 14 && swing is > .27f and < .88f)
            {
                var cut = i == 0 ? swordTrail : impact;
                var edge = copy.transform.position + travel * .18f;
                var normal = Vector3.Cross(Vector3.forward, travel);
                cut.startColor = new Color(.52f, .74f, .98f, .19f);
                cut.endColor = new Color(.81f, .91f, 1, .78f);
                Draw(cut, .045f, edge - travel * .62f - normal * .14f, edge - travel * .32f, edge + normal * .10f);
            }
        }
    }

}
