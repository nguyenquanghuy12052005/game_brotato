using System.Collections.Generic;
using UnityEngine;

public enum MageEffectId
{
    Heal, Shockwave, Gravity, Fireball, Explosion, Burn, IceBreath, Freeze,
    LightPillar, Smite, DarkMissile, Implosion, Curse, TimeStop, StarFall,
    RuneCircle, ArcaneOrb, ArcaneMissiles
}

public sealed class MageEffectLibrary : MonoBehaviour
{
    // The PNG exports have no JSON in the project. Frame sizes and counts are
    // kept here so each original sheet can be used without modifying its pixels.
    private struct Sheet
    {
        public int width, height, frames;
        public float fps;
        public bool loop;

        public Sheet(int width, int height, int frames, float fps, bool loop = false)
        {
            this.width = width;
            this.height = height;
            this.frames = frames;
            this.fps = fps;
            this.loop = loop;
        }
    }

    [SerializeField] private Texture2D[] atlases = new Texture2D[18];
    [SerializeField, Range(0.5f, 3f)] private float visualScale = 2.2f;
    private readonly Dictionary<MageEffectId, Sprite[]> sprites = new Dictionary<MageEffectId, Sprite[]>();

    private static Sheet GetSheet(MageEffectId id)
    {
        switch (id)
        {
            case MageEffectId.Heal: return new Sheet(256, 136, 86, 30);
            case MageEffectId.Shockwave: return new Sheet(224, 160, 58, 30);
            case MageEffectId.Gravity: return new Sheet(168, 160, 96, 30, true);
            case MageEffectId.Fireball: return new Sheet(136, 56, 36, 30, true);
            case MageEffectId.Explosion: return new Sheet(300, 240, 163, 30);
            case MageEffectId.Burn: return new Sheet(72, 80, 36, 30, true);
            case MageEffectId.IceBreath: return new Sheet(184, 104, 36, 30);
            case MageEffectId.Freeze: return new Sheet(176, 192, 72, 30);
            case MageEffectId.LightPillar: return new Sheet(200, 220, 118, 30);
            case MageEffectId.Smite: return new Sheet(224, 176, 85, 30);
            case MageEffectId.DarkMissile: return new Sheet(208, 112, 58, 30);
            case MageEffectId.Implosion: return new Sheet(208, 184, 50, 30);
            case MageEffectId.Curse: return new Sheet(64, 84, 72, 30, true);
            case MageEffectId.TimeStop: return new Sheet(196, 136, 109, 30);
            case MageEffectId.StarFall: return new Sheet(232, 172, 76, 30);
            case MageEffectId.RuneCircle: return new Sheet(136, 100, 120, 30, true);
            case MageEffectId.ArcaneOrb: return new Sheet(136, 112, 72, 30, true);
            default: return new Sheet(240, 116, 54, 30);
        }
    }

    public MageEffectPlayer Play(MageEffectId id, Vector3 position, float scale = 1f,
        Transform parent = null, int sortingOrder = 20, bool loop = false)
    {
        Sprite[] frames = GetFrames(id);
        if (frames == null) return null;

        var effect = new GameObject(id + " Effect");
        effect.transform.SetParent(parent, false);
        effect.transform.position = position;
        effect.transform.localScale = Vector3.one * scale * visualScale;
        var renderer = effect.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;
        var player = effect.AddComponent<MageEffectPlayer>();
        Sheet sheet = GetSheet(id);
        player.Initialize(renderer, frames, sheet.fps, loop || sheet.loop);
        return player;
    }

    public Sprite GetFrame(MageEffectId id, int index)
    {
        Sprite[] frames = GetFrames(id);
        return frames == null ? null : frames[Mathf.Clamp(index, 0, frames.Length - 1)];
    }

    private Sprite[] GetFrames(MageEffectId id)
    {
        if (sprites.TryGetValue(id, out Sprite[] cached)) return cached;
        int index = (int)id;
        if (atlases == null || index >= atlases.Length || atlases[index] == null)
        {
            Debug.LogError("Mage effect atlas is missing: " + id, this);
            return null;
        }

        Texture2D atlas = atlases[index];
        Sheet sheet = GetSheet(id);
        int columns = atlas.width / sheet.width;
        if (columns == 0 || atlas.width % sheet.width != 0 ||
            atlas.height % sheet.height != 0 || columns * (atlas.height / sheet.height) < sheet.frames)
        {
            Debug.LogError("Mage effect atlas size does not match " + id, this);
            return null;
        }

        var result = new Sprite[sheet.frames];
        for (int i = 0; i < result.Length; i++)
        {
            int x = (i % columns) * sheet.width;
            int y = atlas.height - (i / columns + 1) * sheet.height;
            result[i] = Sprite.Create(atlas, new Rect(x, y, sheet.width, sheet.height),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        }
        sprites.Add(id, result);
        return result;
    }

    private void OnDestroy()
    {
        foreach (Sprite[] frames in sprites.Values)
            foreach (Sprite frame in frames)
                if (frame != null) Destroy(frame);
    }
}
