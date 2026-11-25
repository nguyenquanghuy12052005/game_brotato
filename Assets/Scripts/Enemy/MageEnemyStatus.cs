using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public sealed class MageEnemyStatus : MonoBehaviour
{
    private EnemyHealth health;
    private MageEffectLibrary effects;
    private MageEffectPlayer burnEffect;
    private MageEffectPlayer curseEffect;
    private SpriteRenderer iceMark;
    private int iceMarkFromFrame, iceMarkToFrame;
    private float iceMarkGrowthStart;
    private float burnUntil, burnDamage, burnProgress;
    private float curseUntil, curseDamage, curseProgress, curseSlow, curseDamageReduction;
    private float chillUntil, chillSlow, iceMarksUntil;
    private float gravityUntil, gravitySlow, dazzleUntil;
    private Vector2 gravityCenter;
    private float gravityPullUntil, gravityPullSpeed;
    private float frozenUntil, stoppedUntil;
    private int iceMarks, coldFocusStacks, pendingDamage;

    public event Action<MageEnemyStatus> Died;
    public EnemyHealth Health => health;
    public bool IsBoss => GetComponent<MageBossMarker>() != null;
    public bool IsBurning => Time.time < burnUntil;
    public bool IsCursed => Time.time < curseUntil;
    public bool IsChilled => Time.time < chillUntil;
    public bool IsFrozen => Time.time < frozenUntil;
    public int ColdFocusStacks => Time.time < chillUntil ? coldFocusStacks : 0;
    public float MoveMultiplier
    {
        get
        {
            if (IsBoss) return 1f;
            if (IsFrozen || Time.time < stoppedUntil) return 0f;
            float slow = 0f;
            if (IsChilled) slow = Mathf.Max(slow, chillSlow);
            if (IsCursed) slow = Mathf.Max(slow, curseSlow);
            if (Time.time < gravityUntil) slow = Mathf.Max(slow, gravitySlow);
            return Mathf.Clamp01(1f - slow);
        }
    }
    public float OutgoingDamageMultiplier
    {
        get
        {
            if (!IsBoss && (IsFrozen || Time.time < stoppedUntil)) return 0f;
            float reduction = IsCursed ? curseDamageReduction : 0f;
            if (Time.time < dazzleUntil) reduction = Mathf.Max(reduction, 0.25f);
            return Mathf.Clamp01(1f - reduction);
        }
    }
    public Vector2 GravityVelocity => !IsBoss && Time.time < gravityPullUntil
        ? (gravityCenter - (Vector2)transform.position).normalized * gravityPullSpeed : Vector2.zero;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        health.Died += OnHealthDied;
    }

    public void SetEffects(MageEffectLibrary library) { effects = library; }

    private void Update()
    {
        float now = Time.time;
        if (now >= stoppedUntil && pendingDamage > 0)
        {
            int damage = pendingDamage;
            pendingDamage = 0;
            health.TakeDamage(damage);
            if (health.CurrentHealth <= 0) return;
        }

        if (IsBurning)
        {
            burnProgress += burnDamage * Time.deltaTime;
            int damage = Mathf.FloorToInt(burnProgress);
            if (damage > 0) { burnProgress -= damage; TakeMagicDamage(damage); }
        }
        else { burnProgress = 0f; RemoveEffect(ref burnEffect); }

        if (IsCursed)
        {
            curseProgress += curseDamage * Time.deltaTime;
            int damage = Mathf.FloorToInt(curseProgress);
            if (damage > 0) { curseProgress -= damage; TakeMagicDamage(damage); }
        }
        else { curseProgress = 0f; RemoveEffect(ref curseEffect); }

        if (now >= iceMarksUntil && iceMarks > 0) { iceMarks = 0; RemoveIceMark(); }
        if (iceMark != null && iceMark.sprite != null && effects != null)
        {
            float growth = Mathf.Clamp01((now - iceMarkGrowthStart) / 0.25f);
            int frame = Mathf.RoundToInt(Mathf.Lerp(iceMarkFromFrame, iceMarkToFrame, growth));
            iceMark.sprite = effects.GetFrame(MageEffectId.Freeze, frame);
        }
        if (now >= chillUntil) coldFocusStacks = 0;
    }

    public void TakeMagicDamage(int damage)
    {
        if (damage <= 0 || health.CurrentHealth <= 0) return;
        if (!IsBoss && Time.time < stoppedUntil) pendingDamage += damage;
        else health.TakeDamage(damage);
    }

    public void ApplyBurn(float damagePerSecond, float duration)
    {
        if (health.CurrentHealth <= 0) return;
        burnDamage = Mathf.Max(burnDamage, damagePerSecond);
        burnUntil = Time.time + duration;
        if (burnEffect == null && effects != null)
            burnEffect = effects.Play(MageEffectId.Burn, transform.position, 0.75f, transform, 15);
    }

    public void ApplyCurse(float damagePerSecond, float duration, float slow, float damageReduction)
    {
        if (health.CurrentHealth <= 0) return;
        curseDamage = Mathf.Max(curseDamage, damagePerSecond);
        curseSlow = Mathf.Max(curseSlow, slow);
        curseDamageReduction = Mathf.Max(curseDamageReduction, damageReduction);
        curseUntil = Time.time + duration;
        if (curseEffect == null && effects != null)
            curseEffect = effects.Play(MageEffectId.Curse, transform.position, 0.85f, transform, 16);
    }

    public bool ApplyIceHit(float slow, float slowDuration, float freezeDuration, bool focus)
    {
        if (IsBoss || IsFrozen || health.CurrentHealth <= 0) return false;
        chillSlow = Mathf.Max(chillSlow, slow);
        chillUntil = Time.time + slowDuration;
        iceMarksUntil = Time.time + 6f;
        iceMarks++;
        if (focus) coldFocusStacks = Mathf.Min(2, coldFocusStacks + 1);
        if (iceMarks < 6) { ShowIceMark(); return false; }

        iceMarks = 0;
        coldFocusStacks = 0;
        frozenUntil = Time.time + freezeDuration;
        RemoveIceMark();
        if (effects != null) effects.Play(MageEffectId.Freeze, transform.position, 0.55f, transform, 17);
        return true;
    }

    public void AddIceMark(float slow, float duration)
    {
        if (IsBoss || IsFrozen || health.CurrentHealth <= 0) return;
        chillSlow = Mathf.Max(chillSlow, slow);
        chillUntil = Time.time + duration;
        iceMarksUntil = Time.time + 6f;
        iceMarks = Mathf.Min(5, iceMarks + 1);
        ShowIceMark();
    }

    public void SetGravitySlow(float slow) { if (!IsBoss) { gravitySlow = slow; gravityUntil = Time.time + 0.15f; } }
    public void SetGravityPull(Vector2 center, float speed)
    {
        if (IsBoss) return;
        gravityCenter = center;
        gravityPullSpeed = Mathf.Max(0f, speed);
        gravityPullUntil = Time.time + 0.15f;
    }
    public void SetDazzled() { if (!IsBoss) dazzleUntil = Time.time + 0.3f; }
    public void StopTime(float duration) { if (!IsBoss) stoppedUntil = Mathf.Max(stoppedUntil, Time.time + duration); }

    private void ShowIceMark()
    {
        if (effects == null) return;
        if (iceMark == null)
        {
            var child = new GameObject("Ice Mark");
            child.transform.SetParent(transform, false);
            child.transform.localScale = Vector3.one * 0.75f;
            iceMark = child.AddComponent<SpriteRenderer>();
            iceMark.sortingOrder = 16;
        }
        int[] formationFrames = { 10, 19, 24, 30, 35 };
        iceMarkFromFrame = iceMark.sprite == null ? 0 : iceMarkToFrame;
        iceMarkToFrame = formationFrames[Mathf.Clamp(iceMarks - 1, 0, 4)];
        iceMarkGrowthStart = Time.time;
        iceMark.sprite = effects.GetFrame(MageEffectId.Freeze, iceMarkFromFrame);
    }

    private void RemoveIceMark()
    {
        if (iceMark != null) Destroy(iceMark.gameObject);
        iceMark = null;
    }

    private void RemoveEffect(ref MageEffectPlayer effect)
    {
        if (effect != null) Destroy(effect.gameObject);
        effect = null;
    }

    private void OnHealthDied(EnemyHealth _) { Died?.Invoke(this); }
    private void OnDestroy()
    {
        if (health != null) health.Died -= OnHealthDied;
    }
}
