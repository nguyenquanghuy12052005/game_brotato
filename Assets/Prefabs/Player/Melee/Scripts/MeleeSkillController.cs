using System;
using System.Collections.Generic;
using UnityEngine;
using static MeleeSkillController;
using Action = MeleeSkillController.Action;
using Beat = MeleeSkillController.Action;
using CueAction = MeleeSkillController.Action;

internal static class MeleeSkillController
{
    internal enum Action { Prepare, Advance, Vanish, Return, Swing, Hit, Guard, Charge, Rise, Dive, Land, Mark, Burst,
        Aura, Wave, Chase, Recall, Catch, Clone, Rain, Recover }

    internal sealed class Skill
    {
        internal readonly string Name;
        internal readonly int Form;
        internal readonly string Effect;
        internal readonly Action[] Beats;
        internal float[] Starts;

        internal Skill(string name, int form, string effect, params Action[] beats)
        {
            Name = name;
            Form = form;
            Effect = effect;
            Beats = beats;
        }

        internal bool Supports(int form) => Form == -1 || Form == form;
    }

    private static Skill Entry(string name, int form, string effect, params Action[] actions)
    {
        var beats = new Action[actions.Length + 2];
        beats[0] = Action.Prepare;
        Array.Copy(actions, 0, beats, 1, actions.Length);
        beats[^1] = Action.Recover;
        return new Skill(name, form, effect, beats);
    }

    internal static readonly Skill[] Skills =
    {
        Entry("Q1 · Kiếm Lướt / Phong Ảnh Trảm", 0, "", Action.Advance, Action.Swing, Action.Hit),
        Entry("Q2 · Hoành Nguyệt Trảm", 0, "MeleeSkills/DaoWarrior/HoanhTao", Action.Swing, Action.Hit),
        Entry("Q3 · Phản Kiếm Quyết", 0, "", Action.Guard, Action.Swing, Action.Hit),
        Entry("E1 · Ám Bộ Trảm", 1, "", Action.Vanish, Action.Return, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("E2 · Huyết Ấn", 1, "MeleeSkills/Assassin/AnTuHuyet", Action.Swing, Action.Mark, Action.Charge, Action.Burst),
        Entry("E3 · Tàng Ảnh", 1, "", Action.Vanish, Action.Advance, Action.Return, Action.Swing, Action.Hit),
        Entry("E1 · Chấn Địa Trảm", 2, "MeleeSkills/DaoWarrior/DiaLietTram", Action.Charge, Action.Swing, Action.Land),
        Entry("E2 · Hoành Thiên Đao", 2, "MeleeSkills/DaoWarrior/HoanhTao", Action.Charge, Action.Swing, Action.Hit),
        Entry("E3 · Bất Động Chiến Thế", 2, "MeleeSkills/DaoWarrior/TrongDao", Action.Guard, Action.Aura, Action.Swing, Action.Hit, Action.Swing),
        Entry("E1 · Nguyệt Nha Trảm", 3, "QiSwordsman/QiWave", Action.Swing, Action.Wave, Action.Hit),
        Entry("E2 · Lưỡng Nghi Kiếm", 3, "MeleeSkills/QiSwordsman/PhanQuang", Action.Swing, Action.Wave, Action.Swing, Action.Hit, Action.Burst),
        Entry("E3 · Kiếm Giới", 3, "MeleeSkills/QiSwordsman/KiemAn", Action.Swing, Action.Mark, Action.Aura, Action.Burst, Action.Burst),
        Entry("R1 · Thiên Vẫn Trảm", -1, "MeleeUltimates/ThienVan", Action.Rise, Action.Dive, Action.Swing, Action.Land),
        Entry("R2 · Tam Thiên Kiếm Quyết", -1, "QiSwordsman/QiWave", Action.Charge, Action.Swing, Action.Wave, Action.Swing, Action.Wave, Action.Charge, Action.Swing, Action.Wave, Action.Burst),
        Entry("R3 · Vạn Ảnh Tuyệt Sát", -1, "MeleeUltimates/VanAnh", Action.Vanish, Action.Clone, Action.Clone, Action.Return, Action.Swing, Action.Hit),
        Entry("F1 · Dạ Vũ Liên Trảm", 1, "", Action.Advance, Action.Swing, Action.Hit, Action.Advance, Action.Swing, Action.Hit, Action.Advance, Action.Swing, Action.Hit),
        Entry("F2 · Đoạt Mệnh", 1, "MeleeSkills/Assassin/AnTuHuyet", Action.Advance, Action.Charge, Action.Swing, Action.Hit, Action.Burst),
        Entry("F3 · Song Ảnh Truy Sát", 1, "MeleeSkills/Assassin/TamLienThu", Action.Clone, Action.Swing, Action.Clone, Action.Hit),
        Entry("F1 · Cuồng Phong Đao", 2, "MeleeSkills/DaoWarrior/HoanhTao", Action.Charge, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("F2 · Đoạn Sơn Trảm", 2, "MeleeSkills/DaoWarrior/DiaLietTram", Action.Charge, Action.Charge, Action.Swing, Action.Land, Action.Burst),
        Entry("F3 · Bất Diệt Chiến Ý", 2, "MeleeSkills/DaoWarrior/TrongDao", Action.Guard, Action.Aura, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("F1 · Kiếm Vũ Thiên Hà", 3, "MeleeSkills/QiSwordsman/KiemAn", Action.Charge, Action.Swing, Action.Rain, Action.Rain, Action.Hit),
        Entry("F2 · Phi Kiếm Truy Hồn", 3, "QiSwordsman/QiWave", Action.Swing, Action.Chase, Action.Hit),
        Entry("F3 · Hồi Nguyệt Kiếm", 3, "MeleeSkills/QiSwordsman/HoiNguyet", Action.Swing, Action.Wave, Action.Recall, Action.Catch)
    };

    static MeleeSkillController()
    {
        // Prepare first, recover last. Short strikes follow longer anticipation.
        float[][] timing =
        {
            new[] { .20f, .16f, .10f, .13f, .31f },
            new[] { .24f, .60f, .18f, .30f },
            new[] { .22f, .33f, .14f, .15f, .32f },
            new[] { .17f, .24f, .15f, .13f, .12f, .11f, .14f, .25f },
            new[] { .29f, .16f, .27f, .36f, .19f, .26f },
            new[] { .22f, .26f, .29f, .20f, .15f, .15f, .28f },
            new[] { .28f, .26f, .15f, .24f, .35f },
            new[] { .25f, .29f, .21f, .47f, .36f },
            new[] { .23f, .24f, .67f, .19f, .16f, .23f, .33f },
            new[] { .23f, .17f, .32f, .14f, .28f },
            new[] { .22f, .15f, .24f, .17f, .14f, .25f, .30f },
            new[] { .23f, .22f, .23f, .31f, .21f, .23f, .33f },
            new[] { .23f, .28f, .19f, .22f, .24f, .39f },
            new[] { .19f, .32f, .14f, .23f, .17f, .23f, .23f, .20f, .27f, .20f, .34f },
            new[] { .24f, .23f, .24f, .20f, .23f, .15f, .20f, .32f },
            new[] { .19f, .23f, .115f, .115f, .22f, .115f, .115f, .23f, .13f, .17f, .28f },
            new[] { .19f, .27f, .18f, .13f, .13f, .21f, .26f },
            new[] { .21f, .29f, .19f, .27f, .18f, .30f },
            new[] { .23f, .22f, .48f, .14f, .55f, .17f, .34f },
            new[] { .27f, .24f, .26f, .15f, .19f, .20f, .37f },
            new[] { .24f, .23f, .28f, .18f, .14f, .20f, .17f, .35f },
            new[] { .23f, .31f, .17f, .24f, .24f, .20f, .32f },
            new[] { .22f, .17f, 1.57f, .18f, .29f },
            new[] { .25f, .15f, .48f, .55f, .18f, .27f },
        };
        for (int i = 0; i < Skills.Length; i++)
        {
            var skill = Skills[i];
            if (timing[i].Length != skill.Beats.Length) throw new InvalidOperationException(skill.Name);
            skill.Starts = new float[skill.Beats.Length + 1];
            for (int beat = 0; beat < timing[i].Length; beat++)
                skill.Starts[beat + 1] = skill.Starts[beat] + timing[i][beat];
        }
    }
}

public sealed partial class MeleeController
{
    #region Forms, locomotion and skill staging

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
    // Blade tips on the existing Basic contact frames, in N/NE/E/SE/S/SW/W/NW order.
    private static readonly Vector2[] QiBasicFirstTip =
    {
        new(243, 156), new(254, 167), new(259, 168), new(252, 172),
        new(99, 162), new(72, 180), new(68, 167), new(68, 170)
    };
    private static readonly Vector2[] AssassinCutTips =
    {
        new(207, 143), new(210, 96), new(200, 125), new(204, 139),
        new(112, 119), new(67, 146), new(46, 132), new(59, 105),
        new(161, 77), new(205, 133), new(202, 65), new(160, 133),
        new(86, 88), new(126, 134), new(49, 75), new(72, 138)
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
        UpdatePreviewViewport();
        Render();
        scrubbing = false;
    }

    private void InitRoster()
    {
        LoadVisualAnchors();
        InitStyleAtlas();
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
        previewV2 = false;
        SetFreeMode(false);
        SelectForm(form);
        skillIndex = skill;
        skillGroup = Array.FindIndex(V2Slots, group => Array.IndexOf(group, skill) >= 0);
        if (form > 0) branch = form;
        previewLoadout[skillGroup] = Array.IndexOf(V2Slots[skillGroup], skill) % 3;
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
        effect != null && effect.activeSelf || basicWave != null && basicWave.activeSelf ||
        StyleReady && Array.Exists(stylePool, sprite => sprite != null && sprite.enabled);

    public bool HasContinuousSkill(int skill) => continuousEast[skill * 12] != null;
    private void RenderOther()
    {
        ClearFeedback();
        ResetLines();
        if (!freeMode) previewCamera.transform.position += PracticeFramingOffset;
        Vector3 start = freeMode ? castOrigin : new Vector3(-1.4f, .667f);
        Preview mode = freeMode ? Preview.Q1 : selection;
        if (previewV2 && skillIndex < 3 && mode is Preview.Q1 or Preview.Chain) mode = Preview.Basic;
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
                int idleFacing = previewV2 && skillIndex == 3 && ambushFacing >= 0 ? ambushFacing : aim;
                ShowLocomotion(false, follow, mode == Preview.Chain ? locomotionDirection : idleFacing);
            }
            return;
        }
        int beat = 0;
        while (beat + 1 < Chosen.Beats.Length && time >= Chosen.Starts[beat + 1]) beat++;
        float u = Mathf.InverseLerp(Chosen.Starts[beat], Chosen.Starts[beat + 1], time);
        var action = skillIndex == 2 && !parry && beat == 2 ? Action.Recover : Chosen.Beats[beat];
        phase = action.ToString();
        dummy.enabled = !previewV2 || IsCore;
        dummy.transform.position = previewV2 && IsBranchSkill
            ? AimPoint(start) + DummyLift : StageContactPoint(start, start + AimVector * 1.55f);
        if (skillIndex == 0)
        {
            DrawDirectionalDash(time, start);
            return;
        }
        DrawBeat(action, beat, u, time, start);
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
        if (previewV2 && skillIndex == 5) return AimVector * (4.75f * EaseBeat(2, time));
        if (previewV2 && skillIndex == 16) return AimVector * (6.5f * EaseBeat(3, time));
        float speed = skillIndex == 3 || skillIndex == 5 || skillIndex == 16 ? 1.2f : .72f;
        if (skillIndex == 3) speed = previewV2 ? 3.15f : 1.96f;
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
        Vector3 route = first * EaseBeat(1, time) + (second - first) * EaseBeat(4, time) + (third - second) * EaseBeat(7, time);
        return previewV2 && skillIndex == 15 ? route * 1.65f : route;
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
        sampledTake = null;
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
        Sprite sprite = BasicSprite(aim, frame);
        SpritePose(sprite, "Basic Blockout · " + (frame * (formIndex == 1 ? 3 : 2) / count + 1));
        SampleBasicHit(time, frame, count);
        PlayBasicFrame(frame, count);
        float release = (count == 12 ? 9 : 5) * AttackDuration / count;
        if (formIndex == 3 && time >= release && showEffects && !StyleReady)
        {
            float age = time - release;
            basicWave.SetActive(true);
            basicWave.transform.position = swordsman.transform.position + WorldSpriteOffset(QiBasicBlade[aim]) + AimVector * (4 * age);
            basicWave.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AimVector.y, AimVector.x) * Mathf.Rad2Deg);
            basicWaveClip.SampleAnimation(basicWave, age);
        }
    }

    private Sprite BasicSprite(int direction, int frame) => BasicFrameCount(direction) == 12
        ? BasicFrames(direction)[frame] : forms[formIndex].basicDiagonal[direction / 2 * 8 + frame];

    private int AmbushFacing(Vector3 position)
    {
        if (ambushFacing >= 0 && !scrubbing) return ambushFacing;
        int reverse = (aim + 4) % 8;
        Vector3 ground = position + TrainingFloor;
        PreviewTarget target = NearestTarget(ground, DirectionVector(reverse), true);
        Vector3 toward = target == null ? Vector3.zero : target.Ground - ground;
        int facing = toward.sqrMagnitude < .001f ? reverse :
            (Mathf.RoundToInt(Mathf.Atan2(toward.x, toward.y) / (Mathf.PI / 4)) + 8) % 8;
        if (!scrubbing) ambushFacing = facing;
        return facing;
    }

    private void ShowAmbushPose(Action action, int beat, float progress, int facing)
    {
        if (action == Action.Return)
        {
            ShowTurn(aim, facing, progress);
            return;
        }
        int count = BasicFrameCount(facing);
        int firstCut = BasicContactFrame(1, count, 0);
        int secondCut = BasicContactFrame(1, count, 1);
        int frame = beat switch
        {
            3 => Mathf.FloorToInt(Mathf.Lerp(0, firstCut, progress)),
            4 => firstCut,
            5 => Mathf.FloorToInt(Mathf.Lerp(firstCut + 1, secondCut, progress)),
            6 => secondCut,
            _ => progress < .52f ? secondCut + 1 : count - 1
        };
        SpritePose(BasicSprite(facing, frame), action.ToString());
    }

    private void DrawBeat(Action action, int beat, float u, float time, Vector3 origin)
    {
        bool contact = action == Action.Hit || action == Action.Land || action == Action.Burst || action == Action.Catch;
        var position = swordsman.transform.position;
        int bodyDirection = skillIndex == 3 && beat >= 2 ? (previewV2 ? AmbushFacing(position) : (aim + 4) % 8) :
            skillIndex == 15 ? (aim + (beat >= 7 ? 7 : beat >= 4 ? 3 : 0)) % 8 : aim;
        var take = FindTake(formIndex, skillIndex, bodyDirection);
        if (take == null && bodyDirection != aim) take = FindTake(formIndex, skillIndex, aim);
        sampledTake = null;
        bool westCore = aim == 6 && forms[formIndex].heroWest is { Length: 24 };
        bool customSlam = skillIndex == 12 && (aim == 2 && forms[formIndex].slam.Length == 12 || westCore);
        bool throwingMiddleDagger = previewV2 && skillIndex == 17 && beat == 3 && HasCombo(aim);
        if (previewV2 && skillIndex == 3 && beat >= 2 && HasCombo(bodyDirection))
            ShowAmbushPose(action, beat, u, bodyDirection);
        else if (throwingMiddleDagger)
        {
            int contactFrame = BasicContactFrame(1, BasicFrameCount(aim), 0);
            SpritePose(BasicSprite(aim, contactFrame + (u < .34f ? 0 : 1)), action.ToString());
        }
        else if (previewV2 && skillIndex == 8 && action is Action.Guard or Action.Aura or Action.Swing or Action.Hit &&
            FindTake(formIndex, action is Action.Guard or Action.Aura ? 6 : 7, aim) is { } daoPose)
        {
            float pose = action switch
            {
                Action.Guard => Mathf.Lerp(.35f, .65f, u),
                Action.Aura => Mathf.Lerp(.65f, .76f, u),
                Action.Hit => Mathf.Lerp(.70f, .94f, u),
                _ => beat < 4 ? Mathf.Lerp(.23f, .70f, u) : Mathf.Lerp(.94f, 1, u)
            };
            ShowTakeFrame(daoPose, Mathf.FloorToInt(pose * (daoPose.frames.Length - 1)), action);
        }
        else if (skillIndex == 18 && action == Action.Swing && cyclone.Length == 16)
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
        Vector3 weapon = throwingMiddleDagger
            ? position + WorldSpriteOffset(AssassinCutTips[aim])
            : AnchorLocation(position, anchors.tip, position + AimVector * .68f);
        if (previewV2 && IsBranchSkill)
        {
            Vector3 landing = AnchorLocation(position, anchors.landing, position + TrainingFloor + AimVector * 1.1f);
            SampleBranchPreview(action, beat, u, origin, weapon, landing, bodyDirection);
        }
        if (action == Action.Vanish) actor.color = new Color(1, 1, 1, 1 - u);
        if (action == Action.Return) actor.color = new Color(1, 1, 1, u);
        if (action == Action.Clone && skillIndex == 14) actor.enabled = false;
        if (action == Action.Clone && !(previewV2 && skillIndex == 17)) DrawShadows(u, beat);
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
        if (!previewV2 && skillIndex == 4 && (action == Action.Mark || action == Action.Charge))
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
        if (!previewV2 && action == Action.Rain)
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
            if (shake && (!PolishedBranch || skillIndex is not (7 or 9 or 17 or 22)))
                previewCamera.transform.position += AimVector * (.055f * Mathf.Sin(u * 25) * (1 - u));
            Vector3 at = action == Action.Land
                ? customSlam ? position + LandOffset(westCore) : position - Vector3.up * .62f
                : dummy.transform.position;
            at = skillIndex == 12 && action == Action.Land ? LockedLanding(take, position, at) :
                AnchorLocation(position, action == Action.Land ? anchors.landing : anchors.contact, at);
            if (!PolishedBranch) DrawImpactSignature(at, action, u);
        }
        if (lastCue != beat)
        {
            lastCue = beat;
            if (!scrubbing && hitStop && (!PolishedBranch || skillIndex is not (7 or 9 or 17 or 22)))
                hold = ImpactPause(action, beat);
            if (!PolishedBranch || !(skillIndex is 3 or 7 or 9 or 10 or 17 or 21 or 22 or 23 && action is Action.Hit or Action.Burst))
                PlaySkillCue(action);
        }
        if (PolishedBranch) return;
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

        if (previewV2 && skillIndex == 4 && action is Action.Mark or Action.Burst)
        {
            PreviewTarget marked = Array.Find(markedTargets, target => target != null);
            if (marked == null) return;
            effect.transform.SetPositionAndRotation(marked.Ground + DummyLift, Quaternion.identity);
            effect.transform.localScale = Vector3.one * .57f;
            effect.SetActive(true);
            effectClip.SampleAnimation(effect, Mathf.Min(.99f, u) * effectClip.length);
            return;
        }
        Vector3 point = weapon;
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
            point = previewV2 && skillIndex is 9 or 23
                ? ProjectilePosition(action, u, weapon, origin) : point + AimVector * (u * 1.8f);
        }
        else if (action == Action.Recall)
        {
            if (skillIndex == 23 && aim == 2 && HasContinuousSkill(skillIndex))
                point = position + WorldSpriteOffset(new Vector2(188, 171));
            point = previewV2 ? ProjectilePosition(action, u, weapon, origin) : point + AimVector * (1 - u) * 1.8f;
            rotation += 180;
        }
        else if (action == Action.Chase)
        {
            Vector3 path = dummy.transform.position - point;
            point += path * u + side * Mathf.Sin(u * Mathf.PI) * .38f;
            Vector3 tangent = path + side * Mathf.Cos(u * Mathf.PI) * (Mathf.PI * .38f);
            if (previewV2)
            {
                point = ProjectilePosition(action, u, weapon, origin);
                tangent = u < .99f ? ProjectilePosition(action, u + .01f, weapon, origin) - point
                    : point - ProjectilePosition(action, u - .01f, weapon, origin);
            }
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
        if (action == Action.Land)
            point = skillIndex == 12 ? LockedLanding(take, position, point)
                : previewV2 && skillIndex is 6 or 19
                    ? AnchorLocation(position, anchors.landing, position + TrainingFloor + AimVector * 1.1f)
                    : AnchorLocation(position, anchors.landing, point);

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
            float spacing = previewV2 && skillIndex == 17 ? 1.02f : .36f;
            float lane = i == 0 ? spacing : -spacing;
            Vector3 offset = perpendicular * lane;
            float sweep = skillIndex == 14 ? Mathf.SmoothStep(0, 1, u) * 1.75f - .93f : u - .72f;
            copy.transform.position = previewV2 && skillIndex == 17
                ? ActorPosition + travel * (beat == 1 ? .30f : Mathf.Lerp(.30f, 5.35f, u)) + offset
                : dummy.transform.position + travel * sweep + offset;
            float swing = previewV2 && skillIndex == 17 ? (beat == 1 ? 0 : u)
                : Mathf.Clamp01(u + (i == beat % 2 ? .1f : 0));
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
            if (previewV2 && skillIndex == 17)
            {
                copyRenderer.color = new Color(.79f, .62f, 1, beat == 1 ? u * .82f : Mathf.Clamp01(3f * (1 - u)) * .82f);
                if (beat == 3 && u > .12f)
                {
                    DrawCloneEcho(copyRenderer, travel * .31f, .26f * (1 - u));
                    DrawCloneEcho(copyRenderer, travel * .64f, .13f * (1 - u));
                }
            }
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

    #endregion

    #region Directional poses and effects

    [Serializable]
    private sealed class DirectionalTake
    {
        public int form;
        public int skill;
        public int direction;
        public Sprite[] frames;
        public TextAsset anchors;
        [NonSerialized] public FrameAnchors[] measured;
    }

    [Serializable]
    private sealed class AnchorFile
    {
        public FrameAnchors[] frames;
    }

    [Serializable]
    private struct FrameAnchors
    {
        public Vector2 hilt;
        public Vector2 tip;
        public Vector2 foot;
        public Vector2 landing;
        public Vector2 contact;
        public Vector2 aura;
    }

    [Serializable] private sealed class VisualAnchorSet { public VisualAnchor[] poses; }
    [Serializable] private struct VisualAnchor { public int form; public string sprite; public Vector2 tip; public Vector2 landing; }
    private readonly Dictionary<(int, string), VisualAnchor> visualAnchors = new();
    private Sprite[] cyclone;

    private void LoadVisualAnchors()
    {
        cyclone = Resources.LoadAll<Sprite>("HexcoreDaoSpin");
        Array.Sort(cyclone, (left, right) => string.CompareOrdinal(left.name, right.name));
        var asset = Resources.Load<TextAsset>("HexcoreVisualAnchors");
        if (asset == null) return;
        foreach (var pose in JsonUtility.FromJson<VisualAnchorSet>(asset.text).poses)
            visualAnchors.Add((pose.form, pose.sprite), pose);
    }

    [SerializeField] private DirectionalTake[] directionalTakes;
    private DirectionalTake sampledTake;
    private int sampledFrame;

    public bool HasDirectionalTake(int skill, int direction) => FindTake(formIndex, skill, direction) != null;
    public int MeasuredFrameCount(int skill, int direction)
    {
        var take = FindTake(formIndex, skill, direction);
        if (take == null) return 0;
        var native = take.anchors == null ? null : GetAnchors(take);
        int count = 0;
        for (int i = 0; i < take.frames.Length; i++)
        {
            var pose = native == null ? default : native[i];
            if (visualAnchors.ContainsKey((formIndex, take.frames[i].name)) || pose.hilt != Vector2.zero ||
                pose.tip != Vector2.zero || pose.foot != Vector2.zero || pose.landing != Vector2.zero ||
                pose.contact != Vector2.zero || pose.aura != Vector2.zero) count++;
        }
        return count;
    }

    public bool AudioActive => audioSource.isPlaying;
    public bool PlayedSkillAudio => playedCues != 0;
    public bool CameraMoving => (previewCamera.transform.position - cameraRest).sqrMagnitude > .000001f;
    public bool CopiesActive => shadows != null && Array.Exists(shadows, copy => copy.gameObject.activeSelf);
    public bool CopiesUseAuthoredStrikes => shadows != null &&
        Array.TrueForAll(shadows, copy => copy.gameObject.activeSelf && !copy.enabled);

    public bool ActorFitsVertically => !actor.enabled || actor.color.a < .05f ||
        previewCamera.WorldToViewportPoint(actor.bounds.min).y >= 0 &&
        previewCamera.WorldToViewportPoint(actor.bounds.max).y <= 1;

    private DirectionalTake FindTake(int form, int skill, int direction)
    {
        if (directionalTakes == null) return null;
        foreach (var take in directionalTakes)
            if (take.form == form && take.skill == skill && take.direction == direction) return take;
        return null;
    }

    private void ShowDirectionalPose(DirectionalTake take, Beat action, int beat, float amount)
    {
        int last = take.frames.Length - 1;
        int frame = Mathf.FloorToInt((beat + amount) * take.frames.Length / Chosen.Beats.Length);
        switch (skillIndex)
        {
            case 1:
                frame = action switch
                {
                    Beat.Prepare => amount < .4f ? 0 : 1,
                    Beat.Swing => 2 + Mathf.Min(7, Mathf.FloorToInt(amount * 8)),
                    Beat.Hit => 10,
                    _ => last
                };
                break;
            case 2:
                frame = action switch
                {
                    Beat.Prepare => amount < .5f ? 0 : 1,
                    Beat.Guard => amount < .5f ? 2 : 3,
                    Beat.Swing => amount < .5f ? 4 : 5,
                    Beat.Hit => 6,
                    _ => last
                };
                break;
            case 12:
                frame = action switch
                {
                    Beat.Prepare => amount < .5f ? 0 : 1,
                    Beat.Rise => amount < .5f ? 2 : 3,
                    Beat.Dive or Beat.Swing => 4,
                    Beat.Land => amount < .5f ? 5 : 6,
                    _ => last
                };
                break;
            case 13:
                frame = CoreComboPose(beat, amount);
                break;
            case 14:
                frame = beat switch
                {
                    0 => amount < .5f ? 0 : 1, 1 or 2 or 3 => 2, 4 => 3,
                    5 => amount < .5f ? 4 : 5, 6 => 6, _ => last
                };
                break;
        }
        ShowTakeFrame(take, frame, action);
    }

    private void ShowTakeFrame(DirectionalTake take, int frame, Action action)
    {
        sampledTake = take;
        sampledFrame = Mathf.Clamp(frame, 0, take.frames.Length - 1);
        SpritePose(take.frames[sampledFrame], action.ToString());
    }

    private FrameAnchors MeasuredAnchors()
    {
        if (sampledTake == null) return default;
        var pose = sampledTake.anchors == null ? default : GetAnchors(sampledTake)[sampledFrame];
        if (visualAnchors.TryGetValue((formIndex, sampledTake.frames[sampledFrame].name), out var revised))
        {
            if (revised.tip != Vector2.zero) pose.tip = revised.tip;
            if (revised.landing != Vector2.zero) pose.landing = revised.landing;
        }
        return pose;
    }

    private static FrameAnchors[] GetAnchors(DirectionalTake take)
    {
        return take.measured ??= JsonUtility.FromJson<AnchorFile>(take.anchors.text).frames;
    }

    private Vector3 StageContactPoint(Vector3 start, Vector3 defaultPoint)
    {
        var take = FindTake(formIndex, skillIndex, aim);
        if (take?.anchors == null) return defaultPoint;
        var entries = GetAnchors(take);
        for (int i = 0; i < entries.Length; i++)
        {
            Vector2 point = entries[i].contact;
            if (point == Vector2.zero) continue;
            Sprite reference = take.frames[i];
            return start + new Vector3(point.x - reference.pivot.x,
                reference.rect.height - point.y - reference.pivot.y) / reference.pixelsPerUnit;
        }
        return defaultPoint;
    }

    private Vector3 AnchorLocation(Vector3 root, Vector2 measured, Vector3 fallback)
    {
        return measured.x > 0 && measured.y > 0 ? root + WorldSpriteOffset(measured) : fallback;
    }

    private Vector3 LockedLanding(DirectionalTake take, Vector3 root, Vector3 fallback)
    {
        if (take == null) return fallback;
        const int contactFrame = 5;
        var sprite = take.frames[contactFrame];
        Vector2 point = take.anchors == null ? Vector2.zero : GetAnchors(take)[contactFrame].landing;
        if (visualAnchors.TryGetValue((formIndex, sprite.name), out var revised) && revised.landing != Vector2.zero)
            point = revised.landing;
        return point == Vector2.zero ? fallback : root + new Vector3(point.x - sprite.pivot.x,
            sprite.rect.height - point.y - sprite.pivot.y) / sprite.pixelsPerUnit;
    }

    public static bool ValidateAnchors(TextAsset data, Sprite[] frames)
    {
        var file = JsonUtility.FromJson<AnchorFile>(data.text);
        if (file?.frames == null || file.frames.Length != frames.Length) return false;
        for (int i = 0; i < frames.Length; i++)
        {
            FrameAnchors anchor = file.frames[i];
            Rect rect = frames[i].rect;
            foreach (Vector2 p in new[] { anchor.hilt, anchor.tip, anchor.foot, anchor.landing, anchor.contact, anchor.aura })
                if (p != Vector2.zero && (p.x <= 0 || p.y <= 0 || p.x >= rect.width || p.y >= rect.height)) return false;
        }
        return true;
    }

    private void DrawLeap(float time, Vector3 groundOrigin)
    {
        float apex = Chosen.Starts[2];
        float rise = Mathf.InverseLerp(Chosen.Starts[1], apex, time);
        float fall = Mathf.InverseLerp(apex, Chosen.Starts[4], time);
        float height = rise < 1 ? (1 - (1 - rise) * (1 - rise)) * .96f : (1 - fall * fall) * .96f;
        swordsman.transform.position += Vector3.up * height;
        if (height <= .02f) return;
        var ground = groundOrigin - Vector3.up * .65f;
        dust.startColor = dust.endColor = new Color(.015f, .022f, .018f, .65f - height * .4f);
        Draw(dust, .085f - height * .045f, ground - Vector3.right * .31f, ground, ground + Vector3.right * .31f);
        Vector3 landing = ground + AimVector * (.72f - Vector3.Dot(Travel(time), AimVector));
        var x = Vector3.right * .13f;
        var y = Vector3.up * .07f;
        swordTrail.startColor = swordTrail.endColor = new Color(.93f, .72f, .32f, .64f);
        Draw(swordTrail, .018f, landing - x, landing + y, landing + x, landing - y, landing - x);
        if (time >= apex)
        {
            var blade = swordsman.transform.position + AimVector * .18f;
            Draw(impact, .044f, blade + Vector3.up * .34f - AimVector * .19f, blade, blade - Vector3.up * .15f);
        }
    }

    private static int CoreComboPose(int beat, float amount) => beat switch
    {
        0 or 1 => 0,
        2 => amount < .27f ? 0 : 1,
        3 => 1,
        4 => amount < .44f ? 2 : 3,
        5 => 3,
        6 => 4,
        7 => amount < .48f ? 5 : 6,
        8 or 9 => 6,
        _ => 7
    };

    private static int CoreRelease(int beat) => beat == 3 ? 0 : beat == 5 ? 1 : 2;

    private float ImpactPause(Beat action, int beat) => action switch
    {
        Beat.Land => skillIndex == 12 ? .072f : .038f,
        Beat.Burst => skillIndex == 13 || skillIndex == 19 ? .057f : 0,
        Beat.Hit => skillIndex == 14 ? .061f : skillIndex == 15 && beat < 9 ? 0 :
            skillIndex == 3 || skillIndex == 9 || skillIndex == 10 || skillIndex == 18 ? 0 : .022f,
        _ => 0
    };

    private void DrawCoreWave(Vector3 origin, float u, int release)
    {
        var forward = AimVector;
        var side = Vector3.Cross(Vector3.forward, forward);
        var center = origin + forward * (u * (1.45f + release * .45f));
        float span = (.34f + release * .17f) * (1 + u * .35f);
        float taper = Mathf.Lerp(1, .37f, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.51f, 1, u)));
        swordTrail.startColor = new Color(.72f, .96f, 1, .94f * taper);
        swordTrail.endColor = new Color(.31f, .77f, 1, .72f * taper);
        if (release == 0)
        {
            Draw(swordTrail, .074f * taper, center - side * span, center + forward * .17f,
                center + side * span);
        }
        else if (release == 1)
        {
            Draw(swordTrail, .106f * taper, center - side * span - forward * .10f,
                center - side * span * .60f + forward * .17f, center + forward * .30f,
                center + side * span * .60f + forward * .17f, center + side * span - forward * .10f);
        }
        else
        {
            Draw(swordTrail, .148f * taper, center - side * span - forward * .22f,
                center - side * span * .45f + forward * .12f, center + forward * .48f,
                center + side * span * .45f + forward * .12f, center + side * span - forward * .22f);
            dust.startColor = new Color(.20f, .54f, 1, .31f * taper);
            dust.endColor = new Color(.45f, .85f, 1, .76f * taper);
            Draw(dust, .064f * taper, center + side * span * .64f - forward * .35f,
                center + forward * .31f, center - side * span * .64f - forward * .35f);
        }
    }

    private void DrawCyclone(Vector3 origin, float u, int beat)
    {
        var spiral = new Vector3[22];
        for (int i = 0; i < spiral.Length; i++)
        {
            float t = i / (spiral.Length - 1f);
            float angle = t * (Mathf.PI * 1.65f) + u * 6 + beat;
            float radius = (.33f + t * .54f) * (1 + u * .22f);
            spiral[i] = origin + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * .69f) * radius +
                Vector3.up * (1 - t) * .23f;
        }
        swordTrail.startColor = new Color(.86f, .53f, .28f, .37f);
        swordTrail.endColor = new Color(1, .82f, .48f, .94f);
        Draw(swordTrail, .092f, spiral);
    }

    private void DrawHeavyCleave(Vector3 origin, float u)
    {
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        var arc = new Vector3[9];
        for (int i = 0; i < arc.Length; i++)
        {
            float angle = Mathf.Lerp(-1.13f, 1.13f, i / (arc.Length - 1f)) - .32f + u * .64f;
            float radius = previewV2 ? Mathf.Lerp(.65f, 3.60f, Mathf.SmoothStep(0, 1, u)) : .56f + u * .45f;
            arc[i] = origin + (Mathf.Cos(angle) * AimVector + Mathf.Sin(angle) * side) * radius;
        }
        swordTrail.startColor = new Color(1, .71f, .39f, .38f);
        swordTrail.endColor = new Color(1, .89f, .67f, 1 - u * .75f);
        Draw(swordTrail, .082f * (1 - u * .6f), arc);
    }

    private void DrawVanishWisp(Vector3 origin, float opacity)
    {
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        origin -= Vector3.up * .51f;
        float span = 1 - opacity * .45f;
        swordTrail.startColor = new Color(.46f, .57f, .84f, 0);
        swordTrail.endColor = new Color(.55f, .68f, .93f, .64f * (1 - opacity));
        Draw(swordTrail, .035f, origin - side * .52f * span, origin - AimVector * .19f, origin,
            origin + AimVector * .24f, origin + side * .50f * span);
    }

    private void DrawImpactSignature(Vector3 point, Beat action, float u)
    {
        float fade = 1 - u;
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        if (skillIndex == 12 && action == Beat.Land)
        {
            var ring = new Vector3[25];
            float radius = Mathf.Lerp(.22f, 1.29f, u);
            for (int i = 0; i < ring.Length; i++)
            {
                float angle = i * Mathf.PI / 12;
                ring[i] = point + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * .60f) * radius;
            }
            swordTrail.startColor = swordTrail.endColor = new Color(1, .77f, .35f, fade * .86f);
            Draw(swordTrail, .078f * fade, ring);
            Draw(impact, .052f * fade, point - side * .50f, point - AimVector * .18f - side * .24f,
                point, point + AimVector * .27f + side * .11f, point + AimVector * .48f + side * .36f);
            float rise = Mathf.Sin(Mathf.PI * u) * .38f;
            Draw(dust, .047f * fade, point + Vector3.up * rise - side * .54f,
                point + Vector3.up * rise * .65f - side * .41f, point + Vector3.up * rise * 1.5f,
                point + Vector3.up * rise * .79f + side * .24f, point + Vector3.up * rise + side * .50f);
        }
        else if (skillIndex == 10 && action == Beat.Burst)
        {
            Draw(impact, .052f * fade, point - AimVector * .33f - side * .47f, point,
                point + AimVector * .33f + side * .47f);
            Draw(swordTrail, .05f * fade, point - AimVector * .33f + side * .47f, point,
                point + AimVector * .33f - side * .47f);
        }
        else if (action == Beat.Land)
        {
            Draw(impact, .06f * fade, point - side * .37f, point, point + AimVector * .69f,
                point + AimVector * .87f - side * .17f);
        }
        else
        {
            Draw(impact, .032f * fade, point + new Vector3(-.11f, .15f), point, point + new Vector3(.17f, .09f));
        }
    }

    #endregion

    #region Audio cues

    private enum Cue { Foot, Dash, Swing, HeavySwing, Impact, Parry, Charge, Wave, Vanish, Return, Slam, Burst, Finisher }

    [SerializeField] private AudioClip[] cues;
    private AudioSource audioSource;
    private int lastDashFrame = -1;
    private int lastBasicFrame = -1;
    private bool parryCuePlayed;
    private int playedCues;
    private const int DashCueMask = (1 << (int)Cue.Foot) | (1 << (int)Cue.Dash) |
        (1 << (int)Cue.Swing) | (1 << (int)Cue.Impact);

    public static int AudioCueCount => System.Enum.GetValues(typeof(Cue)).Length;
    public static string AudioCueName(int index) => ((Cue)index).ToString();
    public bool PlayedDashCues => (playedCues & DashCueMask) == DashCueMask;

    private void Play(Cue cue)
    {
        if (scrubbing || paused || !hear) return;
        playedCues |= 1 << (int)cue;
        audioSource.PlayOneShot(cues[(int)cue], audioSource.isPlaying ? .7f : 1f);
    }

    private void PlaySkillCue(CueAction action)
    {
        switch (action)
        {
            case CueAction.Advance: Play(Cue.Dash); break;
            case CueAction.Rise: Play(Cue.Foot); break;
            case CueAction.Vanish: Play(Cue.Vanish); break;
            case CueAction.Return: Play(Cue.Return); break;
            case CueAction.Swing: Play(formIndex == 2 ? Cue.HeavySwing : Cue.Swing); break;
            case CueAction.Guard:
                if (skillIndex != 2) Play(Cue.Parry);
                break;
            case CueAction.Charge or CueAction.Aura: Play(Cue.Charge); break;
            case CueAction.Wave or CueAction.Chase or CueAction.Rain: Play(Cue.Wave); break;
            case CueAction.Hit: Play(skillIndex == 14 || skillIndex == 16 ? Cue.Finisher : Cue.Impact); break;
            case CueAction.Land: Play(Cue.Slam); break;
            case CueAction.Burst: Play(Cue.Burst); break;
            case CueAction.Catch: Play(Cue.Parry); break;
        }
    }

    private void PlayDashFrame(int frame)
    {
        if (frame == lastDashFrame) return;
        lastDashFrame = frame;
        switch (frame)
        {
            case 3: Play(Cue.Foot); break;
            case 4: Play(Cue.Dash); break;
            case 6: Play(Cue.Swing); break;
            case 7: Play(Cue.Impact); break;
            case 9: Play(Cue.Foot); break;
        }
    }

    private void PlayBasicFrame(int frame, int count = 12)
    {
        if (frame == lastBasicFrame) return;
        lastBasicFrame = frame;
        if (formIndex == 3 && StyleReady) return; // The two wave releases own their cues.
        if (count == 8)
        {
            if (frame == 2 || frame == (formIndex == 1 ? 4 : 5) || formIndex == 1 && frame == 6)
                Play(formIndex == 2 ? Cue.HeavySwing : frame == 5 && formIndex == 3 ? Cue.Wave : Cue.Swing);
            return;
        }
        if (formIndex == 1 && (frame == 2 || frame == 6 || frame == 10)) Play(Cue.Swing);
        if ((formIndex == 0 || formIndex == 2) && (frame == 3 || frame == (formIndex == 0 ? 7 : 9)))
            Play(formIndex == 2 ? Cue.HeavySwing : Cue.Swing);
        if (formIndex == 3 && frame == 3) Play(Cue.Swing);
        if (formIndex == 3 && frame == 9) Play(Cue.Wave);
    }

    #endregion

    #region V2 preview targets, passives and hit areas

    // Stable indices below refer to the source catalog and serialized scene arrays.
    private static readonly int[][] V2Slots =
    {
        new[] { 0, 1, 2 }, new[] { 3, 4, 15, 6, 7, 18, 9, 10, 21 },
        new[] { 12, 13, 14 }, new[] { 5, 16, 17, 8, 19, 20, 11, 22, 23 }
    };
    private static readonly string[] V2Descriptions =
    {
        "Mỗi 5 đòn thường trúng phát một đường kiếm.", "Mỗi 4 đòn thường trúng phát vòng kiếm 360°.",
        "Khi bị đánh, giảm đòn nhận và phản kiếm về phía nguồn.", "Lướt xuyên cụm địch, quay lại rồi chém thuận và nghịch.",
        "Đặt tối đa 5 ấn, sau đó nổ quanh từng mục tiêu.", "Ẩn trong khói, tái định vị rồi chém phục kích.",
        "Bổ tại chỗ và nổ tròn, làm choáng bia ngắn.", "Sóng đao mở dần 80 độ, tầm 5 u; chạm bia mới đẩy lùi.", "Gom bia thường về tâm rồi quét một nhát.",
        "Sóng bán nguyệt xuyên địch và đẩy lùi bia thường.", "Hai đường chém giao nhau, nổ tại điểm giao.", "Khóa một mục tiêu và giáng đại kiếm từ trên xuống.",
        "Bật nhảy, giáng xuống và tạo xung kích.", "Ba nhát kiếm khí tăng dần.", "Biến mất, bóng ảnh liên trảm rồi tái xuất.",
        "Ba lần lao chém theo các đường khác nhau.", "Xuyên kích, mạnh hơn trên bia còn ít máu.",
        "Hai bóng chém hai bên cùng lúc bản thể phóng dao xuyên ở giữa.", "Hai vòng xoay liên tục quét quanh nhân vật.",
        "Vận lực bổ dọc, nứt thành hành lang dài.", "Cường hóa ngắn để đòn thường phát xung kích.",
        "Mười lăm kiếm phủ vùng ngắm theo ba đợt.", "Phi kiếm chạm tối đa năm bia khác nhau, gồm bia đầu và bốn lần nảy.", "Sóng kiếm bay đi và quay về, mỗi lượt trúng một lần."
    };

    public static int PreviewV2Source(int group, int branch, int slot) =>
        V2Slots[group][(group is 1 or 3 ? (branch - 1) * 3 : 0) + slot];

    public static bool PreviewV2Compatible(int skill, int form) =>
        skill >= 0 && skill < SkillCount && form >= 0 && form < Names.Length &&
        (skill < 3 || Compatible(skill, form));

#if UNITY_EDITOR
    public bool SelectV2Review(int form, int skill, int heading, bool usePolish = false)
    {
        if (!PreviewV2Compatible(skill, form) || heading < 0 || heading >= 8) return false;
        SetFreeMode(false);
        previewV2 = true;
        polishedPreview = usePolish;
        if (usePolish && !StyleReady) return false;
        SelectForm(form);
        branch = form == 0 ? 1 : form;
        aim = locomotionDirection = heading;
        skillIndex = skill;
        skillGroup = Array.FindIndex(V2Slots, slots => Array.IndexOf(slots, skill) >= 0);
        previewLoadout[skillGroup] = Array.IndexOf(V2Slots[skillGroup], skill) % 3;
        selection = skill < 3 ? Preview.Basic : Preview.Q1;
        if (!HasDirection(form, heading) || (skill < 3 ? !HasCombo(heading) : DirectionCoverage(skill, heading) == "MissingArt"))
            return false;
        targetCount = 9;
        halfSpeed = false;
        ResetPractice();
        ConfigureEffect();
        Replay();
        paused = true;
        return true;
    }

    public void SampleV2Review(float time, float elapsed)
    {
        practiceTime += elapsed;
        clock = time;
        UpdatePreviewViewport();
        Render();
    }

    public void EnableV2BasicReview() => paused = true;

    public void ReviewReceiveHit()
    {
        bool previous = paused;
        paused = false;
        ReceivePracticeHit();
        paused = previous;
    }

    public int ReviewPracticeHits => trainingHits;
    public int ReviewPassiveProcs => passiveProcs;
    public int ReviewPassiveHits => passiveHits;
    public float ReviewPracticeHp => playerDummyHp;
    public bool ReviewEmpowered => practiceTime < empowerUntil;
    public int ReviewChainHits => chainHits;
    public int ReviewRainBlades => rainBlades;
    public Vector3 ReviewDummyGround(int index) => trainingTargets[index].Ground;
    public float ReviewDummyHp(int index) => trainingTargets[index].Hp;

    public void PlaceReviewDummy(int index, float distance, float lateral)
    {
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        trainingTargets[index].Renderer.transform.position =
            FreeSpawn + TrainingFloor + DummyLift + AimVector * distance + side * lateral;
    }

    public void ConfigureReviewDummies(int count, bool resistPull)
    {
        bossFixture = resistPull;
        targetCount = count;
        PlaceTraining(FreeSpawn, AimVector);
    }
#endif

    private sealed class PreviewTarget
    {
        public SpriteRenderer Renderer;
        public float Hp, FlashUntil, StaggerUntil, DaggerUntil, PushUntil, PushProgress;
        public Vector3 PushOffset;
        public bool ResistsPull;
        public int HitMask, LastPassive;
        public Vector3 Ground => Renderer.transform.position - DummyLift;
    }

    private enum AreaShape { Circle, Lane, Fan }

    private readonly struct PreviewArea
    {
        public readonly Vector3 Origin, Forward;
        public readonly float Length, Width;
        public readonly AreaShape Shape;

        public PreviewArea(AreaShape shape, Vector3 origin, Vector3 forward, float length, float width)
        {
            Shape = shape;
            Origin = origin;
            Forward = forward;
            Length = length;
            Width = width;
        }

        public bool Contains(Vector3 point)
        {
            Vector2 offset = point - Origin;
            float along = Vector2.Dot(offset, Forward);
            return Shape switch
            {
                AreaShape.Circle => offset.sqrMagnitude <= Length * Length,
                AreaShape.Lane => along >= 0 && along <= Length &&
                    Mathf.Abs(Vector2.Dot(offset, new Vector2(-Forward.y, Forward.x))) <= Width,
                _ => offset.sqrMagnitude <= Length * Length && along >= offset.magnitude * Mathf.Cos(Width * Mathf.Deg2Rad)
            };
        }
    }

    private static PreviewArea Circle(Vector3 origin, float radius) =>
        new(AreaShape.Circle, origin, Vector3.right, radius, 0);
    private static PreviewArea Lane(Vector3 from, Vector3 to, float halfWidth) =>
        new(AreaShape.Lane, from, (to - from).normalized, Vector2.Distance(from, to), halfWidth);
    private static PreviewArea Fan(Vector3 from, Vector3 forward, float range, float halfAngle) =>
        new(AreaShape.Fan, from, forward, range, halfAngle);

    private static readonly Vector3 DummyLift = new(0, .25f, 0);
    private static readonly Vector3 TrainingFloor = new(0, -.63f, 0);
    private readonly PreviewTarget[] trainingTargets = new PreviewTarget[9];
    private readonly PreviewTarget[] markedTargets = new PreviewTarget[5];
    private readonly Vector3[] markBurstPoints = new Vector3[5];
    private readonly LineRenderer[] trainingLines = new LineRenderer[14];
    private readonly int[] previewLoadout = { 0, 0, 0, 0 };
    private bool previewV2 = true, showAreas, aimFollowsMovement = true, weakTargets;
    private bool trainingPlaced, basicCasting, queuedCast, queuedBasic;
    private int branch = 1, targetCount = 5, passiveHits, passiveProcs, basicResolved, firedBeats;
    private int trainingHits;
    private float practiceTime, parryReadyAt, empowerUntil, playerDummyHp = 100;
    private float lastBasicTime = -1, passiveStart = -10, assistStart = -10, lastAreaUntil;
    private float practiceAimDistance = 3.1f, lockedAimDistance = 3.1f;
    private Vector3 formationOrigin, formationForward, passiveForward, projectileOrigin, projectileDestination, projectileLaunchOffset;
    private bool projectileLocked;
    private PreviewArea passivePulse, assistPulse, lastArea;
    private PreviewTarget seekTarget;
    private int usedTrainingLines;
    private bool bossFixture;
    private int chainHits, ambushFacing = -1;
    private float chainLegDistance, daoWaveRadius;
    private Vector3 chainPosition, chainDirection, chainLegStart;
    private const int ChainTargetLimit = 5;
    private const float PullRadius = 3.8f, PullStop = .78f, PullSweep = 2.45f;
    private const float ReturnRange = 7.6f, ReturnWidth = 1.50f;
    private const float RainRadius = 3.25f, RainImpactRadius = 1.08f;

    private bool IsCore => skillIndex is >= 12 and <= 14;
    private bool IsBranchSkill => skillIndex >= 3 && !IsCore;
    private int SelectedPassive => previewLoadout[0];
    private static readonly Color AreaTint = new(.47f, .95f, .69f, .68f);
    private static readonly Color SealTint = new(.97f, .31f, .46f, .84f);

    private Vector3 AimPoint(Vector3 origin) => origin + TrainingFloor + AimVector * lockedAimDistance;
    private bool HasLongPreviewRange => previewV2 && skillIndex is 7 or 11 or 16 or 19 or 21 or 22 or 23;
    private Vector3 PracticeFramingOffset => previewV2 && IsBranchSkill
        ? AimVector * (skillIndex == 8 ? .4f : HasLongPreviewRange ? 3.25f : 1.50f) : Vector3.zero;
    private bool TrainingVisible => previewV2 && (!IsCore || basicCasting ||
        (!freeMode && selection == Preview.Basic) || (freeMode && !freeCasting));

    private void PlaceTraining(Vector3 origin, Vector3 forward)
    {
        formationOrigin = origin;
        formationForward = forward;
        trainingPlaced = true;
        int columns = targetCount <= 1 ? 1 : targetCount <= 5 ? 2 : 3;
        Vector3 lateral = Vector3.Cross(Vector3.forward, forward);
        for (int i = 0; i < trainingTargets.Length; i++)
        {
            if (i >= targetCount)
            {
                if (trainingTargets[i] != null) trainingTargets[i].Hp = 0;
                continue;
            }
            if (trainingTargets[i] == null)
            {
                var sprite = new GameObject("Bia mô phỏng " + (i + 1)).AddComponent<SpriteRenderer>();
                sprite.transform.SetParent(transform, false);
                sprite.sprite = dummy.sprite;
                sprite.sharedMaterial = dummy.sharedMaterial;
                sprite.transform.localScale = dummy.transform.localScale;
                sprite.sortingOrder = dummy.sortingOrder;
                trainingTargets[i] = new PreviewTarget { Renderer = sprite };
            }
            var target = trainingTargets[i];
            int row = i % columns;
            int side = i / columns;
            float angleOffset = side == 0 ? 0 : ((side & 1) == 1 ? 1 : -1) * ((side + 1) / 2) * .68f;
            float distance = columns == 1 ? practiceAimDistance :
                Mathf.Lerp(1.12f, practiceAimDistance + .42f, row / (float)(columns - 1));
            target.Renderer.transform.position = origin + TrainingFloor + DummyLift + forward * distance + lateral * angleOffset;
            target.Hp = weakTargets ? 12 : i == 1 ? 30 : 100;
            target.ResistsPull = bossFixture && i == 0;
            target.FlashUntil = target.StaggerUntil = target.DaggerUntil = 0;
            target.HitMask = 0;
            target.LastPassive = passiveProcs;
            target.PushUntil = 0;
        }
    }

    private void ResetPractice()
    {
        passiveHits = passiveProcs = trainingHits = 0;
        passiveStart = assistStart = -10;
        parryReadyAt = empowerUntil = 0;
        playerDummyHp = 100;
        queuedCast = queuedBasic = basicCasting = false;
        trainingPlaced = false;
        ResetTrainingCast();
    }

    private void ResetTrainingCast()
    {
        styleCues.Clear();
        Array.Clear(basicWaves, 0, basicWaves.Length);
        hasCrescentPrevious = false;
        lockedAimDistance = practiceAimDistance;
        lastBasicTime = -1;
        basicResolved = 0;
        firedBeats = 0;
        lastAreaUntil = 0;
        seekTarget = null;
        chainHits = 0;
        ambushFacing = -1;
        daoWaveRadius = 0;
        rainBlades = 0;
        rev2Beat = -1;
        rev2Progress = 0;
        projectileLocked = false;
        Array.Clear(markedTargets, 0, markedTargets.Length);
        foreach (var target in trainingTargets)
            if (target != null) { target.HitMask = 0; target.DaggerUntil = 0; }
    }

    private void BeginBasic()
    {
        if (freeCasting || basicCasting || !HasCombo(aim)) return;
        basicCasting = true;
        freeVelocity = Vector3.zero;
        freeTurn = 0;
        RestartPlayback();
    }

    private static int BasicContactFrame(int form, int frames, int strike) => frames == 8
        ? strike == 0 ? 2 : form == 1 ? strike == 1 ? 4 : 6 : 5
        : strike == 0 ? form == 1 ? 2 : 3 : form == 1 ? strike == 1 ? 6 : 10 : form == 0 ? 7 : 9;

    private void SampleBasicHit(float time, int frame, int count)
    {
        if (!TrainingVisible || scrubbing) return;
        if (time < lastBasicTime) basicResolved = 0;
        lastBasicTime = time;
        int strikes = formIndex == 1 ? 3 : 2;
        while (basicResolved < strikes && frame >= BasicContactFrame(formIndex, count, basicResolved))
        {
            int strike = basicResolved++;
            Vector3 root = ActorPosition + TrainingFloor;
            if (StyleReady && formIndex == 3)
            {
                float release = BasicContactFrame(formIndex, count, strike) * AttackDuration / count;
                StartStyleBasicWave(strike, time - release);
                continue;
            }
            PreviewArea region = formIndex == 3 && strike == 1
                ? Lane(root, root + AimVector * 2.35f, .60f) : Fan(root, AimVector, 1.82f, 61);
            if (StyleReady)
            {
                region = StyleBasicArea(root, strike, out string cut);
                var blade = MeasuredAnchors();
                Vector3 tip = AnchorLocation(ActorPosition, blade.tip, ActorPosition + AimVector * .65f);
                Vector3 anchor = Vector3.Lerp(root, tip, .45f);
                Vector3 direction = formIndex == 2 && strike == 1
                    ? Quaternion.Euler(0, 0, -24) * region.Forward : region.Forward;
                EmitStyle(cut, anchor, direction, formIndex == 2 ? .27f : .20f,
                    region.Length / .405f, mirror: strike == 1);
            }
            bool connected = HitTargets(region, 7, styleImpact: StyleReady) > 0;
            if (connected)
            {
                var tip = AnchorLocation(ActorPosition, MeasuredAnchors().tip, ActorPosition + AimVector * .55f);
                CreditBasicHit(tip, AimVector);
                if (StyleReady && formIndex == 2)
                {
                    if (hitStop) hold = .033f;
                    Play(Cue.Impact);
                }
            }
            if (connected && practiceTime < empowerUntil)
            {
                assistStart = practiceTime;
                assistPulse = Lane(root, root + AimVector * 5.3f, 1.0f);
                assistPrevious = assistPulse.Origin;
                foreach (var target in trainingTargets)
                    if (target != null) target.HitMask &= ~(1 << 29);
                Play(Cue.Wave);
            }
        }
    }

    private void ReceivePracticeHit()
    {
        if (!previewV2 || paused) return;
        bool counter = SelectedPassive == 2 && practiceTime >= parryReadyAt;
        playerDummyHp = Mathf.Max(0, playerDummyHp - (counter ? 6 : 15));
        if (!counter) return;
        parryReadyAt = practiceTime + 2;
        var attacker = NearestTarget(ActorPosition, AimVector, false);
        var direction = attacker == null ? AimVector : attacker.Ground - (ActorPosition + TrainingFloor);
        direction.z = 0;
        TriggerPassive(ActorPosition + direction.normalized * .42f, direction);
    }

    private void TriggerPassive(Vector3 origin, Vector3 forward)
    {
        Vector3 ground = ActorPosition + TrainingFloor;
        forward = forward.sqrMagnitude > .001f ? forward.normalized : AimVector;
        passiveStart = practiceTime;
        passiveForward = forward;
        orbitPrevious = 0;
        passivePulse = SelectedPassive == 1 ? Circle(ground, StyleReady ? OrbitRange : 1.30f) :
            Lane(ground, ground + forward * (SelectedPassive == 0 ? 3.5f : 3.2f), SelectedPassive == 0 ? .58f : .40f);
        passiveProcs++;
        if (SelectedPassive == 2) EmitStyle("hit_spark", origin, forward, .19f, 1.50f);
        if (SelectedPassive == 1 && !StyleReady) HitTargets(passivePulse, 12);
        Play(SelectedPassive == 2 ? Cue.Parry : Cue.Wave);
    }

    private int HitTargets(PreviewArea area, float damage, int mask = 0, float stagger = 0,
        bool execute = false, bool styleImpact = false)
    {
        int count = 0;
        foreach (var target in trainingTargets)
        {
            if (target == null || target.Hp <= 0 || (target.HitMask & mask) != 0 || !area.Contains(target.Ground)) continue;
            target.Hp = Mathf.Max(0, target.Hp - damage * (execute && target.Hp <= 35 ? 1.6f : 1));
            if (styleImpact) EmitStyle("hit_spark", target.Ground, AimVector, .25f, 1.20f);
            target.HitMask |= mask;
            target.FlashUntil = practiceTime + .24f;
            target.StaggerUntil = practiceTime + stagger;
            count++;
        }
        trainingHits += count;
        lastArea = area;
        lastAreaUntil = practiceTime + .22f;
        return count;
    }

    private PreviewTarget NearestTarget(Vector3 point, Vector3 direction, bool forwardOnly, bool preferBoss = false)
    {
        PreviewTarget best = null;
        float range = forwardOnly ? 49f : float.PositiveInfinity;
        float bestDistance = range;
        foreach (var target in trainingTargets)
        {
            if (target == null || target.Hp <= 0) continue;
            Vector3 delta = target.Ground - point;
            float distance = delta.sqrMagnitude;
            if (distance > range || forwardOnly && Vector3.Dot(direction, delta) <= 0) continue;
            if (preferBoss && Vector3.Dot(direction, delta) < delta.magnitude * .57f) continue;
            bool stronger = preferBoss && target.ResistsPull && (best == null || !best.ResistsPull);
            if (!stronger && (distance >= bestDistance || preferBoss && best != null && best.ResistsPull && !target.ResistsPull))
                continue;
            bestDistance = distance;
            best = target;
        }
        return best;
    }

    private PreviewArea StagedArea(Vector3 origin, Vector3 ground, Vector3 landing, int beat)
    {
        Vector3 forward = AimVector;
        Vector3 start = origin + TrainingFloor;
        Vector3 goal = AimPoint(origin);
        return skillIndex switch
        {
            3 => Lane(start, start + forward * 3.85f, .93f),
            4 => Circle(goal, 1.7f),
            5 => Fan(ground, forward, 2.9f, 65),
            6 => Circle(landing, 1.85f),
            8 => Circle(ground, 2.65f),
            9 => Lane(ground, ground + forward * 4.1f, .86f),
            10 => Circle(goal, 1.52f),
            15 => AssassinSweepArea(start, beat),
            16 => Lane(start, start + forward * 7f, .64f),
            18 => Circle(ground, 2.20f),
            19 => Lane(ground, ground + forward * 6.3f, .88f),
            20 => Circle(ground, .80f),
            22 => Lane(ground, ground + forward * 6.2f, .58f),
            _ => Lane(ground, ground + forward * 6.1f, 1.06f)
        };
    }

    private PreviewArea AssassinSweepArea(Vector3 start, int beat)
    {
        int advance = beat < 4 ? 1 : beat < 7 ? 4 : 7;
        Vector3 from = start + AssassinSlashPath(Chosen.Starts[advance]);
        Vector3 to = start + AssassinSlashPath(Chosen.Starts[advance + 1]);
        return Lane(from, to + (to - from).normalized * .65f, .98f);
    }

    private void SampleBranchPreview(Action action, int beat, float u, Vector3 origin, Vector3 weapon, Vector3 landing, int facing)
    {
        Vector3 ground = ActorPosition + TrainingFloor;
        Vector3 goal = AimPoint(origin);
        PreviewArea region = StagedArea(origin, ground, landing, beat);
        int mask = 1 << beat;
        bool first = (firedBeats & mask) == 0 && !scrubbing;
        if (first) firedBeats |= mask;
        if (skillIndex == 3 && beat >= 2)
        {
            if (action == Action.Hit)
            {
                bool reverseCut = beat == 6;
                Vector3 forward = DirectionVector(facing);
                Vector3 tip = ground + forward * .72f;
                if (HasCombo(facing))
                    tip = ActorPosition + WorldSpriteOffset(AssassinCutTips[facing + (reverseCut ? 8 : 0)]);
                Vector3 cutOrigin = Vector3.Lerp(ground, ActorPosition, .52f);
                var cut = Fan(ground, forward, 1.82f, 52);
                DrawStyle("ambush_cut", u, Vector3.Lerp(cutOrigin, tip, .35f), forward, 3.25f, mirror: reverseCut);
                if (first && HitTargets(cut, 16, mask, styleImpact: true) > 0) Play(Cue.Impact);
                if (showAreas) TraceArea(cut, AreaTint, .027f, true);
            }
            return;
        }
        if (skillIndex == 7)
        {
            if (action == Action.Hit) SampleDaoBladeWave(weapon, u, mask);
            else if (action == Action.Recover && first && projectileLocked)
                SampleDaoBladeWave(weapon, 1, mask >> 1);
            return;
        }

        if (skillIndex is 8 or 10 or 11 or 17 or 21 or 22 or 23)
        {
            SampleRev2Skill(action, beat, u, origin, weapon, ground, goal, mask, first);
            return;
        }

        if (skillIndex == 4)
        {
            if (action == Action.Mark && first)
            {
                int count = 0;
                foreach (var target in trainingTargets)
                    if (target != null && target.Hp > 0 && region.Contains(target.Ground) && count < markedTargets.Length)
                        markedTargets[count++] = target;
            }
            int burstCount = 0;
            foreach (var target in markedTargets)
            {
                if (target == null || target.Hp <= 0) continue;
                if (StyleReady && action is Action.Mark or Action.Charge)
                {
                    float charge = action == Action.Mark ? .44f * u : .44f + .56f * u;
                    DrawStyle("blood_seal", charge, target.Renderer.transform.position, Vector3.right, Mathf.Lerp(.67f, .86f, charge));
                }
                else if (!StyleReady && action is Action.Mark or Action.Charge or Action.Burst)
                    TraceArea(Circle(target.Ground, action == Action.Burst ? 1.12f * Mathf.Lerp(.25f, 1, u) : .22f), SealTint, .04f, showEffects);
                if (action == Action.Burst && first) markBurstPoints[burstCount++] = target.Ground;
            }
            for (int i = 0; i < burstCount; i++)
            {
                var burst = Circle(markBurstPoints[i], 1.12f);
                EmitStyle("blood_burst", burst.Origin, Vector3.right, .42f, burst.Length / .41f, debugRadius: burst.Length);
                HitTargets(burst, 19, mask, styleImpact: StyleReady);
            }
            if (StyleReady && showAreas && action == Action.Mark) TraceArea(region, AreaTint, .024f, true);
            return;
        }

        if (StyleReady && skillIndex == 9)
        {
            if (action == Action.Wave) SampleStyleCrescent(mask, u, weapon);
            else if (action == Action.Hit && first && hasCrescentPrevious)
                SampleStyleCrescent(mask >> 1, 1, weapon);
            return;
        }
        if (StyleReady && skillIndex == 6)
        {
            if (action == Action.Land && first)
            {
                EmitStyle("ground_slam", region.Origin, Vector3.right, .44f, region.Length / .405f, debugRadius: region.Length);
                HitTargets(region, 16, mask, .45f, styleImpact: true);
            }
            return;
        }
        if (skillIndex == 16)
        {
            if (action == Action.Swing)
            {
                Vector3 from = origin + TrainingFloor;
                float before = rev2Beat == beat ? rev2Progress : 0;
                float previous = 6.5f * Mathf.SmoothStep(0, 1, before);
                float current = 6.5f * Mathf.SmoothStep(0, 1, u) + .50f;
                var step = Lane(from + AimVector * previous, from + AimVector * current, .64f);
                DrawStyle("execute_thrust", .19f + .70f * u, weapon, AimVector, 3.2f);
                if (!scrubbing && HitTargets(step, 26, mask, execute: true, styleImpact: true) > 0)
                    Play(Cue.Finisher);
                if (showAreas) TraceArea(step, AreaTint, .027f, true);
                if (!scrubbing) { rev2Beat = beat; rev2Progress = u; }
            }
            return;
        }

        bool pulse = skillIndex switch
        {
            5 or 15 or 18 => action == Action.Hit,
            6 or 19 => action == Action.Land,
            _ => false
        };
        if (skillIndex == 20 && action == Action.Aura && first)
            empowerUntil = practiceTime + 5;

        if (skillIndex == 18 && action is Action.Swing or Action.Hit)
            DrawStyle("dao_orbit", action == Action.Swing ? u * .68f : .68f + u * .32f,
                ground, Vector3.right, region.Length / .40f, order: 0);
        if (skillIndex == 5 && action is Action.Vanish or Action.Return)
            DrawStyle("shadow_gate", u, ground, Vector3.right, 2.7f, order: 0);
        if (skillIndex is 3 or 15 && action is Action.Advance or Action.Vanish)
            DrawStyle("shadow_gate", u, ground - AimVector * .35f, AimVector, 1.7f, order: 0);

        if (pulse && first)
        {
            HitTargets(region, skillIndex == 19 ? 26 : 16, mask, skillIndex == 6 ? .45f : 0,
                styleImpact: true);
            if (skillIndex is 5 or 15 or 19)
            {
                bool finish = skillIndex != 15 || beat == 9;
                string clip = skillIndex switch
                {
                    19 => "rift", 15 when finish => "night_finish",
                    _ => "basic_dagger"
                };
                Vector3 point = skillIndex == 19 ? region.Origin :
                    skillIndex == 15 ? Vector3.Lerp(region.Origin, region.Origin + region.Forward * region.Length, .52f) : weapon;
                EmitStyle(clip, point, region.Forward, finish ? .38f : .23f,
                    skillIndex is 15 or 19 ? region.Length / .82f : 2.5f);
            }
        }
        if (showAreas && (pulse || action is Action.Prepare or Action.Charge))
            TraceArea(region, AreaTint, .027f, true);
    }

    private void SampleRev2Skill(Action action, int beat, float u, Vector3 origin, Vector3 weapon,
        Vector3 ground, Vector3 goal, int mask, bool first)
    {
        Vector3 forward = AimVector;
        Vector3 start = origin + TrainingFloor;
        Vector3 side = new(-forward.y, forward.x);
        switch (skillIndex)
        {
            case 8:
                if (action is Action.Guard or Action.Aura)
                {
                    DrawStyle("ground_slam", action == Action.Guard ? u : .98f, start, Vector3.right, 2.1f, order: -1);
                    if (action == Action.Aura)
                    {
                        Vector3 center = start + forward * .55f;
                        var pull = Circle(center, PullRadius);
                        DrawStyle("gravity", u, center, Vector3.right, PullRadius * 2.5f, order: 0);
                        float stepTime = StepRev2(u, beat);
                        if (!scrubbing && stepTime > 0)
                        {
                            foreach (var target in trainingTargets)
                            {
                                if (target == null || target.Hp <= 0 || target.ResistsPull || !pull.Contains(target.Ground)) continue;
                                Vector3 inward = center - target.Ground;
                                float step = Mathf.Max(0, inward.magnitude - PullStop);
                                target.Renderer.transform.position += inward.normalized * Mathf.Min(step, stepTime * 7.6f);
                            }
                        }
                        if (showAreas) TraceArea(pull, AreaTint, .025f, true);
                    }
                }
                if (action == Action.Hit && first)
                {
                    Vector3 center = start + forward * .55f;
                    EmitStyle("dao_orbit", center, Vector3.right, .42f, PullSweep / .40f);
                    HitTargets(Circle(center, PullSweep), 29, mask, styleImpact: true);
                }
                break;
            case 10:
                Vector3 firstDiagonal = (forward + side * .78f).normalized;
                Vector3 secondDiagonal = (forward - side * .78f).normalized;
                if (action is Action.Wave or Action.Hit)
                {
                    bool second = action == Action.Hit;
                    Vector3 diagonal = second ? secondDiagonal : firstDiagonal;
                    Vector3 from = goal - diagonal * 1.70f;
                    Vector3 to = goal + diagonal * 1.70f;
                    DrawStyle("cross_blade", u, Vector3.Lerp(from, to, u), diagonal, 1.63f, mirror: second);
                    if (second) DrawStyle("cross_blade", .61f + .36f * u, goal, firstDiagonal, 3.4f);
                    SweepCrescent(from, to, u, beat, .48f, mask, 9, false);
                }
                if (action == Action.Swing && beat == 3 && first && rev2Beat == 2)
                    SweepCrescent(goal - firstDiagonal * 1.70f, goal + firstDiagonal * 1.70f, 1, 2, .48f, mask >> 1, 9, false);
                if (action == Action.Burst && first)
                {
                    if (rev2Beat == beat - 1)
                        SweepCrescent(goal - secondDiagonal * 1.70f, goal + secondDiagonal * 1.70f,
                            1, beat - 1, .48f, mask >> 1, 9, false);
                    EmitStyle("qi_burst", goal, Vector3.right, .30f, 2.3f);
                    HitTargets(Circle(goal, 1.00f), 15, mask, styleImpact: true);
                    Play(Cue.Impact);
                }
                break;
            case 11:
                if (action == Action.Mark && first) seekTarget = NearestTarget(start, forward, true, preferBoss: true);
                if (seekTarget == null || seekTarget.Hp <= 0) return;
                Vector3 pin = seekTarget.Ground;
                if (action is Action.Mark or Action.Aura)
                    DrawStyle("qi_seal", u, pin, Vector3.right, 1.5f, order: 0);
                if (action == Action.Aura)
                    DrawStyle("qi_sword", u * .75f, pin + Vector3.up * Mathf.Lerp(3.6f, 1.35f, u * u), Vector3.down, 3.3f);
                if (action == Action.Burst && beat == 4 && first)
                {
                    ApplyStyleContact(seekTarget, 45, Vector3.down);
                    EmitStyle("qi_burst", pin, Vector3.right, .33f, 3.2f);
                    if (showAreas) TraceArea(Circle(pin, .23f), AreaTint, .024f, true);
                }
                break;
            case 17:
                if (action == Action.Clone && beat == 1)
                {
                    DrawStyle("shadow_gate", u * .65f, origin + side, Vector3.right, 2.2f, order: 0);
                    DrawStyle("shadow_gate", u * .65f, origin - side, Vector3.right, 2.2f, order: 0);
                    DrawShadows(u, beat);
                }
                if (action == Action.Clone && beat == 3)
                {
                    DrawShadows(u, beat);
                    if (first)
                    {
                        projectileLaunchOffset = weapon - (start + forward * .30f + DummyLift);
                        Play(Cue.Wave);
                    }
                    for (int lane = -1; lane <= 1; lane += 2)
                    {
                        Vector3 from = start + side * lane + forward * .30f;
                        Vector3 to = from + forward * 5.05f;
                        DrawStyle("night_finish", u, Vector3.Lerp(from, to, u) + Vector3.up * .44f,
                            forward, 2.45f, mirror: lane == -1);
                        SweepCrescent(from, to, u, beat, .76f, mask, 18, true);
                    }
                    SampleMiddleDagger(start + forward * .30f, forward, u, beat, mask);
                }
                if (action == Action.Hit)
                {
                    for (int lane = -1; lane <= 1; lane += 2)
                    {
                        DrawStyle("shadow_gate", .62f + .37f * u, origin + forward * 5.35f + side * lane,
                            forward, 1.6f, order: 0);
                        if (first && rev2Beat == beat - 1)
                            SweepCrescent(start + side * lane + forward * .30f, start + side * lane + forward * 5.35f,
                                1, beat - 1, .76f, mask >> 1, 18, true);
                    }
                    if (first && rev2Beat == beat - 1)
                        SampleMiddleDagger(start + forward * .30f, forward, 1, beat - 1, mask >> 1);
                }
                break;
            case 21:
                if (action is Action.Swing or Action.Rain or Action.Hit)
                    DrawStyle("qi_seal", .35f + .45f * u, goal, Vector3.right, RainRadius * 2.5f, order: 0);
                if (action is Action.Rain or Action.Hit)
                {
                    int wave = beat - 3;
                    for (int blade = 0; blade < 5; blade++)
                    {
                        float angle = (blade * 72 + wave * 24) * Mathf.Deg2Rad;
                        float radius = blade == 0 && wave == 0 ? .65f : RainRadius * (.50f + .16f * ((blade + wave) % 2));
                        Vector3 landing = goal + (forward * Mathf.Cos(angle) + side * Mathf.Sin(angle)) * radius;
                        float fall = Mathf.Clamp01((u - blade * .035f) / .69f);
                        if (fall < 1)
                            DrawStyle("qi_sword", fall * .75f, landing + Vector3.up * Mathf.Lerp(2.55f, .51f, fall * fall), Vector3.down, 1.25f);
                        if (fall >= 1 && !scrubbing && (rainBlades & (1 << (wave * 5 + blade))) == 0)
                        {
                            rainBlades |= 1 << (wave * 5 + blade);
                            HitTargets(Circle(landing, RainImpactRadius), 14, mask, styleImpact: true);
                            EmitStyle("qi_burst", landing, Vector3.right, .25f, RainImpactRadius * 2.4f,
                                debugRadius: RainImpactRadius);
                        }
                    }
                    if (first) Play(Cue.Wave);
                }
                break;
            case 22:
                if (action == Action.Chase) AdvanceSeekingSword(u, beat, weapon, forward);
                break;
            case 23:
                if (action == Action.Wave)
                {
                    if (!projectileLocked) { projectileOrigin = weapon; projectileLocked = !scrubbing; }
                    Vector3 from = projectileOrigin;
                    Vector3 to = from + forward * ReturnRange;
                    projectileDestination = to;
                    SweepCrescent(from, to, u, beat, ReturnWidth, mask, 17, false);
                    DrawStyle("crescent", Mathf.Lerp(.22f, .72f, u), Vector3.Lerp(from, to, u), forward, ReturnWidth * 2 / .84f);
                }
                if (action == Action.Recall)
                {
                    if (first && projectileLocked && rev2Beat == beat - 1)
                        SweepCrescent(projectileOrigin, projectileDestination, 1, beat - 1, ReturnWidth, mask >> 1, 17, false);
                    Vector3 bend = side * (.72f * Mathf.Sin(Mathf.PI * u));
                    Vector3 current = Vector3.Lerp(projectileDestination, weapon, u) + bend;
                    Vector3 previous = rev2Beat == beat && !scrubbing ? returningPrevious : projectileDestination;
                    Vector3 velocity = (current - previous).normalized;
                    SweepCrescent(previous, current, 1, -1, ReturnWidth, mask, 21, false);
                    if (!scrubbing) returningPrevious = current;
                    DrawStyle("crescent", Mathf.Lerp(.23f, .89f, u), current,
                        velocity.sqrMagnitude > .001f ? velocity : -forward, ReturnWidth * 2 / .84f);
                }
                if (action == Action.Catch && first && rev2Beat == beat - 1)
                    SweepCrescent(returningPrevious, weapon, 1, -1, ReturnWidth, mask >> 1, 21, false);
                break;
        }
        if (!scrubbing) { rev2Beat = beat; rev2Progress = u; }
    }

    private int rev2Beat = -1, rainBlades;
    private float rev2Progress;
    private Vector3 returningPrevious;

    private float StepRev2(float progress, int beat) =>
        Mathf.Max(0, progress - (rev2Beat == beat ? rev2Progress : 0)) * (Chosen.Starts[beat + 1] - Chosen.Starts[beat]);

    private void SweepCrescent(Vector3 from, Vector3 to, float u, int beat, float halfWidth, int mask, float damage, bool mark)
    {
        if ((to - from).sqrMagnitude < .000001f) return;
        Vector3 current = Vector3.Lerp(from, to, u);
        Vector3 previous = scrubbing ? current : Vector3.Lerp(from, to, beat >= 0 && rev2Beat == beat ? rev2Progress : 0);
        Vector3 direction = (to - from).normalized;
        Vector3 side = new(-direction.y, direction.x);
        if (!scrubbing)
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || (target.HitMask & mask) != 0) continue;
                Vector3 delta = target.Ground - previous;
                if (!BladeHit(Vector3.Dot(delta, direction), Vector3.Dot(delta, side),
                    Vector2.Distance(previous, current), halfWidth)) continue;
                target.HitMask |= mask;
                if (mark) target.DaggerUntil = practiceTime + .50f;
                ApplyStyleContact(target, damage, direction);
                Play(Cue.Impact);
            }
        if (showAreas && direction != Vector3.zero) TraceBlade(previous, current, direction, halfWidth);
    }

    private void SampleMiddleDagger(Vector3 from, Vector3 direction, float u, int beat, int mask)
    {
        const float length = 5.05f, halfWidth = .31f;
        Vector3 current = from + direction * (u * length);
        Vector3 previous = scrubbing ? current : from + direction * ((rev2Beat == beat ? rev2Progress : 0) * length);
        Vector3 offset = scrubbing ? Vector3.zero : projectileLaunchOffset;
        if (u < 1)
            DrawStyle("shadow_dagger", .13f + u * .56f, ProjectileVisualPoint(current, offset, u * length), direction, 1.34f);
        var area = Lane(previous - direction * .10f, current + direction * .10f, halfWidth);
        if (!scrubbing)
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || (target.HitMask & mask) != 0 || !area.Contains(target.Ground)) continue;
                target.HitMask |= mask;
                target.DaggerUntil = practiceTime + .50f;
                ApplyStyleContact(target, 18, direction);
                Play(Cue.Impact);
            }
        if (showAreas) TraceArea(area, AreaTint, .026f, true);
    }

    private void AdvanceSeekingSword(float u, int beat, Vector3 weapon, Vector3 direction)
    {
        if (scrubbing)
        {
            DrawStyle("qi_sword", .4f, weapon + direction * u * 5f, direction, 1.2f);
            return;
        }
        const int chainMask = 1 << 28;
        if (!projectileLocked)
        {
            projectileLocked = true;
            chainPosition = weapon;
            chainDirection = direction;
            seekTarget = NearestTarget(weapon, direction, true);
            chainLegStart = weapon;
            chainLegDistance = 0;
            if (seekTarget == null) chainLegDistance = -1;
        }
        if (chainHits >= ChainTargetLimit || chainLegDistance < 0) return;
        if (seekTarget == null || seekTarget.Hp <= 0)
        {
            seekTarget = null;
            float best = 3.6f * 3.6f;
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || (target.HitMask & chainMask) != 0) continue;
                float distance = (target.Ground + DummyLift - chainPosition).sqrMagnitude;
                if (distance >= best) continue;
                best = distance;
                seekTarget = target;
            }
            chainLegStart = chainPosition;
            chainLegDistance = 0;
        }
        if (seekTarget == null) return;
        Vector3 aimPoint = seekTarget.Ground + DummyLift;
        Vector3 leg = aimPoint - chainLegStart;
        chainLegDistance += 22f * StepRev2(u, beat);
        float progress = leg.sqrMagnitude > .0001f ? Mathf.Clamp01(chainLegDistance / leg.magnitude) : 1;
        Vector3 normal = new(-leg.y, leg.x);
        float bend = Mathf.Min(.53f, leg.magnitude * .19f) * Mathf.Sin(Mathf.PI * progress);
        Vector3 next = Vector3.Lerp(chainLegStart, aimPoint, progress) + normal.normalized * (chainHits % 2 == 0 ? bend : -bend);
        if ((next - chainPosition).sqrMagnitude > .00001f) chainDirection = (next - chainPosition).normalized;
        chainPosition = next;
        const float swordSize = 1.42f;
        DrawStyle("qi_sword", .35f + .3f * u, next - chainDirection * (46f / 112f * swordSize), chainDirection, swordSize);
        if (showAreas) TraceArea(Circle(next - DummyLift, .23f), AreaTint, .024f, true);
        if ((next - aimPoint).sqrMagnitude > .23f * .23f) return;
        seekTarget.HitMask |= chainMask;
        ApplyStyleContact(seekTarget, 21, chainDirection);
        Play(Cue.Impact);
        chainHits++;
        seekTarget = null;
    }

    private Vector3 ProjectilePosition(Action action, float u, Vector3 weapon, Vector3 origin)
    {
        if (action == Action.Recall)
            return Vector3.Lerp(projectileLocked ? projectileDestination : weapon + AimVector * 6.1f, weapon, u);
        if (!projectileLocked && !scrubbing)
        {
            projectileLocked = true;
            projectileOrigin = weapon;
            seekTarget = action == Action.Chase ? NearestTarget(weapon, AimVector, true) : null;
            projectileDestination = seekTarget == null ? weapon + AimVector * (skillIndex == 9 ? 4.1f : 6.1f)
                : seekTarget.Ground + DummyLift;
        }
        Vector3 start = projectileLocked ? projectileOrigin : weapon;
        Vector3 end = projectileLocked ? projectileDestination : weapon + AimVector * (skillIndex == 9 ? 4.1f : 6.1f);
        if (action == Action.Chase && seekTarget != null && seekTarget.Hp > 0) end = seekTarget.Ground + DummyLift;
        Vector3 side = Vector3.Cross(Vector3.forward, AimVector);
        return Vector3.Lerp(start, end, u) + (action == Action.Chase ? side * Mathf.Sin(u * Mathf.PI) * 1.08f : Vector3.zero);
    }

    private void BeginTrainingDraw()
    {
        usedTrainingLines = 0;
        styleUsed = 0;
    }

    private void FinishTrainingDraw()
    {
        for (int i = usedTrainingLines; i < trainingLines.Length; i++)
            if (trainingLines[i] != null) trainingLines[i].enabled = false;
        foreach (var target in trainingTargets)
        {
            if (target == null) continue;
            if (!scrubbing && target.PushUntil > 0)
            {
                float progress = Mathf.SmoothStep(0, 1, Mathf.Clamp01(1 - (target.PushUntil - practiceTime) / PushDuration));
                target.Renderer.transform.position += target.PushOffset * (progress - target.PushProgress);
                target.PushProgress = progress;
                if (progress >= 1) target.PushUntil = 0;
            }
            target.Renderer.enabled = TrainingVisible && target.Hp > 0;
            target.Renderer.color = practiceTime < target.FlashUntil ? new Color(1, .58f, .36f)
                : practiceTime < target.StaggerUntil ? new Color(.60f, .85f, 1)
                : target.ResistsPull ? new Color(.64f, .83f, 1)
                : target.Hp <= 35 ? new Color(.81f, .64f, .61f) : Color.white;
        }
        if (TrainingVisible && StyleReady)
        {
            DrawStyleBasicWave();
            if (SelectedPassive == 1) DrawStyleOrbit();
            if (practiceTime < empowerUntil)
                DrawStyle("war_flame", Mathf.Repeat(practiceTime * 1.45f, 1), ActorPosition + TrainingFloor,
                    Vector3.up, 1.55f, order: -2);
            foreach (var target in trainingTargets)
                if (target != null && target.Hp > 0 && practiceTime < target.DaggerUntil)
                    DrawStyle("dagger_mark", 1f - (target.DaggerUntil - practiceTime) / .50f,
                        target.Ground + DummyLift + Vector3.up * .20f, Vector3.down, .85f, order: 5);
            DrawStyleCues();
        }
        if (TrainingVisible) DrawTrainingFeedback();
        for (int i = styleUsed; i < stylePool.Length; i++)
            if (stylePool[i] != null) stylePool[i].enabled = false;
    }

    private Vector3 assistPrevious;

    private void DrawTrainingFeedback()
    {
        if (showAreas && practiceTime < lastAreaUntil)
            TraceArea(lastArea, AreaTint, .025f, true);

        float assistAge = (practiceTime - assistStart) / .34f;
        if (assistAge >= 0 && assistStart > -1)
        {
            Vector3 edge = assistPulse.Origin + assistPulse.Forward * (Mathf.Clamp01(assistAge) * assistPulse.Length);
            if (assistAge < 1) DrawStyle("dao_wave", assistAge, edge, assistPulse.Forward, 2f / .84f);
            if (!scrubbing)
            {
                float step = Vector3.Dot(edge - assistPrevious, assistPulse.Forward);
                if (step > .0001f)
                    HitTargets(Lane(assistPrevious, edge, assistPulse.Width), 13, 1 << 29, styleImpact: true);
                assistPrevious = edge;
                if (assistAge >= 1) assistStart = -10;
            }
        }
        float age = (practiceTime - passiveStart) / .46f;
        if (StyleReady && SelectedPassive == 1) return;
        if (passiveStart < 0 || age < 0) return;
        age = Mathf.Clamp01(age);
        if (showAreas) TraceArea(passivePulse, AreaTint, .03f, true);
        if (passivePulse.Shape == AreaShape.Circle)
            TraceArea(Circle(passivePulse.Origin, passivePulse.Length * Mathf.Lerp(.32f, 1, age)),
                new Color(.83f, .95f, .57f, 1 - age), .095f, showEffects);
        else
        {
            Vector3 edge = passivePulse.Origin + passiveForward * (age * passivePulse.Length);
            if (!scrubbing)
            {
                foreach (var target in trainingTargets)
                {
                    if (target == null || target.Hp <= 0 || target.LastPassive == passiveProcs ||
                        !passivePulse.Contains(target.Ground) ||
                        Vector3.Dot(target.Ground - passivePulse.Origin, passiveForward) > age * passivePulse.Length + .18f) continue;
                    target.LastPassive = passiveProcs;
                    ApplyStyleContact(target, 12, passiveForward);
                }
            }
            if (age < 1)
                DrawStyle(SelectedPassive == 0 ? "passive_wave" : "counter_wave", age, edge, passiveForward, passivePulse.Width * 2 / .84f);
        }
        if (age >= 1 && !scrubbing) passiveStart = -10;
    }

    private LineRenderer TrainingLine(Color color, float width)
    {
        if (usedTrainingLines >= trainingLines.Length) return null;
        int index = usedTrainingLines++;
        var line = trainingLines[index];
        if (line == null)
        {
            line = new GameObject("Vùng tác động HEXCORE").AddComponent<LineRenderer>();
            line.transform.SetParent(transform, false);
            line.sharedMaterial = swordTrail.sharedMaterial;
            line.useWorldSpace = true;
            line.sortingOrder = 3;
            trainingLines[index] = line;
        }
        line.enabled = true;
        line.startColor = line.endColor = color;
        line.startWidth = line.endWidth = width;
        return line;
    }

    private void TraceArea(PreviewArea area, Color tint, float width, bool visible)
    {
        if (!visible) return;
        var line = TrainingLine(tint, width);
        if (line == null) return;
        line.loop = true;
        Vector3 side = Vector3.Cross(Vector3.forward, area.Forward);
        if (area.Shape == AreaShape.Lane)
        {
            line.positionCount = 4;
            Vector3 end = area.Origin + area.Forward * area.Length;
            line.SetPosition(0, area.Origin + side * area.Width);
            line.SetPosition(1, end + side * area.Width);
            line.SetPosition(2, end - side * area.Width);
            line.SetPosition(3, area.Origin - side * area.Width);
            return;
        }
        int samples = area.Shape == AreaShape.Fan ? 15 : 32;
        line.positionCount = samples + (area.Shape == AreaShape.Fan ? 1 : 0);
        for (int i = 0; i < samples; i++)
        {
            float angle = area.Shape == AreaShape.Fan ? Mathf.Lerp(-area.Width, area.Width, i / (samples - 1f)) : i * (360f / samples);
            line.SetPosition(i, area.Origin + Quaternion.Euler(0, 0, angle) * area.Forward * area.Length);
        }
        if (area.Shape == AreaShape.Fan) line.SetPosition(samples, area.Origin);
    }

    #endregion

    #region V2.1 vertical slice

    [Serializable]
    private sealed class StyleTiming
    {
        public int cell, columns;
        public StyleClip[] clips;
    }

    [Serializable]
    private sealed class StyleClip
    {
        public string id;
        public int first;
        public int[] durationMs;
        [NonSerialized] public Sprite[] Frames;
        [NonSerialized] public int TotalMs;

        public Sprite Sample(float progress)
        {
            int time = Mathf.FloorToInt(Mathf.Clamp(progress, 0, .9999f) * TotalMs);
            for (int i = 0; i < durationMs.Length; i++)
            {
                time -= durationMs[i];
                if (time < 0) return Frames[i];
            }
            return Frames[^1];
        }
    }

    private readonly struct StyleCue
    {
        public readonly string Clip;
        public readonly Vector3 Position, Direction;
        public readonly float Start, Duration, Width, DebugRadius;
        public readonly bool Mirror;
        public StyleCue(string clip, Vector3 position, Vector3 direction, float start, float duration, float width, bool mirror, float debugRadius)
        {
            Clip = clip;
            Position = position;
            Direction = direction;
            Start = start;
            Duration = duration;
            Width = width;
            Mirror = mirror;
            DebugRadius = debugRadius;
        }
    }

    private readonly Dictionary<string, StyleClip> styleClips = new();
    private readonly List<StyleCue> styleCues = new(24);
    private readonly SpriteRenderer[] stylePool = new SpriteRenderer[40];
    private int styleUsed;
    private bool polishedPreview = true;
    private bool hasCrescentPrevious;
    private float orbitPrevious;
    private Vector3 crescentPrevious;
    private struct BasicWave
    {
        public bool Active, Credited;
        public float Start;
        public Vector3 Origin, Forward, Previous, LaunchOffset;
        public int Targets;
    }
    private readonly BasicWave[] basicWaves = new BasicWave[2];
    private const float OrbitRange = 2.2f, OrbitDuration = .56f, OrbitHalfBand = .14f;
    private const float CrescentWidth = 1.40f, CrescentTravel = 5.0f;
    private const float BasicWaveTravel = 2.15f, BasicWaveDuration = .21f, BasicWaveWidth = .49f;
    private const float PushDuration = .19f, DaoWaveRange = 5f, DaoWaveHalfAngle = 40f, DaoWaveBand = .18f;

    private bool StyleReady => previewV2 && polishedPreview && styleClips.Count > 0;
    private bool PolishedBranch => StyleReady && IsBranchSkill;

    private void InitStyleAtlas()
    {
        var texture = Resources.Load<Texture2D>("HexcoreV21/StyleSheet");
        var timing = Resources.Load<TextAsset>("HexcoreV21/StyleTiming");
        if (texture == null || timing == null) return;
        var atlas = JsonUtility.FromJson<StyleTiming>(timing.text);
        if (atlas == null || atlas.cell < 1 || atlas.columns < 1 || atlas.clips == null ||
            texture.width != atlas.columns * atlas.cell) return;

        foreach (var clip in atlas.clips)
        {
            if (clip.durationMs == null || clip.durationMs.Length == 0 ||
                clip.first < 0 || (clip.first + clip.durationMs.Length - 1) / atlas.columns >= texture.height / atlas.cell)
                continue;
            clip.Frames = new Sprite[clip.durationMs.Length];
            for (int i = 0; i < clip.Frames.Length; i++)
            {
                int index = clip.first + i;
                var rect = new Rect(index % atlas.columns * atlas.cell,
                    texture.height - (index / atlas.columns + 1) * atlas.cell, atlas.cell, atlas.cell);
                float pivotX = clip.id switch
                {
                    "crescent" or "dao_wave" or "passive_wave" or "counter_wave" => 70f / atlas.cell,
                    "shadow_dagger" => 90f / atlas.cell,
                    _ => .5f
                };
                var pivot = new Vector2(pivotX, .5f);
                clip.Frames[i] = Sprite.Create(texture, rect, pivot, atlas.cell, 0, SpriteMeshType.FullRect);
                clip.TotalMs += clip.durationMs[i];
            }
            styleClips.Add(clip.id, clip);
        }
    }

    private void OnDestroy()
    {
        foreach (var clip in styleClips.Values)
            foreach (var frame in clip.Frames)
                Destroy(frame);
    }

    private void DrawStyle(string id, float progress, Vector3 point, Vector3 forward, float width, bool mirror = false, int order = 2)
    {
        if (!StyleReady || !styleClips.TryGetValue(id, out var clip) || AcquireStyleRenderer() is not { } sprite) return;
        sprite.sprite = clip.Sample(progress);
        sprite.sortingOrder = order;
        sprite.color = Color.white;
        sprite.flipX = false;
        sprite.flipY = mirror;
        sprite.transform.localScale = Vector3.one * width;
        sprite.transform.SetPositionAndRotation(point, Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg));
    }

    private void DrawCloneEcho(SpriteRenderer clone, Vector3 offset, float alpha)
    {
        if (!StyleReady || AcquireStyleRenderer() is not { } sprite) return;
        sprite.sprite = clone.sprite;
        sprite.sortingOrder = clone.sortingOrder - 1;
        sprite.color = new Color(.69f, .29f, .96f, alpha);
        sprite.flipX = clone.flipX;
        sprite.flipY = clone.flipY;
        sprite.transform.localScale = clone.transform.lossyScale;
        sprite.transform.SetPositionAndRotation(clone.transform.position - offset, clone.transform.rotation);
    }

    private SpriteRenderer AcquireStyleRenderer()
    {
        if (!showEffects || styleUsed == stylePool.Length) return null;
        var sprite = stylePool[styleUsed];
        if (sprite == null)
        {
            sprite = new GameObject("HEXCORE VFX v2.1").AddComponent<SpriteRenderer>();
            sprite.transform.SetParent(transform, false);
            sprite.sharedMaterial = actor.sharedMaterial;
            stylePool[styleUsed] = sprite;
        }
        styleUsed++;
        sprite.enabled = true;
        return sprite;
    }

    private void EmitStyle(string id, Vector3 position, Vector3 forward, float duration, float width,
        bool mirror = false, float debugRadius = 0)
    {
        if (!StyleReady || scrubbing || styleCues.Count == 24) return;
        styleCues.Add(new StyleCue(id, position, forward, practiceTime, duration, width, mirror, debugRadius));
    }

    private void DrawStyleCues()
    {
        for (int i = styleCues.Count - 1; i >= 0; i--)
        {
            var cue = styleCues[i];
            float age = (practiceTime - cue.Start) / cue.Duration;
            if (age >= 1)
            {
                styleCues.RemoveAt(i);
                continue;
            }
            DrawStyle(cue.Clip, age, cue.Position, cue.Direction, cue.Width, cue.Mirror,
                cue.Clip is "ground_slam" or "blood_burst" ? -1 : 2);
            if (showAreas && cue.DebugRadius > 0)
                TraceArea(Circle(cue.Position, cue.DebugRadius), AreaTint, .024f, true);
        }
    }

    private void CreditBasicHit(Vector3 weapon, Vector3 direction)
    {
        if (SelectedPassive >= 2 || ++passiveHits < (SelectedPassive == 0 ? 5 : 4)) return;
        passiveHits = 0;
        TriggerPassive(weapon, direction);
    }

    private PreviewArea StyleBasicArea(Vector3 root, int strike, out string id)
    {
        id = formIndex switch
        {
            0 => "basic_silver",
            1 => strike == 2 ? "basic_thrust" : "basic_dagger",
            2 => "basic_heavy",
            _ => "crescent"
        };
        if (id == "basic_thrust" || formIndex == 2 && strike == 1)
            return Lane(root, root + AimVector * 1.85f, id == "basic_thrust" ? .32f : .53f);
        float angle = formIndex switch { 0 => 51, 1 => 43, 2 => 63, _ => 46 };
        return Fan(root, AimVector, 1.82f, angle);
    }

    private void StartStyleBasicWave(int strike, float elapsed)
    {
        Vector2 tip = strike == 0 ? QiBasicFirstTip[aim] : QiBasicBlade[aim];
        Vector3 start = ActorPosition + TrainingFloor + AimVector * .42f;
        Vector3 weapon = ActorPosition + WorldSpriteOffset(tip);
        basicWaves[strike] = new BasicWave
        {
            Active = true, Start = practiceTime - Mathf.Max(0, elapsed), Origin = start, Previous = start, Forward = AimVector,
            LaunchOffset = weapon - (start + DummyLift)
        };
        Play(Cue.Wave);
    }

    private static Vector3 ProjectileVisualPoint(Vector3 ground, Vector3 launchOffset, float distance) =>
        ground + DummyLift + launchOffset * (1 - Mathf.SmoothStep(0, 1, Mathf.Clamp01(distance / .80f)));

    // The crescent pivots on its luminous nose. Its tips trail the nose rather than filling a rectangle.
    private const float BladeThickness = .115f;

    private static float BladeRetreat(float sideFraction, float halfWidth) =>
        .57f * sideFraction * sideFraction * halfWidth;

    private static bool BladeHit(float along, float side, float travel, float halfWidth)
    {
        if (Math.Abs(side) > halfWidth) return false;
        float axial = along + BladeRetreat(side / halfWidth, halfWidth);
        return axial >= -BladeThickness && axial <= travel + BladeThickness;
    }

    private static bool RingHit(float distance, float previousRadius, float radius) =>
        distance >= Math.Max(0, previousRadius - OrbitHalfBand) && distance <= radius + OrbitHalfBand;

    private void TraceBlade(Vector3 previous, Vector3 current, Vector3 forward, float halfWidth)
    {
        var line = TrainingLine(AreaTint, .024f);
        if (line == null) return;
        line.loop = true;
        const int sideSteps = 12;
        line.positionCount = 2 * (sideSteps + 1);
        Vector3 side = new(-forward.y, forward.x);
        for (int i = 0; i <= sideSteps; i++)
        {
            float across = Mathf.Lerp(-1, 1, i / (float)sideSteps);
            Vector3 flank = side * (across * halfWidth) - forward * BladeRetreat(across, halfWidth);
            line.SetPosition(i, current + flank + forward * BladeThickness);
            line.SetPosition(2 * sideSteps + 1 - i, previous + flank - forward * BladeThickness);
        }
    }

    private void DrawStyleBasicWave()
    {
        for (int strike = 0; strike < basicWaves.Length; strike++)
        {
            ref var wave = ref basicWaves[strike];
            if (!wave.Active) continue;
            float age = Mathf.Clamp01((practiceTime - wave.Start) / BasicWaveDuration);
            Vector3 current = wave.Origin + wave.Forward * (age * BasicWaveTravel);
            if (!scrubbing)
            {
                Vector3 side = new(-wave.Forward.y, wave.Forward.x);
                float step = Vector3.Dot(current - wave.Previous, wave.Forward);
                bool connected = false;
                for (int i = 0; i < trainingTargets.Length; i++)
                {
                    var target = trainingTargets[i];
                    int bit = 1 << i;
                    if (target == null || target.Hp <= 0 || (wave.Targets & bit) != 0) continue;
                    Vector3 delta = target.Ground - wave.Previous;
                    if (!BladeHit(Vector3.Dot(delta, wave.Forward), Vector3.Dot(delta, side), step, BasicWaveWidth)) continue;
                    wave.Targets |= bit;
                    ApplyStyleContact(target, 7, wave.Forward);
                    connected = true;
                }
                if (connected && !wave.Credited)
                {
                    wave.Credited = true;
                    CreditBasicHit(current, wave.Forward);
                    Play(Cue.Impact);
                }
            }
            if (age < 1)
            {
                Vector3 center = ProjectileVisualPoint(current, wave.LaunchOffset, age * BasicWaveTravel);
                DrawStyle("crescent", age, center, wave.Forward, BasicWaveWidth * 2 / .84f, mirror: strike == 1);
                if (showAreas) TraceBlade(wave.Previous, current, wave.Forward, BasicWaveWidth);
            }
            if (scrubbing) continue;
            wave.Active = age < 1;
            wave.Previous = current;
        }
    }

    private void DrawStyleOrbit()
    {
        if (passiveStart < 0) return;
        float elapsed = practiceTime - passiveStart;
        if (elapsed < 0 || elapsed > OrbitDuration && orbitPrevious >= OrbitRange) return;
        float age = Mathf.Clamp01(elapsed / OrbitDuration);
        float radius = Mathf.Lerp(.25f, OrbitRange, age);
        Vector3 center = passivePulse.Origin;
        float inside = Mathf.Max(0, orbitPrevious - OrbitHalfBand);
        float outside = radius + OrbitHalfBand;
        if (!scrubbing)
        {
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || target.LastPassive == passiveProcs) continue;
                float distance = Vector2.Distance(center, target.Ground);
                if (!RingHit(distance, orbitPrevious, radius)) continue;
                target.LastPassive = passiveProcs;
                ApplyStyleContact(target, 12, (target.Ground - center).normalized);
            }
            orbitPrevious = radius;
        }
        if (age >= 1) return;
        DrawStyle("orbit", age, center, Vector3.right, radius / .40f, order: 0);
        if (!showAreas) return;
        TraceArea(Circle(center, outside), AreaTint, .023f, true);
        if (inside > 0) TraceArea(Circle(center, inside), AreaTint, .023f, true);
    }

    private void ApplyStyleContact(PreviewTarget target, float damage, Vector3 forward)
    {
        target.Hp = Mathf.Max(0, target.Hp - damage);
        target.FlashUntil = practiceTime + .24f;
        trainingHits++;
        EmitStyle("hit_spark", target.Ground, forward, .25f, 1.20f);
    }

    private void PushTrainingTarget(PreviewTarget target, Vector3 direction, float distance)
    {
        target.PushOffset = direction * distance;
        target.PushUntil = practiceTime + PushDuration;
        target.PushProgress = 0;
    }

    private static bool DaoWaveHit(Vector3 delta, Vector3 direction, float previousRadius, float radius)
    {
        float distance = delta.magnitude;
        return distance >= Mathf.Max(0, previousRadius - DaoWaveBand) && distance <= radius &&
            Vector3.Dot(delta, direction) >= distance * Mathf.Cos(DaoWaveHalfAngle * Mathf.Deg2Rad);
    }

    private void SampleDaoBladeWave(Vector3 weapon, float age, int mask)
    {
        Vector3 groundedBlade = ActorPosition + TrainingFloor + AimVector * .42f;
        if (!projectileLocked && !scrubbing)
        {
            projectileOrigin = groundedBlade;
            projectileLaunchOffset = weapon - (groundedBlade + DummyLift);
            projectileLocked = true;
            Play(Cue.Wave);
        }
        Vector3 origin = projectileLocked ? projectileOrigin : groundedBlade;
        Vector3 forward = AimVector;
        float radius = DaoWaveRange * age;
        float previousRadius = scrubbing ? radius : daoWaveRadius;
        if (!scrubbing)
        {
            bool connected = false;
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || (target.HitMask & mask) != 0 ||
                    !DaoWaveHit(target.Ground - origin, forward, previousRadius, radius)) continue;
                target.HitMask |= mask;
                ApplyStyleContact(target, 16, forward);
                PushTrainingTarget(target, forward, .48f);
                connected = true;
            }
            daoWaveRadius = radius;
            if (connected) Play(Cue.Impact);
        }
        if (age < 1 && radius > .025f)
        {
            Vector3 launch = projectileLocked ? projectileLaunchOffset : weapon - (origin + DummyLift);
            DrawStyle("dao_fan", age * .82f, ProjectileVisualPoint(origin, launch, radius), forward, radius * 2f);
        }
        if (showAreas)
        {
            TraceArea(Fan(origin, forward, radius, DaoWaveHalfAngle), AreaTint, .027f, true);
            if (previousRadius > DaoWaveBand) TraceArea(Fan(origin, forward, previousRadius - DaoWaveBand, DaoWaveHalfAngle), AreaTint, .022f, true);
        }
    }

    private void SampleStyleCrescent(int mask, float age, Vector3 weapon)
    {
        Vector3 launch = ActorPosition + TrainingFloor + AimVector * .42f;
        if (!projectileLocked && !scrubbing)
        {
            projectileLocked = true;
            projectileOrigin = launch;
            projectileLaunchOffset = weapon - (launch + DummyLift);
        }
        Vector3 start = projectileLocked ? projectileOrigin : launch;
        Vector3 current = start + AimVector * (age * CrescentTravel);
        Vector3 direction = AimVector;
        Vector3 previous = scrubbing ? current : hasCrescentPrevious ? crescentPrevious : start;
        if (!scrubbing)
        {
            bool connected = false;
            Vector3 side = new(-direction.y, direction.x);
            float step = Vector3.Dot(current - previous, direction);
            foreach (var target in trainingTargets)
            {
                if (target == null || target.Hp <= 0 || (target.HitMask & mask) != 0) continue;
                Vector3 delta = target.Ground - previous;
                if (!BladeHit(Vector3.Dot(delta, direction), Vector3.Dot(delta, side), step, CrescentWidth)) continue;
                target.HitMask |= mask;
                ApplyStyleContact(target, 14, direction);
                if (!target.ResistsPull)
                {
                    PushTrainingTarget(target, direction, .63f);
                    EmitStyle("qi_burst", target.Ground, direction, .24f, 1.32f);
                }
                connected = true;
            }
            if (connected) Play(Cue.Impact);
            crescentPrevious = current;
            hasCrescentPrevious = true;
        }
        if (age < 1)
        {
            Vector3 offset = projectileLocked ? projectileLaunchOffset : weapon - (launch + DummyLift);
            DrawStyle("crescent", age, ProjectileVisualPoint(current, offset, age * CrescentTravel),
                direction, CrescentWidth * 2 / .84f);
        }
        if (showAreas) TraceBlade(previous, current, direction, CrescentWidth);
    }

    #endregion
}
