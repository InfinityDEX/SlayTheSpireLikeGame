using TMPro;
using UnityEngine;
using VContainer;

/// <summary>
/// 現在何階層目にいるかを画面上に表示するクラス
/// </summary>
public class HUDFloorNumberCounter : MonoBehaviour
{
    [field:SerializeField, Header("階層情報を表示するTextMeshProインスタンス")]
    private TextMeshProUGUI floorNumViewUGUI;

    /// <summary>
    /// セーブデータホルダー
    /// </summary>
    [Inject]
    public SaveDataHolder SaveDataHolderInstance { get; private set; }

    private void Update()
    {
        var saveDataHolder = SaveDataHolderInstance;
        if (saveDataHolder != null && floorNumViewUGUI != null)
        {
            floorNumViewUGUI.text = $"[{saveDataHolder.SaveData.currentFloor.ToString("00")}] F";
        }
    }
}
