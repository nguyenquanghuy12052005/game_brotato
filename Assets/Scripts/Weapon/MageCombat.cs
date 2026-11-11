using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MageLoadout), typeof(MageAim), typeof(MageEffectLibrary))]
public sealed class MageCombat : MonoBehaviour
{
    [SerializeField] private float fireInterval = 0.8f;
    [SerializeField] private float iceInterval = 1f;
    [SerializeField] private float lightInterval = 2.5f;
    [SerializeField] private float darkInterval = 1f;
    private MageLoadout loadout;
    private MageAim aim;
    private MageEffectLibrary effects;
    private PlayerHealth health;
    private PlayerController movement;
    private MageUltimate ultimate;
    private float nextAttack, nextStarter;
    private int fireShots;
    private readonly HashSet<MageEnemyStatus> tracked = new HashSet<MageEnemyStatus>();
    private readonly Dictionary<MageEnemyStatus, int> lightHits = new Dictionary<MageEnemyStatus, int>();
    private readonly Dictionary<MageEnemyStatus, int> darkHits = new Dictionary<MageEnemyStatus, int>();

    public float StarterRemaining => Mathf.Max(0f, nextStarter - Time.time);
    public MageEffectLibrary Effects => effects;
    public MageLoadout Loadout => loadout;

    public void ResetPreviewStarter() { nextStarter = 0f; }
    public void ResetPreviewElement() { nextAttack = 0f; fireShots = 0; }

    private void Awake()
    {
        loadout = GetComponent<MageLoadout>();
        aim = GetComponent<MageAim>();
        effects = GetComponent<MageEffectLibrary>();
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerController>();
        ultimate = GetComponent<MageUltimate>();
    }

    private void Update()
    {
        if (health.CurrentHealth <= 0) return;
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame && Time.time >= nextStarter)
            UseStarter();
        if (Time.time < nextAttack) return;
        switch (loadout.Element)
        {
            case MageElement.Fire: Fire(); nextAttack = Time.time + fireInterval; break;
            case MageElement.Ice: Ice(); nextAttack = Time.time + iceInterval; break;
            case MageElement.Light: StartCoroutine(Light()); nextAttack = Time.time + lightInterval; break;
            case MageElement.Dark: Dark(); nextAttack = Time.time + darkInterval; break;
        }
    }

    public MageEnemyStatus Status(EnemyHealth enemy)
    {
        if (enemy == null || enemy.CurrentHealth <= 0) return null;
        MageEnemyStatus status = enemy.GetComponent<MageEnemyStatus>();
        if (status == null) status = enemy.gameObject.AddComponent<MageEnemyStatus>();
        status.SetEffects(effects);
        if (tracked.Add(status)) status.Died += OnEnemyDied;
        return status;
    }

    public void Damage(EnemyHealth enemy, int amount, bool countsForOrb = true)
    {
        MageEnemyStatus status = Status(enemy);
        if (status == null || amount <= 0) return;
        status.TakeMagicDamage(amount);
        if (countsForOrb && ultimate != null) ultimate.NotifyHit();
    }

    public List<EnemyHealth> Enemies(Vector2 center, float radius)
    {
        var found = new List<EnemyHealth>();
        var seen = new HashSet<EnemyHealth>();
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(center, radius))
        {
            EnemyHealth enemy = collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null && enemy.CurrentHealth > 0 && seen.Add(enemy)) found.Add(enemy);
        }
        return found;
    }

    public EnemyHealth Nearest(Vector2 center, float radius, bool preferUncursed = false)
    {
        EnemyHealth best = null;
        float bestDistance = radius * radius;
        foreach (EnemyHealth enemy in Enemies(center, radius))
        {
            if (preferUncursed && enemy.GetComponent<MageEnemyStatus>()?.IsCursed == true) continue;
            float distance = ((Vector2)enemy.transform.position - center).sqrMagnitude;
            if (distance < bestDistance) { best = enemy; bestDistance = distance; }
        }
        return best;
    }

    public MageProjectile Projectile(MageEffectId effect, Vector2 origin, Vector2 direction,
        EnemyHealth target, float speed, float life, System.Action<EnemyHealth> onHit, float scale = 0.7f)
    {
        MageEffectPlayer visual = effects.Play(effect, origin, scale, null, 12, true);
        if (visual == null) return null;
        MageProjectile projectile = visual.gameObject.AddComponent<MageProjectile>();
        projectile.Initialize(direction, target, speed, life, onHit);
        return projectile;
    }

    private void UseStarter()
    {
        switch (loadout.StarterSkill)
        {
            case MageStarterSkill.Heal:
                if (!health.Heal(25)) return;
                effects.Play(MageEffectId.Heal, transform.position, 0.8f, transform);
                nextStarter = Time.time + 25f;
                break;
            case MageStarterSkill.Shockwave:
                effects.Play(MageEffectId.Shockwave, transform.position, 1.25f);
                foreach (EnemyHealth enemy in Enemies(transform.position, 2.5f))
                {
                    MageEnemyStatus status = Status(enemy);
                    Damage(enemy, 15);
                    if (status != null && !status.IsBoss && enemy != null && enemy.CurrentHealth > 0)
                    {
                        Vector2 away = ((Vector2)enemy.transform.position - (Vector2)transform.position).normalized;
                        enemy.transform.position += (Vector3)(away * 1f);
                    }
                }
                nextStarter = Time.time + 12f;
                break;
            case MageStarterSkill.Gravity:
                StartCoroutine(Gravity(aim.PointAtMouse(7f)));
                nextStarter = Time.time + 20f;
                break;
        }
    }

    private IEnumerator Gravity(Vector2 center)
    {
        MageEffectPlayer visual = effects.Play(MageEffectId.Gravity, center, 1.6f);
        float end = Time.time + 4f;
        var progress = new Dictionary<MageEnemyStatus, float>();
        while (Time.time < end)
        {
            foreach (EnemyHealth enemy in Enemies(center, 2.5f))
            {
                MageEnemyStatus status = Status(enemy);
                if (status == null) continue;
                status.SetGravitySlow(0.35f);
                progress.TryGetValue(status, out float value);
                value += 3f * Time.deltaTime;
                int damage = Mathf.FloorToInt(value);
                if (damage > 0) { Damage(enemy, damage); value -= damage; }
                progress[status] = value;
            }
            yield return null;
        }
        if (visual != null) Destroy(visual.gameObject);
    }

    private void Fire()
    {
        fireShots++;
        LaunchFire(aim.Direction, 1f);
        if (loadout.ExtraFireballs && fireShots % 3 == 0)
        {
            LaunchFire(Quaternion.Euler(0f, 0f, 16f) * aim.Direction, 0.6f);
            LaunchFire(Quaternion.Euler(0f, 0f, -16f) * aim.Direction, 0.6f);
        }
    }

    private void LaunchFire(Vector2 direction, float factor)
    {
        Projectile(MageEffectId.Fireball, aim.CastPoint, direction, null, 9f, 2f,
            enemy => FireExplosion(enemy.transform.position, factor), 0.7f);
    }

    private void FireExplosion(Vector2 center, float factor)
    {
        effects.Play(MageEffectId.Explosion, center, 0.65f);
        foreach (EnemyHealth enemy in Enemies(center, 1.5f * (1f + 0.2f * loadout.BlastRadius)))
        {
            MageEnemyStatus status = Status(enemy);
            float bonus = loadout.FocusBurning && status.IsBurning ? 1.3f : 1f;
            Damage(enemy, Mathf.RoundToInt(20f * (1f + 0.15f * loadout.BlastPower) * factor * bonus));
            if (status != null && enemy != null && enemy.CurrentHealth > 0)
                status.ApplyBurn(4f * (1f + 0.25f * loadout.BurnPower), loadout.LongBurn ? 5f : 3f);
        }
    }

    private void Ice()
    {
        Vector2 origin = transform.position;
        MageEffectPlayer breath = effects.Play(MageEffectId.IceBreath,
            aim.CastPoint + aim.Direction * 0.55f, 0.9f);
        if (breath != null)
            breath.transform.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(aim.Direction.y, aim.Direction.x) * Mathf.Rad2Deg);
        foreach (EnemyHealth enemy in Enemies(origin, 3f))
        {
            Vector2 delta = (Vector2)enemy.transform.position - origin;
            if (Vector2.Angle(aim.Direction, delta) > 35f) continue;
            MageEnemyStatus status = Status(enemy);
            if (status == null) continue;
            float bonus = 1f + (status.IsChilled ? 0.15f * loadout.ChilledDamage : 0f)
                + (status.IsFrozen && loadout.Brittle ? 0.35f : 0f)
                + (loadout.ColdFocus ? 0.1f * status.ColdFocusStacks : 0f);
            Damage(enemy, Mathf.RoundToInt(15f * bonus));
            if (enemy == null || enemy.CurrentHealth <= 0) continue;
            float slow = loadout.DeepCold ? 0.45f : 0.3f + 0.1f * loadout.StrongerSlow;
            bool frozen = status.ApplyIceHit(slow, loadout.DeepCold ? 3.5f : 2.5f,
                2f + 0.5f * loadout.LongerFreeze, loadout.ColdFocus);
            if (frozen && loadout.SpreadCold)
            {
                int applied = 0;
                foreach (EnemyHealth nearby in Enemies(enemy.transform.position, 2.5f))
                {
                    if (nearby == enemy) continue;
                    Status(nearby)?.AddIceMark(slow, 2.5f);
                    if (++applied >= 2) break;
                }
            }
        }
    }

    private IEnumerator Light()
    {
        Vector2 center = aim.PointAtMouse(7f);
        effects.Play(MageEffectId.LightPillar, center, 0.9f);
        float radius = 1.2f * (1f + 0.2f * loadout.PillarRadius);
        int pulses = 4 + loadout.ExtraPulses;
        for (int i = 0; i < pulses; i++)
        {
            LightPulse(center, radius, 1f);
            yield return new WaitForSeconds(0.25f);
        }
        if (loadout.Afterglow) LightPulse(center, radius, 0.5f);
        if (movement != null) movement.ExternalSpeedMultiplier = 1f;
    }

    private void LightPulse(Vector2 center, float radius, float factor)
    {
        if (loadout.GuideLight && movement != null)
            movement.ExternalSpeedMultiplier = Vector2.Distance(transform.position, center) <= radius ? 1.25f : 1f;
        foreach (EnemyHealth enemy in Enemies(center, radius))
        {
            MageEnemyStatus status = Status(enemy);
            Damage(enemy, Mathf.RoundToInt(8f * (1f + 0.15f * loadout.PulsePower) * factor));
            if (status == null) continue;
            if (loadout.Dazzle) status.SetDazzled();
            if (!loadout.Judgement || enemy == null || enemy.CurrentHealth <= 0) continue;
            lightHits.TryGetValue(status, out int hits);
            if (++hits >= 3)
            {
                effects.Play(MageEffectId.Smite, enemy.transform.position, 0.75f);
                Damage(enemy, 18);
                hits = 0;
            }
            lightHits[status] = hits;
        }
    }

    private void Dark()
    {
        int cap = Mathf.Max(loadout.ManySeals ? 5 : 3, 3 + loadout.ExtraCurseTargets);
        int cursed = 0;
        foreach (EnemyHealth enemy in Enemies(transform.position, 7f))
            if (enemy.GetComponent<MageEnemyStatus>()?.IsCursed == true) cursed++;
        EnemyHealth target = Nearest(transform.position, 7f, cursed < cap);
        if (target == null) target = Nearest(transform.position, 7f);
        if (target == null) return;
        Projectile(MageEffectId.DarkMissile, aim.CastPoint, aim.Direction, target, 8f, 2f, hit =>
        {
            MageEnemyStatus status = Status(hit);
            Damage(hit, 8);
            if (hit == null || hit.CurrentHealth <= 0 || status == null) return;
            status.ApplyCurse(5f * (1f + 0.25f * loadout.CursePower), 4f,
                Mathf.Max(loadout.Frailty ? 0.25f : 0f, 0.15f + 0.06f * loadout.CurseWeakness),
                Mathf.Max(loadout.Frailty ? 0.2f : 0f, 0.06f * loadout.CurseWeakness));
            darkHits.TryGetValue(status, out int hits);
            if (loadout.Torment && ++hits >= 3)
            {
                effects.Play(MageEffectId.Implosion, hit.transform.position, 0.75f);
                Damage(hit, 25);
                hits = 0;
            }
            darkHits[status] = hits;
        });
    }

    private void OnEnemyDied(MageEnemyStatus dead)
    {
        if (loadout.Element == MageElement.Fire && loadout.DeathExplosion && dead.IsBurning)
        {
            Vector2 center = dead.transform.position;
            effects.Play(MageEffectId.Explosion, center, 0.5f);
            foreach (EnemyHealth enemy in Enemies(center, 1.8f))
                if (enemy != dead.Health) Damage(enemy, 10);
        }
        if (loadout.Element == MageElement.Dark && loadout.Testament && dead.IsCursed)
        {
            EnemyHealth next = Nearest(dead.transform.position, 4f, true);
            if (next != null) Status(next).ApplyCurse(5f * (1f + 0.25f * loadout.CursePower), 4f,
                0.15f + 0.06f * loadout.CurseWeakness, 0.06f * loadout.CurseWeakness);
        }
        dead.Died -= OnEnemyDied;
        tracked.Remove(dead);
        lightHits.Remove(dead);
        darkHits.Remove(dead);
    }

    private void OnDisable()
    {
        if (movement != null) movement.ExternalSpeedMultiplier = 1f;
    }
}
