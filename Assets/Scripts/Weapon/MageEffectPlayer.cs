using UnityEngine;

public sealed class MageEffectPlayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private float fps;
    private float elapsed;
    private bool loop;

    public void Initialize(SpriteRenderer renderer, Sprite[] animationFrames, float framesPerSecond, bool shouldLoop)
    {
        spriteRenderer = renderer;
        frames = animationFrames;
        fps = framesPerSecond;
        loop = shouldLoop;
        spriteRenderer.sprite = frames[0];
    }

    private void Update()
    {
        if (frames == null) return;
        elapsed += Time.deltaTime;
        int frame = Mathf.FloorToInt(elapsed * fps);
        if (frame >= frames.Length)
        {
            if (!loop)
            {
                Destroy(gameObject);
                return;
            }
            frame %= frames.Length;
        }
        spriteRenderer.sprite = frames[frame];
    }
}
