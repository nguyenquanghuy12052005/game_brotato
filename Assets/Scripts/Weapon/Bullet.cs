using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition =
            bulletRigidbody.position
            + (Vector2)transform.right
            * speed
            * Time.fixedDeltaTime;

        bulletRigidbody.MovePosition(nextPosition);
    }
}