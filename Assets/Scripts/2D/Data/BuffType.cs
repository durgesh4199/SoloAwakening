using UnityEngine;

public enum BuffType { Attack, Accuracy, Defense, Speed, Stun, Poison }

[CreateAssetMenu(fileName = "NewBuff", menuName = "Buffs/BuffData")]
public class Buff : ScriptableObject
{
    public BuffType type;
    public int amount; // Can be negative for debuffs
    public int duration; // In turns
    public string iconName; // Optional: for UI icons
    public string description;
}
