using System.Collections;
using UnityEngine;

/// <summary>
/// Quái đánh gần: đuổi Player, tới tầm thì dừng, giơ dao lấy đà rồi mới gây sát thương.
/// Chỉ trừ máu đúng lúc chém trúng và Player còn trong tầm — né ra thì trượt.
/// </summary>
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(Animator))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Tầm đánh")]
    [SerializeField, Min(0.1f)] private float attackRange = 1.2f;
    [SerializeField, Min(0f)] private float windupTime = 0.35f;
    [SerializeField, Min(0.1f)] private float cooldown = 1.5f;
    [SerializeField, Min(1)] private int damage = 10;

    private EnemyController movement;
    private Animator animator;
    private SpriteRenderer view;
    private Transform player;
    private PlayerHealth playerHealth;
    private float nextAttackTime;
    private bool attacking;
    private int currentState;

    private static readonly int IdleState = Animator.StringToHash("Idle");
    private static readonly int WalkState = Animator.StringToHash("Walk");
    private static readonly int AttackState = Animator.StringToHash("Attack");

    // EnemyGrowth nâng sát thương theo thời gian sống.
    public int Damage
    {
        get => damage;
        set => damage = Mathf.Max(1, value);
    }

    private void Awake()
    {
        movement = GetComponent<EnemyController>();
        animator = GetComponent<Animator>();
        view = GetComponentInChildren<SpriteRenderer>();

        // Dừng đúng ở tầm đánh, nếu không quái lao qua Player rồi quay lại.
        movement.SetStopDistance(attackRange);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (attacking)
        {
            return;
        }

        if (player == null)
        {
            PlayState(IdleState);
            return;
        }

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            PlayState(WalkState);
            return;
        }

        PlayState(IdleState);
        if (Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
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

    private void FacePlayer()
    {
        if (view == null || player == null)
        {
            return;
        }
        view.flipX = player.position.x < transform.position.x;
    }

    private IEnumerator Attack()
    {
        attacking = true;
        nextAttackTime = Time.time + cooldown + windupTime;
        PlayState(AttackState);

        yield return new WaitForSeconds(windupTime);

        // Chỉ trừ máu nếu Player vẫn còn trong tầm lúc dao chém xuống.
        if (player != null && playerHealth != null
            && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            var status = GetComponent<MageEnemyStatus>();
            float multiplier = status != null ? status.OutgoingDamageMultiplier : 1f;
            playerHealth.TakeDamage(Mathf.CeilToInt(damage * multiplier));
        }

        attacking = false;
        currentState = 0;
    }
}
