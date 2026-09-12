using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[System.Serializable]
public class MapGrid : MonoBehaviour
{
    [Header("機能有効状態")]
    [SerializeField]
    public bool isEnabled = false; // デフォルトは無効

    [Header("グリッド画像表示オブジェクト")]
    [SerializeField]
    private Image gridImage;

    [Header("グリッド情報")]
    [SerializeField]
    public MapGridInfo gridInfo;

    // マップグリッド情報のID
    public int gridInfoId;

    [Header("隣接する下層")]
    [SerializeField]
    public List<MapGrid> underLayer;

    [Header("隣接する上層")]
    [SerializeField]
    public List<MapGrid> upperLayer;

    [Header("グリッド間を繋ぐ線のPrefab")]
    [SerializeField]
    private UILineConnector line;

    public MapGridJson.MapGridPos pos;

#if UNITY_EDITOR
    [Header("グリッド位置表示ディスプレイ(デバッグ用)")]
    [SerializeField]
    private TMPro.TextMeshProUGUI gridPosViewDisplay;
#endif

    /// <summary>
    /// グリッド画像を点滅させる
    /// </summary>
    /// <param name="enable">点滅有効状態</param>
    /// <param name="fastBlinkMode">高速点滅モードか</param>
    public void FlickerGrid(bool enable, bool fastBlinkMode = false)
    {
        // 点滅機構はgridImageのオブジェクトが持っている
        var flicker = gridImage.gameObject.GetComponent<MapGirdFlicker>();
        if (flicker == null)
        {
            Debug.LogWarning("MapGirdFlicker コンポーネントが見つからなかったため、新規追加します。");

            // Flickerコンポーネントが存在しない場合はアタッチする
            flicker = gridImage.gameObject.AddComponent<MapGirdFlicker>();
        }
        flicker.isEnabled = enable;
        flicker.fastBlinkMode = fastBlinkMode;
    }

    /// <summary>
    /// 隣接する上層のグリッドにLineRendererで線分を引く
    /// </summary>
    public void DrawLinesToUpperLayers()
    {
        if (upperLayer == null || line == null)
            return;

        foreach (var upper in upperLayer)
        {
            if (upper == null)
                continue;

            // このマスを親としてUILineConnectorインスタンスを生成
            UILineConnector ulc = Instantiate(line, this.transform);

            // 自身のgridImageとupperのgridImageのrectTransformを設定
            ulc.transforms = new RectTransform[] { this.gridImage.rectTransform, upper.gridImage.rectTransform };
        }
    }

    /// <summary>
    /// グリッド情報を登録
    /// </summary>
    /// <param name="info">グリッド情報</param>
    public void SetInfo(MapGridInfo info)
    { 
        gridInfoId = info.id;
        gridInfo = info;
        // グリッド情報の登録と同時にImageに画像データを登録する
        gridImage.sprite = gridInfo.gridSprite;
    }

    /// <summary>
    /// 隣接する下層フロアを追加
    /// </summary>
    /// <param name="grid">フロアグリッド</param>
    public void SetUnderLayer(MapGrid grid)
    {
        underLayer.Add(grid);
    }

    /// <summary>
    /// 隣接する上層フロアを追加
    /// </summary>
    /// <param name="grid">フロアグリッド</param>
    public void SetUpperLayer(MapGrid grid)
    {
        upperLayer.Add(grid);
    }
#if UNITY_EDITOR
    public void SetDebugView_GridPosition(int col, int row)
    {
        if(gridPosViewDisplay != null)
        {
            gridPosViewDisplay.text = $"Col:{col}\nRow:{row}";
        }
    }
#endif
}
