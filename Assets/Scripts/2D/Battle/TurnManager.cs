using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    private Queue<CharacterStats> turnQueue = new Queue<CharacterStats>();

    public void Initialize(CharacterStats player, CharacterStats enemy)
    {
        turnQueue.Clear();
        if (player.speed >= enemy.speed)
        {
            turnQueue.Enqueue(player);
            turnQueue.Enqueue(enemy);
        }
        else
        {
            turnQueue.Enqueue(enemy);
            turnQueue.Enqueue(player);
        }
    }

    public void NextTurn()
    {
        CharacterStats current = turnQueue.Dequeue();
        // Reduce buff durations
        UpdateBuffDurations(current);

        turnQueue.Enqueue(current);
    }

    public bool IsPlayerTurn()
    {
        return turnQueue.Peek().CompareTag("Player");
    }
    

    private void UpdateBuffDurations(CharacterStats character)
    {
        List<Buff> toRemove = new();

        foreach (Buff buff in character.activeBuffs)
        {
            buff.duration--;
            if (buff.duration <= 0)
            {
                toRemove.Add(buff);
            }
        }

        foreach (Buff buff in toRemove)
        {
            character.activeBuffs.Remove(buff);
            Debug.Log($"{character.characterName} lost {buff.type} buff.");
        }
    }

}