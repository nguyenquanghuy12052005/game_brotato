using UnityEngine;

/// <summary>
/// Đạn của quái (mũi tên của cung thủ). Bay thẳng theo hướng đã gán,
/// chỉ gây sát thương cho Player, chạm là mất và tự huỷ sau vài giây.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Đạn")]
    [SerializeField, Min(0.1f)] private float speed = 8f;
    [SerializeField, Min(1)] private int damage = 8;
    [Tooltip("Tự huỷ sau bao lâu nếu bay trượt.")]
    [SerializeField, Min(0.1f)] private float lifeTime = 4f;

    private Rigidbody2D projectileRigidbody;
    private Vector2 direction = Vector2.right;
    private bool hasHit;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Cung thủ gọi ngay sau khi tạo đạn, truyền hướng bay về phía Player.
    // Xoay luôn mũi tên cho khớp hướng bay, nếu không nó luôn nằm ngang.
    public void SetDirection(Vector2 newDirection)
    {
        if (newDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        direction = newDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = projectileRigidbody.position
            + direction * speed * Time.fixedDeltaTime;
        projectileRigidbody.MovePosition(nextPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || !gameObject.activeInHierarchy)
        {
            return;
        }

        // Chỉ trừ máu Player; quái khác hay vật cản thì bỏ qua, đạn bay tiếp.
        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        // Destroy diễn ra cuối frame; chặn callback lặp ngay lập tức.
        hasHit = true;
        playerHealth.TakeDamage(damage);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
