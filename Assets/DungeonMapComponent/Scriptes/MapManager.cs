using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("マップグリッドのPrefab")]
    [SerializeField]
    private MapGrid mapGridPrefab;

    [Header("マップグリッドを配置するCanvas")]
    [SerializeField]
    private Canvas mapGridCanvas;

    [Header("マップの分岐数")]
    [SerializeField]
    private int branchCount = 3;

    [Header("マップのフロア数")]
    [SerializeField]
    private int floorCount = 5;

    [Header("グリッド同士の横スペース")]
    [SerializeField]
    private float xOffset = 2.0f; // グリッド同士の横スペース

    [Header("グリッド同士の縦スペース")]
    [SerializeField]
    private float yOffset = 4.0f; // グリッド同士の縦スペース

    [Header("生成できるマップグリッド情報リスト")]
    [SerializeField]
    private MapGridInfoList availableMapGridInfoList;
    
    // マップのマス配置情報
    private List<List<MapGridInfo>> mapGridInfos;
    
    // マップのマスオブジェクトリスト
    private List<List<MapGrid>> mapGrids;
    
    // スタートマスとボスマス（終端）のMapGridInfo(1列しかないので別枠で管理)
    [Header("スタートマス情報")]
    [SerializeField]
    private MapGridInfo startGridInfo;
    
    [Header("ボスマス(終端)情報")]
    [SerializeField]
    private MapGridInfo bossGridInfo;

    private void Start()
    {
        var dungeonFactory = new DungeonMapFactory();
        // 全てのマップグリッド情報をファクトリーに登録
        foreach (var item in availableMapGridInfoList.mapGridInfos)
        {
            dungeonFactory.AddMapGridInfo(item);
        }

        // マップのマス配置情報をランダムに生成
        mapGridInfos = dungeonFactory.GenerateDungeonMap(branchCount, floorCount);

        // ダンジョンマップの実体を生成
        mapGrids = dungeonFactory.SpawnDungeonMap(
            mapGridInfos,
            startGridInfo,
            bossGridInfo,
            mapGridPrefab,
            mapGridCanvas,
            xOffset,
            yOffset
        );

        // マップグリッド(マス)間をランダムに接続させる
        dungeonFactory.ConnectGridCells(ref mapGrids);
    }
}