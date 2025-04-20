using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIManager : MonoBehaviour
{
    public CharacterStats enemy;

    public Image healthBar;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;

    [Header("HP Bar Animation")]
    public float hpLerpSpeed = 5f;
    private float displayedHPPercent = 1f;

    private void Update()
    {
        if (enemy == null) return;

        float targetHPPercent = (float)enemy.currentHP / enemy.maxHP;
        displayedHPPercent = Mathf.Lerp(displayedHPPercent, targetHPPercent, Time.deltaTime * hpLerpSpeed);

        healthBar.fillAmount = displayedHPPercent;

        //nameText.text = enemy.characterName;
        //hpText.text = $"{enemy.currentHP}/{enemy.maxHP}";
    }
}
