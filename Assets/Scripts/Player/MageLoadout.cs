using UnityEngine;

public enum MageStarterSkill { Heal, Shockwave, Gravity }
public enum MageElement { Fire, Ice, Light, Dark }
public enum MageUltimateKind { TimeStop, StarFall, ArcaneOrb }

// Inspector choices let the team test every skill before adding EXP and level UI.
public sealed class MageLoadout : MonoBehaviour
{
    [Header("Choose for Play Mode")]
    [SerializeField] private MageStarterSkill starterSkill = MageStarterSkill.Heal;
    [SerializeField] private MageElement element = MageElement.Fire;
    [SerializeField] private MageUltimateKind ultimate = MageUltimateKind.TimeStop;

    [Header("Fire, levels 10-12")]
    [SerializeField, Range(0, 3)] private int blastPower;
    [SerializeField, Range(0, 3)] private int burnPower;
    [SerializeField, Range(0, 3)] private int blastRadius;
    [Header("Fire cores")]
    [SerializeField] private bool longBurn;
    [SerializeField] private bool extraFireballs;
    [SerializeField] private bool focusBurning;
    [SerializeField] private bool deathExplosion;

    [Header("Ice, levels 10-12")]
    [SerializeField, Range(0, 3)] private int strongerSlow;
    [SerializeField, Range(0, 3)] private int longerFreeze;
    [SerializeField, Range(0, 3)] private int chilledDamage;
    [Header("Ice cores")]
    [SerializeField] private bool deepCold;
    [SerializeField] private bool coldFocus;
    [SerializeField] private bool brittle;
    [SerializeField] private bool spreadCold;

    [Header("Light, levels 10-12")]
    [SerializeField, Range(0, 3)] private int extraPulses;
    [SerializeField, Range(0, 3)] private int pillarRadius;
    [SerializeField, Range(0, 3)] private int pulsePower;
    [Header("Light cores")]
    [SerializeField] private bool afterglow;
    [SerializeField] private bool judgement;
    [SerializeField] private bool guideLight;
    [SerializeField] private bool dazzle;

    [Header("Dark, levels 10-12")]
    [SerializeField, Range(0, 3)] private int extraCurseTargets;
    [SerializeField, Range(0, 3)] private int cursePower;
    [SerializeField, Range(0, 3)] private int curseWeakness;
    [Header("Dark cores")]
    [SerializeField] private bool manySeals;
    [SerializeField] private bool torment;
    [SerializeField] private bool frailty;
    [SerializeField] private bool testament;

    public MageStarterSkill StarterSkill => starterSkill;
    public MageElement Element => element;
    public MageUltimateKind Ultimate => ultimate;
    public int BlastPower => blastPower;
    public int BurnPower => burnPower;
    public int BlastRadius => blastRadius;
    public bool LongBurn => longBurn;
    public bool ExtraFireballs => extraFireballs;
    public bool FocusBurning => focusBurning;
    public bool DeathExplosion => deathExplosion;
    public int StrongerSlow => strongerSlow;
    public int LongerFreeze => longerFreeze;
    public int ChilledDamage => chilledDamage;
    public bool DeepCold => deepCold;
    public bool ColdFocus => coldFocus;
    public bool Brittle => brittle;
    public bool SpreadCold => spreadCold;
    public int ExtraPulses => extraPulses;
    public int PillarRadius => pillarRadius;
    public int PulsePower => pulsePower;
    public bool Afterglow => afterglow;
    public bool Judgement => judgement;
    public bool GuideLight => guideLight;
    public bool Dazzle => dazzle;
    public int ExtraCurseTargets => extraCurseTargets;
    public int CursePower => cursePower;
    public int CurseWeakness => curseWeakness;
    public bool ManySeals => manySeals;
    public bool Torment => torment;
    public bool Frailty => frailty;
    public bool Testament => testament;

    public void SelectStarter(MageStarterSkill value) { starterSkill = value; }
    public void SelectElement(MageElement value) { element = value; }
    public void SelectUltimate(MageUltimateKind value) { ultimate = value; }

    public int GetTier(int index)
    {
        switch (element)
        {
            case MageElement.Fire: return index == 0 ? blastPower : index == 1 ? burnPower : blastRadius;
            case MageElement.Ice: return index == 0 ? strongerSlow : index == 1 ? longerFreeze : chilledDamage;
            case MageElement.Light: return index == 0 ? extraPulses : index == 1 ? pillarRadius : pulsePower;
            default: return index == 0 ? extraCurseTargets : index == 1 ? cursePower : curseWeakness;
        }
    }

    public void SetTier(int index, int value)
    {
        value = Mathf.Clamp(value, 0, 3);
        switch (element)
        {
            case MageElement.Fire:
                if (index == 0) blastPower = value;
                else if (index == 1) burnPower = value;
                else blastRadius = value;
                break;
            case MageElement.Ice:
                if (index == 0) strongerSlow = value;
                else if (index == 1) longerFreeze = value;
                else chilledDamage = value;
                break;
            case MageElement.Light:
                if (index == 0) extraPulses = value;
                else if (index == 1) pillarRadius = value;
                else pulsePower = value;
                break;
            case MageElement.Dark:
                if (index == 0) extraCurseTargets = value;
                else if (index == 1) cursePower = value;
                else curseWeakness = value;
                break;
        }
    }

    public bool GetCore(int index)
    {
        switch (element)
        {
            case MageElement.Fire: return index == 0 ? longBurn : index == 1 ? extraFireballs : index == 2 ? focusBurning : deathExplosion;
            case MageElement.Ice: return index == 0 ? deepCold : index == 1 ? coldFocus : index == 2 ? brittle : spreadCold;
            case MageElement.Light: return index == 0 ? afterglow : index == 1 ? judgement : index == 2 ? guideLight : dazzle;
            default: return index == 0 ? manySeals : index == 1 ? torment : index == 2 ? frailty : testament;
        }
    }

    public void SetCore(int index, bool value)
    {
        switch (element)
        {
            case MageElement.Fire:
                if (index == 0) longBurn = value;
                else if (index == 1) extraFireballs = value;
                else if (index == 2) focusBurning = value;
                else deathExplosion = value;
                break;
            case MageElement.Ice:
                if (index == 0) deepCold = value;
                else if (index == 1) coldFocus = value;
                else if (index == 2) brittle = value;
                else spreadCold = value;
                break;
            case MageElement.Light:
                if (index == 0) afterglow = value;
                else if (index == 1) judgement = value;
                else if (index == 2) guideLight = value;
                else dazzle = value;
                break;
            case MageElement.Dark:
                if (index == 0) manySeals = value;
                else if (index == 1) torment = value;
                else if (index == 2) frailty = value;
                else testament = value;
                break;
        }
    }
}
