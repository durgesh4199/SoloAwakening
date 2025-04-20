using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator enemyAnimator;

    public void UseSkill(CharacterStats caster, CharacterStats target, SkillData skill)
    {
        if (caster.currentMP < skill.manaCost)
        {
            Debug.Log($"{caster.characterName} does not have enough mana!");
            return;
        }

        caster.currentMP -= skill.manaCost;

        /*Animator animator = caster.CompareTag("Player") ? playerAnimator : enemyAnimator;
        animator.SetTrigger(skill.animationTrigger);*/

        switch (skill.effectType)
        {
            case SkillEffectType.Damage:
                float hitChance = caster.GetCurrentAccuracy();
                if (Random.value > hitChance)
                {
                    Debug.Log("Missed!");
                    return;
                }

                int damage = caster.GetCurrentAttack() + skill.power;
                target.currentHP -= damage;
                Debug.Log($"{target.characterName} took {damage} damage.");
                break;

            case SkillEffectType.Buff:
                if (skill.appliedBuff != null)
                {
                    Buff newBuff = new Buff
                    {
                        type = skill.appliedBuff.type,
                        amount = skill.appliedBuff.amount,
                        duration = skill.appliedBuff.duration
                    };
                    caster.activeBuffs.Add(newBuff);
                    Debug.Log($"{caster.characterName} gained {newBuff.type} buff!");
                }
                break;

            case SkillEffectType.Debuff:
                if (skill.appliedBuff != null)
                {
                    Buff newDebuff = new Buff
                    {
                        type = skill.appliedBuff.type,
                        amount = -Mathf.Abs(skill.appliedBuff.amount),
                        duration = skill.appliedBuff.duration
                    };
                    target.activeBuffs.Add(newDebuff);
                    Debug.Log($"{target.characterName} got hit with {newDebuff.type} debuff!");
                }
                break;
        }
    }


}