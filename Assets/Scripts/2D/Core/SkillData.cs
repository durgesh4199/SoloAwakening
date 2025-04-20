using UnityEngine;

public enum SkillEffectType { Damage, Buff, Debuff, Heal }

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData")]
[System.Serializable]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int power;
    public int manaCost;
    public string animationTrigger;
    public float cooldown;
    public SkillEffectType effectType;
    public Buff appliedBuff;

    // Add this property
    public SkillTargetType targetType; // Defines the target type (Self, Enemy, Ally)
}
