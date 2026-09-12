using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("DEBUG:常に新しくダンジョンマップを生成する")]
    [SerializeField]
    private bool isAlwaysMakeNewDungeonMap = false;
    
    [Header("ステージデータ")]
    [SerializeField]
    private StageData stageData;

    [Header("敵パターンジェネレータ")]
    [SerializeField]
    private EnemyCombinationsDataGenerator enemyCombinationsDataGenerator;

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

    // セーブデータの保存パス
    private string saveDataFilePath = "SaveData/CurrentDungeonData.json";

    // 移動経路
    private List<MapGridJson.MapGridPos> movePath;
    
    // 現在の位置
    private MapGridJson.MapGridPos currentGridPos;

    [Header("セレクト音")]
    [SerializeField]
    private AudioClip selectSE;

    [Header("決定音")]
    [SerializeField]
    private AudioClip decideSE;

    // 移動予定の列：currentGridPosのUpperLayerのインデックス
    private int nextGridCol;

    private void Start()
    {
        movePath = new();

        var dungeonFactory = new DungeonMapFactory();
        // 全てのマップグリッド情報をファクトリーに登録
        foreach (var item in availableMapGridInfoList.mapGridInfos)
        {
            dungeonFactory.AddMapGridInfo(item);
        }

        // セーブデータ読み込み
        var loadedDungeonMapSaveData = SearchDungeonMapSaveData();

        // セーブデータの有無でマップ生成処理を分ける
        if (!isAlwaysMakeNewDungeonMap && loadedDungeonMapSaveData != null)
        {
            // セーブデータからマップを読み込み
            mapGridInfos = dungeonFactory.GenerateDungeonMap(loadedDungeonMapSaveData);
            movePath = loadedDungeonMapSaveData.movePath;
            currentGridPos = loadedDungeonMapSaveData.currentGridPos;

            // ダンジョンマップの実体を生成
            mapGrids = dungeonFactory.SpawnDungeonMap(
                mapGridInfos,
                startGridInfo,
                bossGridInfo,
                mapGridPrefab,
                mapGridCanvas,
                xOffset,
                yOffset,
                false,
                loadedDungeonMapSaveData
            );
            
            // マップグリッド(マス)の位置情報を更新する
            dungeonFactory.OrganizeMapGridPos(ref mapGrids);
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
            dungeonFactory.ConnectRandomGridCells(ref mapGrids);

            // マップグリッド(マス)の位置情報を更新する
            dungeonFactory.OrganizeMapGridPos(ref mapGrids);
            
            // 現在位置をスタートマス（row : 0, col : 0）に設定する
            currentGridPos.row = 0;
            currentGridPos.col = 0;
            movePath.Add(currentGridPos);

            // データセーブ
            SaveDungeonMapData();
        }
    
        // 最初に選択状態になるグリッドは、現在いるグリッドから進行可能なグリッド（UpperLayer）の中で
        // 一番最初（左端）に登録されているグリッドを登録する
        if (mapGrids[currentGridPos.row][currentGridPos.col].upperLayer != null &&
            mapGrids[currentGridPos.row][currentGridPos.col].upperLayer.Count > 0)
        {
            nextGridCol = 0;
        }

        var nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridCol];
        nextGrid.FlickerGrid(true);
    }

    private void Update()
    {
        var nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridCol];

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 今選択しているグリッドの点滅を止める
            nextGrid.FlickerGrid(false);
       
            var upperLayerCount = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer.Count;
            nextGridCol = (nextGridCol - 1 + upperLayerCount) % upperLayerCount;
         
            AudioController.Instance.PlaySE(selectSE);

            nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridCol];
            nextGrid.FlickerGrid(true);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 今選択しているグリッドの点滅を止める
            nextGrid.FlickerGrid(false);
       
            var upperLayerCount = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer.Count;
            nextGridCol = (nextGridCol + 1) % upperLayerCount;
         
            AudioController.Instance.PlaySE(selectSE);

            nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridCol];
            nextGrid.FlickerGrid(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
           EnemyCombinationsDataGenerator.EnemyType enemyType = EnemyCombinationsDataGenerator.EnemyType.Enemy; // 未割当でエラーになってしまう為、一旦Enemyを登録
            
            bool shouldNotTransition = false;
            switch (nextGrid.gridInfo.type)
            {
                case MapGridInfo.FloorInfo.StartFloor:
                    shouldNotTransition = true;
                    break; // 何もしない。
                case MapGridInfo.FloorInfo.EnemyFloor:
                    enemyType = EnemyCombinationsDataGenerator.EnemyType.Enemy;
                    break;
                case MapGridInfo.FloorInfo.EliteEnemyFloor:
                    enemyType = EnemyCombinationsDataGenerator.EnemyType.EliteEnemy;
                    break;
                case MapGridInfo.FloorInfo.BossFloor:
                    enemyType = EnemyCombinationsDataGenerator.EnemyType.Boss;
                    break;
                case MapGridInfo.FloorInfo.ShopFloor:
                    shouldNotTransition = true;
                    break; // TODO:現状は何もしない。ショップ実装後変更する。
            }

            if (!shouldNotTransition)
            {
                // 高速点滅
                nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridCol];
                nextGrid.FlickerGrid(true, true);

                // 選択したグリッドに合わせて画面遷移する
                AudioController.Instance.PlaySE(decideSE);
                enemyCombinationsDataGenerator.GenerateData(enemyType, stageData);
                StartCoroutine(LoadBattleSceneAfterDelay(2f));
            }
        }
    }

    /// <summary>
    /// 指定した遅延時間後にバトルシーンへ遷移するコルーチン
    /// </summary>
    /// <param name="delaySeconds">遅延時間（秒）</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator LoadBattleSceneAfterDelay(float delaySeconds)
    {
        // 指定した秒数だけ待機
        yield return new WaitForSeconds(delaySeconds);

        // BGMをランダムで再生
        var bgmCandidates = stageData.battleBgmCandidates;
        if (bgmCandidates != null && bgmCandidates.Length > 0)
        {
            int randomIndex = Random.Range(0, bgmCandidates.Length);
            AudioController.Instance.PlayBGM(bgmCandidates[randomIndex]);
        }

        // 移動経路（movePath）と現在のフロア（currentGridPos）を更新
        movePath.Add(currentGridPos); // 現在位置を移動経路に追加
        currentGridPos = new MapGridJson.MapGridPos{ 
            row = currentGridPos.row + 1,
            col = nextGridCol ,
        };

        // マップ保存
        SaveDungeonMapData();

        // バトルシーンへ遷移
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
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

        DungeonMapJson dungeonMapJson;
        dungeonMapJson = JsonUtility.FromJson<DungeonMapJson>(jsonText);
        return dungeonMapJson;
    }

    /// <summary>
    /// ダンジョンマップ情報の永続化（セーブ）
    /// </summary>
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
        dungeonMapJson.movePath = movePath;
        dungeonMapJson.currentGridPos = currentGridPos;

        var jsonText = JsonUtility.ToJson(dungeonMapJson, true);
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, saveDataFilePath);
        System.IO.File.WriteAllText(saveFilePath, jsonText, Encoding.UTF8);
    }
}