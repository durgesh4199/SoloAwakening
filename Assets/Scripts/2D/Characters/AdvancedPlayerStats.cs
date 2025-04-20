using UnityEngine;

[System.Serializable]
public class PlayerCoreStats
{
    public int Strength;      // Increases attack power
    public int Agility;       // Increases speed and evasion
    public int Intelligence;  // Increases MP and magic power
    public int Perception;    // Increases accuracy and crit chance
    public int Vitality;      // Increases HP and defense

    public PlayerCoreStats(int str, int agi, int intel, int per, int vit)
    {
        Strength = str;
        Agility = agi;
        Intelligence = intel;
        Perception = per;
        Vitality = vit;
    }
}

public class AdvancedPlayerStats : MonoBehaviour
{
    public PlayerCoreStats coreStats;
    public CharacterStats characterStats;
    public float critChance = 0f;

    private void Start()
    {
        ApplyCoreStats();
    }

    public void ApplyCoreStats()
    {
        // Derived stats based on core stats
        characterStats.maxHP = (coreStats.Vitality * 10);
        characterStats.currentHP = (coreStats.Vitality * 10);

        characterStats.maxMP = (coreStats.Intelligence * 5);
        characterStats.currentMP = (coreStats.Intelligence * 5);

        characterStats.attackPower = coreStats.Strength * 2;

        characterStats.speed = coreStats.Agility;

        characterStats.accuracy = 0.5f + (coreStats.Perception * 0.01f);

        characterStats.critChance = coreStats.Perception * 0.01f;
    }

}