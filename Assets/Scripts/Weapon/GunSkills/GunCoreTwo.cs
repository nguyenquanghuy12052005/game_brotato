using System.Collections;
using System.Collections.Generic;
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

    private sealed class Poison
    {
        public float ExpiresAt;
        public GunSkillTargetEffects Movement;
    }

    private static readonly Vector2[] PoisonBodySpots =
    {
        new Vector2(-0.15f, 0.67f),
        new Vector2(0.12f, 0.46f),
        new Vector2(-0.06f, 0.24f)
    };
    private readonly Dictionary<EnemyHealth, Poison> poisons = new Dictionary<EnemyHealth, Poison>();
    private readonly Sprite[][] frames = new Sprite[11][];
    private GunSkillWeapon weapon;
    private Transform player, muzzle, visuals;
    private Material material;
    private Texture2D[] sheets;
    private GunNewSkillArt newArt;
    private GunBranch branch;
    private int slots, generation, shotNumber, shieldCharge, heat, decayCharge;
    private Vector3Int ranks;
    private float shieldUntil, shieldReady, hotUntil, heatExpires, decayUntil, nextDecayHit;
    private float portalUntil;
    private readonly List<RaycastHit2D> portalBeamHits = new List<RaycastHit2D>();
    private readonly HashSet<EnemyHealth> damagedByBeam = new HashSet<EnemyHealth>();
    private readonly HashSet<GunSkillTargetEffects> anchorStuns = new HashSet<GunSkillTargetEffects>();
    private Vector2 entrance, exit, portalDirection;
    private EnemyHealth boss;
    private bool integratedCast;

    internal bool HasShield => Time.time < shieldUntil;
    internal bool Overcharged => Time.time < hotUntil;
    internal bool HasPortal => Alive(boss) && Time.time < portalUntil;

    internal void Setup(GunSkillWeapon gun, Transform playerRoot, Transform firePoint,
        Material spriteMaterial, Texture2D[] textures, GunNewSkillArt importedArt)
    {
        weapon = gun;
        player = playerRoot;
        muzzle = firePoint;
        material = spriteMaterial;
        sheets = textures;
        newArt = importedArt;
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
        ClearPoison();
        ClearAnchorStuns();
        foreach (Transform child in visuals)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        generation++;
        shotNumber = shieldCharge = heat = decayCharge = 0;
        shieldUntil = shieldReady = hotUntil = heatExpires = decayUntil = nextDecayHit = portalUntil = 0f;
    }

    internal bool TryIntegratedCast(GunBranch mode, int choice, EnemyHealth enemy, Vector2 heading)
    {
        if ((mode == GunBranch.Piercing || choice == 1) && !Alive(enemy)) return false;
        integratedCast = true;
        branch = mode;
        slots = 1 << choice;
        boss = enemy;
        switch (mode)
        {
            case GunBranch.Spread:
                if (choice == 0)
                {
                    shieldUntil = Time.time + 3.6f;
                    StartCoroutine(Shield());
                }
                else StartCoroutine(Saw(enemy));
                break;
            case GunBranch.Burst:
                if (choice == 0) hotUntil = Time.time + 5f;
                else
                {
                    decayUntil = Time.time + 4.5f;
                    StartCoroutine(DecayAura(enemy));
                }
                break;
            case GunBranch.Piercing:
                if (choice == 0) OpenPortal(enemy, heading);
                else StartCoroutine(Anchor(enemy, heading));
                break;
            default: return false;
        }
        return true;
    }

    internal Volley BeginVolley()
    {
        if (slots == 0 || branch == GunBranch.Basic || (!integratedCast && !Alive(boss))) return null;
        shotNumber++;
        int interval = branch == GunBranch.Spread ? 2 : 4;
        return new Volley { Generation = generation, Special = shotNumber % interval == 0 };
    }

    internal bool PlasmaRound(Volley volley) =>
        Current(volley) && branch == GunBranch.Burst && (slots & 1) != 0 && Overcharged;

    internal void Hit(Volley volley, EnemyHealth enemy, Vector2 heading, bool crossedPortal)
    {
        if (!Current(volley) || !Alive(enemy)) return;
        if (integratedCast)
        {
            if (branch == GunBranch.Burst && (slots & 2) != 0 && enemy == boss && Time.time < decayUntil)
                Decay(enemy);
            return;
        }
        if (enemy != boss) return;
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
                        hotUntil = Time.time + 5f;
                    }
                }
                if ((slots & 2) != 0) Decay(enemy);
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
        shieldUntil = 0f;
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
        var sprite = Visual("Counter shield", player.position, Vector2.right, 1f);
        float start = Time.time;
        while (HasShield)
        {
            sprite.transform.position = player.position;
            sprite.sprite = newArt.Loop(GunNewSkillArt.Clip.Shield, Time.time - start, 30f, 0, 72);
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private IEnumerator Saw(EnemyHealth enemy)
    {
        const float duration = 2.4f;
        const float revolutions = 3f;
        var sprite = Visual("Pursuit saw", enemy.transform.position, Vector2.right, 1.30f);
        var targetView = enemy.GetComponentInChildren<SpriteRenderer>();
        float start = Time.time;
        float nextHit = start + 0.35f;
        while (Alive(enemy) && Time.time < start + duration)
        {
            float elapsed = Time.time - start;
            float angle = elapsed / duration * revolutions * 2f * Mathf.PI;
            var radial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var tangent = new Vector2(-radial.y, radial.x);
            Vector2 center = enemy.transform.position;
            float radius = 0.32f;
            if (targetView != null && targetView.sprite != null)
            {
                // The practice collider is much larger than the visible enemy.
                Bounds bounds = targetView.bounds;
                center = bounds.center;
                var ellipse = new Vector2(radial.x / Mathf.Max(0.05f, bounds.extents.x),
                    radial.y / Mathf.Max(0.05f, bounds.extents.y));
                // Let the tooth rim overlap the visible body as the disc circles.
                radius = 1f / ellipse.magnitude + 0.10f;
            }
            sprite.transform.SetPositionAndRotation(center + radial * radius, Rotation(tangent));
            bool close = Vector2.Distance(player.position, enemy.transform.position) <= 3.5f;
            sprite.color = close ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            sprite.sprite = newArt.Loop(GunNewSkillArt.Clip.Saw, elapsed, 30f, 0, 15);
            if (Time.time >= nextHit)
            {
                if (close) enemy.TakeDamage(7 + ranks.x);
                nextHit += 0.35f;
            }
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    internal void ApplyPoison(Volley volley, EnemyHealth enemy)
    {
        if (!Current(volley) || !Alive(enemy)) return;
        if (poisons.TryGetValue(enemy, out var existing))
        {
            existing.ExpiresAt = Time.time + 4f;
            return;
        }

        var poison = new Poison
        {
            ExpiresAt = Time.time + 4f,
            Movement = enemy.GetComponent<GunSkillTargetEffects>()
        };
        poisons.Add(enemy, poison);
        StartCoroutine(PoisonTarget(enemy, poison));
    }

    private IEnumerator PoisonTarget(EnemyHealth enemy, Poison poison)
    {
        const float interval = 0.5f;
        var view = enemy.GetComponentInChildren<SpriteRenderer>();
        var ring = new SpriteRenderer[3];
        for (int i = 0; i < ring.Length; i++)
            ring[i] = Visual("Plasma poison ring", enemy.transform.position, Vector2.right, 1f);
        var spots = new SpriteRenderer[PoisonBodySpots.Length];
        if (newArt.NoGlowMaterial != null)
        {
            var key = new MaterialPropertyBlock();
            key.SetFloat("_GlowCutoff", 0.34f);
            key.SetFloat("_PoisonInk", 1f);
            for (int i = 0; i < spots.Length; i++)
            {
                spots[i] = Visual("Plasma poison body spot", enemy.transform.position, Vector2.right, 1f);
                spots[i].sharedMaterial = newArt.NoGlowMaterial;
                spots[i].SetPropertyBlock(key);
            }
        }
        float start = Time.time;
        float nextTick = start + interval;
        poison.Movement?.SetPoisonSlow(0.10f);
        while (Alive(enemy))
        {
            float now = Time.time;
            while (Alive(enemy) && now >= nextTick && nextTick <= poison.ExpiresAt)
            {
                enemy.TakeDamage(3);
                float slow = Mathf.Min(0.45f, 0.10f + 0.10f * (nextTick - start));
                poison.Movement?.SetPoisonSlow(slow);
                nextTick += interval;
            }
            if (!Alive(enemy) || now >= poison.ExpiresAt) break;

            Bounds bounds = view != null ? view.bounds : new Bounds(enemy.transform.position, Vector3.one * 0.75f);
            var foot = new Vector2(bounds.center.x, bounds.min.y + 0.04f);
            float scale = Mathf.Max(0.7f, bounds.size.x / 0.74f);
            int frame = 5 + Mathf.FloorToInt(Mathf.PingPong((now - start) * 16f, 7f));
            float opacity = Mathf.Clamp01((poison.ExpiresAt - now) * 4f);
            for (int i = 0; i < ring.Length; i++)
            {
                ring[i].transform.position = foot + newArt.PoisonRingOffset(i) * scale;
                ring[i].transform.localScale = new Vector3(scale, scale, 1f);
                ring[i].sprite = newArt.PoisonPartFrame(i, frame);
                ring[i].color = new Color(1f, 1f, 1f, opacity);
            }
            float elapsed = now - start;
            for (int i = 0; i < spots.Length; i++)
            {
                if (spots[i] == null) continue;
                Vector2 anchor = PoisonBodySpots[i];
                float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * 7f + i * 2.1f);
                spots[i].transform.position = new Vector2(
                    bounds.center.x + bounds.size.x * anchor.x,
                    bounds.min.y + bounds.size.y * anchor.y);
                float size = Mathf.Max(0.60f, bounds.size.y / 0.84f);
                spots[i].transform.localScale = new Vector3(size, size, 1f);
                int spotFrame = 5 + Mathf.FloorToInt(Mathf.PingPong(elapsed * 16f + i * 2f, 7f));
                spots[i].sprite = newArt.PoisonPartFrame(3 + i, spotFrame);
                spots[i].color = new Color(1f, 1f, 1f, opacity * (0.45f + pulse * 0.48f));
            }
            yield return null;
        }
        if (poison.Movement != null) poison.Movement.SetPoisonSlow(0f);
        poisons.Remove(enemy);
        foreach (var piece in ring)
            if (piece != null) Destroy(piece.gameObject);
        foreach (var spot in spots)
            if (spot != null) Destroy(spot.gameObject);
    }

    private void ClearPoison()
    {
        foreach (Poison poison in poisons.Values)
            if (poison.Movement != null) poison.Movement.SetPoisonSlow(0f);
        poisons.Clear();
    }

    private void Decay(EnemyHealth enemy)
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
        enemy.TakeDamage(4 + 2 * ranks.y);
    }

    private IEnumerator DecayAura(EnemyHealth enemy)
    {
        var sprite = Visual("Disintegration aura", enemy.transform.position, Vector2.right, 1f);
        var targetView = enemy.GetComponentInChildren<SpriteRenderer>();
        float start = Time.time;
        while (Alive(enemy) && Time.time < decayUntil)
        {
            if (targetView != null && targetView.sprite != null)
            {
                Bounds bounds = targetView.bounds;
                sprite.transform.position = new Vector2(bounds.center.x, bounds.min.y);
                float scale = Mathf.Max(0.7f, bounds.size.y / 0.65f);
                sprite.transform.localScale = new Vector3(scale, scale, 1f);
            }
            else sprite.transform.position = enemy.transform.position;
            sprite.sprite = newArt.Loop(GunNewSkillArt.Clip.DecayAura, Time.time - start, 20f, 18, 18);
            sprite.color = new Color(1f, 1f, 1f, Mathf.Clamp01((decayUntil - Time.time) * 4f));
            yield return null;
        }
        Destroy(sprite.gameObject);
    }

    private void OpenPortal(EnemyHealth enemy, Vector2 heading)
    {
        bool alreadyOpen = HasPortal;
        portalDirection = heading.sqrMagnitude > 0.0001f ? heading.normalized : Vector2.right;
        Vector2 center = enemy.transform.position;
        float offset = 1.40f;
        if (enemy.TryGetComponent<Collider2D>(out var hitbox))
        {
            center = hitbox.bounds.center;
            Vector2 extents = hitbox.bounds.extents;
            // Keep both openings outside the target silhouette and its collision bounds.
            var projection = new Vector2(Mathf.Abs(portalDirection.x), Mathf.Abs(portalDirection.y));
            offset = Mathf.Max(1.25f, Vector2.Dot(extents, projection) + 0.48f);
        }
        entrance = center - portalDirection * offset;
        exit = center + portalDirection * offset;
        portalUntil = Time.time + 2.5f;
        if (!alreadyOpen) StartCoroutine(Portals());
    }

    private IEnumerator Portals()
    {
        const float beamInterval = 0.25f;
        const float beamWidthInSheet = 180f / 64f;
        var entrySprite = Visual("Portal entrance", entrance, Vector2.right, 1.25f);
        var exitSprite = Visual("Portal exit", exit, Vector2.right, 1.25f);
        var beam = Visual("Portal arcane beam", (entrance + exit) * 0.5f, portalDirection, 1f);
        entrySprite.sharedMaterial = exitSprite.sharedMaterial = beam.sharedMaterial = newArt.NoGlowMaterial;
        beam.sortingOrder = entrySprite.sortingOrder - 1;
        float start = Time.time;
        float nextTick = start;
        while (HasPortal)
        {
            Vector2 from = entrance;
            Vector2 to = exit;
            float elapsed = Time.time - start;
            entrySprite.transform.position = from;
            exitSprite.transform.position = to;
            entrySprite.sprite = exitSprite.sprite = newArt.Loop(GunNewSkillArt.Clip.Portal, elapsed, 30f, 0, 72);
            beam.transform.SetPositionAndRotation((from + to) * 0.5f, Rotation(to - from));
            beam.transform.localScale = new Vector3(Vector2.Distance(from, to) / beamWidthInSheet, 0.78f, 1f);
            beam.sprite = newArt.Loop(GunNewSkillArt.Clip.PortalBeam, elapsed, 30f, 0, 36);
            if (Time.time >= nextTick)
            {
                DamagePortalBeam(from, to);
                nextTick = Time.time + beamInterval;
            }
            yield return null;
        }
        Destroy(entrySprite.gameObject);
        Destroy(exitSprite.gameObject);
        Destroy(beam.gameObject);
    }

    private void DamagePortalBeam(Vector2 from, Vector2 to)
    {
        const float beamRadius = 0.18f;
        const int damage = 8;
        Vector2 segment = to - from;
        var filter = new ContactFilter2D { useTriggers = true };
        portalBeamHits.Clear();
        damagedByBeam.Clear();
        Physics2D.CircleCast(from, beamRadius, segment.normalized, filter, portalBeamHits, segment.magnitude);
        foreach (var hit in portalBeamHits)
        {
            var enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (Alive(enemy) && damagedByBeam.Add(enemy)) enemy.TakeDamage(damage);
        }
    }

    private IEnumerator Anchor(EnemyHealth enemy, Vector2 heading)
    {
        const float lifetime = 2.35f, breakDistance = 2.65f;
        var view = enemy.GetComponentInChildren<SpriteRenderer>();
        Bounds body = TargetBounds(enemy, view);
        Vector2 ground = new Vector2(body.center.x, body.min.y);
        Vector2 origin = ground + heading * 1.1f;
        Vector2 terminal = origin + Vector2.up * 0.70f;
        var coil = Visual("Tesla anchor", origin, Vector2.right, 1f);
        var cable = Visual("Tesla lightning tether", terminal, Vector2.right, 1f);
        cable.sharedMaterial = newArt.NoGlowMaterial;
        cable.sortingOrder = coil.sortingOrder - 1;
        float start = Time.time;
        bool snapped = false;
        while (Alive(enemy) && Time.time < start + lifetime)
        {
            body = TargetBounds(enemy, view);
            Vector2 foot = new Vector2(body.center.x, body.min.y);
            if (Vector2.Distance(origin, foot) >= breakDistance) { snapped = true; break; }
            Vector2 contact = Vector2.Lerp(foot, body.center, 0.85f);
            float elapsed = Time.time - start;
            coil.sprite = newArt.Loop(GunNewSkillArt.Clip.TeslaCoil, elapsed, 30f, 0, 72);
            newArt.UpdateLightningLink(cable, terminal, contact, elapsed);
            yield return null;
        }

        coil.gameObject.SetActive(false);
        cable.gameObject.SetActive(false);
        Destroy(coil.gameObject);
        Destroy(cable.gameObject);
        if (!Alive(enemy)) yield break;
        if (snapped)
        {
            StartCoroutine(Thunderclap(origin, 1f));
            if (enemy.TryGetComponent<GunSkillTargetEffects>(out var status))
            {
                status.Stun(1.25f);
                if (anchorStuns.Add(status)) StartCoroutine(ShowAnchorStun(enemy, view, status));
            }
            enemy.TakeDamage(30 + 5 * ranks.x);
            yield break;
        }

        float radius = 1.45f + ranks.z * 0.24f;
        StartCoroutine(Thunderclap(origin, radius / 1.45f));
        foreach (EnemyHealth nearby in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (!Alive(nearby)) continue;
            Bounds nearbyBody = TargetBounds(nearby, nearby.GetComponentInChildren<SpriteRenderer>());
            Vector2 foot = new Vector2(nearbyBody.center.x, nearbyBody.min.y);
            if (Vector2.Distance(foot, origin) <= radius)
                nearby.TakeDamage(15 + 3 * ranks.x);
        }
    }

    private static Bounds TargetBounds(EnemyHealth enemy, SpriteRenderer view) =>
        view != null && view.sprite != null ? view.bounds : new Bounds(enemy.transform.position, Vector3.one * 0.65f);

    private IEnumerator Thunderclap(Vector2 origin, float scale)
    {
        var blast = Visual("Tesla anchor explosion", origin, Vector2.right, scale);
        const int first = 4, end = 52;
        const float fps = 46f;
        for (float elapsed = 0f; elapsed < (end - first + 1) / fps; elapsed += Time.deltaTime)
        {
            blast.sprite = newArt.Frame(GunNewSkillArt.Clip.Thunderclap, Mathf.Min(end, first + Mathf.FloorToInt(elapsed * fps)));
            yield return null;
        }
        Destroy(blast.gameObject);
    }

    private IEnumerator ShowAnchorStun(EnemyHealth enemy, SpriteRenderer view, GunSkillTargetEffects status)
    {
        var stars = Visual("Tesla stunned stars", enemy.transform.position, Vector2.right, 1f);
        stars.sortingOrder = Mathf.Max(stars.sortingOrder, view != null ? view.sortingOrder + 1 : 0);
        float start = Time.time;
        while (Alive(enemy) && status != null && status.IsStunned)
        {
            Bounds body = TargetBounds(enemy, view);
            stars.transform.position = new Vector2(body.center.x, body.max.y + 0.1f);
            stars.sprite = newArt.Loop(GunNewSkillArt.Clip.Stunned, Time.time - start, 30f, 0, 60);
            yield return null;
        }
        anchorStuns.Remove(status);
        Destroy(stars.gameObject);
    }

    private void ClearAnchorStuns()
    {
        foreach (var status in anchorStuns)
            if (status != null) status.ClearStun();
        anchorStuns.Clear();
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
        ClearPoison();
        ClearAnchorStuns();
        foreach (var clip in frames)
            if (clip != null)
                foreach (var frame in clip) Destroy(frame);
    }
}
