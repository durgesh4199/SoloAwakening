using System.Collections.Generic;
using UnityEngine;

public class GaugeManager : MonoBehaviour
{
    public List<CharacterStats> allCharacters = new();
    public BattleManager battleManager;

    void Update()
    {
        foreach (CharacterStats character in allCharacters)
        {
            if (character.currentHP <= 0)
                continue;

            if (character.currentGauge < character.actionThreshold)
            {
                character.currentGauge += character.speed * Time.deltaTime;

                if (character.currentGauge >= character.actionThreshold)
                {
                    character.currentGauge = character.actionThreshold; // clamp to threshold

                    if (character.CompareTag("Player"))
                    {
                        battleManager.OnPlayerGaugeReady(character);
                    }
                    else
                    {
                        character.currentGauge = 0f; // reset right away for enemy
                        battleManager.OnEnemyGaugeReady(character);
                    }
                }
            }
        }
    }
}
