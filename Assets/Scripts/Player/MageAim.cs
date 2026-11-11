using UnityEngine;
using UnityEngine.InputSystem;

public sealed class MageAim : MonoBehaviour
{
    [SerializeField] private Transform staff;
    [SerializeField] private SpriteRenderer staffRenderer;
    [SerializeField] private Vector2 handOffset = new Vector2(0.28f, 0.33f);
    [SerializeField] private float staffTipDistance = 0.43f;
    private Camera gameplayCamera;
    private float facingSign = 1f;

    public Vector2 Direction { get; private set; } = Vector2.right;
    public Vector2 CastPoint => (staff != null ? (Vector2)staff.position : (Vector2)transform.position)
        + Direction * staffTipDistance;

    private void Awake()
    {
        gameplayCamera = Camera.main;
    }

    private void Update()
    {
        if (gameplayCamera == null || Mouse.current == null || staff == null) return;
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = gameplayCamera.ScreenToWorldPoint(
            new Vector3(screen.x, screen.y, -gameplayCamera.transform.position.z));
        Vector2 delta = (Vector2)world - (Vector2)transform.position;
        if (delta.sqrMagnitude > 0.0001f) Direction = delta.normalized;

        if (Mathf.Abs(Direction.x) > 0.2f) facingSign = Mathf.Sign(Direction.x);
        staff.localPosition = new Vector3(handOffset.x * facingSign,
            handOffset.y + Direction.y * 0.06f, 0f);
        staff.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
        if (staffRenderer != null) staffRenderer.flipY = Direction.x < 0f;
    }

    public Vector2 PointAtMouse(float maximumDistance)
    {
        if (gameplayCamera == null || Mouse.current == null)
            return (Vector2)transform.position + Direction * maximumDistance;
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = gameplayCamera.ScreenToWorldPoint(
            new Vector3(screen.x, screen.y, -gameplayCamera.transform.position.z));
        Vector2 delta = (Vector2)world - (Vector2)transform.position;
        return (Vector2)transform.position + Vector2.ClampMagnitude(delta, maximumDistance);
    }
}
