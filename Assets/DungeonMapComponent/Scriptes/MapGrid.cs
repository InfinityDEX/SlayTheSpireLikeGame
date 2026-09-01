using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

}
