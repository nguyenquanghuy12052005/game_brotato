using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;

    [Header("Chết")]
    [SerializeField, Min(0f)] private float deathDelay = 1.2f;

    private int currentHealth;
    private bool dying;
    private Animator animator;
    private Collider2D hitbox;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public event Action<EnemyHealth> Died;

    private static readonly int DeathState = Animator.StringToHash("Death");

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        hitbox = GetComponent<Collider2D>();
    }

    public void TakeDamage(int damage)
    {
        if (dying || currentHealth <= 0 || damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void InitializeHealth(int health)
    {
        maxHealth = Mathf.Max(1, health);
        currentHealth = maxHealth;
    }

    // Dùng cho EnemyGrowth: nâng máu tối đa và cộng phần chênh vào máu hiện tại,
    // để quái đang bị thương không tự hồi đầy khi mạnh lên.
    public void GrowHealth(int newMaxHealth)
    {
        int target = Mathf.Max(1, newMaxHealth);
        int gain = target - maxHealth;
        maxHealth = target;
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, gain), 0, maxHealth);
    }

    // Chạy animation chết rồi mới xoá, để người chơi thấy quái ngã xuống.
    private void Die()
    {
        dying = true;

        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            if (behaviour == null || behaviour == this || behaviour == animator) continue;
            behaviour.StopAllCoroutines();
            behaviour.enabled = false;
        }
        if (hitbox != null) hitbox.enabled = false;
        if (animator != null) animator.Play(DeathState, 0, 0f);

        Died?.Invoke(this);
        Destroy(gameObject, deathDelay);
    }
}
