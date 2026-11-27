using System;
using UnityEngine;

public sealed class MageExperience : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private int level = 1;
    [SerializeField, Min(0)] private int currentExperience;
    [SerializeField, Min(1f)] private float experienceMultiplier = 1f;

    public int Level => level;
    public int CurrentExperience => currentExperience;
    public int ExperienceToNextLevel => level >= 20 ? 0 : 10 + 2 * (level - 1);
    public float ExperienceMultiplier => experienceMultiplier;
    public event Action<int> LevelReached;

    public void AddExperience(int baseAmount)
    {
        if (baseAmount <= 0 || level >= 20) return;
        currentExperience += Mathf.Max(1, Mathf.RoundToInt(baseAmount * experienceMultiplier));
        while (level < 20 && currentExperience >= ExperienceToNextLevel)
        {
            currentExperience -= ExperienceToNextLevel;
            level++;
            LevelReached?.Invoke(level);
        }
        if (level >= 20) currentExperience = 0;
    }

    public void SetExperienceMultiplier(float value)
    {
        experienceMultiplier = Mathf.Max(1f, value);
    }
}
