using UnityEngine;

/// <summary>
/// Mỗi con quái tự mạnh lên theo thời gian NÓ CÒN SỐNG, không phụ thuộc lúc nó ra trận.
/// Con ra sau không tự động mạnh hơn con ra trước; con nào sống lâu mới mạnh.
///
/// Mặc định: máu +2%/giây, sát thương +1%/giây, dừng tăng ở giây thứ 60
/// (tức máu tối đa 220%, sát thương tối đa 160% so với lúc mới ra).
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class EnemyGrowth : MonoBehaviour
{
    [Header("Mạnh lên theo thời gian sống")]
    [SerializeField, Min(0f)] private float healthPercentPerSecond = 0.02f;
    [SerializeField, Min(0f)] private float damagePercentPerSecond = 0.01f;
    [SerializeField, Min(0f)] private float maxGrowthSeconds = 60f;

    private EnemyHealth health;
    private EnemyMeleeAttack melee;
    private int baseHealth;
    private int baseDamage;
    private int appliedHealth;
    private int appliedDamage;
    private float aliveTime;

    private int CurrentDamage
    {
        get { return melee != null ? melee.Damage : 0; }
        set
        {
            if (melee != null)
            {
                melee.Damage = value;
            }
        }
    }

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        melee = GetComponent<EnemyMeleeAttack>();
    }

    private void Start()
    {
        CaptureBase();
    }

    private void Update()
    {
        if (health == null || health.CurrentHealth <= 0)
        {
            return;
        }

        // EnemySpawner gán lại máu/sát thương theo wave, hoặc chỉ số bị đặt lại từ bên ngoài.
        // Khi đó lấy mốc mới và tính lại từ đầu, tránh cộng dồn nhiều lần.
        if (health.MaxHealth != appliedHealth || CurrentDamage != appliedDamage)
        {
            CaptureBase();
        }

        aliveTime += Time.deltaTime;
        ApplyGrowth();
    }

    private void CaptureBase()
    {
        baseHealth = health.MaxHealth;
        baseDamage = CurrentDamage;
        appliedHealth = baseHealth;
        appliedDamage = baseDamage;
        aliveTime = 0f;
    }

    private void ApplyGrowth()
    {
        float grown = Mathf.Min(aliveTime, maxGrowthSeconds);

        int targetHealth = Mathf.RoundToInt(baseHealth * (1f + grown * healthPercentPerSecond));
        if (targetHealth != appliedHealth)
        {
            health.GrowHealth(targetHealth);
            appliedHealth = targetHealth;
        }

        int targetDamage = Mathf.RoundToInt(baseDamage * (1f + grown * damagePercentPerSecond));
        if (targetDamage != appliedDamage)
        {
            CurrentDamage = targetDamage;
            appliedDamage = targetDamage;
        }
    }
}
