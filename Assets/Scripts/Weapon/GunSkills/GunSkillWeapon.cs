using System.Collections;
using UnityEngine;

public sealed class GunSkillWeapon : MonoBehaviour
{
    private const float BurstSpacing = 0.32f;
    private const int BaseDamage = 10;
    private Sprite bulletSprite;
    private Sprite piercingSprite;
    private Sprite shockwaveSprite;
    private Sprite[] flameFrames;
    private Material material;
    private Transform muzzle;
    private Transform effects;
    private GunSkill skill;
    private GunBranch branch;
    private Vector3Int ranks;
    private GunLevel11 level11;
    private GunLevel11.Volley burstVolley;
    private int burstRemaining;
    private Vector2 burstDirection;
    private float nextShot;
    private float nextBurstRound;

    public void Setup(Transform firePoint, Transform effectsRoot, Material spriteMaterial,
        Sprite basic, Sprite piercing, Sprite shockwave, Sprite[] flames)
    {
        muzzle = firePoint;
        effects = effectsRoot;
        material = spriteMaterial;
        bulletSprite = basic;
        piercingSprite = piercing;
        shockwaveSprite = shockwave;
        flameFrames = flames;
    }

    public void Select(GunSkill effect, GunBranch firingMode, Vector3Int upgradeRanks)
    {
        if (effect == skill && firingMode == branch && upgradeRanks == ranks) return;
        skill = effect;
        branch = firingMode;
        ranks = upgradeRanks;
        ResetShots();
    }

    internal void SetLevel11(GunLevel11 prototype) => level11 = prototype;
    internal int DamageAgainst(EnemyHealth enemy, int damage) =>
        level11 != null ? level11.DamageAgainst(enemy, damage) : damage;
    internal void RegisterHit(GunLevel11.Volley volley, EnemyHealth enemy) => level11?.Hit(volley, enemy);
    internal void FireEcho(Vector2 position, Vector2 heading) =>
        SpawnProjectile(position, heading, false, 6, default, new Color(0.35f, 0.9f, 1f));

    public void ResetShots()
    {
        burstRemaining = 0;
        burstVolley = null;
        nextShot = Time.time;
    }

    public void Tick(bool fireRequested)
    {
        if (burstRemaining > 0)
        {
            if (Time.time >= nextBurstRound)
            {
                FireRound(burstDirection, burstVolley);
                burstRemaining--;
                nextBurstRound = Time.time + BurstSpacing;
                if (burstRemaining == 0) nextShot = Time.time + 1.20f;
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
                float halfAngle = (60f - 15f * ranks.y) * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    float angle = Mathf.Lerp(-halfAngle, halfAngle, i / (float)(count - 1));
                    FireRound(Quaternion.Euler(0f, 0f, angle) * heading, spreadVolley);
                }
                nextShot = Time.time + 2.60f;
                break;
            case GunBranch.Burst:
                burstVolley = level11?.BeginVolley(muzzle.position, heading, 5 + 2 * ranks.x);
                FireRound(heading, burstVolley);
                burstDirection = heading;
                burstRemaining = 4 + 2 * ranks.x;
                nextBurstRound = Time.time + BurstSpacing;
                break;
            default:
                FireRound(heading, level11?.BeginVolley(muzzle.position, heading, 1));
                nextShot = Time.time + (branch == GunBranch.Piercing ? 0.80f : 0.20f);
                break;
        }
    }

    private void FireRound(Vector2 heading, GunLevel11.Volley volley)
    {
        bool isBurst = branch == GunBranch.Burst;
        bool isPiercing = branch == GunBranch.Piercing;
        int scatterRank = isBurst ? ranks.z : 0;
        var effectsOnHit = new GunShotEffects
        {
            Volley = volley,
            Piercing = isPiercing,
            Ricochets = Mathf.Max(skill == GunSkill.Ricochet ? 1 : 0, isBurst ? ranks.y : 0),
            Fragments = skill == GunSkill.DeathScatter || scatterRank > 0 ? 3 + scatterRank : 0,
            Knockback = branch == GunBranch.Spread ? 0.25f * ranks.z : 0f,
            DamagePerPierce = isPiercing ? 3 * ranks.x : 0,
            Slow = isPiercing ? 0.10f * ranks.y : 0f,
            ShockwaveRadius = isPiercing && ranks.z > 0 ? 0.8f + 0.3f * (ranks.z - 1) : 0f
        };
        SpawnProjectile(muzzle.position, heading, skill == GunSkill.WeakPoint, BaseDamage, effectsOnHit);
    }

    public Sprite GetFlameFrame(float elapsed)
    {
        return flameFrames[Mathf.FloorToInt(elapsed * 12f) % flameFrames.Length];
    }

    public void ScatterFrom(Vector2 position, int count)
    {
        float startAngle = Random.Range(0f, 360f);
        float spacing = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + i * spacing + Random.Range(-spacing * 0.29f, spacing * 0.29f);
            Vector2 heading = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            SpawnProjectile(position, heading, false, BaseDamage / 2, default,
                new Color(1f, 0.75f, 0.30f));
        }
        ShowImpact(position, new Color(1f, 0.75f, 0.30f));
    }

    private void SpawnProjectile(Vector2 position, Vector2 direction, bool weakPoint,
        int damage, GunShotEffects effectsOnHit, Color? tint = null)
    {
        var projectile = new GameObject(tint.HasValue ? "Scatter fragment" : "Gun skill projectile");
        projectile.transform.SetParent(effects);
        projectile.transform.SetPositionAndRotation(position,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction)));

        // Scale the artwork separately from the physics collider.
        var visual = new GameObject("Projectile art");
        visual.transform.SetParent(projectile.transform, false);
        var artwork = AddSprite(visual, effectsOnHit.Piercing ? piercingSprite : bulletSprite, 5);
        if (tint.HasValue) artwork.color = tint.Value;
        Vector2 size = artwork.sprite.bounds.size;
        visual.transform.localScale = new Vector3(0.38f / size.x, 0.12f / size.y, 1f);

        var body = projectile.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.12f;

        SpriteRenderer flame = null;
        if (weakPoint)
        {
            var flameObject = new GameObject("Weak point flame");
            flameObject.transform.SetParent(projectile.transform, false);
            flameObject.transform.localPosition = new Vector3(0.16f, 0f, 0f);
            flame = AddSprite(flameObject, flameFrames[0], 6);
            float scale = 0.25f / flame.sprite.bounds.size.x;
            flameObject.transform.localScale = new Vector3(scale, scale, 1f);
        }

        projectile.AddComponent<GunSkillProjectile>().Launch(this, direction, damage, effectsOnHit, flame);
    }

    public void Shockwave(Vector2 position, float radius, EnemyHealth source)
    {
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy != source && enemy.CurrentHealth > 0 &&
                ((Vector2)enemy.transform.position - position).sqrMagnitude <= radius * radius)
            {
                enemy.TakeDamage(BaseDamage / 2);
            }
        }

        if (shockwaveSprite == null) return;

        var wave = new GameObject("Piercing shockwave");
        wave.transform.SetParent(effects);
        wave.transform.position = position;
        var renderer = AddSprite(wave, shockwaveSprite, 7);
        // The outer glow occupies about 85% of the full sprite width.
        float fullScale = 2f * radius / (shockwaveSprite.bounds.size.x * 0.85f);
        StartCoroutine(AnimateShockwave(renderer, fullScale));
    }

    private IEnumerator AnimateShockwave(SpriteRenderer wave, float fullScale)
    {
        const float duration = 0.38f;
        Destroy(wave.gameObject, duration);
        for (float elapsed = 0f; wave != null && elapsed < duration; elapsed += Time.deltaTime)
        {
            float progress = elapsed / duration;
            float expansion = 1f - (1f - progress) * (1f - progress);
            float scale = Mathf.Lerp(0.22f, 1f, expansion) * fullScale;
            wave.transform.localScale = new Vector3(scale, scale, 1f);
            wave.color = new Color(1f, 1f, 1f, 0.85f * (1f - progress * progress));
            yield return null;
        }
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
