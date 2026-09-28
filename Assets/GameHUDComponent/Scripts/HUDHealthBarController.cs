using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD上の体力バーの操作クラス
/// </summary>
public class HUDHealthBarController : MonoBehaviour
{
    [field: SerializeField, Header("HUD上の体力バー")]
    private Slider healthBar;
    [field: SerializeField, Header("HUD上の体力バーテキスト")]
    private TextMeshProUGUI healthText;

    public void Update()
    {
        // 体力バーの更新
        if (SaveDataHolder.Instance != null)
        {
            var saveData = SaveDataHolder.Instance.SaveData;
            if (healthBar != null)
            {
                healthBar.maxValue = saveData.maxHealth;
                healthBar.value = saveData.currentHealth;
            }
            if (healthText != null)
            {
                healthText.text = $"{saveData.currentHealth}/{saveData.maxHealth}";
            }
        }

    }
}
