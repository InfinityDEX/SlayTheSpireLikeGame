using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ダンジョンマップマネージャクラス
/// </summary>
public class DungeonMapManager : MonoBehaviour
{
    [field:SerializeField, Header("DEBUG:常に新しくダンジョンマップを生成する")]
    private bool isAlwaysMakeNewDungeonMap = false;

    [field:SerializeField, Header("メインカメラ")]
    private Camera mainCamera;

    /// <summary>
    /// メインカメラのデフォルトY座標
    /// </summary>
    private float mainCameraDefault_YPos;
    
    [field:SerializeField, Header("ステージデータ")]
    private StageData stageData;

    [field:SerializeField, Header("敵パターンジェネレータ")]
    private EnemyCombinationsDataGenerator enemyCombinationsDataGenerator;

    [field:SerializeField, Header("マップグリッドのPrefab")]
    private MapGrid mapGridPrefab;

    [field:SerializeField, Header("マップグリッドを配置するCanvas")]
    private Canvas mapGridCanvas;

    [field:SerializeField, Header("経路線を配置するCanvas")]
    private Canvas routeLineCanvas;

    [field:SerializeField, Header("マップの分岐数")]
    private int branchCount = 3;

    [field:SerializeField, Header("マップのフロア数")]
    private int floorCount = 5;

    [field:SerializeField, Header("グリッド同士の横スペース")]
    private float xOffset = 2.0f;

    [field:SerializeField, Header("グリッド同士の縦スペース")]
    private float yOffset = 4.0f;

    [field:SerializeField, Header("生成できるマップグリッド情報リスト")]
    private MapGridInfoList availableMapGridInfoList;

    // マップのマス配置情報
    private List<List<MapGridInfo>> mapGridInfos;
    
    // マップのマスオブジェクトリスト
    private List<List<MapGrid>> mapGrids;
    
    // スタートマスとボスマス（終端）のMapGridInfo(1列しかないので別枠で管理)
    [field:SerializeField, Header("スタートマス情報")]
    private MapGridInfo startGridInfo;

    [field:SerializeField, Header("ボスマス(終端)情報")]
    private MapGridInfo bossGridInfo;

    /// <summary>
    /// セーブデータの保存パス
    /// </summary>
    public static string saveDataFilePath { private set; get;} = "SaveData/CurrentDungeonData.json" ;

    /// <summary>
    /// 移動経路
    /// </summary>
    private List<MapGridJsonEntity.MapGridPos> movePath;
    
    /// <summary>
    /// 現在の位置
    /// </summary>
    private MapGridJsonEntity.MapGridPos currentGridPos;

    [field:SerializeField, Header("セレクト音")]
    private AudioClip selectSE;

    [field:SerializeField, Header("決定音")]
    private AudioClip decideSE;

    /// <summary>
    /// 次に進むマップグリッドが、現在いるマップグリッドのupperLayerの何番目かを表すインデックス
    /// </summary>
    private int nextGridUpperLayerIndex;

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
                routeLineCanvas,
                xOffset,
                yOffset,
                false,
                loadedDungeonMapSaveData
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
                routeLineCanvas,
                xOffset,
                yOffset,
                true
            );

            // マップグリッド(マス)間をランダムに接続させる
            dungeonFactory.ConnectRandomGridCells(ref mapGrids, routeLineCanvas);
            
            // 現在位置をスタートマス（row : 0, col : 0）に設定する
            currentGridPos.row = 0;
            currentGridPos.col = 0;
            movePath.Add(currentGridPos);

            // データセーブ
            SaveDungeonMapData();
        }
    
        var currentGrid = mapGrids[currentGridPos.row][currentGridPos.col];
        // 最初に選択状態になるグリッドは、現在いるグリッドから進行可能なグリッド（UpperLayer）の中で
        // 一番最初（左端）に登録されているグリッドを登録する
        if (currentGrid.upperLayer != null &&
            currentGrid.upperLayer.Count > 0)
        {
            nextGridUpperLayerIndex = 0;
        }

        var nextGrid = currentGrid.upperLayer[nextGridUpperLayerIndex];

        // 進行予定のマスを点滅させる
        nextGrid.FlickerGrid(true);

        // 進行経路データを渡して着色する
        var startGrid = mapGrids[0][0]; // 最初のマスから辿っていく
        // 進行予定の経路も含めいて線に色を付ける
        var nextMovePath = new List<MapGridJsonEntity.MapGridPos>(movePath);
        nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridUpperLayerIndex];
        nextMovePath.Add(nextGrid.pos);
        startGrid.ColoringRouteLine(nextMovePath);

        // カメラのY座標を現在のマスと次のフロアの中間になるように設定する
        mainCameraDefault_YPos = (currentGrid.transform.position.y + nextGrid.transform.position.y) / 2;
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = mainCameraDefault_YPos;
        mainCamera.transform.position = cameraPos;
    }

    private void Update()
    {
        var nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridUpperLayerIndex];

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // カーソルを左に移動
            MoveSelectCursor(nextGrid, -1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // カーソルを右に移動
            MoveSelectCursor(nextGrid, 1);
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
                nextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridUpperLayerIndex];
                nextGrid.FlickerGrid(true, true);

                // 選択したグリッドに合わせて画面遷移する
                AudioController.Instance.PlaySE(decideSE);
                enemyCombinationsDataGenerator.GenerateData(enemyType, stageData);
                StartCoroutine(LoadBattleSceneAfterDelay(2f));
            }
        }
    }

    /// <summary>
    /// 選択するマップグリッドを変更する
    /// </summary>
    /// <param name="currentNextGrid">現在選択中のマップグリッド</param>
    /// <param name="dir">移動方向（右：１　左：-1）</param>
    private void MoveSelectCursor(MapGrid currentNextGrid, int dir)
    {
        if (dir != 1 && dir != -1) throw new System.ArgumentException("dirは1または-1でなければなりません。", nameof(dir));
        
        // 経路線のマテリアルを一旦リセットする
        mapGrids[0][0].ResetAllLinkLineMaterial();

        // 今選択しているグリッドの点滅を止める
        currentNextGrid.FlickerGrid(false);

        // dir方向にカーソル移動した際に範囲外になるなら、ループしてdir方向の反対側の端を選択する
        var upperLayerCount = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer.Count;
        nextGridUpperLayerIndex = (nextGridUpperLayerIndex + dir + upperLayerCount) % upperLayerCount;
        
        AudioController.Instance.PlaySE(selectSE);

        currentNextGrid = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridUpperLayerIndex];
        
        currentNextGrid.FlickerGrid(true);

        // 進行予定の経路も含めいて線に色を付ける
        var nextMovePath = new List<MapGridJsonEntity.MapGridPos>(movePath);
        nextMovePath.Add(currentNextGrid.pos);
        mapGrids[0][0].ColoringRouteLine(nextMovePath);
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
        currentGridPos = mapGrids[currentGridPos.row][currentGridPos.col].upperLayer[nextGridUpperLayerIndex].pos;
        
        movePath.Add(currentGridPos); // 現在位置を移動経路に追加

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
        if (!System.IO.File.Exists(saveFilePath)) return null;
        var jsonText = System.IO.File.ReadAllText(saveFilePath, Encoding.UTF8);
   
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
                MapGridJsonEntity mapGridJson = new()
                {
                    mapGridInfoID = mapGrids[row][column].gridInfoId,
                    pos = mapGrids[row][column].pos
                };
                foreach (var item in mapGrids[row][column].upperLayer)
                {
                    mapGridJson.upperMapGridPos.Add(item.pos);
                }
                foreach (var item in mapGrids[row][column].lowerLayer)
                {
                    mapGridJson.lowerMapGridPos.Add(item.pos);
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