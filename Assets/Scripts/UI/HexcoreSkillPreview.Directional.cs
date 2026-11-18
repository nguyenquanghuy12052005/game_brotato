using System;
using System.Collections.Generic;
using UnityEngine;
using Beat = HexcorePreviewCatalog.Action;

public sealed partial class HexcoreSkillPreview
{
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
        sampledTake = take;
        sampledFrame = Mathf.Clamp(frame, 0, last);
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
            arc[i] = origin + (Mathf.Cos(angle) * AimVector + Mathf.Sin(angle) * side) * (.56f + u * .45f);
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
}
