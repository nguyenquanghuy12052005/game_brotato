using System.Collections;
using UnityEngine;

public sealed class GunSkillWeapon : MonoBehaviour
{
    private const float BurstSpacing = 0.32f;
    private const int BaseDamage = 10;
    private Sprite bulletSprite;
    private Sprite piercingSprite;
    private Sprite[] pistolFlash;
    private Sprite[] pistolTracer;
    private Sprite[] shotgunFlash;
    private Sprite[] smgFlash;
    private Sprite[] branchTracer;
    private Sprite[] iceTracer;
    private Sprite[] echoLaser;
    private Sprite[] plasmaTracer;
    private Sprite[] iceMuzzle;
    private GunHexcoreUltimate grenadeArt;
    private Sprite[] rocketFlash;
    private Sprite[] rocketFlight;
    private GunNewSkillArt rocketArt;
    private Material material;
    private Transform muzzle;
    private Transform effects;
    private GunSkill skill;
    private GunBranch branch;
    private Vector3Int ranks;
    private GunLevel11 level11;
    private GunCoreTwo coreTwo;
    private GunCoreTwo.Volley coreBurstVolley;
    private GunLevel11.Volley burstVolley;
    private int burstRemaining;
    private Vector2 burstDirection;
    private float nextShot;
    private float nextBurstRound;

    public void Setup(Transform firePoint, Transform effectsRoot, Material spriteMaterial,
        Sprite basic, Sprite piercing)
    {
        muzzle = firePoint;
        effects = effectsRoot;
        material = spriteMaterial;
        bulletSprite = basic;
        piercingSprite = piercing;
    }

    internal void SetPistolShots(Texture2D atlas)
    {
        if (atlas == null) return;

        // The source sheet records a full shot. Keep the flash at the muzzle
        // and align the moving tracer frames by their bright leading tips.
        pistolFlash = new[]
        {
            PistolFrame(atlas, 5, 110, 15, 49, 51, 144, 40),
            PistolFrame(atlas, 6, 110, 15, 49, 51, 144, 40),
            PistolFrame(atlas, 8, 110, 15, 76, 51, 144, 40),
            PistolFrame(atlas, 9, 110, 15, 76, 51, 144, 40),
            PistolFrame(atlas, 10, 110, 15, 76, 51, 144, 40),
            PistolFrame(atlas, 11, 110, 15, 76, 51, 144, 40),
            PistolFrame(atlas, 12, 110, 15, 76, 51, 144, 40)
        };
        pistolTracer = new[]
        {
            PistolFrame(atlas, 5, 158, 24, 39, 34, 177, 41),
            PistolFrame(atlas, 6, 196, 24, 39, 34, 215, 41),
            PistolFrame(atlas, 7, 234, 24, 39, 34, 253, 41)
        };
    }

    private static Sprite PistolFrame(Texture2D atlas, int frame, int x, int y,
        int width, int height, int pivotX, int pivotY)
    {
        const int cellWidth = 378, cellHeight = 76, columns = 4;
        var rect = new Rect(frame % columns * cellWidth + x,
            atlas.height - frame / columns * cellHeight - y - height, width, height);
        var pivot = new Vector2((pivotX - x) / (float)width, 1f - (pivotY - y) / (float)height);
        return Sprite.Create(atlas, rect, pivot, 80f, 0, SpriteMeshType.FullRect);
    }

    internal void SetPlasmaArt(Texture2D atlas)
    {
        plasmaTracer = SliceFrames(atlas, 144, 60, 24, new Vector2(108, 30), 95f);
    }

    internal void SetEchoArt(Texture2D atlas)
    {
        echoLaser = SliceFrames(atlas, 128, 32, 15, new Vector2(109, 16), 110f,
            new Rect(56, 0, 72, 32));
    }

    internal void SetBranchArt(Texture2D shotgun, Texture2D smg, Texture2D tracer)
    {
        shotgunFlash = SliceFrames(shotgun, 160, 88, 40, new Vector2(50, 53), 100f);
        // This sheet repeats the firing cycle every three cells.
        smgFlash = SliceFrames(smg, 112, 72, 3, new Vector2(49, 39), 100f);
        // Use the pistol's pixel scale and retain the tracer head with its baked glow.
        branchTracer = SliceFrames(tracer, 200, 40, 18, new Vector2(187, 19), 80f,
            new Rect(160, 2, 40, 34));
    }

    internal void SetSmgUpgradeArt(Texture2D iceLance, Texture2D frostBreath, GunHexcoreUltimate grenadeSource)
    {
        // Keep the lance tip and its blue glow at the same visible scale as the standard tracer.
        iceTracer = SliceFrames(iceLance, 176, 64, 24, new Vector2(143, 32), 112f,
            new Rect(106, 13, 56, 38));
        // Crop to the short blue flash at the muzzle, before the plume of the continuous breath.
        iceMuzzle = SliceFrames(frostBreath, 184, 104, 3, new Vector2(12, 50), 96f,
            new Rect(0, 30, 42, 40));
        grenadeArt = grenadeSource;
    }

    internal void SetRocketArt(Texture2D cannonball, GunNewSkillArt importedArt)
    {
        rocketArt = importedArt;
        if (cannonball == null) return;
        const int width = 208, height = 116, columns = 9;
        // Frames 4–14 contain the old projectile to the right of the smoke.
        int[] muzzleWidths = { 88, 98, 109, 119, 129, 140, 150, 161, 171, 182, 192 };
        rocketFlash = new Sprite[108];
        for (int i = 0; i < rocketFlash.Length; i++)
        {
            int visibleWidth = i >= 4 && i <= 14 ? muzzleWidths[i - 4] : width;
            var rect = new Rect(i % columns * width, cannonball.height - (i / columns + 1) * height,
                visibleWidth, height);
            var pivot = new Vector2(57f / visibleWidth, 1f - 87f / height);
            rocketFlash[i] = Sprite.Create(cannonball, rect, pivot, 100f, 0, SpriteMeshType.FullRect);
        }

        // The shared atlas cache owns these sprites. Use only the frames with a clear rocket in flight.
        rocketFlight = new Sprite[9];
        for (int i = 0; i < rocketFlight.Length; i++)
            rocketFlight[i] = importedArt.Frame(GunNewSkillArt.Clip.Rocket, 25 + i);
    }

    private static Sprite[] SliceFrames(Texture2D atlas, int width, int height,
        int count, Vector2 anchor, float pixelsPerUnit, Rect? crop = null)
    {
        if (atlas == null) return null;
        int columns = atlas.width / width;
        var frames = new Sprite[count];
        var region = crop ?? new Rect(0, 0, width, height);
        var pivot = new Vector2((anchor.x - region.x) / region.width, 1f - (anchor.y - region.y) / region.height);
        for (int i = 0; i < count; i++)
        {
            var rect = new Rect(i % columns * width + region.x,
                atlas.height - i / columns * height - region.y - region.height, region.width, region.height);
            frames[i] = Sprite.Create(atlas, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect);
        }
        return frames;
    }

    private IEnumerator PlayMuzzleFlash(string label, Sprite[] frames, Vector2 position, Vector2 direction, float fps, bool fade = false)
    {
        var flash = new GameObject(label);
        flash.transform.SetParent(effects);
        flash.transform.SetPositionAndRotation(position,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction)));
        var renderer = AddSprite(flash, frames[0], 7);
        for (float elapsed = 0f; renderer != null && elapsed < frames.Length / fps; elapsed += Time.deltaTime)
        {
            renderer.sprite = frames[Mathf.FloorToInt(elapsed * fps)];
            if (fade) renderer.color = new Color(1f, 1f, 1f, 1f - elapsed * fps / frames.Length);
            yield return null;
        }
        if (flash != null) Destroy(flash);
    }

    private void OnDestroy()
    {
        DestroyFrames(pistolFlash);
        DestroyFrames(pistolTracer);
        DestroyFrames(shotgunFlash);
        DestroyFrames(smgFlash);
        DestroyFrames(branchTracer);
        DestroyFrames(iceTracer);
        DestroyFrames(echoLaser);
        DestroyFrames(plasmaTracer);
        DestroyFrames(iceMuzzle);
        DestroyFrames(rocketFlash);
    }

    private static void DestroyFrames(Sprite[] frames)
    {
        if (frames == null) return;
        foreach (var sprite in frames) Destroy(sprite);
    }

    public void Select(GunSkill effect, GunBranch firingMode, Vector3Int upgradeRanks)
    {
        if (effect == skill && firingMode == branch && upgradeRanks == ranks) return;
        skill = effect;
        branch = firingMode;
        ranks = upgradeRanks;
        ResetShots();
    }

    internal void SetCoreTwo(GunCoreTwo core) => coreTwo = core;

    internal void FireCounter(Vector2 position, Vector2 aim, int count, float angle)
    {
        for (int i = 0; i < count; i++)
        {
            float rotation = Mathf.Lerp(-angle / 2f, angle / 2f, i / (float)(count - 1));
            Vector2 heading = Quaternion.Euler(0f, 0f, rotation) * aim;
            SpawnProjectile(position, heading, 8, default, new Color(1f, 0.65f, 0.2f));
        }
    }

    internal Vector2 MuzzlePosition => muzzle.position;

    internal void SetLevel11(GunLevel11 prototype) => level11 = prototype;
    internal int DamageAgainst(EnemyHealth enemy, int damage) =>
        level11 != null ? level11.DamageAgainst(enemy, damage) : damage;
    internal void RegisterHit(GunLevel11.Volley volley, EnemyHealth enemy) => level11?.Hit(volley, enemy);

    internal void FireEcho(Vector2 position, Vector2 heading) =>
        SpawnProjectile(position, heading, 6, default, projectileName: "Echo laser bolt", tracerFrames: echoLaser);

    public void ResetShots()
    {
        burstRemaining = 0;
        burstVolley = null;
        coreBurstVolley = null;
        nextShot = Time.time;
    }

    public void Tick(bool fireRequested, Vector2? aimPoint = null)
    {
        if (burstRemaining > 0)
        {
            if (Time.time >= nextBurstRound)
            {
                FireRound(burstDirection, burstVolley, coreBurstVolley);
                burstRemaining--;
                nextBurstRound = Time.time + BurstSpacing;
                if (burstRemaining == 0)
                {
                    if (ranks.y > 0 && grenadeArt != null) ThrowSmgGrenades(aimPoint);
                    nextShot = Time.time + 1.20f;
                }
            }
            return;
        }

        if (!fireRequested || Time.time < nextShot) return;
        Vector2 heading = muzzle.right;
        switch (branch)
        {
            case GunBranch.Spread:
                int count = 3 + ranks.x;
                var spreadVolley = level11?.BeginVolley(muzzle.position, heading, count);
                var coreSpreadVolley = coreTwo?.BeginVolley();
                float halfAngle = (60f - 15f * ranks.y) * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    float angle = Mathf.Lerp(-halfAngle, halfAngle, i / (float)(count - 1));
                    FireRound(Quaternion.Euler(0f, 0f, angle) * heading, spreadVolley, coreSpreadVolley);
                }
                if (shotgunFlash != null)
                    StartCoroutine(PlayMuzzleFlash("Shotgun muzzle flash", shotgunFlash, muzzle.position, heading, 60f));
                nextShot = Time.time + 2.60f;
                break;
            case GunBranch.Burst:
                int burstCount = 5 + 2 * ranks.z;
                burstVolley = level11?.BeginVolley(muzzle.position, heading, burstCount);
                coreBurstVolley = coreTwo?.BeginVolley();
                FireRound(heading, burstVolley, coreBurstVolley);
                burstDirection = heading;
                burstRemaining = burstCount - 1;
                nextBurstRound = Time.time + BurstSpacing;
                break;
            case GunBranch.Piercing:
                int rocketCount = 1 + ranks.z;
                var rocketVolley = level11?.BeginVolley(muzzle.position, heading, rocketCount);
                var coreRocketVolley = coreTwo?.BeginVolley();
                for (int i = 0; i < rocketCount; i++)
                {
                    float angle = (i - (rocketCount - 1) * 0.5f) * 5f;
                    FireRound(Quaternion.Euler(0f, 0f, angle) * heading, rocketVolley, coreRocketVolley);
                }
                if (rocketFlash != null)
                    StartCoroutine(PlayMuzzleFlash("Rocket muzzle flash", rocketFlash, muzzle.position, heading, 90f));
                nextShot = Time.time + 0.80f;
                break;
            default:
                FireRound(heading, level11?.BeginVolley(muzzle.position, heading, 1), coreTwo?.BeginVolley());
                nextShot = Time.time + 0.20f;
                break;
        }
    }

    private void FireRound(Vector2 heading, GunLevel11.Volley volley, GunCoreTwo.Volley coreVolley)
    {
        bool isBurst = branch == GunBranch.Burst;
        bool isRocket = branch == GunBranch.Piercing;
        bool isPlasma = isBurst && coreTwo != null && coreTwo.PlasmaRound(coreVolley);
        bool isIce = isBurst && ranks.x > 0;
        var effectsOnHit = new GunShotEffects
        {
            Volley = volley,
            CoreVolley = coreVolley,
            Plasma = isPlasma,
            Piercing = skill == GunSkill.Piercing,
            Ricochets = skill == GunSkill.Ricochet ? 1 : 0,
            Fragments = skill == GunSkill.DeathScatter ? 3 : 0,
            SlowFraction = isIce ? 0.15f * ranks.x : 0f,
            Knockback = branch == GunBranch.Spread ? 0.25f * ranks.z : isRocket ? 0.25f * ranks.y : 0f,
            BlastDamage = isRocket && ranks.x > 0 ? 5 + 3 * (ranks.x - 1) : 0,
            BlastRadius = isRocket && ranks.x > 0 ? 0.8f + 0.3f * (ranks.x - 1) : 0f
        };
        Sprite[] tracer = branch switch
        {
            GunBranch.Basic => pistolTracer,
            GunBranch.Piercing => rocketFlight,
            GunBranch.Burst when isPlasma && plasmaTracer != null => plasmaTracer,
            GunBranch.Burst when isIce && iceTracer != null => iceTracer,
            _ => branchTracer
        };
        SpawnProjectile(muzzle.position, heading, BaseDamage,
            effectsOnHit, tracerFrames: tracer, fallbackSprite: isRocket ? piercingSprite : bulletSprite);
        if (branch == GunBranch.Basic && pistolFlash != null)
            StartCoroutine(PlayMuzzleFlash("Pistol muzzle flash", pistolFlash, muzzle.position, heading, 48f));
        if (isIce && !isPlasma && iceMuzzle != null)
            StartCoroutine(PlayMuzzleFlash("SMG frost muzzle", iceMuzzle, muzzle.position, heading, 40f, fade: true));
        else if (isBurst && smgFlash != null)
            StartCoroutine(PlayMuzzleFlash("SMG muzzle flash", smgFlash, muzzle.position, heading, 30f));
    }

    public void ScatterFrom(Vector2 position, int count)
    {
        float startAngle = Random.Range(0f, 360f);
        float spacing = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + i * spacing + Random.Range(-spacing * 0.29f, spacing * 0.29f);
            Vector2 heading = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            SpawnProjectile(position, heading, BaseDamage / 2, default,
                projectileName: "Scatter fragment", tracerFrames: pistolTracer);
        }
        ShowImpact(position, new Color(1f, 0.75f, 0.30f));
    }

    private void SpawnProjectile(Vector2 position, Vector2 direction, int damage, GunShotEffects effectsOnHit,
        Color? tint = null, string projectileName = null, Sprite[] tracerFrames = null, Sprite fallbackSprite = null)
    {
        var projectile = new GameObject(projectileName ?? (tint.HasValue ? "Scatter fragment" : "Gun skill projectile"));
        projectile.transform.SetParent(effects);
        projectile.transform.SetPositionAndRotation(position,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction)));

        // Scale the artwork separately from the physics collider.
        var visual = new GameObject("Projectile art");
        visual.transform.SetParent(projectile.transform, false);
        var artwork = AddSprite(visual, fallbackSprite != null ? fallbackSprite : bulletSprite, 5);
        if (tint.HasValue) artwork.color = tint.Value;
        Vector2 size = artwork.sprite.bounds.size;
        visual.transform.localScale = new Vector3(0.38f / size.x, 0.12f / size.y, 1f);

        if (tracerFrames != null)
        {
            artwork.sprite = tracerFrames[0];
            float tracerScale = tracerFrames == plasmaTracer ? 3f : tracerFrames == rocketFlight ? 0.55f : 1f;
            visual.transform.localScale = new Vector3(tracerScale, tracerScale, 1f);
        }

        var body = projectile.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.12f;

        projectile.AddComponent<GunSkillProjectile>().Launch(this, direction, damage, effectsOnHit, coreTwo, artwork, tracerFrames);
    }

    internal void RocketBlast(Vector2 position, float radius, int blastDamage, EnemyHealth source)
    {
        DamageNearby(position, radius, blastDamage, source);
        if (rocketArt == null) return;
        StartCoroutine(AnimateRocketBlast(position, radius));
    }

    private void DamageNearby(Vector2 position, float radius, int damage, EnemyHealth excluded = null)
    {
        float radiusSquared = radius * radius;
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            if (enemy != excluded && enemy.CurrentHealth > 0 &&
                ((Vector2)enemy.transform.position - position).sqrMagnitude <= radiusSquared)
                enemy.TakeDamage(damage);
    }

    private void ThrowSmgGrenades(Vector2? aimPoint)
    {
        Vector2 origin = muzzle.position;
        Vector2 aim = aimPoint.HasValue ? grenadeArt.GroundAim(aimPoint.Value) : origin + burstDirection * 4f;
        Vector2 axis = (aim - origin).normalized;
        Vector2 side = new Vector2(-axis.y, axis.x);
        for (int i = 0; i < ranks.y; i++)
        {
            float offset = (i - (ranks.y - 1) * 0.5f) * 0.75f;
            StartCoroutine(ThrowSmgGrenade(origin, aim + side * offset));
        }
    }

    private IEnumerator ThrowSmgGrenade(Vector2 origin, Vector2 impact)
    {
        const float flight = 0.55f;
        const int blastStart = 49;
        const float blastFps = 70f;
        var frames = grenadeArt.GetGrenadeFrames();
        var grenade = new GameObject("SMG grenade");
        grenade.transform.SetParent(effects);
        grenade.transform.localScale = Vector3.one * 0.8f;
        var renderer = AddSprite(grenade, frames[0], 9);
        for (float elapsed = 0f; renderer != null && elapsed < flight; elapsed += Time.deltaTime)
        {
            float t = elapsed / flight;
            grenade.transform.position = Vector2.Lerp(origin, impact, t) + Vector2.up * (0.75f * Mathf.Sin(t * Mathf.PI));
            renderer.sprite = frames[Mathf.Min(blastStart - 1, Mathf.FloorToInt(t * blastStart))];
            yield return null;
        }
        if (renderer == null || !grenade.activeInHierarchy) yield break;

        grenade.name = "SMG grenade blast";
        grenade.transform.position = impact;
        DamageNearby(impact, 1.15f, 18);
        for (float elapsed = 0f; renderer != null && elapsed < (frames.Length - blastStart) / blastFps; elapsed += Time.deltaTime)
        {
            renderer.sprite = frames[Mathf.Min(frames.Length - 1, blastStart + Mathf.FloorToInt(elapsed * blastFps))];
            yield return null;
        }
        if (grenade != null) Destroy(grenade);
    }

    private IEnumerator AnimateRocketBlast(Vector2 position, float radius)
    {
        const int firstFrame = 34, lastFrame = 169;
        const float fps = 90f;
        const float duration = (lastFrame - firstFrame + 1) / fps;
        var blast = new GameObject("Rocket blast");
        blast.transform.SetParent(effects);
        blast.transform.position = position;
        // The authored explosion spans about two world units at its original scale.
        blast.transform.localScale = Vector3.one * radius;
        var renderer = AddSprite(blast, rocketArt.Frame(GunNewSkillArt.Clip.Rocket, firstFrame), 7);
        for (float elapsed = 0f; renderer != null && elapsed < duration; elapsed += Time.deltaTime)
        {
            renderer.sprite = rocketArt.Progress(GunNewSkillArt.Clip.Rocket, elapsed / duration, firstFrame, lastFrame);
            renderer.color = new Color(1f, 1f, 1f, Mathf.Clamp01((duration - elapsed) / 0.18f));
            yield return null;
        }
        if (blast != null) Destroy(blast);
    }

    public void ShowImpact(Vector2 position, Color color)
    {
        var flash = new GameObject("Skill impact flash");
        flash.transform.SetParent(effects);
        flash.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, 45f));
        var renderer = AddSprite(flash, bulletSprite, 4);
        renderer.color = color;
        float scale = 0.24f / renderer.sprite.bounds.size.x;
        flash.transform.localScale = new Vector3(scale, scale, 1f);
        Destroy(flash, 0.12f);
    }

    private SpriteRenderer AddSprite(GameObject target, Sprite sprite, int sortingOrder)
    {
        var renderer = target.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sharedMaterial = material;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }
}
