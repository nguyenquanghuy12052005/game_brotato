using UnityEngine;
using UnityEngine.InputSystem;

// Kịch bản minh họa trong Demo, dùng chung một đồng hồ để tạm dừng và phát lại.
public sealed class MeleeDemoCombat : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Material spriteMaterial;
    [SerializeField] private Material goldVfxMaterial;
    [SerializeField] private Sprite sweepSprite;
    [SerializeField] private Sprite returningWaveSprite;
    [SerializeField] private Sprite fanSprite;
    [SerializeField] private Material barrierMaterial;
    [SerializeField] private Material cleanVfxMaterial;
    [SerializeField] private float moveSpeed = 2.8f;
    private Vector3 controlledPosition;
    private bool manualControlTaken;
    private bool moving;
    private string moveFacing = "Right";
    private MaterialPropertyBlock shieldProperties;
    private Sprite barrierQuad;
    private GameObject stage;
    private Animator actor;
    private Animator effect;
    private SpriteRenderer[] enemies;
    private Vector3[] enemyPositions;
    private int skill;
    private float time;
    private bool paused;
    public const float CycleLength = 5.2f;
    public float TimeInCycle => time;
    public int CharacterIndex { get; private set; }
    public Vector3 ActorPosition => actor.transform.localPosition;
    public SpriteRenderer EffectRenderer => effect == null ? null : effect.GetComponent<SpriteRenderer>();
    public int EnemyCount => enemies == null ? 0 : enemies.Length;
    public bool Paused { get => paused; set => paused = value; }
    public bool Repeat { get; set; } = true;

    public void Show(int index, GameObject effectPrefab, int ultimateBranch)
    {
        if (stage != null) { stage.SetActive(false); Destroy(stage); }
        if (barrierQuad != null) Destroy(barrierQuad);
        stage = new GameObject("Demo Encounter");
        stage.transform.SetParent(transform, false);
        skill = index;
        CharacterIndex = index < 3 ? 0 : index < 12 ? index / 3 : ultimateBranch + 1;
        actor = Instantiate(characterPrefabs[CharacterIndex], stage.transform).GetComponent<Animator>();
        actor.speed = 0;
        actor.GetComponent<SpriteRenderer>().sortingOrder = 2;
        effect = Instantiate(effectPrefab, stage.transform).GetComponent<Animator>();
        effect.speed = 0;
        effect.GetComponent<SpriteRenderer>().sortingOrder = 4;
        EffectRenderer.sharedMaterial = cleanVfxMaterial;
        if (skill == 14) EffectRenderer.sharedMaterial = goldVfxMaterial;
        if (skill == 0)
        {
            EffectRenderer.sharedMaterial = barrierMaterial;
            shieldProperties = new MaterialPropertyBlock();
            Sprite source = EffectRenderer.sprite;
            barrierQuad = Sprite.Create(source.texture, source.rect, Vector2.one * .5f,
                source.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        }
        manualControlTaken = false;
        moving = false;
        enemies = new SpriteRenderer[4];
        enemyPositions = new[] { new Vector3(1.1f, -.45f), new Vector3(1.65f, .5f),
            new Vector3(.9f, 1.1f), new Vector3(2.25f, -.6f) };
        for (int i = 0; i < enemies.Length; i++)
        {
            var enemy = new GameObject("Quái demo " + (i + 1));
            enemy.transform.SetParent(stage.transform, false);
            enemies[i] = enemy.AddComponent<SpriteRenderer>();
            enemies[i].sprite = enemySprite;
            enemies[i].sharedMaterial = spriteMaterial;
            enemies[i].sortingOrder = 1;
            float size = .55f / Mathf.Max(enemySprite.bounds.size.x, enemySprite.bounds.size.y);
            enemy.transform.localScale = Vector3.one * size;
        }
        Replay();
    }

    public void Replay()
    {
        time = 0;
        paused = false;
        manualControlTaken = false;
        moving = false;
        Sample();
    }

    private void Update()
    {
        if (stage == null || paused) return;
        Vector2 input = Vector2.zero;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            input.x = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
            input.y = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
        }
        Move(input, Time.unscaledDeltaTime);
        time = Mathf.Min(CycleLength, time + Time.unscaledDeltaTime);
        if (time >= CycleLength && Repeat) time = 0;
        Sample();
    }

    public void Move(Vector2 input, float deltaTime)
    {
        moving = input.sqrMagnitude > 0;
        if (!moving || paused || actor == null) return;
        if (!manualControlTaken) controlledPosition = actor.transform.localPosition - Vector3.up * .667f;
        manualControlTaken = true;
        Vector2 direction = Vector2.ClampMagnitude(input, 1);
        controlledPosition += (Vector3)(direction * moveSpeed * Mathf.Max(0, deltaTime));
        controlledPosition.x = Mathf.Clamp(controlledPosition.x, -2.7f, 2.7f);
        controlledPosition.y = Mathf.Clamp(controlledPosition.y, -1.9f, 1.7f);
        moveFacing = Mathf.Abs(direction.x) >= Mathf.Abs(direction.y) ?
            (direction.x >= 0 ? "Right" : "Left") : (direction.y >= 0 ? "Up" : "Down");
    }

    public void Seek(float seconds)
    {
        time = Mathf.Clamp(seconds, 0, CycleLength);
        Sample();
    }

    private void Sample()
    {
        Vector3 origin = new Vector3(-1.25f, -.35f);
        Vector3 target = enemyPositions[0];
        Vector3 position = origin;
        if (skill >= 3 && skill <= 8)
            position = Vector3.Lerp(origin, target - Vector3.right * .75f, Smooth(.1f, .55f));
        string facing = "Right";
        string action = "Idle";
        float attackStart = 1.85f;
        if (skill >= 3 && skill < 12 && skill != 8 && time > .55f && time < 1.65f)
        {
            action = "Attack";
            attackStart = time < 1.1f ? .55f : 1.1f;
        }
        if (time >= 1.85f && time < 3.05f && skill != 0) action = "Attack";
        if (skill == 1)
        {
            position = Vector3.Lerp(origin, new Vector3(1.7f, -.35f), Smooth(1.9f, 2.22f));
            if (time > 1.9f && time < 2.3f) action = "Run";
        }
        if (skill == 4)
        {
            Vector3 attackPosition = target - Vector3.right * .75f;
            position = time < 1.4f ? Vector3.Lerp(origin, attackPosition, Smooth(.1f, .55f)) :
                Vector3.Lerp(attackPosition, origin + Vector3.left, Smooth(1.4f, 2.1f));
            if (time > 1.4f && time < 2.1f) action = "Run";
            if (time >= 1.4f) facing = "Left";
        }
        if (skill == 5)
        {
            float walk = Smooth(.85f, 2.1f);
            position = Vector3.Lerp(origin, target + Vector3.right * .8f, walk);
            position.y += Mathf.Sin(walk * Mathf.PI) * 1.2f;
            if (time > .85f && time < 2.1f) action = "Run";
            if (time >= 2.1f) facing = "Left";
        }
        if (skill == 13 && time > 1.4f && time < 2.7f)
        {
            action = "Attack";
            attackStart = 1.4f;
        }
        if (skill == 11)
        {
            position = Vector3.Lerp(origin, target - Vector3.right * .5f, Smooth(1.4f, 2f));
            if (time > 1.4f && time < 2f) action = "Run";
        }
        if (skill == 12)
        {
            position = Vector3.Lerp(origin, Vector3.zero, Smooth(1.1f, 1.95f));
            if (time > 1.1f && time < 1.95f) position.y += Mathf.Sin(Smooth(1.1f, 1.95f) * Mathf.PI) * 2;
        }
        if (manualControlTaken)
        {
            position = controlledPosition;
            if (moving) { action = "Run"; facing = moveFacing; }
            else if (action == "Idle") facing = moveFacing;
        }
        actor.transform.localPosition = position + Vector3.up * .667f;
        AnimateActor(action + "_" + facing, action == "Attack" ? time - attackStart : time);
        float release = skill == 13 ? 1.25f : 1.9f;
        if (skill == 8) release = 2.41f;
        if (skill == 6) release = 2.2f;
        if (skill == 7) release = 2.4f;
        float progress = (time - release) / (skill == 2 ? 1.8f : skill >= 12 ? 1.5f : 1f);
        effect.gameObject.SetActive(progress >= 0 && progress <= 1);
        effect.transform.localScale = Vector3.one * (skill >= 12 ? 1f : .55f);
        Vector3 effectPosition = position + new Vector3(.55f, .35f);
        if (skill == 2 || skill == 7) effectPosition = position + Vector3.up * .35f;
        if (skill == 5 || skill == 11) effectPosition = target;
        if (skill >= 12) effectPosition = Vector3.zero;
        if (skill == 4) effectPosition = target - new Vector3(.7f, -.3f);
        if (skill == 9)
        {
            float travel = progress <= .5f ? Smooth(1.9f, 2.4f) : 1 - Smooth(2.4f, 2.9f);
            effectPosition = position + new Vector3(.7f + 2.4f * travel, .5f);
        }
        if (skill == 10) effectPosition = position + new Vector3(1.2f + 1.3f * Mathf.Clamp01(progress), .5f);
        if (skill == 13) effectPosition = position + new Vector3(1.5f, .5f);
        if (skill == 0)
        {
            effectPosition = actor.transform.localPosition;
            effect.transform.localScale = Vector3.one * .48f;
            effect.gameObject.SetActive(true);
        }
        effect.transform.localPosition = effectPosition;
        effect.transform.localRotation = Quaternion.identity;
        EffectRenderer.flipX = false;
        EffectRenderer.color = Color.white;
        if (effect.gameObject.activeSelf)
        {
            float normalized = skill == 2 ? Mathf.Repeat(progress * 3, 1) : Mathf.Clamp01(progress) * .98f;
            if (skill == 8) normalized = Mathf.Lerp(.56f, .98f, Mathf.Clamp01(progress * 2));
            effect.Play(0, 0, normalized);
            effect.Update(0);
            if (skill == 0)
            {
                // Giữ quad hoạt động xuyên chu kỳ, shader vẽ màn chắn quanh người.
                effect.Play(0, 0, .25f);
                effect.Update(0);
                EffectRenderer.sprite = barrierQuad;
                shieldProperties.SetFloat("_DemoTime", time);
                EffectRenderer.SetPropertyBlock(shieldProperties);
            }
            if (skill == 7)
            {
                EffectRenderer.sprite = sweepSprite;
                effect.transform.localRotation = Quaternion.Euler(0, 0, -360 * Mathf.SmoothStep(0, 1, progress));
            }
            if (skill == 9)
            {
                EffectRenderer.sprite = returningWaveSprite;
                EffectRenderer.flipX = progress > .5f;
            }
            if (skill == 10) EffectRenderer.sprite = fanSprite;
            if (skill == 7 || skill == 9 || skill == 10)
                EffectRenderer.color = new Color(1, 1, 1, Mathf.Clamp01(progress * 12) * Mathf.Clamp01((1 - progress) * 8));
        }
        float hitTime = skill == 13 ? 2.05f : skill == 14 ? 2.6f : 2.25f;
        if (skill == 6 || skill == 7 || skill == 8) hitTime = 2.48f;
        for (int i = 0; i < enemies.Length; i++)
        {
            bool inRange = skill == 0 || skill == 1 || skill == 3 || skill == 4 || skill == 5 || skill == 8 || skill == 11;
            float hit = time - hitTime - i * .045f;
            bool inArea = skill != 14 || new Vector2(enemyPositions[i].x / 2.1f, enemyPositions[i].y / 1.6f).sqrMagnitude <= 1;
            bool struck = skill != 1 && hit > 0 && (!inRange || i == 0) && inArea;
            Vector3 enemyPosition = enemyPositions[i];
            if (skill == 0 && i == 0) enemyPosition = Vector3.Lerp(enemyPosition, origin + Vector3.right * .55f, Smooth(.6f, 1.9f));
            if (struck)
            {
                float knockback = Mathf.SmoothStep(0, .45f, Mathf.Clamp01(hit / .25f));
                enemyPosition += (enemyPositions[i] - position).normalized * knockback;
                if (skill == 12 && hit < .6f) enemyPosition.y += Mathf.Sin(hit / .6f * Mathf.PI) * .5f;
            }
            enemies[i].transform.localPosition = enemyPosition;
            bool normalHit = skill >= 3 && skill < 12 && skill != 8 && i == 0 &&
                ((time > .8f && time < .94f) || (time > 1.35f && time < 1.49f));
            enemies[i].color = normalHit || (struck && hit < .15f) ? new Color(1, .35f, .3f) : Color.white;
        }
    }

    private float Smooth(float start, float end) => Mathf.SmoothStep(0, 1, Mathf.InverseLerp(start, end, time));

    private void OnDestroy()
    {
        if (barrierQuad != null) Destroy(barrierQuad);
    }

    private void AnimateActor(string state, float seconds)
    {
        float length = .7f;
        foreach (var clip in actor.runtimeAnimatorController.animationClips)
            if (clip.name == state) { length = clip.length; break; }
        float normalized = state.StartsWith("Attack") ? Mathf.Clamp01(seconds / length) * .99f : Mathf.Repeat(seconds / length, 1);
        actor.Play(state, 0, normalized);
        actor.Update(0);
    }
}
