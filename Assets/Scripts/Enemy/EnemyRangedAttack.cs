using System.Collections;
using UnityEngine;

/// <summary>
/// Cung thủ: đứng trong khoảng cách bắn thì bắn mũi tên về phía Player.
/// Mỗi phát có thời gian giơ cung rồi mới bắn ra đạn, nên người chơi thấy trước và né được.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Bắn")]
    [Tooltip("Chỉ bắn khi Player trong tầm này. Phải LỚN HƠN approachDistance của EnemyRangedMovement, " +
             "nếu bằng nhau thì cung thủ dừng đúng ranh giới và không bao giờ bắn.")]
    [SerializeField, Min(1f)] private float fireRange = 7f;
    [Tooltip("Giơ cung bao lâu rồi mới bắn ra đạn.")]
    [SerializeField, Min(0f)] private float windupTime = 0.3f;
    [Tooltip("Khoảng cách giữa hai phát bắn.")]
    [SerializeField, Min(0.1f)] private float cooldown = 1f;

    [Header("Đạn")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [Tooltip("Đẩy mũi tên ra trước mặt. Phải lớn hơn nửa bề rộng collider của quái, " +
             "nếu không đạn sinh ra bên trong thân và biến mất ngay.")]
    [SerializeField, Min(0f)] private float spawnOffset = 1.3f;

    private Animator animator;
    private EnemyHealth health;
    private Transform player;
    private float nextFireTime;
    private bool firing;
    private int currentState;

    private static readonly int IdleState = Animator.StringToHash("Idle");
    private static readonly int WalkState = Animator.StringToHash("Walk");
    private static readonly int AttackState = Animator.StringToHash("Attack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (firing || health == null || health.CurrentHealth <= 0)
        {
            return;
        }

        if (player == null)
        {
            PlayState(IdleState);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Chưa vào tầm thì để animation đi bộ, EnemyRangedMovement lo việc tiến lại.
        if (distance > fireRange)
        {
            PlayState(WalkState);
            return;
        }

        PlayState(IdleState);
        if (Time.time >= nextFireTime)
        {
            StartCoroutine(Fire());
        }
    }

    // Chỉ gọi Play khi đổi state, gọi mỗi frame sẽ reset animation về frame 0.
    private void PlayState(int state)
    {
        if (currentState == state)
        {
            return;
        }
        currentState = state;
        animator.Play(state, 0, 0f);
    }

    private IEnumerator Fire()
    {
        firing = true;
        nextFireTime = Time.time + cooldown + windupTime;
        PlayState(AttackState);

        yield return new WaitForSeconds(windupTime);

        // Player có thể đã chết hoặc chạy khỏi tầm trong lúc giơ cung.
        if (player != null && health != null && health.CurrentHealth > 0)
        {
            Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
            if (toPlayer.magnitude <= fireRange)
            {
                SpawnProjectile(toPlayer.normalized);
            }
        }

        firing = false;
        currentState = 0;
    }

    private void SpawnProjectile(Vector2 direction)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("EnemyRangedAttack: chưa gắn đạn (Enemy Projectile Prefab) trong Inspector.", this);
            return;
        }

        Vector3 spawnPosition = transform.position
            + (Vector3)(direction * Mathf.Max(0f, spawnOffset));
        EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        projectile.SetDirection(direction);

        // Không ghi view.flipX ở đây: EnemyRangedMovement đã lo hướng nhìn.
        // Ghi ở cả hai nơi (FixedUpdate vs Update) sẽ làm hình giật khi ngắm.
    }
}
