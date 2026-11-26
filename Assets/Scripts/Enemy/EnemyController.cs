using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0f;
    private bool trainingDummy;

    private Rigidbody2D enemyRigidbody;

    private void Start()
    {
        if (target == null && !trainingDummy)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
    }
    public void SetTrainingDummy(bool value)
    {
        trainingDummy = value;
        if (!value) return;
        if (TryGetComponent<EnemyMeleeAttack>(out var melee)) Destroy(melee);
        if (TryGetComponent<EnemyRangedAttack>(out var rangedAttack)) Destroy(rangedAttack);
        if (TryGetComponent<EnemyRangedMovement>(out var rangedMovement)) Destroy(rangedMovement);
    }

    public float MoveSpeed => moveSpeed;
    public bool IsTrainingDummy => trainingDummy;

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0f, speed);
    }

    // Quái dừng lại khi còn cách mục tiêu chừng này, tránh lao qua rồi quay lại.
    public void SetStopDistance(float distance)
    {
        stopDistance = Mathf.Max(0f, distance);
    }

    private void FixedUpdate()
    {
        var status = GetComponent<MageEnemyStatus>();
        Vector2 step = Vector2.zero;
        if (!trainingDummy && target != null && moveSpeed > 0f)
        {
            Vector2 toTarget = (Vector2)target.position - enemyRigidbody.position;
            float distance = toTarget.magnitude;
            if (distance > stopDistance)
            {
                float speed = moveSpeed * (status != null ? status.MoveMultiplier : 1f);
                step = toTarget / distance * Mathf.Min(speed * Time.fixedDeltaTime, distance - stopDistance);
            }
        }
        if (status != null) step += status.GravityVelocity * Time.fixedDeltaTime;
        if (step.sqrMagnitude > 0f) enemyRigidbody.MovePosition(enemyRigidbody.position + step);
    }
}
