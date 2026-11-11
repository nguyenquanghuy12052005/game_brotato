using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2f;
    private bool trainingDummy;

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
    public void SetTrainingDummy(bool value) { trainingDummy = value; }

    public float MoveSpeed => moveSpeed;

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0.1f, speed);
    }

    private void FixedUpdate()
    {
        if (target == null && !trainingDummy)
        {
            return;
        }

        Vector2 direction = trainingDummy ? Vector2.zero :
            ((Vector2)target.position - enemyRigidbody.position).normalized;

        MageEnemyStatus status = GetComponent<MageEnemyStatus>();
        float speedMultiplier = status != null ? status.MoveMultiplier : 1f;
        Vector2 nextPosition = enemyRigidbody.position
            + (direction * moveSpeed * speedMultiplier +
                (status != null ? status.GravityVelocity : Vector2.zero)) * Time.fixedDeltaTime;

        enemyRigidbody.MovePosition(nextPosition);
    }
}
