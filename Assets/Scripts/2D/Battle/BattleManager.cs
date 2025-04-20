using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;

public class BattleManager : MonoBehaviour
{
    public BattleState currentState;

    public CharacterStats player;
    public CharacterStats enemy;
    public EnemyAI enemyAI;
    public SkillManager skillManager;

    private void Start()
    {
        Debug.Log("Player Rating: " + player.GetPowerRating());
        Debug.Log("Enemy Rating: " + enemy.GetPowerRating());
        enemyAI.Init(enemy, player);
    }

    public void OnPlayerGaugeReady(CharacterStats character)
    {
        //character.HasActed = false;
        Debug.Log($"{character.characterName} is ready to act!");
        // Show skill selection UI here
    }

    public void OnEnemyGaugeReady(CharacterStats enemy)
    {
        //enemy.HasActed = false;
        StartCoroutine(EnemyGaugeAction(enemy));
    }

    private IEnumerator EnemyGaugeAction(CharacterStats enemy)
    {
        if (enemy.IsStunned) yield break; // skip stunned enemies

        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));

        EnemyBehaviorMode behavior = GetEnemyBehavior(enemy, player);
        SkillData skill = enemyAI.ChooseSkill();
        if (skill != null)
            skillManager.UseSkill(enemy, player, skill);
        else
            Debug.LogWarning($"{enemy.characterName} has no valid skill to use!");


        player.TickBuffDurations();
        enemy.TickBuffDurations();
    }

    private EnemyBehaviorMode GetEnemyBehavior(CharacterStats enemy, CharacterStats player)
    {
        int playerRating = player.GetPowerRating();
        int enemyRating = enemy.GetPowerRating();
        int diff = enemyRating - playerRating;

        if (diff >= 50)
            return EnemyBehaviorMode.Aggressive;
        else if (diff <= -100)
            return EnemyBehaviorMode.Flee;
        else if (diff <= -50)
            return EnemyBehaviorMode.Defensive;
        else
            return EnemyBehaviorMode.Balanced;
    }

    public void OnPlayerSkillClicked(SkillData skill)
    {

        skillManager.UseSkill(player, enemy, skill);
        player.TickBuffDurations();
    }
  
    public void OnAttackButtonClicked()
    {
        if (player.currentGauge < player.actionThreshold)
        {
            Debug.Log("Not ready to act yet!");
            return;
        }

        if (player.skills.Count > 0)
        {
            OnPlayerSkillClicked(player.skills[0]); // Use first skill
            player.currentGauge = 0f;
            //player.HasActed = true;
        }
    }

}