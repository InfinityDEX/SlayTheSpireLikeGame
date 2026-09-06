using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[System.Serializable]
public class MapGrid : MonoBehaviour
{
    [Header("グリッド画像表示オブジェクト")]
    [SerializeField]
    private Image gridImage;

    [Header("グリッド情報")]
    [SerializeField]
    private MapGridInfo gridInfo;

    [Header("隣接する下層")]
    [SerializeField]
    private List<MapGrid> underLayer;

    [Header("隣接する上層")]
    [SerializeField]
    private List<MapGrid> upperLayer;

    [Header("グリッド間を繋ぐ線のPrefab")]
    [SerializeField]
    private UILineConnector line;
#if UNITY_EDITOR
    [Header("グリッド位置表示ディスプレイ(デバッグ用)")]
    [SerializeField]
    private TMPro.TextMeshProUGUI gridPosViewDisplay;
#endif
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
