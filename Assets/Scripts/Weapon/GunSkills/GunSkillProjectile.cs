using System.Collections.Generic;
using UnityEngine;

public enum GunSkill { None, Ricochet, WeakPoint, DeathScatter }
public enum GunBranch { Basic, Spread, Burst, Piercing }

internal struct GunShotEffects
{
    public GunLevel11.Volley Volley;
    public GunCoreTwo.Volley CoreVolley;
    public bool Heavy;
    public bool Piercing;
    public int Ricochets;
    public int Fragments;
    public float Knockback;
    public int DamagePerPierce;
    public float Slow;
    public float ShockwaveRadius;
}

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public sealed class GunSkillProjectile : MonoBehaviour
{
    private readonly HashSet<int> hitEnemies = new HashSet<int>();
    private GunSkillWeapon weapon;
    private Rigidbody2D body;
    private SpriteRenderer flame;
    private SpriteRenderer artwork;
    private GunCoreTwo coreTwo;
    private bool crossedPortal;
    private EnemyHealth ricochetTarget;
    private Vector2 direction;
    private GunShotEffects effects;
    private int remainingRicochets;
    private bool consumed;
    private int damage;
    private float flightTime;

    internal void Launch(GunSkillWeapon owner, Vector2 heading, int hitDamage,
        GunShotEffects effectsOnHit, SpriteRenderer flameRenderer, GunCoreTwo core, SpriteRenderer visual)
    {
        weapon = owner;
        body = GetComponent<Rigidbody2D>();
        direction = heading;
        effects = effectsOnHit;
        remainingRicochets = effects.Ricochets;
        damage = hitDamage;
        flame = flameRenderer;
        coreTwo = core;
        artwork = visual;
        Destroy(gameObject, 2.5f);
    }

    private void Update()
    {
        if (flame == null && !effects.Heavy) return;
        flightTime += Time.deltaTime;
        if (flame != null) flame.sprite = weapon.GetFlameFrame(flightTime);
        if (effects.Heavy) artwork.sprite = coreTwo.Frame(GunCoreTwo.Art.Heavy, flightTime, 32);
    }

    private void FixedUpdate()
    {
        if (ricochetTarget != null && ricochetTarget.CurrentHealth > 0)
        {
            Vector2 toTarget = (Vector2)ricochetTarget.transform.position - body.position;
            if (toTarget.sqrMagnitude > 0.001f)
                direction = toTarget.normalized;
        }

        Vector2 next = body.position + direction * ((effects.Heavy ? 15f : 10f) * Time.fixedDeltaTime);
        if (effects.Piercing && !crossedPortal && coreTwo != null &&
            coreTwo.CrossPortal(effects.CoreVolley, body.position, next, direction, out Vector2 exit, out Vector2 back))
        {
            crossedPortal = true;
            damage += effects.DamagePerPierce;
            body.position = exit;
            direction = back;
            ricochetTarget = null;
            body.rotation = Vector2.SignedAngle(Vector2.right, direction);
            return;
        }
        body.MoveRotation(Vector2.SignedAngle(Vector2.right, direction));
        body.MovePosition(next);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null || enemy.CurrentHealth <= 0)
            return;
        if (ricochetTarget != null && ricochetTarget.CurrentHealth > 0 && enemy != ricochetTarget)
            return;
        if (!hitEnemies.Add(enemy.GetInstanceID()))
            return;

        ricochetTarget = null;
        Vector2 impact = enemy.transform.position;
        enemy.TakeDamage(weapon.DamageAgainst(enemy, damage));
        if (effects.Volley != null) weapon.RegisterHit(effects.Volley, enemy);
        if (effects.CoreVolley != null) coreTwo.Hit(effects.CoreVolley, enemy, direction, crossedPortal);

        if (effects.Fragments > 0 && enemy.CurrentHealth == 0)
            weapon.ScatterFrom(impact, effects.Fragments);

        if (enemy.CurrentHealth > 0 && enemy.TryGetComponent<GunSkillTargetEffects>(out var targetEffects))
        {
            if (effects.Knockback > 0f) targetEffects.Push(direction * effects.Knockback);
            if (effects.Slow > 0f) targetEffects.Slow(effects.Slow);
        }

        if (effects.Piercing)
        {
            damage += effects.DamagePerPierce;
            if (effects.ShockwaveRadius > 0f && hitEnemies.Count == 2)
                weapon.Shockwave(impact, effects.ShockwaveRadius, enemy);
        }

        if (remainingRicochets > 0)
        {
            ricochetTarget = FindNearestEnemy(impact);
            if (ricochetTarget != null)
            {
                remainingRicochets--;
                Vector2 toTarget = (Vector2)ricochetTarget.transform.position - body.position;
                if (toTarget.sqrMagnitude > 0.001f)
                    direction = toTarget.normalized;
                weapon.ShowImpact(impact, new Color(0.55f, 1f, 0.55f));
                return;
            }
        }

        if (effects.Piercing)
        {
            weapon.ShowImpact(impact, new Color(0.45f, 0.85f, 1f));
            return;
        }

        consumed = true;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // A new ricochet target may already overlap this projectile.
        if (ricochetTarget != null) OnTriggerEnter2D(other);
    }

    private EnemyHealth FindNearestEnemy(Vector2 impact)
    {
        EnemyHealth nearest = null;
        float nearestDistance = float.PositiveInfinity;
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy.CurrentHealth <= 0 || hitEnemies.Contains(enemy.GetInstanceID())) continue;
            float distance = ((Vector2)enemy.transform.position - impact).sqrMagnitude;
            if (distance >= nearestDistance) continue;
            nearest = enemy;
            nearestDistance = distance;
        }
        return nearest;
    }
}
