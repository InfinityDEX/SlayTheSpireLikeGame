using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    [Header("DEBUG:常に新しくダンジョンマップを生成する")]
    [SerializeField]
    private bool isAlwaysMakeNewDungeonMap = false;
    
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
    [SerializeField]
    private List<List<MapGrid>> mapGrids;
    
    // スタートマスとボスマス（終端）のMapGridInfo(1列しかないので別枠で管理)
    [Header("スタートマス情報")]
    [SerializeField]
    private MapGridInfo startGridInfo;
    
    [Header("ボスマス(終端)情報")]
    [SerializeField]
    private MapGridInfo bossGridInfo;

    private string saveDataFilePath = "SaveData/CurrentDungeonData.json";

    private void Start()
    {
        var dungeonFactory = new DungeonMapFactory();
        // 全てのマップグリッド情報をファクトリーに登録
        foreach (var item in availableMapGridInfoList.mapGridInfos)
        {
            dungeonFactory.AddMapGridInfo(item);
        }

        // セーブデータ読み込み
        var dungeonMapSaveData = SearchDungeonMapSaveData();

        if (!isAlwaysMakeNewDungeonMap && dungeonMapSaveData != null)
        {
            // セーブデータからマップを読み込み
            mapGridInfos = dungeonFactory.GenerateDungeonMap(dungeonMapSaveData);

            // ダンジョンマップの実体を生成
            mapGrids = dungeonFactory.SpawnDungeonMap(
                mapGridInfos,
                startGridInfo,
                bossGridInfo,
                mapGridPrefab,
                mapGridCanvas,
                xOffset,
                yOffset,
                false
            );
        }
        else
        {
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
                yOffset,
                true
            );

            
            // マップグリッド(マス)間をランダムに接続させる
            dungeonFactory.ConnectGridCells(ref mapGrids);

            // マップグリッド(マス)の位置情報を更新する
            dungeonFactory.OrganizeMapGridPos(ref mapGrids);

            SaveDungeonMapData();
        }

    }

    /// <summary>
    /// セーブデータからダンジョンマップの情報（DungeonMapJson）を読み込む
    /// </summary>
    /// <returns>セーブファイルから取得したDungeonMapJsonオブジェクト</returns>
    public DungeonMapJson SearchDungeonMapSaveData()
    {
        var saveFilePath = System.IO.Path.Combine(Application.dataPath, saveDataFilePath);
        var jsonText = System.IO.File.ReadAllText(saveFilePath, Encoding.UTF8);
        
        // もしも見つからなかったらnullを返す
        if (jsonText == null) return null;

        DungeonMapJson dungeonMapJson = new ();
        dungeonMapJson = JsonUtility.FromJson<DungeonMapJson>(jsonText);
        return dungeonMapJson;
    }

    public void SaveDungeonMapData()
    {
        DungeonMapJson dungeonMapJson = new ();

        for (int row = 0; row < mapGrids.Count; row++)
        {
            DungeonFloorJson floor = new();
            for (int column = 0; column < mapGrids[row].Count; column++)
            {
                MapGridJson mapGridJson = new();
                mapGridJson.mapGridInfoID = mapGrids[row][column].gridInfoId;
                mapGridJson.pos = mapGrids[row][column].pos;
                foreach (var item in mapGrids[row][column].upperLayer)
                {
                    mapGridJson.upperMapGridPos.Add(item.pos);
                }
                foreach (var item in mapGrids[row][column].underLayer)
                {
                    mapGridJson.underMapGridPos.Add(item.pos);
                }

                floor.floorGrids.Add(mapGridJson);
            }
            dungeonMapJson.dungeonMapGrid.Add(floor);
        }
        var jsonText = JsonUtility.ToJson(dungeonMapJson, true);
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, saveDataFilePath);
        System.IO.File.WriteAllText(saveFilePath, jsonText, Encoding.UTF8);
    }
}