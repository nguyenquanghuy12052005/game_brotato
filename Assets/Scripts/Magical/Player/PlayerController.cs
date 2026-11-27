using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform playerVisual;

    [Header("Biên map")]
    [Tooltip("Nửa bề rộng vùng chơi. Map rộng 30 ô nên 15 là vừa khít.")]
    [SerializeField, Min(1f)] private float halfWidth = 14f;
    [SerializeField, Min(1f)] private float halfHeight = 8f;
    [SerializeField] private float centerX;

    private Rigidbody2D playerRigidbody;
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;
    public float ExternalSpeedMultiplier { get; set; } = 1f;
    public float LevelUpSpeedMultiplier { get; set; } = 1f;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // MapManager gọi khi sang map mới: dời tâm biên theo vùng mới.
    public void SetMapBounds(float mapCenterX, float mapHalfWidth, float mapHalfHeight)
    {
        centerX = mapCenterX;
        halfWidth = mapHalfWidth;
        halfHeight = mapHalfHeight;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            movementInput = Vector2.zero;
            return;
        }

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (keyboard.aKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            horizontalInput += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            verticalInput -= 1f;
        }

        if (keyboard.wKey.isPressed)
        {
            verticalInput += 1f;
        }

        movementInput = new Vector2(horizontalInput, verticalInput).normalized;
        UpdateFacingDirection();
    }



private void UpdateFacingDirection()
{
    if (playerVisual != null && movementInput.sqrMagnitude > 0f)
    {
        float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
        playerVisual.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}

    private void FixedUpdate()
    {
        Vector2 nextPosition = playerRigidbody.position
            + movementInput * moveSpeed * Mathf.Max(0f, ExternalSpeedMultiplier)
                * Mathf.Max(0f, LevelUpSpeedMultiplier) * Time.fixedDeltaTime;

        // Không cho đi sang vùng map khác: giữ trong biên của map hiện tại.
        nextPosition.x = Mathf.Clamp(nextPosition.x, centerX - halfWidth, centerX + halfWidth);
        nextPosition.y = Mathf.Clamp(nextPosition.y, -halfHeight, halfHeight);

        playerRigidbody.MovePosition(nextPosition);
    }
}
