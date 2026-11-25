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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyController>();
    }

    internal void ApplySlow(float fraction)
    {
        if (movement == null || fraction < slowFraction) return;
        if (slowFraction == 0f) normalSpeed = movement.MoveSpeed;
        slowFraction = fraction;
        slowUntil = Time.time + SlowDuration;
        movement.SetMoveSpeed(normalSpeed * (1f - slowFraction));
    }

    private void Update()
    {
        if (slowFraction == 0f || Time.time < slowUntil) return;
        movement.SetMoveSpeed(normalSpeed);
        slowFraction = 0f;
    }

    internal bool IgnorePush { get; set; }

    public void Push(Vector2 displacement)
    {
        if (IgnorePush) return;
        Vector2 position = body.position + displacement;
        // Keep practice targets inside the shooting area.
        body.position = new Vector2(Mathf.Clamp(position.x, -7.5f, 7.5f),
            Mathf.Clamp(position.y, -4.5f, 4.5f));
    }

}
