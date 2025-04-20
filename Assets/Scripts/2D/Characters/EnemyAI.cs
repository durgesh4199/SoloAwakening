// File: Assets/Scripts/Characters/EnemyAI.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyBehaviorMode
{
    Aggressive,
    Defensive,
    Balanced,
    Flee
}
public enum SkillTargetType
{
    Self,
    Enemy,
    Ally
}
public class EnemyAI : MonoBehaviour
{
    private CharacterStats self;
    private CharacterStats target;

    // --- Tunable AI Parameters ---
    [Header("AI Tuning")]
    [Range(0f, 1f)]
    [Tooltip("HP percentage below which the AI prioritizes healing.")]
    public float criticalHealthThreshold = 0.3f; // Prioritize healing below 30% HP

    [Range(0f, 1f)]
    [Tooltip("Target HP percentage below which the AI prioritizes finishing moves.")]
    public float targetKillThreshold = 0.2f; // Prioritize damage when target is below 20% HP

    [Range(1, 100)]
    [Tooltip("How much the AI prioritizes healing when below the threshold.")]
    public int healingPriorityBoost = 1000;

    [Range(1, 100)]
    [Tooltip("How much the AI prioritizes damage when the target is below the kill threshold.")]
    public int killPriorityBoost = 500;

    [Range(1, 100)]
    [Tooltip("Base score multiplier for buff skills.")]
    public int buffUtilityValue = 50;

    [Range(1, 100)]
    [Tooltip("Base score multiplier for debuff skills.")]
    public int debuffUtilityValue = 60; // Slightly higher than buffs maybe?

    [Tooltip("Should the AI consider fleeing?")]
    public bool canFlee = true;
    [Range(0f, 1f)]
    [Tooltip("HP percentage below which the AI might consider fleeing if significantly weaker.")]
    public float fleeHealthThreshold = 0.15f; // Consider fleeing below 15% HP
    [Tooltip("Power rating difference needed to trigger flee consideration (Enemy Rating - Player Rating). Must be negative.")]
    public int fleePowerDifferenceThreshold = -75;

    // --- Core Logic ---

    public void Init(CharacterStats enemySelf, CharacterStats playerTarget)
    {
        self = enemySelf;
        target = playerTarget;

        if (self == null || target == null)
        {
            Debug.LogError("EnemyAI Init failed: Self or Target CharacterStats is null!");
        }
        if (self.skills == null || self.skills.Count == 0)
        {
            Debug.LogWarning("EnemyAI Init: Self has no skills assigned!");
        }
    }

    public SkillData ChooseSkill()
    {
        if (self == null || target == null || self.skills == null || self.skills.Count == 0)
        {
            Debug.LogError("Cannot choose skill: AI not initialized properly or has no skills.");
            return null; // No valid action
        }

        // --- Flee Check (Early Exit) ---
        if (ShouldConsiderFleeing())
        {
            Debug.Log($"{self.characterName} considers fleeing!");
            var defensiveSkill = FindBestDefensiveSkill();
            if (defensiveSkill != null) return defensiveSkill;
            return self.skills.Where(s => s.manaCost <= self.currentMP)
                              .OrderBy(s => s.manaCost)
                              .FirstOrDefault();
        }

        // --- Evaluate Usable Skills ---
        List<SkillEvaluation> evaluatedSkills = new List<SkillEvaluation>();
        foreach (SkillData skill in self.skills)
        {
            if (skill.manaCost > self.currentMP)
            {
                continue; // Skip skills we can't afford
            }

            int score = CalculateSkillScore(skill);
            evaluatedSkills.Add(new SkillEvaluation { skill = skill, score = score });
        }

        // --- Select Best Skill ---
        if (evaluatedSkills.Count == 0)
        {
            Debug.LogWarning($"{self.characterName} has no usable skills (not enough mana?).");
            return null;
        }

        evaluatedSkills = evaluatedSkills.OrderByDescending(e => e.score)
                                         .ThenByDescending(e => e.skill.power)
                                         .ToList();

        SkillData chosenSkill = evaluatedSkills[0].skill;
        Debug.Log($"{self.characterName} chose skill: {chosenSkill.skillName} with score: {evaluatedSkills[0].score}");
        return chosenSkill;
    }

    private int CalculateSkillScore(SkillData skill)
    {
        int score = 0;
        float selfHealthPercent = self.GetHealthPercentage();
        float targetHealthPercent = target.GetHealthPercentage();

        switch (skill.effectType)
        {
            case SkillEffectType.Damage:
                score = skill.power;
                if (targetHealthPercent <= targetKillThreshold)
                {
                    score += killPriorityBoost;
                }
                if (selfHealthPercent < criticalHealthThreshold && targetHealthPercent > targetKillThreshold)
                {
                    score /= 2;
                }
                break;

            case SkillEffectType.Heal:
                if (skill.targetType == SkillTargetType.Self)
                {
                    float missingHealthPercent = 1.0f - selfHealthPercent;
                    score = (int)(skill.power * missingHealthPercent * 2);

                    if (selfHealthPercent < criticalHealthThreshold)
                    {
                        score += healingPriorityBoost;
                    }
                    if (selfHealthPercent > 0.9f) score /= 4;
                }
                break;

            case SkillEffectType.Buff:
                if (skill.targetType == SkillTargetType.Self)
                {
                    score = buffUtilityValue + skill.power;
                    if (target.GetPowerRating() > self.GetPowerRating() + 30) score += buffUtilityValue / 2;
                    if (selfHealthPercent < 0.5f) score += buffUtilityValue / 2;
                }
                break;

            case SkillEffectType.Debuff:
                if (skill.targetType == SkillTargetType.Enemy)
                {
                    score = debuffUtilityValue + skill.power;
                    if (target.GetPowerRating() > self.GetPowerRating()) score += debuffUtilityValue / 2;
                }
                break;

            default:
                score = skill.power;
                break;
        }

        return Mathf.Max(0, score);
    }

    private bool ShouldConsiderFleeing()
    {
        if (!canFlee) return false;

        int powerDiff = self.GetPowerRating() - target.GetPowerRating();

        if (self.GetHealthPercentage() < fleeHealthThreshold && powerDiff < fleePowerDifferenceThreshold)
        {
            return true;
        }
        return false;
    }

    private SkillData FindBestDefensiveSkill()
    {
        return self.skills
            .Where(s => s.manaCost <= self.currentMP && (s.effectType == SkillEffectType.Heal || s.effectType == SkillEffectType.Buff) && s.targetType == SkillTargetType.Self)
            .OrderByDescending(s => CalculateSkillScore(s))
            .FirstOrDefault();
    }

    private class SkillEvaluation
    {
        public SkillData skill;
        public int score;
    }
}
