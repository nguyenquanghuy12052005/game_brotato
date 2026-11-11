using System.Collections;
using UnityEngine;

internal sealed class GunCoreTwo : MonoBehaviour
{
    // Order matches the texture references in GunSkillTest.
    internal enum Art { Shield, Break, Saw, Muzzle, Heavy, Decay, DecayHit, Portal, Transit, Anchor, Snap }

    internal sealed class Volley
    {
        public int Generation;
        public int Hits;
        public bool Special;
    }

    private readonly Sprite[][] frames = new Sprite[11][];
    private GunSkillWeapon weapon;
    private Transform player, muzzle, visuals;
    private Material material;
    private Texture2D[] sheets;
    private GunBranch branch;
    private int slots, generation, shotNumber, shieldCharge, heat, decayCharge;
    private Vector3Int ranks;
    private float shieldUntil, shieldReady, hotUntil, heatExpires, decayUntil, nextDecayHit;
    private float portalUntil;
    private Vector2 entrance, exit, portalDirection;
    private EnemyHealth boss;

    internal bool HasShield => Time.time < shieldUntil;
    internal bool Overcharged => Time.time < hotUntil;
    internal bool HasPortal => Alive(boss) && Time.time < portalUntil;

    internal void Setup(GunSkillWeapon gun, Transform playerRoot, Transform firePoint,
        Material spriteMaterial, Texture2D[] textures)
    {
        weapon = gun;
        player = playerRoot;
        muzzle = firePoint;
        material = spriteMaterial;
        sheets = textures;
        visuals = new GameObject("Core two effects").transform;
        visuals.SetParent(transform, false);
    }

    internal void Select(GunBranch mode, int selection, Vector3Int upgrades, EnemyHealth target)
    {
        if (branch == mode && slots == selection && ranks == upgrades && boss == target) return;
        ResetEffects();
        branch = mode;
        slots = selection;
        ranks = upgrades;
        boss = target;
    }

    internal void ResetEffects()
    {
        StopAllCoroutines();
        foreach (Transform child in visuals)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        generation++;
        shotNumber = shieldCharge = heat = decayCharge = 0;
        shieldUntil = shieldReady = hotUntil = heatExpires = decayUntil = nextDecayHit = portalUntil = 0f;
    }

    internal Volley BeginVolley()
    {
        if (slots == 0 || branch == GunBranch.Basic || !Alive(boss)) return null;
        shotNumber++;
        int interval = branch == GunBranch.Spread ? 2 : 4;
        return new Volley { Generation = generation, Special = shotNumber % interval == 0 };
    }

    internal bool HeavyRound(Volley volley, int remaining) =>
        Current(volley) && branch == GunBranch.Burst && (slots & 1) != 0 &&
        Overcharged && remaining <= 2 + ranks.x;

    internal void Hit(Volley volley, EnemyHealth enemy, Vector2 heading, bool crossedPortal)
    {
        if (!Current(volley) || enemy != boss || !Alive(enemy)) return;
        volley.Hits++;
        switch (branch)
        {
            case GunBranch.Spread:
                float distance = Vector2.Distance(player.position, boss.transform.position);
                if ((slots & 1) != 0 && distance <= 3.3f && Time.time >= shieldReady && !HasShield &&
                    ++shieldCharge >= 2)
                {
                    shieldCharge = 0;
                    shieldUntil = Time.time + 3.6f;
                    shieldReady = Time.time + 4.4f;
                    StartCoroutine(Shield());
                }

                if ((slots & 2) != 0 && volley.Special && volley.Hits == 2)
                    StartCoroutine(Saw(enemy));
                break;
            case GunBranch.Burst:
                if ((slots & 1) != 0 && !Overcharged && volley.Hits == 3)
                {
                    if (Time.time > heatExpires) heat = 0;
                    heatExpires = Time.time + 8f;
                    if (++heat >= 3)
                    {
                        heat = 0;
                        hotUntil = Time.time + 5.5f;
                        StartCoroutine(HotMuzzle());
                    }
                }
                if ((slots & 2) != 0) Decay(enemy, heading);
                break;
            case GunBranch.Piercing:
                if (volley.Special && volley.Hits == 1)
                {
                    if ((slots & 1) != 0 && !crossedPortal) OpenPortal(enemy, heading);
                    if ((slots & 2) != 0) StartCoroutine(Anchor(enemy, heading));
                }
                break;
        }
    }

    internal bool Block(Vector2 source)
    {
        if (!HasShield) return false;
        Vector2 aim = muzzle.right;
        Vector2 towardSource = (source - (Vector2)player.position).normalized;
        if (Vector2.Dot(aim, towardSource) < 0.45f) return false;
        shieldUntil = 0f;
        StartCoroutine(Flash("Counter shield break", Art.Break, player.position, aim, 2.6f, 36));
        weapon.FireCounter(muzzle.position, aim, 3 + ranks.x, 60f - 15f * ranks.y);
        return true;
    }

    internal bool CrossPortal(Volley volley, Vector2 from, Vector2 next, Vector2 heading,
        out Vector2 destination, out Vector2 newHeading)
    {
        destination = from;
        newHeading = heading;
        if (!Current(volley) || !HasPortal || Vector2.Dot(heading, portalDirection) < 0.8f) return false;
        Vector2 movement = next - from;
        float t = Vector2.Dot(entrance - from, movement) / Mathf.Max(movement.sqrMagnitude, 0.0001f);
        if (t < 0f || t > 1f || (from + movement * t - entrance).sqrMagnitude > 0.34f * 0.34f) return false;

        newHeading = -portalDirection;
        destination = exit + newHeading * 0.18f;
        StartCoroutine(Flash("Portal entrance flash", Art.Transit, entrance, heading, 1.45f, 40));
        StartCoroutine(Flash("Portal exit flash", Art.Transit, exit, newHeading, 1.45f, 40));
        return true;
    }

    private bool Current(Volley volley) => volley != null && volley.Generation == generation;
    private static bool Alive(EnemyHealth enemy) => enemy != null && enemy.CurrentHealth > 0;
    private static Quaternion Rotation(Vector2 heading) =>
        Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, heading));

    private IEnumerator Shield()
    {
        var sprite = Visual("Counter shield", player.position, muzzle.right, 2.6f);
        float start = Time.time;
        while (HasShield)
        {
            sprite.transform.SetPositionAndRotation(player.position, Rotation(muzzle.right));
            sprite.sprite = Frame(Art.Shield, Time.time - start, 24);
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private IEnumerator Saw(EnemyHealth enemy)
    {
        var sprite = Visual("Pursuit saw", enemy.transform.position, Vector2.right, 2.7f);
        float start = Time.time;
        float nextHit = start + 0.35f;
        while (Alive(enemy) && Time.time < start + 2.4f)
        {
            sprite.transform.position = enemy.transform.position;
            bool close = Vector2.Distance(player.position, enemy.transform.position) <= 3.5f;
            sprite.color = close ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            sprite.sprite = Frame(Art.Saw, Time.time - start, 32);
            if (Time.time >= nextHit)
            {
                if (close) enemy.TakeDamage(7 + ranks.x);
                nextHit += 0.35f;
            }
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private IEnumerator HotMuzzle()
    {
        var sprite = Visual("Escalation muzzle", muzzle.position, muzzle.right, 1.8f);
        float start = Time.time;
        while (Overcharged)
        {
            sprite.transform.SetPositionAndRotation(muzzle.position, Rotation(muzzle.right));
            sprite.sprite = Frame(Art.Muzzle, Time.time - start, 24);
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private void Decay(EnemyHealth enemy, Vector2 heading)
    {
        if (Time.time >= decayUntil)
        {
            if (++decayCharge >= 8)
            {
                decayCharge = 0;
                decayUntil = Time.time + 4.5f;
                StartCoroutine(DecayAura(enemy));
            }
            return;
        }

        if (Time.time < nextDecayHit) return;
        nextDecayHit = Time.time + 0.24f;
        StartCoroutine(Flash("Disintegration hit", Art.DecayHit, enemy.transform.position, heading, 1.6f, 40));
        enemy.TakeDamage(4 + 2 * ranks.z);
    }

    private IEnumerator DecayAura(EnemyHealth enemy)
    {
        var sprite = Visual("Disintegration aura", enemy.transform.position, Vector2.right, 2.3f);
        float start = Time.time;
        while (Alive(enemy) && Time.time < decayUntil)
        {
            sprite.transform.position = enemy.transform.position;
            sprite.sprite = Frame(Art.Decay, Time.time - start, 24);
            sprite.color = new Color(1f, 1f, 1f, Mathf.Clamp01((decayUntil - Time.time) * 4f));
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private void OpenPortal(EnemyHealth enemy, Vector2 heading)
    {
        portalDirection = heading;
        Vector2 center = enemy.transform.position;
        float offset = 1.55f;
        if (enemy.TryGetComponent<Collider2D>(out var hitbox))
        {
            center = hitbox.bounds.center;
            Vector2 extents = hitbox.bounds.extents;
            // Place both gates outside the boss collider, including room for the projectile.
            offset = Vector2.Dot(extents, new Vector2(Mathf.Abs(heading.x), Mathf.Abs(heading.y))) + 0.55f;
        }
        entrance = center - heading * offset;
        exit = center + heading * offset;
        if (!HasPortal) StartCoroutine(Portals());
        portalUntil = Time.time + 2.5f;
    }

    private IEnumerator Portals()
    {
        var entrySprite = Visual("Portal entrance", entrance, portalDirection, 1.7f);
        var exitSprite = Visual("Portal exit", exit, -portalDirection, 1.7f);
        float start = Time.time;
        do
        {
            entrySprite.transform.SetPositionAndRotation(entrance, Rotation(portalDirection));
            exitSprite.transform.SetPositionAndRotation(exit, Rotation(-portalDirection));
            entrySprite.sprite = exitSprite.sprite = Frame(Art.Portal, Time.time - start, 24);
            yield return null;
        } while (HasPortal);
        Destroy(entrySprite.gameObject);
        Destroy(exitSprite.gameObject);
    }

    private IEnumerator Anchor(EnemyHealth enemy, Vector2 heading)
    {
        Vector2 origin = (Vector2)enemy.transform.position + heading * 1.1f;
        var sprite = Visual("Heart anchor", origin, Vector2.right, 1.9f);
        var line = sprite.gameObject.AddComponent<LineRenderer>();
        line.sharedMaterial = material;
        line.startColor = new Color(0.6f, 0.35f, 1f);
        line.endColor = Color.cyan;
        line.positionCount = 3;
        line.sortingOrder = 7;
        float start = Time.time;
        bool snapped = false;
        while (Alive(enemy) && Time.time < start + 2.35f)
        {
            Vector2 destination = enemy.transform.position;
            float distance = Vector2.Distance(origin, destination);
            float tension = Mathf.InverseLerp(1.1f, 2.65f, distance);
            line.widthMultiplier = Mathf.Lerp(0.075f, 0.027f, tension);
            line.SetPosition(0, origin);
            line.SetPosition(1, (origin + destination) * 0.5f + Vector2.up * (1f - tension) * 0.23f);
            line.SetPosition(2, destination);
            sprite.sprite = Frame(Art.Anchor, Time.time - start, 24);
            if (tension >= 1f) { snapped = true; break; }
            yield return null;
        }

        Destroy(sprite.gameObject);
        if (!Alive(enemy)) yield break;
        Vector2 target = enemy.transform.position;
        Vector2 direction = (target - origin).normalized;
        if (snapped)
        {
            StartCoroutine(Flash("Heart anchor snap", Art.Snap, (origin + target) * 0.5f, direction, 2.8f, 36));
            enemy.TakeDamage(30 + 5 * ranks.x);
            yield break;
        }

        float radius = 1.45f + ranks.z * 0.24f;
        StartCoroutine(Flash("Heart anchor expiry", Art.Snap, origin, heading, radius * 1.8f, 36));
        foreach (EnemyHealth nearby in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            if (Alive(nearby) && Vector2.Distance(nearby.transform.position, origin) <= radius)
                nearby.TakeDamage(15 + 3 * ranks.x);
    }

    private SpriteRenderer Visual(string label, Vector2 position, Vector2 heading, float size)
    {
        var obj = new GameObject(label);
        obj.transform.SetParent(visuals, false);
        obj.transform.SetPositionAndRotation(position, Rotation(heading));
        obj.transform.localScale = new Vector3(size, size, 1f);
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sharedMaterial = material;
        sprite.sortingOrder = 8;
        return sprite;
    }

    private IEnumerator Flash(string label, Art art, Vector2 position, Vector2 heading, float size, int fps)
    {
        var sprite = Visual(label, position, heading, size);
        for (float elapsed = 0f; elapsed * fps < 32f; elapsed += Time.deltaTime)
        {
            sprite.sprite = Frame(art, elapsed, fps);
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    internal Sprite Frame(Art art, float elapsed, int fps)
    {
        int index = (int)art;
        Sprite[] clip = frames[index];
        if (clip == null)
        {
            clip = new Sprite[32];
            for (int i = 0; i < clip.Length; i++)
            {
                var rect = new Rect(i % 8 * 256, (3 - i / 8) * 256, 256, 256);
                clip[i] = Sprite.Create(sheets[index], rect, Vector2.one * 0.5f, 256f, 0, SpriteMeshType.FullRect);
            }
            frames[index] = clip;
        }
        return clip[Mathf.FloorToInt(elapsed * fps) % 32];
    }

    private void OnDestroy()
    {
        foreach (var clip in frames)
            if (clip != null)
                foreach (var frame in clip) Destroy(frame);
    }
}
