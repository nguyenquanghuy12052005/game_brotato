using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class HexcoreSkillPreview
{
    [SerializeField] private bool startInFreeMode = true;
    [SerializeField, Min(.1f)] private float freeMoveSpeed = 3.6f;

    private static readonly Vector3 FreeSpawn = new(-1.4f, .667f);
    private const float FreeTurnDuration = .10f;
    private bool freeMode, freeCasting;
    private Vector3 freePosition, castOrigin, freeVelocity, cameraFollow, cameraFollowVelocity;
    private float locomotionTime, freeTurn;
    private int turnFrom;
    public bool InFreePlay => freeMode;
    public bool CastingInFreePlay => freeMode && freeCasting;
    public Vector3 ActorPosition => swordsman.transform.position;

    public void EnableFreePlay()
    {
        SetFreeMode(true);
        RestartPlayback();
    }

    private float CurrentSkillDuration => skillIndex == 2 && !parry ? Chosen.Starts[2] + .25f : SkillDuration(skillIndex);

    private Vector3 SkillDisplacement(float time) =>
        skillIndex == 0 ? AimVector * DashOffset(time) : Travel(time);

    private void SetFreeMode(bool active)
    {
        if (freeMode == active) return;
        if (freeCasting) CommitCastPosition();
        freeMode = active;
        freeVelocity = Vector3.zero;
        freeTurn = 0;
        if (active) cameraFollow = freePosition - FreeSpawn;
        cameraFollowVelocity = Vector3.zero;
    }

    private void BeginFreeCast()
    {
        if (DirectionCoverage(skillIndex, aim) == "MissingArt") return;
        if (freeCasting) CommitCastPosition();
        castOrigin = freePosition;
        freeVelocity = Vector3.zero;
        freeTurn = 0;
        freeCasting = true;
        ConfigureEffect();
        RestartPlayback();
    }

    private void CommitCastPosition()
    {
        freePosition = castOrigin + SkillDisplacement(Mathf.Min(clock, CurrentSkillDuration));
        freeCasting = false;
    }

    private void ResetFreePlay()
    {
        freeCasting = false;
        freePosition = FreeSpawn;
        freeVelocity = cameraFollow = cameraFollowVelocity = Vector3.zero;
        locomotionTime = freeTurn = 0;
        aim = locomotionDirection = 2;
        RestartPlayback();
    }

    private void FaceFreeDirection(int direction)
    {
        if (freeCasting || aim == direction) return;
        turnFrom = aim;
        aim = direction;
        freeTurn = FreeTurnDuration;
    }

    private void UpdateFreePlay(float delta)
    {
        var keyboard = Keyboard.current;
        if (keyboard?.rKey.wasPressedThisFrame == true)
        {
            ResetFreePlay();
            return;
        }

        var move = keyboard == null ? Vector2.zero : new Vector2(
            Axis(keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed,
                keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed),
            Axis(keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed,
                keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)).normalized;

        if (!paused)
        {
            bool wasCasting = freeCasting;
            if (!freeCasting)
            {
                if (move != Vector2.zero)
                {
                    locomotionDirection = (Mathf.RoundToInt(Mathf.Atan2(move.x, move.y) / (Mathf.PI / 4)) + 8) % 8;
                    FaceFreeDirection(locomotionDirection);
                }
                if (keyboard?.spaceKey.wasPressedThisFrame == true) BeginFreeCast();
            }

            if (freeCasting)
            {
                if (wasCasting)
                {
                    if (hold > 0) hold -= delta;
                    else clock = Mathf.Min(CurrentSkillDuration, clock + delta * (halfSpeed ? .5f : 1f));
                    if (clock >= CurrentSkillDuration) CommitCastPosition();
                }
            }
            else
            {
                freeVelocity = Vector3.MoveTowards(freeVelocity, (Vector3)move * freeMoveSpeed, delta * freeMoveSpeed * 12);
                freePosition += freeVelocity * delta;
                locomotionTime += delta;
                freeTurn = Mathf.Max(0, freeTurn - delta);
            }
        }

        Vector3 ground = freeCasting ? castOrigin + SkillDisplacement(clock) : freePosition;
        cameraFollow = Vector3.SmoothDamp(cameraFollow, ground - FreeSpawn, ref cameraFollowVelocity, .12f, Mathf.Infinity, delta);
        RenderFreePlay();
    }

    private static int Axis(bool positive, bool negative) => (positive ? 1 : 0) - (negative ? 1 : 0);

    private void RenderFreePlay()
    {
        if (freeCasting) RenderOther();
        else
        {
            ClearFeedback();
            ResetLines();
            swordsman.transform.position = freePosition;
            dummy.enabled = false;
            if (freeTurn > 0) ShowTurn(turnFrom, aim, 1 - freeTurn / FreeTurnDuration);
            else ShowLocomotion(freeVelocity.sqrMagnitude > .025f, locomotionTime, aim);
        }
        previewCamera.transform.position += cameraFollow;
    }
}
