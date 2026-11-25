using UnityEngine;

// Shared frame cache for the imported skill sheets. Each sprite keeps its authored contact point.
internal sealed class GunNewSkillArt : MonoBehaviour
{
    internal enum Clip { Quake, Portal, Shield, Orbit, Rocket, SkyCircle, Fireball, Breath, Sweep, Fuse, RecallOut, RecallReturn, Impact, Saw, HotMuzzle, Heavy, DecayAura, DecayHit, Anchor, Tether, AnchorSnap, CageField, CagePulse, CageCollapse, HealRing, HealHearts, HealSlime, PoisonRing, PortalBeam, TeslaCoil, Stunned, Thunderclap, ChainLightning, BallLightning, EnergyCore, PlasmaExplosion, BlizzardVortex }
    private Texture2D[] textures;
    internal Material NoGlowMaterial { get; private set; }
    private readonly Sprite[][] frames = new Sprite[(int)Clip.BlizzardVortex + 1][];
    private static readonly Vector2Int[] Cells =
    {
        new Vector2Int(288,104), new Vector2Int(152,116), new Vector2Int(88,84), new Vector2Int(184,100),
        new Vector2Int(480,244), new Vector2Int(200,220), new Vector2Int(356,244), new Vector2Int(160,116),
        new Vector2Int(136,104), new Vector2Int(240,116), new Vector2Int(192,128), new Vector2Int(192,128), new Vector2Int(320,288), new Vector2Int(120,52), new Vector2Int(192,128), new Vector2Int(192,128), new Vector2Int(72,80), new Vector2Int(320,288), new Vector2Int(320,288), new Vector2Int(16,8), new Vector2Int(320,288), new Vector2Int(320,288), new Vector2Int(320,288), new Vector2Int(320,288),
        new Vector2Int(256,136), new Vector2Int(60,76), new Vector2Int(112,88), new Vector2Int(72,64), new Vector2Int(264,100),
        new Vector2Int(224,136), new Vector2Int(60,64), new Vector2Int(288,156), new Vector2Int(248,104), new Vector2Int(136,104), new Vector2Int(112,100), new Vector2Int(224,200), new Vector2Int(128,116)
    };
    private static readonly Vector2[] Anchors =
    {
        new Vector2(144,94), new Vector2(76,65), new Vector2(44,44), new Vector2(140,65),
        new Vector2(374,208), new Vector2(100,191), new Vector2(250,208), new Vector2(15,70),
        new Vector2(13,67), new Vector2(178,66), new Vector2(128,88), new Vector2(128,88), new Vector2(160,181), new Vector2(92,26), new Vector2(64,88), new Vector2(128,88), new Vector2(36,66), new Vector2(160,181), new Vector2(160,181), new Vector2(0,4), new Vector2(160,181), new Vector2(160,160), new Vector2(160,181), new Vector2(160,181),
        new Vector2(128,111), new Vector2(30,52), new Vector2(56,73), new Vector2(36,54), new Vector2(125,60),
        new Vector2(112,122), new Vector2(30,28), new Vector2(144,126), new Vector2(136,55), new Vector2(90,41), new Vector2(56,89), new Vector2(112,100), new Vector2(64,102)
    };
    private static readonly float[] PixelsPerUnit = { 100, 85, 40, 70, 90, 42, 70, 32, 21f, 95f, 60f, 60f, 70f, 50f, 60f, 40f, 65f, 60f, 50f, 64f, 55f, 32.692308f, 40.384615f, 50f, 112f, 68f, 67f, 48f, 64f, 88f, 68f, 80f, 64f, 54f, 52f, 24f, 34f };
    // Clip order matches the texture references in GunSkillTest.
    internal void Setup(Texture2D[] sheets, Texture2D[] pixelSheets, Material noGlowMaterial)
    {
        NoGlowMaterial = noGlowMaterial;
        textures = new Texture2D[sheets.Length + pixelSheets.Length];
        System.Array.Copy(sheets, textures, sheets.Length);
        System.Array.Copy(pixelSheets, 0, textures, sheets.Length, pixelSheets.Length);
    }

    internal Sprite Frame(Clip clip, int frame)
    {
        int index = (int)clip;
        Vector2Int cell = Cells[index];
        if (frames[index] == null)
            frames[index] = new Sprite[textures[index].width / cell.x * (textures[index].height / cell.y)];

        frame = Mathf.Clamp(frame, 0, frames[index].Length - 1);
        if (frames[index][frame] != null) return frames[index][frame];
        Vector2 anchor = Anchors[index];
        if (clip == Clip.Rocket && frame >= 19 && frame <= 33) anchor = RocketCenters[frame - 19];
        if (clip == Clip.Fireball && frame >= 7 && frame <= 42) anchor = FireballCenters[frame - 7];
        int columns = textures[index].width / cell.x;
        var rect = new Rect(frame % columns * cell.x, textures[index].height - (frame / columns + 1) * cell.y,
            cell.x, cell.y);
        if (clip == Clip.ChainLightning)
        {
            // Exclude the source's three ground markers beneath the lightning.
            rect = new Rect(rect.x + 40f, rect.y + 26f, 192f, 64f);
            anchor -= new Vector2(40f, 14f);
        }
        else if (clip == Clip.BlizzardVortex)
        {
            // The bottom 15 rows contain the stationary ground streak.
            rect.yMin += 15f;
            anchor.y = rect.height;
        }
        var pivot = new Vector2(anchor.x / rect.width, 1f - anchor.y / rect.height);
        return frames[index][frame] = Sprite.Create(textures[index], rect, pivot, PixelsPerUnit[index], 0, SpriteMeshType.FullRect);
    }

    internal Sprite Loop(Clip clip, float elapsed, float fps, int first, int count) =>
        Frame(clip, first + Mathf.FloorToInt(elapsed * fps) % count);

    internal void UpdateLightningLink(SpriteRenderer cable, Vector2 from, Vector2 to, float elapsed)
    {
        const float chainSpan = 175f / 64f;
        Vector2 segment = to - from;
        cable.transform.SetPositionAndRotation((from + to) * 0.5f,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, segment)));
        cable.transform.localScale = new Vector3(segment.magnitude / chainSpan, 0.55f, 1f);
        cable.sprite = Loop(Clip.ChainLightning, elapsed, 30f, 10, 12);
    }

    internal Sprite Progress(Clip clip, float progress, int first, int last) =>
        Frame(clip, Mathf.Min(last, first + Mathf.FloorToInt(Mathf.Clamp01(progress) * (last - first + 1))));

    // The first three slices are the ring outside/below the actor. The last three
    // isolate the authored shoulder, waist and leg sparks for color keying.
    private static readonly Rect[] PoisonPieces =
    {
        new Rect(0, 47, 27, 14),
        new Rect(47, 47, 25, 14),
        new Rect(27, 55, 20, 6),
        new Rect(31, 25, 6, 6),
        new Rect(34, 33, 4, 6),
        new Rect(33, 45, 6, 6)
    };
    private readonly Sprite[,] poisonFrames = new Sprite[PoisonPieces.Length, 13];

    internal Vector2 PoisonRingOffset(int piece)
    {
        Vector2 center = PoisonPieces[piece].center;
        return new Vector2(center.x - 36f, 54f - center.y) / 48f;
    }

    internal Sprite PoisonPartFrame(int piece, int frame)
    {
        if (poisonFrames[piece, frame] != null) return poisonFrames[piece, frame];
        Texture2D atlas = textures[(int)Clip.PoisonRing];
        Rect crop = PoisonPieces[piece];
        var rect = new Rect(frame % 7 * 72 + crop.x,
            atlas.height - frame / 7 * 64 - crop.yMax, crop.width, crop.height);
        return poisonFrames[piece, frame] = Sprite.Create(atlas, rect, Vector2.one * 0.5f, 48f, 0, SpriteMeshType.FullRect);
    }

    private void OnDestroy()
    {
        foreach (Sprite sprite in poisonFrames)
            if (sprite != null) Destroy(sprite);
        foreach (var clip in frames)
            if (clip != null)
                foreach (var sprite in clip)
                    if (sprite != null) Destroy(sprite);
    }

    private static readonly Vector2[] RocketCenters =
    {
        new Vector2(201.3619f, 174.5111f),
        new Vector2(209.9970f, 173.8214f),
        new Vector2(210.4771f, 173.0092f),
        new Vector2(201.8663f, 172.1854f),
        new Vector2(209.6747f, 171.5994f),
        new Vector2(220.1705f, 171.5607f),
        new Vector2(227.1748f, 171.0280f),
        new Vector2(234.8131f, 170.2995f),
        new Vector2(240.4935f, 170.3109f),
        new Vector2(246.7914f, 170.4149f),
        new Vector2(255.0955f, 171.1701f),
        new Vector2(266.4508f, 174.2172f),
        new Vector2(279.1376f, 175.3228f),
        new Vector2(292.0323f, 175.2097f),
        new Vector2(304.8663f, 174.9840f),
    };
    private static readonly Vector2[] FireballCenters =
    {
        new Vector2(75.8021f, 172.1979f),
        new Vector2(78.0286f, 171.6476f),
        new Vector2(78.0455f, 171.9773f),
        new Vector2(77.5570f, 170.0127f),
        new Vector2(78.3614f, 169.5904f),
        new Vector2(79.1549f, 167.7887f),
        new Vector2(78.3333f, 168.1667f),
        new Vector2(78.0000f, 168.9398f),
        new Vector2(75.2179f, 170.4103f),
        new Vector2(75.8786f, 173.4643f),
        new Vector2(76.0351f, 173.0088f),
        new Vector2(78.7080f, 173.9381f),
        new Vector2(76.4231f, 170.1827f),
        new Vector2(76.7200f, 169.1600f),
        new Vector2(77.0000f, 170.9902f),
        new Vector2(75.2553f, 168.1277f),
        new Vector2(77.8125f, 167.7750f),
        new Vector2(79.1831f, 166.2817f),
        new Vector2(77.9882f, 167.6235f),
        new Vector2(78.2065f, 167.7174f),
        new Vector2(80.3846f, 168.4308f),
        new Vector2(79.0918f, 170.3673f),
        new Vector2(92.0000f, 175.8043f),
        new Vector2(102.3906f, 175.3438f),
        new Vector2(115.1724f, 175.6379f),
        new Vector2(128.6909f, 175.8545f),
        new Vector2(139.5417f, 175.3750f),
        new Vector2(148.1277f, 175.1915f),
        new Vector2(159.4889f, 175.5556f),
        new Vector2(172.2250f, 175.6750f),
        new Vector2(181.9091f, 175.7273f),
        new Vector2(196.2326f, 176.0233f),
        new Vector2(205.6905f, 176.5000f),
        new Vector2(216.0385f, 176.8077f),
        new Vector2(225.2549f, 177.0588f),
        new Vector2(237.6275f, 176.1569f),
    };

}
