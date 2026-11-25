using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static partial class HexcoreSkillPreviewSetup
{
    private const string VerifyingInteraction = "HexcoreSkillPreview.VerifyingInteraction";
    private const string InteractionFolder = "Library/HexcoreInteraction";
    private static readonly string[] HeadingNames = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
    private static readonly Key[][] Wasd =
    {
        new[] { Key.W }, new[] { Key.W, Key.D }, new[] { Key.D }, new[] { Key.S, Key.D },
        new[] { Key.S }, new[] { Key.S, Key.A }, new[] { Key.A }, new[] { Key.W, Key.A }
    };
    private static readonly Key[][] Arrows =
    {
        new[] { Key.UpArrow }, new[] { Key.UpArrow, Key.RightArrow }, new[] { Key.RightArrow },
        new[] { Key.DownArrow, Key.RightArrow }, new[] { Key.DownArrow },
        new[] { Key.DownArrow, Key.LeftArrow }, new[] { Key.LeftArrow }, new[] { Key.UpArrow, Key.LeftArrow }
    };
    private static Keyboard testKeyboard;
    private static int interactionStep, movementCase, testedSkill;
    private static double interactionStart;
    private static Vector3 interactionOrigin, spawn;
    private static float jumpHeight;
    private static bool tookUi;

    private static bool CaptureInteractionUi =>
        Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreInteractionUi") >= 0;

    public static void VerifyInteraction()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        if (CaptureInteractionUi)
        {
            var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            gameView.Focus();
        }
        SessionState.SetBool(VerifyingInteraction, true);
        EnterVerification();
    }

    private static bool AdvanceInteraction(HexcoreSkillPreview preview)
    {
        if (testKeyboard == null)
        {
            if (!preview.InFreePlay || preview.CastingInFreePlay)
                throw new InvalidOperationException("Scene must start ready for free movement.");
            testKeyboard = InputSystem.AddDevice<Keyboard>("HexcorePreviewQA");
            spawn = preview.ActorPosition;
            StartMovement(preview);
            return false;
        }

        double elapsed = EditorApplication.timeSinceStartup - interactionStart;
        if (interactionStep == 1 && elapsed > .59)
        {
            int direction = movementCase % 8;
            float angle = direction * Mathf.PI / 4;
            Vector3 delta = preview.ActorPosition - interactionOrigin;
            Vector3 expected = new(Mathf.Sin(angle), Mathf.Cos(angle));
            if (delta.magnitude < .35f || Vector3.Dot(delta.normalized, expected) < .98f ||
                preview.Phase != "Run · " + HeadingNames[direction])
                throw new InvalidOperationException($"Incorrect movement for form {movementCase / 8 + 1}, {HeadingNames[direction]}.");
            if (direction % 2 != 0 && !preview.CurrentSprite.name.StartsWith(HeadingNames[direction] + "_", StringComparison.Ordinal))
                throw new InvalidOperationException("Run loop sampled a neighboring direction.");
            Press();
            AdvanceInteractionStep(2);
        }
        else if (interactionStep == 2 && elapsed > .16)
        {
            if (preview.Phase != "Idle · " + HeadingNames[movementCase % 8] || preview.CastingInFreePlay)
                throw new InvalidOperationException("Idle did not retain the last keyboard heading.");
            if (++movementCase < 32) StartMovement(preview);
            else
            {
                Debug.Log("HEXCORE KEYBOARD QA: 4 forms × 8 directions, WASD and arrow input, Run/Idle.");
                StartTestCast(preview, 0, 0);
            }
        }
        else if (interactionStep == 3)
        {
            if (preview.CastingInFreePlay)
            {
                jumpHeight = Mathf.Max(jumpHeight, preview.ActorPosition.y - interactionOrigin.y);
                if (testedSkill == 12 && preview.Elapsed > .49f && CaptureInteractionUi && !tookUi)
                {
                    Directory.CreateDirectory(InteractionFolder);
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath(InteractionFolder + "/GameViewAfter.png"));
                    tookUi = true;
                }
            }
            if (elapsed < .18) return false;
            if (elapsed > .35) Press();
            if (preview.CastingInFreePlay) return false;
            Vector3 delta = preview.ActorPosition - interactionOrigin;
            float expected = testedSkill == 0 ? 2.05f : .72f;
            if (Mathf.Abs(delta.x - expected) > .025f || Mathf.Abs(delta.y) > .015f ||
                testedSkill == 12 && jumpHeight < .50f)
                throw new InvalidOperationException("Dash or leap did not commit the skill endpoint.");
            if (testedSkill == 0) StartTestCast(preview, 2, 12);
            else
            {
                interactionOrigin = preview.ActorPosition;
                preview.Replay();
                if (!preview.CastingInFreePlay || Vector3.Distance(preview.ActorPosition, interactionOrigin) > .015f)
                    throw new InvalidOperationException("Replay moved the free-play actor back to the demo origin.");
                AdvanceInteractionStep(4);
            }
        }
        else if (interactionStep == 4 && elapsed > .20)
        {
            Press(Key.R);
            AdvanceInteractionStep(5);
        }
        else if (interactionStep == 5 && elapsed > .15)
        {
            Press();
            if (Vector3.Distance(preview.ActorPosition, spawn) > .015f || preview.CastingInFreePlay || preview.HasEffect)
                throw new InvalidOperationException("Explicit reset did not restore the spawn and clear the effect.");
            Debug.Log("HEXCORE FREE CAST QA: Space, Q1 displacement, R1 airborne/landing, replay in place, explicit R reset.");
            if (CaptureInteractionUi && !File.Exists(InteractionFolder + "/GameViewAfter.png"))
                throw new InvalidOperationException("Game View screenshot not written.");
            CaptureTransparencyCases(preview);
            return true;
        }
        return false;
    }

    private static void StartMovement(HexcoreSkillPreview preview)
    {
        int form = movementCase / 8;
        if (movementCase != 0 && movementCase % 8 == 0)
        {
            preview.SelectSkill(form, form * 3, 2, 2, false, false);
            preview.EnableFreePlay();
        }
        interactionOrigin = preview.ActorPosition;
        Press((form % 2 == 0 ? Wasd : Arrows)[movementCase % 8]);
        AdvanceInteractionStep(1);
    }

    private static void StartTestCast(HexcoreSkillPreview preview, int form, int skill)
    {
        testedSkill = skill;
        jumpHeight = 0;
        preview.SelectSkill(form, skill, 2, 2, false, false);
        preview.EnableFreePlay();
        interactionOrigin = preview.ActorPosition;
        Press(Key.D, Key.Space);
        AdvanceInteractionStep(3);
    }

    private static void CaptureTransparencyCases(HexcoreSkillPreview preview)
    {
        var material = Load<Material>(PlayerRoot + "MeleeUltimates/VanAnh/HexcoreGoldVfx.mat");
        if (material.shader == null || material.shader.name != "Brotato/HEXCORE Gold VFX" || !material.shader.isSupported)
            throw new InvalidOperationException("Preview gold material failed to load its shader.");
        var camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        float previousSize = camera.orthographicSize;
        camera.orthographicSize = 2.4f;
        preview.SelectSkill(1, 14, 2, 3, false, false);
        for (int frame = 16; frame <= 19; frame++)
        {
            preview.ReviewAt(frame * .075f + .003f, true);
            CaptureFrame(camera, InteractionFolder, "VanAnh-" + frame, 320, 180, rejectPink: true);
        }
        camera.orthographicSize = previousSize;
    }

    private static void Press(params Key[] keys) => InputSystem.QueueStateEvent(testKeyboard, new KeyboardState(keys));

    private static void AdvanceInteractionStep(int next)
    {
        interactionStep = next;
        interactionStart = EditorApplication.timeSinceStartup;
    }

    private static void ResetInteractionVerification()
    {
        if (testKeyboard != null) InputSystem.RemoveDevice(testKeyboard);
        testKeyboard = null;
        SessionState.SetBool(VerifyingInteraction, false);
        interactionStep = movementCase = 0;
        tookUi = false;
    }
}
