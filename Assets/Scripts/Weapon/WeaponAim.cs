using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private float nextFireTime;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        RotateTowardsMouse();
        Shoot();
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null || Mouse.current == null)
        {
            return;
        }

        // Lấy vị trí chuột trên màn hình
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        // Chuyển vị trí chuột sang World Position
        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -mainCamera.transform.position.z
                )
            );

        // Tính hướng từ súng đến chuột
        Vector2 aimDirection =
            mouseWorldPosition - transform.position;

        // Tính góc xoay
        float angle =
            Mathf.Atan2(
                aimDirection.y,
                aimDirection.x
            ) * Mathf.Rad2Deg;

        // Xoay súng
        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        // Lật sprite khi súng quay sang trái
        if (spriteRenderer != null)
        {
            spriteRenderer.flipY =
                angle > 90f || angle < -90f;
        }
    }

    private void Shoot()
    {
        if (Mouse.current == null)
        {
            return;
        }

        // Giữ chuột trái để bắn tự động
        if (
            Mouse.current.leftButton.isPressed &&
            Time.time >= nextFireTime
        )
        {
            Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

            nextFireTime =
                Time.time + fireRate;
        }
    }
}