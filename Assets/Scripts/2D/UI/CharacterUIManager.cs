using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIManager : MonoBehaviour
{
    public CharacterStats character;

    [Header("UI Elements")]
    public Image healthBar;
    public Image manaBar;
    public Image gaugeBar;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;

    private void Update()
    {
        if (character == null) return;

        float hpRatio = (float)character.currentHP / character.maxHP;
        float mpRatio = (float)character.currentMP / character.maxMP;
        float gaugeRatio = character.currentGauge / character.actionThreshold;

        healthBar.fillAmount = hpRatio;
        //manaBar.fillAmount = mpRatio;
        gaugeBar.fillAmount = gaugeRatio;

        //nameText.text = character.characterName;
        //hpText.text = $"{character.currentHP}/{character.maxHP}";
        //mpText.text = $"{character.currentMP}/{character.maxMP}";
    }
}