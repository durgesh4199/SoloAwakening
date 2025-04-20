using UnityEngine;

public class EnemyInitializer : MonoBehaviour
{
    public EnemyData data;
    private CharacterStats stats;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();

        if (data != null && stats != null)
        {
            stats.characterName = data.enemyName;
            stats.maxHP = data.GetMaxHP();
            stats.currentHP = stats.maxHP;

            stats.maxMP = data.GetMaxMP();
            stats.currentMP = stats.maxMP;

            stats.attackPower = data.GetAttackPower();
            stats.speed = data.GetSpeed();
            stats.accuracy = data.GetAccuracy();
            stats.skills = data.skills;

            // Optionally set sprite if you have a visual system
            // GetComponent<SpriteRenderer>().sprite = data.enemySprite;
        }
    }
}
