using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2f;

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

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction =
            ((Vector2)target.position - enemyRigidbody.position).normalized;

        Vector2 nextPosition = enemyRigidbody.position
            + direction * moveSpeed * Time.fixedDeltaTime;

        enemyRigidbody.MovePosition(nextPosition);
    }
}