using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Standalone prototype for GunSkillTest. Secondary attacks deal damage without generating skill hits.
internal sealed class GunLevel11 : MonoBehaviour
{
    internal sealed class Volley
    {
        public int Generation;
        public int Size;
        public bool FireBreathReady;
        public Vector2 Heading;
        public readonly Dictionary<EnemyHealth, int> Hits = new Dictionary<EnemyHealth, int>();
    }

    private sealed class TargetState
    {
        public int Charge;
        public float LastHit = float.NegativeInfinity;
        public float SunderUntil;
        public bool SunderPending;
    }

    private enum Art { Sweep, Sunder, Charge, Fuse, Echo, Rift, Recall, Sky, Orbit, Satellite }
    private readonly Dictionary<EnemyHealth, TargetState> states = new Dictionary<EnemyHealth, TargetState>();
    private readonly Sprite[][] frames = new Sprite[10][];
        private Texture2D[] sheets;
    private GunNewSkillArt newArt;
    private Transform visuals;
    private Transform player;
    private EnemyHealth[] targets;
    private GunSkillWeapon weapon;
    private SpriteRenderer orbit;
    private float orbitStarted;
    private GunBranch branch;
    private Vector3Int ranks;
    private int slot;
    private int shotNumber;
    private int generation;
    private bool inBossMode;

    internal void Setup(GunSkillWeapon owner, Transform playerRoot, Transform effects,
        EnemyHealth[] arenaTargets, Texture2D[] atlases, GunNewSkillArt importedArt)
    {
        weapon = owner;
        player = playerRoot;
        targets = arenaTargets;
        sheets = atlases;
        newArt = importedArt;
        visuals = new GameObject("Level 11 effects").transform;
        visuals.SetParent(effects.parent, false);
    }

    internal void Select(GunBranch mode, int selection, Vector3Int upgradeRanks, bool bossMode)
    {
        if (branch != mode || slot != selection || ranks != upgradeRanks || inBossMode != bossMode) ResetEffects();
        branch = mode;
        slot = selection;
        ranks = upgradeRanks;
        inBossMode = bossMode;
    }

    internal void ResetEffects(bool restartCycle = true)
    {
        StopAllCoroutines();
        if (visuals != null)
        {
            foreach (Transform child in visuals)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
        orbit = null;
        states.Clear();
        if (restartCycle) shotNumber = 0;
        generation++;
    }

    private void OnDestroy()
    {
        if (visuals != null) Destroy(visuals.gameObject);
        foreach (Sprite[] clip in frames)
        {
            if (clip == null) continue;
            foreach (Sprite frame in clip) Destroy(frame);
        }
    }

    internal Volley BeginVolley(Vector2 origin, Vector2 heading, int size)
    {
        if (slot == 0 || branch == GunBranch.Basic) return null;
        shotNumber++;

        if (branch == GunBranch.Spread && slot == 1 && shotNumber % 2 == 0)
            StartCoroutine(Sweep(origin, heading));
        if (branch == GunBranch.Piercing && shotNumber % 4 == 0)
        {
            if (slot == 1) StartCoroutine(Rift(origin, heading));
            if (slot == 2) StartCoroutine(Recall(origin, heading));
        }
        return new Volley
        {
            Generation = generation,
            Size = size,
            FireBreathReady = branch == GunBranch.Spread && slot == 3 && shotNumber % 2 == 0,
            Heading = heading
        };
    }

    internal int DamageAgainst(EnemyHealth enemy, int damage)
    {
        return states.TryGetValue(enemy, out var state) && state.SunderUntil > Time.time
            ? Mathf.CeilToInt(damage * 1.25f) : damage;
    }

    internal void Hit(Volley volley, EnemyHealth enemy)
    {
        if (volley == null || volley.Generation != generation || !Alive(enemy)) return;
        volley.Hits.TryGetValue(enemy, out int hits);
        volley.Hits[enemy] = ++hits;

        switch (branch)
        {
            case GunBranch.Spread:
                if (slot == 2 && hits == 1) Sunder(enemy);
                if (slot == 3 && volley.FireBreathReady)
                {
                    volley.FireBreathReady = false;
                    StartCoroutine(FireBreath(enemy, volley));
                }
                break;
            case GunBranch.Burst:
                if (slot == 1 && Charge(enemy, 5))
                    StartCoroutine(Detonate(enemy));
                if (slot == 2 && hits == Mathf.CeilToInt(volley.Size * 0.6f))
                    StartCoroutine(Echo(enemy));
                if (slot == 3)
                {
                    if (orbit == null)
                    {
                        orbitStarted = Time.time;
                        orbit = CreateVisual("Orbit charge", player.position, Vector2.right, 1.35f, 1.35f);
                        StartCoroutine(OrbitCharge(orbit));
                    }
                    if (Charge(enemy, 8))
                    {
                        float phase = (Time.time - orbitStarted) * Mathf.PI * 2f;
                        Destroy(orbit.gameObject);
                        orbit = null;
                        StartCoroutine(LaunchOrbit(enemy, phase));
                    }
                }
                break;
            case GunBranch.Piercing:
                if (slot == 3 && Charge(enemy, 3))
                    StartCoroutine(SkyLance(enemy.transform.position));
                break;
        }
    }

    private TargetState State(EnemyHealth enemy)
    {
        if (!states.TryGetValue(enemy, out var state))
        {
            state = new TargetState();
            states.Add(enemy, state);
        }
        return state;
    }

    private bool Charge(EnemyHealth enemy, int threshold)
    {
        var state = State(enemy);
        if (++state.Charge < threshold) return false;
        state.Charge = 0;
        return true;
    }

    private void Sunder(EnemyHealth enemy)
    {
        var state = State(enemy);
        if (state.SunderPending || state.SunderUntil > Time.time) return;
        if (Time.time - state.LastHit > 6f) state.Charge = 0;
        state.LastHit = Time.time;
        state.Charge += 2 + ranks.z;
        if (state.Charge < 6) return;
        state.Charge = 0;
        StartCoroutine(Fireball(enemy, state));
    }

    private const float FireballChargeDuration = 0.30f;

    private IEnumerator Fireball(EnemyHealth enemy, TargetState state)
    {
        state.SunderPending = true;
        const float spacing = 0.16f;
        StartCoroutine(LaunchFireball(enemy, state));
        yield return new WaitForSeconds(FireballChargeDuration + spacing);
        StartCoroutine(LaunchFireball(enemy, state, charge: false));
        yield return new WaitForSeconds(spacing);
        yield return LaunchFireball(enemy, state, charge: false);
        state.SunderPending = false;
    }

    private IEnumerator FireBreath(EnemyHealth firstTarget, Volley volley)
    {
        // Let the remaining pellets contribute before the flame starts.
        yield return new WaitForSeconds(0.12f);
        const float reach = 5.4f;
        const float halfAngle = 30f;
        float sideGrowth = Mathf.Tan(halfAngle * Mathf.Deg2Rad);
        const float tickInterval = 0.20f;
        const int ticks = 8;
        const float duration = tickInterval * ticks;
        int damage = 10 + 8 * volley.Hits[firstTarget];
        Vector2 heading = volley.Heading;
        Vector2 side = new Vector2(-heading.y, heading.x);
        const float wingAngle = 21f;
        var centerFlame = CreateFlameStream("Shotgun fire breath", heading, 0f, 1.65f, 1.65f);
        var leftFlame = CreateFlameStream("Shotgun flame left", heading, wingAngle, 1.72f, 1.35f);
        var rightFlame = CreateFlameStream("Shotgun flame right", heading, -wingAngle, 1.72f, 1.35f);
        centerFlame.sortingOrder = leftFlame.sortingOrder + 1;
        int nextTick = 0;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            Vector2 origin = weapon.MuzzlePosition;
            float fade = Mathf.Clamp01((duration - elapsed) / 0.16f);
            ShowFlameStream(centerFlame, origin, elapsed, fade);
            ShowFlameStream(leftFlame, origin, elapsed + 0.20f, fade * 0.88f);
            ShowFlameStream(rightFlame, origin, elapsed + 0.43f, fade * 0.88f);
            while (nextTick < ticks && elapsed >= nextTick * tickInterval)
            {
                int tickDamage = damage / ticks + (nextTick < damage % ticks ? 1 : 0);
                nextTick++;
                foreach (EnemyHealth enemy in targets)
                {
                    if (!Alive(enemy)) continue;
                    Vector2 delta = (Vector2)enemy.transform.position - origin;
                    float along = Vector2.Dot(delta, heading);
                    if (along >= 0f && along <= reach && Mathf.Abs(Vector2.Dot(delta, side)) <= along * sideGrowth)
                        enemy.TakeDamage(tickDamage);
                }
            }
            yield return null;
        }
        Destroy(centerFlame.gameObject);
        Destroy(leftFlame.gameObject);
        Destroy(rightFlame.gameObject);
    }

    private SpriteRenderer CreateFlameStream(string label, Vector2 heading, float angle, float length, float width)
    {
        // The flame in the atlas rises about 0.15 units for every unit forward.
        float correction = Mathf.Atan2(0.15f * width, length) * Mathf.Rad2Deg;
        Vector2 spriteHeading = Quaternion.Euler(0f, 0f, angle - correction) * heading;
        var renderer = CreateVisual(label, weapon.MuzzlePosition, spriteHeading, length, width);
        renderer.sharedMaterial = newArt.NoGlowMaterial;
        return renderer;
    }

    private void ShowFlameStream(SpriteRenderer renderer, Vector2 origin, float elapsed, float opacity)
    {
        renderer.transform.position = origin;
        renderer.sprite = newArt.Loop(GunNewSkillArt.Clip.Breath, elapsed, 30f, 0, 45);
        renderer.color = new Color(1f, 1f, 1f, opacity);
    }

    private IEnumerator LaunchFireball(EnemyHealth target, TargetState sunder, bool charge = true)
    {
        if (!Alive(target)) yield break;
        const GunNewSkillArt.Clip clip = GunNewSkillArt.Clip.Fireball;
        Vector2 origin = player.position;
        Vector2 destination = target.transform.position;
        Vector2 heading = (destination - origin).normalized;
        var fx = CreateVisual("Công phá fireball", origin, heading, 1f, 1f);
        if (charge)
        {
            for (float elapsed = 0f; elapsed < FireballChargeDuration; elapsed += Time.deltaTime)
            {
                fx.transform.position = player.position;
                fx.sprite = newArt.Progress(clip, elapsed / FireballChargeDuration, 7, 28);
                yield return null;
            }
            origin = fx.transform.position;
        }

        const float duration = 0.55f;
        Vector2 previous = origin;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            if (Alive(target)) destination = target.transform.position;
            Vector2 next = Vector2.Lerp(origin, destination, Mathf.Clamp01(elapsed / duration));
            Vector2 delta = next - previous;
            if (delta.sqrMagnitude > 0.0001f) heading = delta.normalized;
            fx.transform.SetPositionAndRotation(next, Rotation(heading));
            fx.sprite = newArt.Progress(clip, elapsed / duration, 29, 42);
            previous = next;
            yield return null;
        }
        fx.transform.SetPositionAndRotation(destination, Quaternion.identity);
        Area(destination, 1.0f, 18 + ranks.z * 3);
        if (Alive(target)) sunder.SunderUntil = Time.time + 5f;
        yield return NewAnimation(fx, clip, 43, 175, 70f);
        Destroy(fx.gameObject);
    }

    private IEnumerator NewAnimation(SpriteRenderer renderer, GunNewSkillArt.Clip clip, int first, int last, float fps)
    {
        float duration = (last - first + 1) / fps;
        for (float elapsed = 0f; renderer != null && elapsed < duration; elapsed += Time.deltaTime)
        {
            renderer.sprite = newArt.Frame(clip, Mathf.Min(last, first + Mathf.FloorToInt(elapsed * fps)));
            float fade = Mathf.Clamp01((duration - elapsed) / 0.18f);
            renderer.color = new Color(1f, 1f, 1f, fade);
            yield return null;
        }
    }

    private IEnumerator Detonate(EnemyHealth enemy)
    {
        var fx = CreateVisual("Chain detonator", enemy.transform.position, Vector2.right, 1f, 1f);
        StartCoroutine(Follow(fx, enemy));
        StartCoroutine(PixelPlay(fx, GunNewSkillArt.Clip.Fuse, 90, 20f / (21f / 32f)));
        yield return new WaitForSeconds(21f / 32f);
        if (Alive(enemy)) Area(enemy.transform.position, 0.8f, 25 + 5 * ranks.y);
    }

    private IEnumerator Echo(EnemyHealth enemy)
    {
        Vector2 heading = ((Vector2)enemy.transform.position - (Vector2)player.position).normalized;
        Vector2 offset = new Vector2(-heading.y, heading.x) * 0.62f;
        Vector2 position = (Vector2)player.position + offset;
        var fx = CreateVisual("Echo gun", position, heading, 2.1f, 2.1f);
        StartCoroutine(Play(fx, Art.Echo, 32));
        yield return new WaitForSeconds(0.30f);
        for (int i = 0; i < 3; i++)
        {
            if (!Alive(enemy)) yield break;
            heading = ((Vector2)enemy.transform.position - position).normalized;
            fx.transform.rotation = Rotation(heading);
            Vector2 firePoint = position + heading * 0.34f;
            weapon.FireEcho(firePoint, heading);
            if (i < ranks.z)
            {
                yield return new WaitForSeconds(0.07f);
                weapon.FireEcho(firePoint, heading);
                yield return new WaitForSeconds(0.10f);
            }
            else yield return new WaitForSeconds(0.17f);
        }
    }

    private IEnumerator OrbitCharge(SpriteRenderer fx)
    {
        while (fx != null && orbit == fx)
        {
            fx.transform.position = player.position;
            fx.sprite = newArt.Loop(GunNewSkillArt.Clip.Orbit, Time.time - orbitStarted, 24f, 1, 20);
            yield return null;
        }
    }

    private IEnumerator LaunchOrbit(EnemyHealth enemy, float phase)
    {
        int count = 6 + ranks.z;
        for (int i = 0; i < count && Alive(enemy); i++)
        {
            float angle = phase + (i * Mathf.PI * 2f / count) + i * 0.07f * Mathf.PI * 2f;
            Vector2 from = (Vector2)player.position + new Vector2(Mathf.Cos(angle), 0.82f * Mathf.Sin(angle)) * 0.48f;
            StartCoroutine(Satellite(enemy, from, i % 2 == 0 ? 1f : -1f));
            yield return new WaitForSeconds(0.07f);
        }
    }

    private IEnumerator Satellite(EnemyHealth enemy, Vector2 from, float side)
    {
        var fx = CreateVisual("Orbit satellite", from, Vector2.right, 1.35f, 1.35f);
        Vector2 previous = from;
        for (float elapsed = 0f; elapsed < 0.32f && Alive(enemy); elapsed += Time.deltaTime)
        {
            float t = elapsed / 0.32f;
            Vector2 destination = enemy.transform.position;
            Vector2 axis = (destination - from).normalized;
            Vector2 position = Vector2.Lerp(from, destination, t)
                + new Vector2(-axis.y, axis.x) * (side * Mathf.Sin(t * Mathf.PI) * 0.65f);
            fx.transform.SetPositionAndRotation(position, Rotation(position - previous));
            fx.sprite = newArt.Loop(GunNewSkillArt.Clip.Orbit, elapsed, 32f, 1, 20);
            previous = position;
            yield return null;
        }
        if (Alive(enemy))
        {
            weapon.ShowImpact(enemy.transform.position, Color.cyan);
            enemy.TakeDamage(5 + ranks.y);
        }
        Destroy(fx.gameObject);
    }

    private IEnumerator Sweep(Vector2 origin, Vector2 heading)
    {
        const float reach = 5.2f;
        const float lengthScale = 1.33f;
        float halfWidth = 3.4f - ranks.y * 0.46f;
        float width = 1.6f * halfWidth / 2.6f;
        var fx = CreateVisual("Sweeping blade", origin, heading, lengthScale, width);
        // Straighten the atlas's diagonal: its bright arc is 59 px forward and 13 px up from the pivot.
        float atlasTilt = Mathf.Atan2(13f * width, 59f * lengthScale) * Mathf.Rad2Deg;
        fx.transform.rotation *= Quaternion.Euler(0f, 0f, -atlasTilt);
        if (newArt.NoGlowMaterial != null)
        {
            fx.sharedMaterial = newArt.NoGlowMaterial;
            var properties = new MaterialPropertyBlock();
            properties.SetFloat("_GlowCutoff", 0.47f);
            fx.SetPropertyBlock(properties);
        }
        var hit = new HashSet<EnemyHealth>();
        yield return new WaitForSeconds(0.19f);
        StartCoroutine(PixelPlay(fx, GunNewSkillArt.Clip.Sweep, 18, 16f, firstFrame: 1));
        for (float elapsed = 0.19f; elapsed < 0.65f; elapsed += Time.deltaTime)
        {
            float t = Mathf.InverseLerp(0.19f, 0.65f, elapsed);
            float front = Mathf.Lerp(0.7f, reach, 1f - Mathf.Pow(1f - t, 3f));
            foreach (EnemyHealth enemy in targets)
            {
                if (!Alive(enemy) || hit.Contains(enemy)) continue;
                Vector2 delta = (Vector2)enemy.transform.position - origin;
                float along = Vector2.Dot(delta, heading);
                float across = Mathf.Abs(Vector2.Dot(delta, new Vector2(-heading.y, heading.x)));
                if (along < -0.2f || along > front || across > halfWidth * front / reach + 0.25f) continue;
                hit.Add(enemy);
                enemy.TakeDamage(22 + 5 * ranks.x);
                if (Alive(enemy) && ranks.z > 0 && enemy.TryGetComponent<GunSkillTargetEffects>(out var status))
                    status.Push(heading * (0.36f * ranks.z));
            }
            yield return null;
        }
    }

    private IEnumerator Rift(Vector2 origin, Vector2 heading)
    {
        const float length = 8f;
        const float duration = 1.35f;
        Vector2 end = origin + heading * length;
        var fx = CreateVisual("Spatial rift", (origin + end) / 2f, heading, length / 2.6f, 2.0f);
        float nextTick = 0.28f;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            fx.sprite = newArt.Progress(GunNewSkillArt.Clip.Quake, elapsed / duration, 2, 95);
            if (elapsed >= nextTick)
            {
                foreach (EnemyHealth enemy in targets)
                {
                    if (!Alive(enemy) || DistanceSquared(enemy.transform.position, origin, end) > 0.50f * 0.50f) continue;
                    enemy.TakeDamage(5 + 2 * ranks.x);
                }
                nextTick += 0.23f;
            }
            yield return null;
        }
        Destroy(fx.gameObject);
    }

    private IEnumerator Recall(Vector2 origin, Vector2 heading)
    {
        const float length = 8f;
        Vector2 end = origin + heading * length;
        var fx = CreateVisual("Returning lance", origin, heading, 1f, 1f);
        var hit = new HashSet<EnemyHealth>();
        for (int leg = 0; leg < 2; leg++)
        {
            Vector2 from = leg == 0 ? origin : end;
            Vector2 to = leg == 0 ? end : origin;
            var clip = leg == 0 ? GunNewSkillArt.Clip.RecallOut : GunNewSkillArt.Clip.RecallReturn;
            fx.sprite = newArt.Frame(clip, 0);
            fx.transform.SetPositionAndRotation(from, Rotation(heading * (leg == 0 ? 1f : -1f)));
            hit.Clear();
            Vector2 previous = from;
            float elapsed = 0f;
            while (elapsed < 0.48f)
            {
                yield return null;
                elapsed += Time.deltaTime;
                Vector2 next = Vector2.Lerp(from, to, Mathf.Clamp01(elapsed / 0.48f));
                foreach (EnemyHealth enemy in targets)
                {
                    if (!Alive(enemy) || hit.Contains(enemy)
                        || DistanceSquared(enemy.transform.position, previous, next) > 0.42f * 0.42f) continue;
                    hit.Add(enemy);
                    enemy.TakeDamage(leg == 0 ? 15 : 19 + 4 * ranks.x);
                }
                fx.sprite = newArt.Loop(clip, elapsed, 30f, 0, 14);
                fx.transform.position = previous = next;
            }
            var impact = CreateVisual("Recall impact", to, Vector2.right, 1f, 1f);
            StartCoroutine(PixelPlay(impact, GunNewSkillArt.Clip.Impact, 36, 30f));
        }
        Destroy(fx.gameObject);
    }

    private IEnumerator SkyLance(Vector2 position)
    {
        float radius = 0.85f + 0.17f * ranks.z;
        var fx = CreateVisual("Sky lance", position, Vector2.right, radius / 0.95f, radius / 0.95f);
        bool struck = false;
        const float duration = 1.6f;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            int frame = Mathf.Min(87, Mathf.FloorToInt(elapsed / duration * 88f));
            fx.sprite = newArt.Frame(GunNewSkillArt.Clip.Holy, frame);
            if (!struck && frame >= 17)
            {
                struck = true;
                Area(position, radius, 38 + 6 * ranks.x);
            }
            yield return null;
        }
        if (!struck) Area(position, radius, 38 + 6 * ranks.x);
        Destroy(fx.gameObject);
    }

    private void Area(Vector2 center, float radius, int damage)
    {
        foreach (EnemyHealth enemy in targets)
            if (Alive(enemy) && ((Vector2)enemy.transform.position - center).sqrMagnitude <= radius * radius)
                enemy.TakeDamage(damage);
    }

    private static bool Alive(EnemyHealth enemy) => enemy != null && enemy.CurrentHealth > 0;

    private static float DistanceSquared(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / Mathf.Max(segment.sqrMagnitude, 0.0001f));
        return (point - (start + segment * t)).sqrMagnitude;
    }

    private SpriteRenderer CreateVisual(string label, Vector2 position, Vector2 heading, float width, float height)
    {
        var obj = new GameObject(label);
        obj.transform.SetParent(visuals, false);
        obj.transform.SetPositionAndRotation(position, Rotation(heading));
        obj.transform.localScale = new Vector3(width, height, 1f);
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 8;
        return renderer;
    }

    private static Quaternion Rotation(Vector2 heading) =>
        Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, heading));

    private IEnumerator Follow(SpriteRenderer fx, EnemyHealth target)
    {
        while (fx != null && Alive(target))
        {
            fx.transform.position = target.transform.position;
            yield return null;
        }
        if (fx != null) Destroy(fx.gameObject);
    }

    private IEnumerator Play(SpriteRenderer renderer, Art art, int fps)
    {
        Sprite[] clip = GetFrames(art);
        for (float elapsed = 0f; renderer != null && elapsed < clip.Length / (float)fps; elapsed += Time.deltaTime)
        {
            renderer.sprite = clip[Mathf.FloorToInt(elapsed * fps)];
            yield return null;
        }
        if (renderer != null) Destroy(renderer.gameObject);
    }

    private Sprite[] GetFrames(Art art)
    {
        int index = (int)art;
        if (frames[index] != null) return frames[index];
        var clip = new Sprite[32];
        for (int i = 0; i < clip.Length; i++)
            clip[i] = Sprite.Create(sheets[index], FrameRect(i), Vector2.one * 0.5f, 256f, 0, SpriteMeshType.FullRect);
        return frames[index] = clip;
    }

    private IEnumerator PixelPlay(SpriteRenderer renderer, GunNewSkillArt.Clip clip, int count, float fps, int firstFrame = 0)
    {
        for (float elapsed = 0f; renderer != null && elapsed < (count - firstFrame) / fps; elapsed += Time.deltaTime)
        {
            renderer.sprite = newArt.Frame(clip, Mathf.Min(count - 1, firstFrame + Mathf.FloorToInt(elapsed * fps)));
            yield return null;
        }
        if (renderer != null) Destroy(renderer.gameObject);
    }

    private static Rect FrameRect(int index) =>
        new Rect(index % 8 * 256, (3 - index / 8) * 256, 256, 256);
}
