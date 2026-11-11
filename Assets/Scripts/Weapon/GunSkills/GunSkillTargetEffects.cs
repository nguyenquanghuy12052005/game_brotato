using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyController))]
public sealed class GunSkillTargetEffects : MonoBehaviour
{
    private const float SlowDuration = 1.5f;
    private Rigidbody2D body;
    private EnemyController movement;
    private SpriteRenderer view;
    private Color normalColor;
    private float normalSpeed;
    private float slowFraction;
    private float slowUntil;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyController>();
        view = GetComponentInChildren<SpriteRenderer>();
        normalSpeed = movement.MoveSpeed;
        if (view != null) normalColor = view.color;
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

    public void Slow(float fraction)
    {
        if (fraction < slowFraction && Time.time < slowUntil) return;
        slowUntil = Time.time + SlowDuration;
        slowFraction = fraction;
        movement.SetMoveSpeed(normalSpeed * (1f - fraction));
        if (view != null) view.color = normalColor * new Color(0.45f, 0.8f, 1f);
    }

    private void Update()
    {
        if (slowFraction == 0f || Time.time < slowUntil) return;
        slowFraction = 0f;
        movement.SetMoveSpeed(normalSpeed);
        if (view != null) view.color = normalColor;
    }
}
