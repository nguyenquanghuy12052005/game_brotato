using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal enum HexcoreUltimate { None, Grenades, Meteors, Turret, Rail, Cage }

internal sealed class GunHexcoreUltimate : MonoBehaviour
{
    // Texture order matches GunSkillTest's serialized references.
    private enum Art { Grenade, Blast, Helicopter, AirstrikeMark, AirstrikeImpact,
        Charge, Beam, CageField, CageShot, CageCollapse }

    internal const int TurretLimit = 2;
    internal const int GrenadeCapacity = 3;
    private const float GrenadeRechargeSeconds = 2.4f;
    private int grenadeCharges = GrenadeCapacity;
    private float nextGrenadeCharge;
    private float nextGrenadeThrow;
    private Texture2D grenadeAtlas;
    private Texture2D meteorAtlas;
    private Sprite[] meteorFrames;
    private Sprite[] grenadeFrames;
    private static readonly Vector2[] GrenadeCenters =
    {
        new Vector2(19.0575f, 100.6092f),
        new Vector2(25.2333f, 95.9778f),
        new Vector2(31.0213f, 92.7128f),
        new Vector2(36.8710f, 90.3226f),
        new Vector2(42.5870f, 88.6630f),
        new Vector2(48.5889f, 87.9889f),
        new Vector2(54.1868f, 88.3187f),
        new Vector2(59.7912f, 89.6044f),
        new Vector2(65.1778f, 91.9889f),
        new Vector2(70.5957f, 95.3298f),
        new Vector2(75.8804f, 99.7717f),
        new Vector2(81.2283f, 105.3913f),
        new Vector2(86.6023f, 112.0341f),
        new Vector2(91.9880f, 119.7229f),
        new Vector2(97.2747f, 128.5165f),
        new Vector2(102.4886f, 138.2841f),
        new Vector2(108.2778f, 149.3000f),
        new Vector2(113.5955f, 161.2809f),
        new Vector2(118.5294f, 160.5412f),
        new Vector2(123.1494f, 158.0575f),
        new Vector2(128.0112f, 156.1798f),
        new Vector2(132.5824f, 155.2527f),
        new Vector2(137.3563f, 155.3563f),
        new Vector2(142.0899f, 156.1798f),
        new Vector2(146.9674f, 157.9674f),
        new Vector2(151.6923f, 160.8352f),
        new Vector2(156.3810f, 163.4881f),
        new Vector2(159.9091f, 162.5682f),
        new Vector2(163.3763f, 162.0860f),
        new Vector2(166.8913f, 162.7826f),
        new Vector2(169.9889f, 162.4778f),
        new Vector2(172.6111f, 162.2222f),
        new Vector2(174.8556f, 162.3889f),
        new Vector2(177.3933f, 162.6966f),
        new Vector2(179.6559f, 163.1398f),
        new Vector2(181.9655f, 163.3793f),
        new Vector2(184.0562f, 163.8652f),
        new Vector2(186.2921f, 163.8539f),
        new Vector2(188.1978f, 164.2857f),
        new Vector2(190.1613f, 164.2903f),
        new Vector2(192.1222f, 164.3889f),
        new Vector2(193.3793f, 164.3103f),
        new Vector2(195.0667f, 164.1556f),
        new Vector2(196.2889f, 164.2000f),
        new Vector2(197.5730f, 163.9888f),
        new Vector2(198.7111f, 163.8556f),
        new Vector2(199.6374f, 163.7912f),
        new Vector2(200.8864f, 163.8182f),
        new Vector2(201.1667f, 163.6310f),
    };
    internal int GrenadeCharges { get { RefreshGrenades(); return grenadeCharges; } }
    internal float GrenadeRechargeLeft
    {
        get
        {
            RefreshGrenades();
            return grenadeCharges < GrenadeCapacity ? Mathf.Max(0f, nextGrenadeCharge - Time.time) : 0f;
        }
    }

    private void RefreshGrenades()
    {
        while (grenadeCharges < GrenadeCapacity && Time.time >= nextGrenadeCharge)
        {
            grenadeCharges++;
            nextGrenadeCharge += GrenadeRechargeSeconds;
        }
    }

    private const float TurretRange = 7f;
    private const float TurretLifetime = 8f;
    private const int FramesPerClip = 32;
    private readonly Sprite[][] frames = new Sprite[10][];
    private readonly List<RaycastHit2D> lineHits = new List<RaycastHit2D>();
    private readonly HashSet<EnemyHealth> damaged = new HashSet<EnemyHealth>();
    private Texture2D[] sheets;
    private GunNewSkillArt newArt;
    private Texture2D turretAtlas;
    private Sprite[] turretSprites;
    private EnemyHealth[] targets;
    private Transform player;
    private Transform muzzle;
    private Transform visuals;
    private GunSkillWeapon weapon;
    private Material material;
    private float readyAt;
    private int turretCount;
    private int turretPlacement;
    private bool breathing;

    internal float CooldownLeft => Mathf.Max(0f, readyAt - Time.time);
    internal int TurretCount => turretCount;

    internal void Setup(GunSkillWeapon owner, Transform playerRoot, Transform firePoint,
        Material spriteMaterial, EnemyHealth[] practiceTargets, Texture2D[] textures, Texture2D turretTexture, Texture2D grenadeTexture, Texture2D meteorTexture, GunNewSkillArt importedArt)
    {
        weapon = owner;
        player = playerRoot;
        muzzle = firePoint;
        material = spriteMaterial;
        targets = practiceTargets;
        sheets = textures;
        newArt = importedArt;
        turretAtlas = turretTexture;
        grenadeAtlas = grenadeTexture;
        meteorAtlas = meteorTexture;
        visuals = new GameObject("Hexcore ultimate effects").transform;
        visuals.SetParent(transform, false);
    }

    internal bool CanCast(HexcoreUltimate selected)
    {
        if (breathing) return false;
        RefreshGrenades();
        if (selected == HexcoreUltimate.Grenades)
            return grenadeCharges > 0 && Time.time >= nextGrenadeThrow && CooldownLeft <= 0f;
        return selected != HexcoreUltimate.None && CooldownLeft <= 0f &&
            (selected != HexcoreUltimate.Turret || turretCount < TurretLimit);
    }

    internal bool Cast(HexcoreUltimate selected, Vector2 aimPoint, bool bossMode)
    {
        if (!CanCast(selected)) return false;

        Vector2 origin = muzzle.position;
        Vector2 center = GroundAim(aimPoint);
        Vector2 line = center - origin;
        Vector2 heading = line.normalized;
        switch (selected)
        {
            case HexcoreUltimate.Grenades:
                if (grenadeCharges == GrenadeCapacity) nextGrenadeCharge = Time.time + GrenadeRechargeSeconds;
                grenadeCharges--;
                nextGrenadeThrow = Time.time + 0.18f;
                StartCoroutine(ThrowGrenade(origin, center));
                break;
            case HexcoreUltimate.Meteors:
                readyAt = Time.time + 5f;
                StartCoroutine(MeteorShower(center));
                break;
            case HexcoreUltimate.Turret:
                readyAt = Time.time + 2.4f;
                Vector2 side = new Vector2(-heading.y, heading.x);
                float offset = turretPlacement++ % 2 == 0 ? 0.78f : -0.78f;
                turretCount++;
                StartCoroutine(Turret((Vector2)player.position + side * offset, heading, bossMode));
                break;
            case HexcoreUltimate.Rail:
                readyAt = Time.time + 9f;
                breathing = true;
                StartCoroutine(FireBreath());
                break;
            case HexcoreUltimate.Cage:
                readyAt = Time.time + 5f;
                StartCoroutine(Cage(center, heading));
                break;
            default:
                return false;
        }
        return true;
    }

    internal Vector2 GroundAim(Vector2 point)
    {
        Vector2 origin = muzzle.position;
        Vector2 delta = point - origin;
        Vector2 heading = delta.sqrMagnitude > 0.01f ? delta.normalized : (Vector2)muzzle.right;
        return origin + heading * Mathf.Clamp(delta.magnitude, 2f, 8f);
    }

    internal void ResetEffects()
    {
        StopAllCoroutines();
        foreach (Transform child in visuals)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        readyAt = 0f;
        breathing = false;
        grenadeCharges = GrenadeCapacity;
        nextGrenadeCharge = nextGrenadeThrow = 0f;
        turretCount = 0;
        turretPlacement = 0;
        damaged.Clear();
        lineHits.Clear();
    }

    private IEnumerator ThrowGrenade(Vector2 origin, Vector2 impact)
    {
        const float flight = 0.66f;
        var sprite = Visual("Hexcore grenade", origin, Vector2.right, 1f, 1f, 10);
        var clip = GetGrenadeFrames();
        for (float elapsed = 0f; elapsed < flight; elapsed += Time.deltaTime)
        {
            float progress = Mathf.Clamp01(elapsed / flight);
            Vector2 position = Vector2.Lerp(origin, impact, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.85f;
            sprite.transform.position = position;
            sprite.sprite = clip[Mathf.Min(48, Mathf.FloorToInt(progress * 49f))];
            yield return null;
        }

        sprite.name = "Hexcore grenade blast";
        sprite.transform.position = impact;
        DamageArea(impact, 1.30f, 38);
        const int explosionStart = 49;
        const int explosionEnd = 118;
        const float explosionFps = 50f;
        for (float elapsed = 0f; elapsed < (explosionEnd - explosionStart) / explosionFps; elapsed += Time.deltaTime)
        {
            int frame = explosionStart + Mathf.FloorToInt(elapsed * explosionFps);
            sprite.sprite = clip[Mathf.Min(explosionEnd - 1, frame)];
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private Sprite[] GetGrenadeFrames()
    {
        if (grenadeFrames != null) return grenadeFrames;
        grenadeFrames = new Sprite[118];
        for (int i = 0; i < grenadeFrames.Length; i++)
        {
            var rect = new Rect(i % 10 * 300, grenadeAtlas.height - (i / 10 + 1) * 200, 300, 200);
            Vector2 center = i < GrenadeCenters.Length ? GrenadeCenters[i] : new Vector2(200f, 162f);
            var pivot = new Vector2(center.x / 300f, 1f - center.y / 200f);
            grenadeFrames[i] = Sprite.Create(grenadeAtlas, rect, pivot, 40f, 0, SpriteMeshType.FullRect);
        }
        return grenadeFrames;
    }

    private IEnumerator MeteorShower(Vector2 center)
    {
        // Keep every strike within one region around the point selected when R was pressed.
        Vector2[] offsets =
        {
            Vector2.zero, new Vector2(-0.72f, 0.48f), new Vector2(0.68f, -0.44f),
            new Vector2(-0.54f, -0.66f), new Vector2(0.62f, 0.70f)
        };
        for (int i = 0; i < offsets.Length; i++)
        {
            StartCoroutine(MeteorStrike(center + offsets[i]));
            yield return new WaitForSeconds(0.22f);
        }
    }

    private IEnumerator MeteorStrike(Vector2 impact)
    {
        const float fps = 40f;
        const int impactFrame = 19;
        const int count = 144;
        Sprite[] clip = GetMeteorFrames();
        var sprite = Visual("Hexcore meteor", impact, Vector2.right, 1f, 1f, 10);
        bool hit = false;
        for (float elapsed = 0f; elapsed < count / fps; elapsed += Time.deltaTime)
        {
            int frame = Mathf.Min(count - 1, Mathf.FloorToInt(elapsed * fps));
            sprite.sprite = clip[frame];
            if (!hit && frame >= impactFrame)
            {
                hit = true;
                sprite.name = "Hexcore meteor impact";
                DamageArea(impact, 1.36f, 47);
            }
            sprite.color = new Color(1f, 1f, 1f, Mathf.Clamp01((count - frame) / 12f));
            yield return null;
        }
        // Preserve damage even if a long frame skips the contact frame.
        if (!hit) DamageArea(impact, 1.36f, 47);
        Destroy(sprite.gameObject);
    }

    private Sprite[] GetMeteorFrames()
    {
        if (meteorFrames != null) return meteorFrames;
        meteorFrames = new Sprite[144];
        for (int i = 0; i < meteorFrames.Length; i++)
        {
            var rect = new Rect(i % 12 * 272, meteorAtlas.height - (i / 12 + 1) * 232, 272, 232);
            // Ground contact stays fixed while the artwork moves from the upper left.
            var pivot = new Vector2(150f / 272f, 1f - 208f / 232f);
            meteorFrames[i] = Sprite.Create(meteorAtlas, rect, pivot, 60f, 0, SpriteMeshType.FullRect);
        }
        return meteorFrames;
    }

    private IEnumerator Turret(Vector2 position, Vector2 heading, bool bossMode)
    {
        const float size = 2.25f;
        const float shotInterval = 0.60f;
        var layers = GetTurretSprites();
        var stand = Visual("Hexcore turret", position, Vector2.right, size, size, 7);
        stand.sprite = layers[0];
        var barrel = TurretLayer("Turret barrel", stand.transform, layers[1], new Vector2(0f, 0.098f), 8);
        var groundRing = TurretLayer("Turret ground ring", stand.transform, layers[2], Vector2.zero, 6);
        var flash = TurretLayer("Turret muzzle flash", barrel.transform, layers[3], new Vector2(0.345f, 0f), 9);
        barrel.transform.localRotation = Rotation(heading);
        flash.enabled = false;

        yield return AnimateTurretBody(stand, barrel, groundRing, position, size, deploying: true);

        float expiresAt = Time.time + TurretLifetime;
        float nextRound = Time.time;
        float lastRound = float.NegativeInfinity;
        while (Time.time < expiresAt)
        {
            EnemyHealth target = FindTurretTarget(position, bossMode);
            if (target != null)
            {
                Vector2 delta = (Vector2)target.transform.position - (Vector2)barrel.transform.position;
                if (delta.sqrMagnitude > 0.001f) heading = delta.normalized;
            }

            barrel.transform.localRotation = Rotation(heading);
            if (target != null && Time.time >= nextRound)
            {
                weapon.FireTurretRound(flash.transform.position, heading);
                lastRound = Time.time;
                nextRound = Time.time + shotInterval;
            }

            float recoil = 1f - Mathf.Clamp01((Time.time - lastRound) / 0.18f);
            barrel.transform.localPosition = new Vector2(0f, 0.098f) - heading * (0.036f * recoil);
            flash.enabled = recoil > 0.42f;
            flash.color = new Color(1f, 1f, 1f, recoil);
            flash.transform.localScale = Vector3.one * Mathf.Lerp(0.62f, 1.12f, recoil);
            groundRing.color = new Color(1f, 1f, 1f, 0.72f + 0.22f * Mathf.Sin(Time.time * 7f));
            yield return null;
        }

        flash.enabled = false;
        barrel.transform.localPosition = new Vector2(0f, 0.098f);
        yield return AnimateTurretBody(stand, barrel, groundRing, position, size, deploying: false);
        Destroy(stand.gameObject);
        turretCount--;
    }

    private IEnumerator AnimateTurretBody(SpriteRenderer stand, SpriteRenderer barrel, SpriteRenderer groundRing,
        Vector2 position, float size, bool deploying)
    {
        float duration = deploying ? 0.70f : 0.55f;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float visibility = deploying ? progress : 1f - progress;
            stand.transform.position = position + Vector2.up * (0.34f * (1f - visibility));
            stand.transform.localScale = Vector3.one * (size * Mathf.Lerp(0.65f, 1f, visibility));
            stand.color = barrel.color = new Color(1f, 1f, 1f, visibility);
            groundRing.transform.localScale = Vector3.one * Mathf.Lerp(1.22f, 1f, visibility);
            groundRing.color = new Color(1f, 1f, 1f, visibility);
            yield return null;
        }

        if (!deploying) yield break;
        stand.transform.position = position;
        stand.transform.localScale = Vector3.one * size;
        groundRing.transform.localScale = Vector3.one;
        stand.color = barrel.color = groundRing.color = Color.white;
    }

    private SpriteRenderer TurretLayer(string label, Transform parent, Sprite sprite, Vector2 offset, int order)
    {
        var obj = new GameObject(label);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = offset;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sharedMaterial = material;
        renderer.sortingOrder = order;
        return renderer;
    }

    private Sprite[] GetTurretSprites()
    {
        if (turretSprites != null) return turretSprites;
        turretSprites = new Sprite[4];
        for (int i = 0; i < turretSprites.Length; i++)
        {
            var rect = new Rect(i % 2 * 256, (1 - i / 2) * 256, 256, 256);
            turretSprites[i] = Sprite.Create(turretAtlas, rect, Vector2.one * 0.5f, 256f, 0, SpriteMeshType.FullRect);
        }
        return turretSprites;
    }

    private EnemyHealth FindTurretTarget(Vector2 position, bool bossMode)
    {
        float bestDistance = TurretRange * TurretRange;
        if (bossMode && Alive(targets[0]) &&
            ((Vector2)targets[0].transform.position - position).sqrMagnitude <= bestDistance)
            return targets[0];

        EnemyHealth nearest = null;
        foreach (EnemyHealth enemy in targets)
        {
            if (!Alive(enemy)) continue;
            float distance = ((Vector2)enemy.transform.position - position).sqrMagnitude;
            if (distance > bestDistance) continue;
            bestDistance = distance;
            nearest = enemy;
        }
        return nearest;
    }

    private IEnumerator FireBreath()
    {
        const float duration = 5f;
        const float reach = 4.4f;
        const float tickInterval = 0.25f;
        var fx = Visual("Hexcore fire breath", muzzle.position, muzzle.right, 1f, 1f, 10);
        fx.sharedMaterial = newArt.NoGlowMaterial;
        float nextTick = 0f;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            Vector2 origin = muzzle.position;
            Vector2 heading = muzzle.right;
            fx.transform.SetPositionAndRotation(origin, Rotation(heading));
            fx.sprite = newArt.Loop(GunNewSkillArt.Clip.Breath, elapsed, 30f, 0, 45);
            fx.color = new Color(1f, 1f, 1f, Mathf.Min(1f, (duration - elapsed) / 0.2f));
            if (elapsed >= nextTick)
            {
                foreach (EnemyHealth enemy in targets)
                {
                    if (!Alive(enemy)) continue;
                    Vector2 delta = (Vector2)enemy.transform.position - origin;
                    float along = Vector2.Dot(delta, heading);
                    float across = Mathf.Abs(Vector2.Dot(delta, new Vector2(-heading.y, heading.x)));
                    if (along >= 0f && along <= reach && across <= 0.18f + along * 0.27f)
                        enemy.TakeDamage(9);
                }
                nextTick += tickInterval;
            }
            yield return null;
        }
        Destroy(fx.gameObject);
        breathing = false;
    }

    private IEnumerator Cage(Vector2 center, Vector2 heading)
    {
        const float radius = 2.6f;
        const float radiusToSpriteSize = 1f;
        var field = Visual("Hexcore cage", center, heading, radiusToSpriteSize, radiusToSpriteSize, 3);
        float start = Time.time;
        float nextPulse = start + 0.44f;
        int pulses = 0;
        while (pulses < 6)
        {
            field.sprite = newArt.Loop(GunNewSkillArt.Clip.CageField, Time.time - start, 30f, 0, 48);
            if (Time.time >= nextPulse)
            {
                damaged.Clear();
                float angle = (pulses % 3) * 60f;
                Vector2 edge = Quaternion.Euler(0f, 0f, angle) * heading * radius;
                CagePulse(center + edge, center);
                CagePulse(center - edge, center);
                pulses++;
                nextPulse = Time.time + 0.28f;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.30f);
        Destroy(field.gameObject);
        StartCoroutine(PixelAt("Hexcore cage collapse", GunNewSkillArt.Clip.CageCollapse,
            center, Vector2.right, 48, 30f));
    }

    private void CagePulse(Vector2 node, Vector2 center)
    {
        Vector2 heading = (center - node).normalized;
        StartCoroutine(PixelAt("Hexcore cage node", GunNewSkillArt.Clip.CagePulse, node, heading, 14, 48f));
        StartCoroutine(PixelAt("Hexcore cage impact", GunNewSkillArt.Clip.Impact, center, Vector2.right, 36, 48f));
        DamageLine(node, center, 0.33f, 27);
    }

    private IEnumerator PixelAt(string label, GunNewSkillArt.Clip clip, Vector2 position,
        Vector2 heading, int count, float fps)
    {
        var sprite = Visual(label, position, heading, 1f, 1f, 9);
        for (float elapsed = 0f; elapsed < count / fps; elapsed += Time.deltaTime)
        {
            sprite.sprite = newArt.Frame(clip, Mathf.Min(count - 1, Mathf.FloorToInt(elapsed * fps)));
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private void DamageArea(Vector2 center, float radius, int damage)
    {
        foreach (EnemyHealth enemy in targets)
        {
            if (!Alive(enemy)) continue;
            Vector2 closest = enemy.TryGetComponent<Collider2D>(out var hitbox)
                ? hitbox.ClosestPoint(center) : (Vector2)enemy.transform.position;
            if ((closest - center).sqrMagnitude <= radius * radius)
                enemy.TakeDamage(damage);
        }
    }

    private void DamageLine(Vector2 start, Vector2 end, float radius, int damage)
    {
        Vector2 segment = end - start;
        var filter = new ContactFilter2D { useTriggers = true };
        lineHits.Clear();
        Physics2D.CircleCast(start, radius, segment.normalized, filter, lineHits, segment.magnitude);
        foreach (var hit in lineHits)
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (Alive(enemy) && damaged.Add(enemy)) enemy.TakeDamage(damage);
        }
    }

    private SpriteRenderer Visual(string label, Vector2 position, Vector2 heading, float width, float height, int layer)
    {
        var obj = new GameObject(label);
        obj.transform.SetParent(visuals, false);
        obj.transform.SetPositionAndRotation(position, Rotation(heading));
        obj.transform.localScale = new Vector3(width, height, 1f);
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sharedMaterial = material;
        sprite.sortingOrder = layer;
        return sprite;
    }

    private IEnumerator PlayAt(string label, Art art, Vector2 position, Vector2 heading, float width, float height, int fps)
    {
        var sprite = Visual(label, position, heading, width, height, 9);
        yield return Animate(sprite, art, FramesPerClip / (float)fps);
        Destroy(sprite.gameObject);
    }

    private IEnumerator Animate(SpriteRenderer sprite, Art art, float duration)
    {
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            sprite.sprite = ClipProgress(art, elapsed / duration);
            yield return null;
        }
        sprite.sprite = ClipProgress(art, 1f);
    }

    private Sprite Frame(Art art, float elapsed, float fps) =>
        GetFrame(art, Mathf.FloorToInt(elapsed * fps) % FramesPerClip);

    private Sprite ClipProgress(Art art, float progress) =>
        GetFrame(art, Mathf.Min(FramesPerClip - 1, Mathf.FloorToInt(Mathf.Clamp01(progress) * FramesPerClip)));

    private Sprite GetFrame(Art art, int frame)
    {
        int index = (int)art;
        if (frames[index] == null)
        {
            var clip = new Sprite[FramesPerClip];
            for (int i = 0; i < clip.Length; i++)
            {
                var rect = new Rect(i % 8 * 256, (3 - i / 8) * 256, 256, 256);
                clip[i] = Sprite.Create(sheets[index], rect, Vector2.one * 0.5f, 256f, 0, SpriteMeshType.FullRect);
            }
            frames[index] = clip;
        }
        return frames[index][frame];
    }

    private static bool Alive(EnemyHealth enemy) => enemy != null && enemy.CurrentHealth > 0;
    private static Quaternion Rotation(Vector2 heading) =>
        Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, heading));

    private void OnDestroy()
    {
        if (meteorFrames != null)
            foreach (Sprite sprite in meteorFrames) Destroy(sprite);
        if (grenadeFrames != null)
            foreach (Sprite sprite in grenadeFrames) Destroy(sprite);
        if (turretSprites != null)
            foreach (Sprite sprite in turretSprites) Destroy(sprite);
        foreach (Sprite[] clip in frames)
            if (clip != null)
                foreach (Sprite sprite in clip) Destroy(sprite);
    }
}
