using UnityEngine;
using UnityEngine.InputSystem;

// Gan vao PlayerVisual; controller va HP van nam tren Player goc.
[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerController movement;
    private PlayerHealth health;
    private string facing = "Down";
    private string currentState;
    private bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerController>();
        health = GetComponentInParent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.Died += PlayDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.Died -= PlayDeath;
    }

    private void Update()
    {
        if (isDead || Time.timeScale == 0f)
            return;

        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
        if (movement != null && movement.enabled && keyboard != null)
        {
            input.x = (keyboard.dKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed ? 1f : 0f);
            input.y = (keyboard.wKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed ? 1f : 0f);
        }

        // Di cheo uu tien huong ngang; dung lai giu huong vua di.
        if (input.x != 0f)
            facing = input.x > 0f ? "Right" : "Left";
        else if (input.y != 0f)
            facing = input.y > 0f ? "Up" : "Down";

        PlayState((input.sqrMagnitude > 0f ? "Run_" : "Idle_") + facing);
    }

    private void PlayDeath()
    {
        isDead = true;
        // GameManager dung timeScale khi HP = 0; chi animation chet dung gio thuc.
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        PlayState("Death_" + facing);
    }

    private void PlayState(string stateName)
    {
        if (currentState == stateName || animator.runtimeAnimatorController == null)
            return;

        currentState = stateName;
        animator.Play(stateName, 0, 0f);
    }
}
