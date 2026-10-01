using TMPro;
using UnityEngine;

/// <summary>
/// 所持金を画面上に表示するクラス
/// </summary>
public class HUDCoinCounter : MonoBehaviour
{
    [field:SerializeField, Header("所持金を表示するTextMeshProインスタンス")]
    private TextMeshProUGUI coinCountViewUGUI;

    private void Update()
    {
        var saveDataHolder = SaveDataHolder.Instance;
        if (saveDataHolder != null && coinCountViewUGUI != null)
        {
            coinCountViewUGUI.text = $"X {saveDataHolder.SaveData.money.ToString("00000")}";
        }
    }
}
