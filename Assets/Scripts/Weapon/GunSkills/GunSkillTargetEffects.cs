using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class GunSkillTargetEffects : MonoBehaviour
{
    private const float SlowDuration = 1.5f;
    private Rigidbody2D body;
    private EnemyController movement;
    private float normalSpeed;
    private float slowFraction;
    private float slowUntil;
    private float poisonSlow;
    private float stunUntil;

    internal bool IsStunned => Time.time < stunUntil;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyController>();
    }

    internal void ApplySlow(float fraction)
    {
        if (movement == null || fraction < slowFraction) return;
        CaptureSpeed();
        slowFraction = fraction;
        slowUntil = Time.time + SlowDuration;
        RefreshSpeed();
    }

    internal void SetPoisonSlow(float fraction)
    {
        if (movement == null || fraction == poisonSlow) return;
        CaptureSpeed();
        poisonSlow = fraction;
        RefreshSpeed();
    }

    internal void Stun(float duration)
    {
        CaptureSpeed();
        stunUntil = Mathf.Max(stunUntil, Time.time + duration);
        body.linearVelocity = Vector2.zero;
        RefreshSpeed();
    }

    internal void ClearStun()
    {
        if (stunUntil == 0f) return;
        stunUntil = 0f;
        RefreshSpeed();
    }

    private void CaptureSpeed()
    {
        if (movement != null && slowFraction == 0f && poisonSlow == 0f && stunUntil == 0f)
            normalSpeed = movement.MoveSpeed;
    }

    private void RefreshSpeed()
    {
        if (movement != null)
            movement.SetMoveSpeed(IsStunned ? 0f : normalSpeed * (1f - Mathf.Max(slowFraction, poisonSlow)));
    }

    private void Update()
    {
        bool slowExpired = slowFraction != 0f && Time.time >= slowUntil;
        bool stunExpired = stunUntil != 0f && !IsStunned;
        if (!slowExpired && !stunExpired) return;
        if (slowExpired) slowFraction = 0f;
        if (stunExpired) stunUntil = 0f;
        RefreshSpeed();
    }

    internal bool IgnorePush { get; set; }

    public void Push(Vector2 displacement)
    {
        if (IgnorePush) return;
        MoveWithinArena(body.position + displacement);
    }

    internal void PullToward(Vector2 center, float radius, float step)
    {
        Vector2 position = body.position;
        if ((center - position).sqrMagnitude > radius * radius) return;
        // Bosses resist ordinary knockback and are drawn into the tornado more slowly.
        MoveWithinArena(Vector2.MoveTowards(position, center, step * (IgnorePush ? 0.55f : 1f)));
    }

    private void MoveWithinArena(Vector2 position)
    {
        // Keep practice targets inside the shooting area.
        body.position = new Vector2(Mathf.Clamp(position.x, -7.5f, 7.5f),
            Mathf.Clamp(position.y, -4.5f, 4.5f));
    }

}
