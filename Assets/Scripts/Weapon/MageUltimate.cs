using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MageCombat))]
public sealed class MageUltimate : MonoBehaviour
{
    private MageCombat combat;
    private MageAim aim;
    private PlayerHealth health;
    private float nextUse;
    private bool chargingOrb;
    private int orbCharge;
    private MageEffectPlayer orbVisual;
    private Coroutine orbTimer;

    public float Remaining => Mathf.Max(0f, nextUse - Time.time);
    public int OrbCharge => orbCharge;

    public void ResetPreviewUltimate()
    {
        chargingOrb = false;
        orbCharge = 0;
        if (orbTimer != null) StopCoroutine(orbTimer);
        if (orbVisual != null) Destroy(orbVisual.gameObject);
        orbTimer = null;
        orbVisual = null;
        nextUse = 0f;
    }

    private void Awake()
    {
        combat = GetComponent<MageCombat>();
        aim = GetComponent<MageAim>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (health.CurrentHealth <= 0 || Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;
        if (chargingOrb) { ReleaseOrb(); return; }
        if (Time.time < nextUse) return;
        switch (combat.Loadout.Ultimate)
        {
            case MageUltimateKind.TimeStop: StartCoroutine(TimeStop()); break;
            case MageUltimateKind.StarFall: StartCoroutine(StarFall()); break;
            case MageUltimateKind.ArcaneOrb: StartOrb(); break;
        }
    }

    public void NotifyHit()
    {
        if (chargingOrb) orbCharge = Mathf.Min(6, orbCharge + 1);
    }

    private IEnumerator TimeStop()
    {
        combat.Effects.Play(MageEffectId.TimeStop, transform.position, 1.4f);
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            combat.Status(enemy)?.StopTime(3f);
        nextUse = Time.time + 63f;
        yield return new WaitForSeconds(3f);
        // Queued spell damage is applied by MageEnemyStatus when the stop ends.
    }

    private IEnumerator StarFall()
    {
        return StarFallAt(aim.PointAtMouse(8f));
    }

    private IEnumerator StarFallAt(Vector2 center)
    {
        MageEffectPlayer marker = combat.Effects.Play(MageEffectId.RuneCircle, center, 1.5f);
        nextUse = Time.time + 47f;
        for (int i = 0; i < 5; i++)
        {
            EnemyHealth target = combat.Nearest(center, 3f);
            if (target != null)
            {
                Vector2 impact = target.transform.position;
                combat.Effects.Play(MageEffectId.StarFall, impact, 0.9f);
                foreach (EnemyHealth enemy in combat.Enemies(impact, 1f)) combat.Damage(enemy, 12);
            }
            yield return new WaitForSeconds(0.4f);
        }
        if (marker != null) Destroy(marker.gameObject);
    }

    private void StartOrb()
    {
        chargingOrb = true;
        orbCharge = 0;
        orbVisual = combat.Effects.Play(MageEffectId.ArcaneOrb,
            transform.position + Vector3.up * 0.9f, 0.9f, transform);
        orbTimer = StartCoroutine(ReleaseAfterTimeout());
    }

    private IEnumerator ReleaseAfterTimeout()
    {
        yield return new WaitForSeconds(8f);
        ReleaseOrb();
    }

    private void ReleaseOrb()
    {
        if (!chargingOrb) return;
        chargingOrb = false;
        if (orbTimer != null) StopCoroutine(orbTimer);
        if (orbVisual != null) Destroy(orbVisual.gameObject);
        int count = 2 + orbCharge;
        for (int i = 0; i < count; i++)
        {
            EnemyHealth target = combat.Nearest(transform.position, 8f);
            if (target == null) break;
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            combat.Projectile(MageEffectId.ArcaneMissiles, aim.CastPoint, direction,
                target, 12f, 2.5f, enemy => combat.Damage(enemy, 12), 0.6f);
        }
        nextUse = Time.time + 45f;
    }
}
