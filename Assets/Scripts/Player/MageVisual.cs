using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class MageVisual : MonoBehaviour
{
    [SerializeField] private Texture2D characterSheet;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float framesPerSecond = 9f;

    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private int directionRow;
    private float animationTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (characterSheet == null || characterSheet.width != 1664 || characterSheet.height != 640)
        {
            Debug.LogError("Mage character sheet must be 13 x 4 cells of 128 x 160.", this);
            enabled = false;
            return;
        }

        frames = new Sprite[52];
        for (int row = 0; row < 4; row++)
            for (int column = 0; column < 13; column++)
                frames[row * 13 + column] = Sprite.Create(characterSheet,
                    new Rect(column * 128, 640 - (row + 1) * 160, 128, 160),
                    new Vector2(0.5f, 0.1f), 128f, 0, SpriteMeshType.FullRect);

        spriteRenderer.sprite = frames[0];
    }

    private void Update()
    {
        if (controller == null || frames == null) return;
        Vector2 input = controller.MovementInput;
        bool moving = input.sqrMagnitude > 0.01f;
        if (moving)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) directionRow = input.x < 0f ? 1 : 2;
            else directionRow = input.y > 0f ? 3 : 0;
            animationTime += Time.deltaTime;
        }
        else animationTime = 0f;

        int column = moving ? 4 + Mathf.FloorToInt(animationTime * framesPerSecond) % 5 : 0;
        spriteRenderer.sprite = frames[directionRow * 13 + column];
    }

    private void OnDestroy()
    {
        if (frames == null) return;
        foreach (Sprite frame in frames)
            if (frame != null) Destroy(frame);
    }
}
