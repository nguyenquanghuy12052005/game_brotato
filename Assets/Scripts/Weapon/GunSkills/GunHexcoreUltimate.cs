using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal enum HexcoreUltimate { None, Grenades, Airstrike, Turret, Rail, Cage }

internal sealed class GunHexcoreUltimate : MonoBehaviour
{
    // Texture order matches GunSkillTest's serialized references.
    private enum Art { Grenade, Blast, Helicopter, AirstrikeMark, AirstrikeImpact,
        Charge, Beam, CageField, CageShot, CageCollapse }

    internal const int TurretLimit = 2;
    private const float TurretRange = 7f;
    private const float TurretLifetime = 8f;
    private const int FramesPerClip = 32;
    private readonly Sprite[][] frames = new Sprite[10][];
    private readonly List<RaycastHit2D> lineHits = new List<RaycastHit2D>();
    private readonly HashSet<EnemyHealth> damaged = new HashSet<EnemyHealth>();
    private Texture2D[] sheets;
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

    internal float CooldownLeft => Mathf.Max(0f, readyAt - Time.time);
    internal int TurretCount => turretCount;

    internal void Setup(GunSkillWeapon owner, Transform playerRoot, Transform firePoint,
        Material spriteMaterial, EnemyHealth[] practiceTargets, Texture2D[] textures, Texture2D turretTexture)
    {
        weapon = owner;
        player = playerRoot;
        muzzle = firePoint;
        material = spriteMaterial;
        targets = practiceTargets;
        sheets = textures;
        turretAtlas = turretTexture;
        visuals = new GameObject("Hexcore ultimate effects").transform;
        visuals.SetParent(transform, false);
    }

    internal bool CanCast(HexcoreUltimate selected) =>
        selected != HexcoreUltimate.None && CooldownLeft <= 0f &&
        (selected != HexcoreUltimate.Turret || turretCount < TurretLimit);

    internal bool Cast(HexcoreUltimate selected, Vector2 aimPoint, bool bossMode)
    {
        if (!CanCast(selected)) return false;

        Vector2 origin = muzzle.position;
        Vector2 center = GroundAim(aimPoint);
        Vector2 line = center - origin;
        Vector2 heading = line.normalized;
        float reach = line.magnitude;
        switch (selected)
        {
            case HexcoreUltimate.Grenades:
                readyAt = Time.time + 4f;
                ThrowGrenades(origin, heading, reach);
                break;
            case HexcoreUltimate.Airstrike:
                readyAt = Time.time + 5f;
                StartCoroutine(Airstrike(center, heading));
                break;
            case HexcoreUltimate.Turret:
                readyAt = Time.time + 2.4f;
                Vector2 side = new Vector2(-heading.y, heading.x);
                float offset = turretPlacement++ % 2 == 0 ? 0.78f : -0.78f;
                turretCount++;
                StartCoroutine(Turret((Vector2)player.position + side * offset, heading, bossMode));
                break;
            case HexcoreUltimate.Rail:
                readyAt = Time.time + 4f;
                StartCoroutine(Rail(heading));
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
        turretCount = 0;
        turretPlacement = 0;
        damaged.Clear();
        lineHits.Clear();
    }

    private void ThrowGrenades(Vector2 origin, Vector2 heading, float reach)
    {
        Vector2 side = new Vector2(-heading.y, heading.x);
        for (int i = 0; i < 5; i++)
        {
            float along = (i - 2) * (0.24f + reach * 0.078f);
            float across = (i - 2) * (0.14f + reach * 0.08f);
            Vector2 impact = origin + heading * (reach + along) + side * across;
            StartCoroutine(DropBomb("Hexcore grenade", origin, impact, 0.48f + 0.14f * i,
                0.8f, Art.Blast, "Hexcore grenade blast", 1.26f, 38));
        }
    }

    private IEnumerator DropBomb(string label, Vector2 origin, Vector2 impact, float flight,
        float arcHeight, Art explosion, string explosionName, float radius, int damage)
    {
        var sprite = Visual(label, origin, Vector2.right, 1.65f, 1.65f, 10);
        for (float elapsed = 0f; elapsed < flight; elapsed += Time.deltaTime)
        {
            float progress = elapsed / flight;
            Vector2 position = Vector2.Lerp(origin, impact, progress);
            position.y += Mathf.Sin(Mathf.PI * progress) * arcHeight;
            sprite.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, -elapsed * 540f));
            sprite.sprite = Frame(Art.Grenade, elapsed, 24f);
            yield return null;
        }
        Destroy(sprite.gameObject);
        StartCoroutine(PlayAt(explosionName, explosion, impact, Vector2.right, radius * 2.45f, radius * 2.45f, 36));
        DamageArea(impact, radius, damage);
    }

    private IEnumerator Airstrike(Vector2 center, Vector2 heading)
    {
        var marker = Visual("Airstrike warning", center, heading, 9.2f, 4.15f, 2);
        for (float elapsed = 0f; elapsed < 0.68f; elapsed += Time.deltaTime)
        {
            marker.sprite = Frame(Art.AirstrikeMark, elapsed, 24f);
            yield return null;
        }

        var aircraft = Visual("Hexcore helicopter", center, heading, 3.3f, 3.3f, 12);
        int dropped = 0;
        const float duration = 1.24f;
        for (float elapsed = 0f; ; elapsed += Time.deltaTime)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float distance = Mathf.Lerp(-6.7f, 6.7f, progress);
            Vector2 position = center + heading * distance + Vector2.up * 0.88f;
            aircraft.transform.position = position;
            aircraft.sprite = Frame(Art.Helicopter, elapsed, 32f);
            marker.sprite = Frame(Art.AirstrikeMark, elapsed + 0.68f, 24f);
            marker.color = new Color(1f, 1f, 1f, 1f - progress);
            while (dropped < 5 && distance >= (dropped - 2) * 1.48f)
            {
                Vector2 impact = center + heading * ((dropped - 2) * 1.48f);
                StartCoroutine(DropBomb("Airstrike bomb", position, impact, 0.28f, 0.22f,
                    Art.AirstrikeImpact, "Airstrike impact", 1.36f, 47));
                dropped++;
            }
            if (progress >= 1f) break;
            yield return null;
        }
        Destroy(marker.gameObject);
        Destroy(aircraft.gameObject);
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

    private IEnumerator Rail(Vector2 heading)
    {
        var charge = Visual("Hexcore rail charge", muzzle.position, heading, 2.9f, 2.9f, 10);
        const float delay = 0.78f;
        for (float elapsed = 0f; elapsed < delay; elapsed += Time.deltaTime)
        {
            charge.transform.position = muzzle.position;
            charge.sprite = ClipProgress(Art.Charge, elapsed / delay);
            yield return null;
        }
        Destroy(charge.gameObject);

        Vector2 origin = muzzle.position;
        const float length = 12f;
        StartCoroutine(PlayAt("Hexcore rail beam", Art.Beam, origin + heading * (length * 0.5f), heading,
            length * 256f / 224f, 3.15f, 48));
        damaged.Clear();
        DamageLine(origin, origin + heading * length, 0.32f, 145);
    }

    private IEnumerator Cage(Vector2 center, Vector2 heading)
    {
        const float radius = 2.6f;
        float radiusToSpriteSize = radius * 256f / 89f;
        var field = Visual("Hexcore cage", center, heading, radiusToSpriteSize, radiusToSpriteSize, 3);
        float start = Time.time;
        float nextPulse = start + 0.44f;
        int pulses = 0;
        while (pulses < 6)
        {
            field.sprite = Frame(Art.CageField, Time.time - start, 32f);
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
        StartCoroutine(PlayAt("Hexcore cage collapse", Art.CageCollapse, center, heading,
            radiusToSpriteSize, radiusToSpriteSize, 36));
    }

    private void CagePulse(Vector2 node, Vector2 center)
    {
        Vector2 heading = (center - node).normalized;
        StartCoroutine(PlayAt("Hexcore cage node", Art.CageShot, node, heading, 6.5f, 2.2f, 48));
        DamageLine(node, center, 0.33f, 27);
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
        if (turretSprites != null)
            foreach (Sprite sprite in turretSprites) Destroy(sprite);
        foreach (Sprite[] clip in frames)
            if (clip != null)
                foreach (Sprite sprite in clip) Destroy(sprite);
    }
}
