using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void DamagePlayer(Collider2D other)
    {
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            MageEnemyStatus status = GetComponentInParent<MageEnemyStatus>();
            float multiplier = status != null ? status.OutgoingDamageMultiplier : 1f;
            int damage = Mathf.CeilToInt(contactDamage * multiplier);
            if (damage > 0) playerHealth.TakeDamage(damage);
        }
    }
}
