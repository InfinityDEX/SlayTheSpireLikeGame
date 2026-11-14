using TMPro;
using UnityEngine;
using VContainer;

/// <summary>
/// 所持金を画面上に表示するクラス
/// </summary>
public class HUDCoinCounter : MonoBehaviour
{
    [field:SerializeField, Header("所持金を表示するTextMeshProインスタンス")]
    private TextMeshProUGUI coinCountViewUGUI;

    /// <summary>
    /// セーブデータホルダー
    /// </summary>
    [Inject]
    public SaveDataHolder SaveDataHolderInstance { get; private set; }

    private void Update()
    {
        var saveDataHolder = SaveDataHolderInstance;
        if (saveDataHolder != null && coinCountViewUGUI != null)
        {
            coinCountViewUGUI.text = $"X {saveDataHolder.SaveData.money.ToString("00000")}";
        }
    }
}
