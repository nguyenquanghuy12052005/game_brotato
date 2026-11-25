using UnityEngine;

/// <summary>
/// Cung thủ giữ khoảng cách với Player thay vì lao vào chém.
/// Xa quá thì tiến lại, gần quá thì lùi ra, trong khoảng giữa thì đứng yên.
/// Không tự bắn — việc bắn do EnemyRangedAttack lo.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRangedMovement : MonoBehaviour
{
    [Header("Khoảng cách")]
    [Tooltip("Xa hơn mức này thì tiến lại gần Player.")]
    [SerializeField, Min(0.5f)] private float approachDistance = 6f;
    [Tooltip("Gần hơn mức này thì lùi ra xa Player.")]
    [SerializeField, Min(0.1f)] private float retreatDistance = 3.5f;
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    private Rigidbody2D enemyRigidbody;
    private Transform target;
    private SpriteRenderer view;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        view = GetComponentInChildren<SpriteRenderer>();

        // Prefab dùng chung với lính thường nên có sẵn EnemyController (lao vào Player).
        // Cung thủ phải giữ khoảng cách, nếu để cả hai chạy thì chúng kéo ngược nhau.
        EnemyController meleeChase = GetComponent<EnemyController>();
        if (meleeChase != null)
        {
            meleeChase.enabled = false;
        }
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
    }

    // EnemySpawner gọi khi vào đợt mới để đặt tốc độ theo wave.
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0f, speed);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector2 toTarget = (Vector2)target.position - enemyRigidbody.position;
        float distance = toTarget.magnitude;
        FaceTarget(toTarget.x);

        if (distance < 0.01f || moveSpeed <= 0f)
        {
            return;
        }

        // Trong khoảng [retreatDistance, approachDistance] thì đứng yên bắn.
        Vector2 direction;
        float travel;
        if (distance > approachDistance)
        {
            direction = toTarget / distance;
            travel = distance - approachDistance;
        }
        else if (distance < retreatDistance)
        {
            direction = -toTarget / distance;
            travel = retreatDistance - distance;
        }
        else
        {
            return;
        }

        float step = Mathf.Min(moveSpeed * Time.fixedDeltaTime, travel);
        enemyRigidbody.MovePosition(enemyRigidbody.position + direction * step);
    }

    // Chỉ có một hướng nhìn nên đi sang trái thì lật hình.
    private void FaceTarget(float horizontalOffset)
    {
        if (view == null)
        {
            return;
        }
        view.flipX = horizontalOffset < 0f;
    }
}
