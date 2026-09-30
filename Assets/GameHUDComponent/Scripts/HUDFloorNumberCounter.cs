using TMPro;
using UnityEngine;

/// <summary>
/// 現在何階層目にいるかを画面上に表示するクラス
/// </summary>
public class HUDFloorNumberCounter : MonoBehaviour
{
    [field:SerializeField, Header("階層情報を表示するTextMeshProインスタンス")]
    private TextMeshProUGUI floorNumViewUGUI;

    private void Update()
    {
        var saveDataHolder = SaveDataHolder.Instance;
        if (saveDataHolder != null && floorNumViewUGUI != null)
        {
            floorNumViewUGUI.text = $"[{saveDataHolder.SaveData.currentFloor.ToString("00")}] F";
        }
    }
}
