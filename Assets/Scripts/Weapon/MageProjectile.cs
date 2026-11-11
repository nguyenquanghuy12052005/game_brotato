using System;
using UnityEngine;

public sealed class MageProjectile : MonoBehaviour
{
    private Vector2 direction;
    private EnemyHealth target;
    private float speed;
    private float expiresAt;
    private Action<EnemyHealth> onHit;
    private bool consumed;

    public void Initialize(Vector2 aim, EnemyHealth homingTarget, float moveSpeed,
        float lifetime, Action<EnemyHealth> hitAction)
    {
        direction = aim.sqrMagnitude > 0.001f ? aim.normalized : Vector2.right;
        target = homingTarget;
        speed = moveSpeed;
        expiresAt = Time.time + lifetime;
        onHit = hitAction;
        TryHit(transform.position);
    }

    private void Update()
    {
        if (consumed) return;
        if (Time.time >= expiresAt) { Destroy(gameObject); return; }
        if (target != null && target.CurrentHealth > 0)
        {
            Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;
            if (toTarget.sqrMagnitude > 0.001f) direction = toTarget.normalized;
        }

        Vector2 next = (Vector2)transform.position + direction * speed * Time.deltaTime;
        transform.position = next;
        transform.rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        TryHit(next);
    }

    private void TryHit(Vector2 position)
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(position, 0.23f))
        {
            EnemyHealth enemy = collider.GetComponentInParent<EnemyHealth>();
            if (enemy == null || enemy.CurrentHealth <= 0 || (target != null && enemy != target)) continue;
            consumed = true;
            onHit?.Invoke(enemy);
            Destroy(gameObject);
            return;
        }
    }
}
