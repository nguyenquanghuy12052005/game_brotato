using UnityEngine;
using CueAction = HexcorePreviewCatalog.Action;

public sealed partial class HexcoreSkillPreview
{
    private enum Cue { Foot, Dash, Swing, HeavySwing, Impact, Parry, Charge, Wave, Vanish, Return, Slam, Burst, Finisher }

    [SerializeField] private AudioClip[] cues;
    private AudioSource audioSource;
    private int lastDashFrame = -1;
    private int lastBasicFrame = -1;
    private bool parryCuePlayed;
    private int playedCues;
    private const int DashCueMask = (1 << (int)Cue.Foot) | (1 << (int)Cue.Dash) |
        (1 << (int)Cue.Swing) | (1 << (int)Cue.Impact);

    public static int AudioCueCount => System.Enum.GetValues(typeof(Cue)).Length;
    public static string AudioCueName(int index) => ((Cue)index).ToString();
    public bool PlayedDashCues => (playedCues & DashCueMask) == DashCueMask;

    private void Play(Cue cue)
    {
        if (scrubbing || paused || !hear) return;
        playedCues |= 1 << (int)cue;
        audioSource.PlayOneShot(cues[(int)cue], audioSource.isPlaying ? .7f : 1f);
    }

    private void PlaySkillCue(CueAction action)
    {
        switch (action)
        {
            case CueAction.Advance: Play(Cue.Dash); break;
            case CueAction.Rise: Play(Cue.Foot); break;
            case CueAction.Vanish: Play(Cue.Vanish); break;
            case CueAction.Return: Play(Cue.Return); break;
            case CueAction.Swing: Play(formIndex == 2 ? Cue.HeavySwing : Cue.Swing); break;
            case CueAction.Guard:
                if (skillIndex != 2) Play(Cue.Parry);
                break;
            case CueAction.Charge or CueAction.Aura: Play(Cue.Charge); break;
            case CueAction.Wave or CueAction.Chase or CueAction.Rain: Play(Cue.Wave); break;
            case CueAction.Hit: Play(skillIndex == 14 || skillIndex == 16 ? Cue.Finisher : Cue.Impact); break;
            case CueAction.Land: Play(Cue.Slam); break;
            case CueAction.Burst: Play(Cue.Burst); break;
            case CueAction.Catch: Play(Cue.Parry); break;
        }
    }

    private void PlayDashFrame(int frame)
    {
        if (frame == lastDashFrame) return;
        lastDashFrame = frame;
        switch (frame)
        {
            case 3: Play(Cue.Foot); break;
            case 4: Play(Cue.Dash); break;
            case 6: Play(Cue.Swing); break;
            case 7: Play(Cue.Impact); break;
            case 9: Play(Cue.Foot); break;
        }
    }

    private void PlayBasicFrame(int frame, int count = 12)
    {
        if (frame == lastBasicFrame) return;
        lastBasicFrame = frame;
        if (count == 8)
        {
            if (frame == 2 || frame == (formIndex == 1 ? 4 : 5) || formIndex == 1 && frame == 6)
                Play(formIndex == 2 ? Cue.HeavySwing : frame == 5 && formIndex == 3 ? Cue.Wave : Cue.Swing);
            return;
        }
        if (formIndex == 1 && (frame == 2 || frame == 6 || frame == 10)) Play(Cue.Swing);
        if ((formIndex == 0 || formIndex == 2) && (frame == 3 || frame == (formIndex == 0 ? 7 : 9)))
            Play(formIndex == 2 ? Cue.HeavySwing : Cue.Swing);
        if (formIndex == 3 && frame == 3) Play(Cue.Swing);
        if (formIndex == 3 && frame == 9) Play(Cue.Wave);
    }
}
