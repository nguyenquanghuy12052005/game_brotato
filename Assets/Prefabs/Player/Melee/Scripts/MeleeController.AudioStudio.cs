using System;
using UnityEngine;

public sealed partial class MeleeController
{
    // Used only by AudioPreviewScene. The existing sandbox keeps its own time and GUI.
    internal bool AudioStudio { get; set; }
    internal bool StudioReady => actor != null;
    internal float StudioDuration => selection == Preview.Basic && skillIndex != 0 ? AttackDuration : CurrentSkillDuration;

    internal void StudioPrepare(int form, int skill, bool targets, bool effects)
    {
        hear = hitStop = shake = false;
        previewV2 = skill != 0;
        polishedPreview = true;
        SetFreeMode(false);
        SelectForm(form);
        branch = Mathf.Max(1, form);
        aim = locomotionDirection = 2;
        selection = skill < 0 ? Preview.Basic : Preview.Q1;
        skillIndex = Mathf.Max(0, skill);
        if (skill < 0) skillIndex = 1;
        skillGroup = Array.FindIndex(V2Slots, slots => Array.IndexOf(slots, skillIndex) >= 0);
        previewLoadout[0] = -1;
        targetCount = targets ? 3 : 0;
        showEffects = effects;
        ResetPractice();
        ConfigureEffect();
        RestartPlayback();
        paused = true;
        if (skill == 0) dummy.enabled = targets;
    }

    internal float StudioTimeOf(int beat, float fraction = 0)
    {
        int index = Mathf.Clamp(beat, 0, Chosen.Beats.Length - 1);
        return Mathf.Lerp(Chosen.Starts[index], Chosen.Starts[index + 1], fraction);
    }

    internal void StudioSample(float time, float delta, bool effects, bool targets, bool freezeActor)
    {
        Sprite previous = actor.sprite;
        Vector3 previousPosition = actor.transform.position;
        Color previousColor = actor.color;
        showEffects = effects;
        practiceTime += Mathf.Max(0, delta);
        clock = time;
        Render();
        if (!targets)
        {
            dummy.enabled = false;
            foreach (var target in trainingTargets)
                if (target != null) target.Renderer.enabled = false;
        }
        if (!freezeActor) return;
        actor.sprite = previous;
        actor.transform.position = previousPosition;
        actor.color = previousColor;
        actor.enabled = true;
    }

    internal void StudioPassive(int choice, bool targets, bool effects)
    {
        StudioPrepare(0, -1, targets, effects);
        previewLoadout[0] = choice;
        if (targets && trainingTargets[0] != null)
            trainingTargets[0].Renderer.transform.position = FreeSpawn + TrainingFloor + DummyLift + AimVector * 1.45f;
    }

    internal void StudioFirePassive()
    {
        TriggerPassive(ActorPosition, AimVector);
    }
}
