using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0f;

    private Rigidbody2D enemyRigidbody;
    
        private void Start()
    {
        if (target == null)
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

    public float MoveSpeed => moveSpeed;

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
        if (target == null || moveSpeed <= 0f)
        {
            return;
        }

        Vector2 toTarget = (Vector2)target.position - enemyRigidbody.position;
        float distance = toTarget.magnitude;

        // Đã tới sát thì đứng yên, không đẩy lấn vào mục tiêu.
        if (distance <= stopDistance)
        {
            return;
        }

        Vector2 step = toTarget / distance * moveSpeed * Time.fixedDeltaTime;
        if (step.magnitude > distance - stopDistance)
        {
            step = step.normalized * (distance - stopDistance);
        }

        enemyRigidbody.MovePosition(enemyRigidbody.position + step);
    }
}
