using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;

    [Header("Core Stats")]
    public int Strength;
    public int Agility;
    public int Intelligence;
    public int Perception;
    public int Vitality;

    [Header("Skills")]
    public List<SkillData> skills;

    // Derived from core stats during runtime
    public int GetMaxHP() => (Vitality * 10);
    public int GetMaxMP() => (Intelligence * 5);
    public int GetAttackPower() => Strength * 2;
    public int GetSpeed() => Agility;
    public float GetAccuracy() => 0.5f + (Perception * 0.01f);
}