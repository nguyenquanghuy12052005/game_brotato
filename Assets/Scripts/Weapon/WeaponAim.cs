using UnityEngine;
using UnityEngine.InputSystem; 

public class WeaponAim : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        RotateTowardsMouse();
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null || Mouse.current == null) return;

     
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z));

        
        Vector2 aimDirection = (mouseWorldPos - transform.position);
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

       
        if (spriteRenderer != null)
        {
            spriteRenderer.flipY = (angle > 90f || angle < -90f);
        }
    }
}