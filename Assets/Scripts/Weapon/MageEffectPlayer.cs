using UnityEngine;

public sealed class MageEffectPlayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private float fps;
    private float elapsed;
    private float animationElapsed;
    private float animationSpeed = 1f;
    private int frameLimit;
    private bool loop;
    private MageAim followingAim;
    private float followDistance;
    private float startScale, endScale, scaleDuration;

    public void FollowAim(MageAim aim, float distance)
    {
        followingAim = aim;
        followDistance = distance;
        UpdateAimPosition();
    }

    public void Grow(float fromScale, float toScale, float duration)
    {
        startScale = fromScale;
        endScale = toScale;
        scaleDuration = Mathf.Max(0.01f, duration);
        transform.localScale = Vector3.one * startScale;
    }

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Max(0.05f, speed);
    }

    public void SetFrameLimit(int count)
    {
        frameLimit = frames == null ? 0 : Mathf.Clamp(count, 1, frames.Length);
    }

    public void Initialize(SpriteRenderer renderer, Sprite[] animationFrames, float framesPerSecond, bool shouldLoop)
    {
        spriteRenderer = renderer;
        frames = animationFrames;
        frameLimit = frames.Length;
        fps = framesPerSecond;
        loop = shouldLoop;
        spriteRenderer.sprite = frames[0];
    }

    private void Update()
    {
        if (frames == null) return;
        elapsed += Time.deltaTime;
        animationElapsed += Time.deltaTime * animationSpeed;
        if (followingAim != null) UpdateAimPosition();
        if (scaleDuration > 0f)
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale,
                Mathf.Clamp01(elapsed / scaleDuration));
        int frame = Mathf.FloorToInt(animationElapsed * fps);
        if (frame >= frameLimit)
        {
            if (!loop)
            {
                Destroy(gameObject);
                return;
            }
            frame %= frameLimit;
        }
        spriteRenderer.sprite = frames[frame];
    }

    private void UpdateAimPosition()
    {
        transform.position = followingAim.CastPoint + followingAim.Direction * followDistance;
        transform.rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(followingAim.Direction.y, followingAim.Direction.x) * Mathf.Rad2Deg);
    }
}
