// File: Assets/Scripts/Characters/CharacterStats.cs

using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public string characterName;

    public int maxHP;
    public int currentHP;

    public int maxMP;
    public int currentMP;

    public int attackPower;
    public int speed;
    public float accuracy; // 0 to 1 (e.g., 0.85 = 85% hit chance)
    public float critChance;

    public bool IsStunned = false;
    public List<SkillData> skills;
    public List<Buff> activeBuffs = new();

    // Gauge system (for active time turns)
    public float currentGauge = 0f;
    public float actionThreshold = 100f;
    public Dictionary<SkillData, float> skillCooldowns = new();

    public int GetCurrentAttack()
    {
        int modifiedAttack = attackPower;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.type == BuffType.Attack)
                modifiedAttack += buff.amount;
        }
        return modifiedAttack;
    }
    public float GetHealthPercentage()
    {
        return (float)currentHP / maxHP;
    }

    public float GetCurrentAccuracy()
    {
        float modifiedAccuracy = accuracy;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.type == BuffType.Accuracy)
                modifiedAccuracy += buff.amount * 0.01f;
        }
        return Mathf.Clamp(modifiedAccuracy, 0f, 1f);
    }
    public int GetPowerRating()
    {
        int rating = 0;
        rating += maxHP / 10;
        rating += attackPower * 2;
        rating += speed * 3;
        rating += Mathf.RoundToInt(accuracy * 100);
        rating += maxMP / 10;

        string letterWithRating = "Rank " + GetRatingLetter(rating).ToString() + " " + rating;
        // You can also factor in equipped gear, skills, etc.
        return rating;
    }
    string GetRatingLetter(float rating)
    {
        if (rating >= 15000) return "GOD";
        else if (rating >= 10000) return "S";
        else if(rating >= 5000) return "A";
        else if(rating >= 1000) return "B";
        else if(rating >= 500) return "C";
        else if(rating >= 200) return "D";
        return "E";
    }
    public void TickBuffDurations()
    {
        List<Buff> expiredBuffs = new List<Buff>();

        foreach (Buff buff in activeBuffs)
        {
            buff.duration--;
            if (buff.duration <= 0)
            {
                expiredBuffs.Add(buff);
            }
        }

        foreach (Buff expired in expiredBuffs)
        {
            activeBuffs.Remove(expired);
            Debug.Log($"{characterName}'s {expired.type} buff expired.");
        }
    }

}
