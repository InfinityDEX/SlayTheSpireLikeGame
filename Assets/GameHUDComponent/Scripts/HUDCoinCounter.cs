using TMPro;
using UnityEngine;
using VContainer;
using SaveDataComponent;

namespace GameHUDComponent
{
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
            if (SaveDataHolderInstance.SaveData != null && coinCountViewUGUI != null)
            {
                coinCountViewUGUI.text = $"X {SaveDataHolderInstance.SaveData.money.ToString("00000")}";
            }
        }
    }
}
