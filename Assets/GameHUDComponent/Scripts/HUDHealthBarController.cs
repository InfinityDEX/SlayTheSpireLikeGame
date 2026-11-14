using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// HUD上の体力バーの操作クラス
/// </summary>
public class HUDHealthBarController : MonoBehaviour
{
    [field: SerializeField, Header("HUD上の体力バー")]
    private Slider healthBar;
    [field: SerializeField, Header("HUD上の体力バーテキスト")]
    private TextMeshProUGUI healthText;

    /// <summary>
    /// セーブデータホルダー
    /// </summary>
    [Inject]
    public SaveDataHolder SaveDataHolderInstance { get; private set; }

    public void Update()
    {
        // 体力バーの更新
        if (SaveDataHolderInstance != null)
        {
            var saveData = SaveDataHolderInstance.SaveData;
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
