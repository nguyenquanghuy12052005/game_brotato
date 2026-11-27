using System;
using UnityEngine;

public sealed class MageExperience : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private int level = 1;
    [SerializeField, Min(0)] private int currentExperience;
    [SerializeField, Min(1f)] private float experienceMultiplier = 1f;
    [SerializeField, Range(1, 20)] private int levelLimit = 20;

    public int Level => level;
    public int LevelLimit => levelLimit;
    public int CurrentExperience => currentExperience;
    public int ExperienceToNextLevel => RequiredExperienceForLevel(level);
    public float ExperienceMultiplier => experienceMultiplier;
    public event Action<int> LevelReached;

    public int RequiredExperienceForLevel(int currentLevel) =>
        currentLevel >= levelLimit ? 0 : 10 + 2 * (currentLevel - 1);

    public void SetLevelLimit(int value)
    {
        levelLimit = Mathf.Clamp(value, 1, 20);
        level = Mathf.Min(level, levelLimit);
        if (level >= levelLimit) currentExperience = 0;
    }

    public void AddExperience(int baseAmount)
    {
        if (baseAmount <= 0 || level >= levelLimit) return;
        currentExperience += Mathf.Max(1, Mathf.RoundToInt(baseAmount * experienceMultiplier));
        while (level < levelLimit && currentExperience >= ExperienceToNextLevel)
        {
            currentExperience -= ExperienceToNextLevel;
            level++;
            LevelReached?.Invoke(level);
        }
        if (level >= levelLimit) currentExperience = 0;
    }

    public void SetExperienceMultiplier(float value)
    {
        experienceMultiplier = Mathf.Max(1f, value);
    }
}
