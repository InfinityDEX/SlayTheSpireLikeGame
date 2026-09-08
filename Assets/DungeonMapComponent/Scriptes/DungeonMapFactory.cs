using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonMapFactory
{    
    // マップグリッド情報のリスト
    private List<MapGridWithID> mapGridInfoList = new List<MapGridWithID>();

    /// <summary>
    /// マップグリッド情報をリストに追加
    /// </summary>
    /// <param name="data">追加するMapGridInf</param>
    public void AddMapGridInfo(MapGridWithID data)
    {
        // グリッド情報がちゃんと登録されていて、重複するデータがない（重複するIDがあってもダメ）場合は登録する。
        if (data.info != null &&
            !mapGridInfoList.Contains(data) &&
            mapGridInfoList.All(item => item.id != data.id)
            )
        {
            mapGridInfoList.Add(data);
        }
        else
        {
            Debug.LogError($"[DungeonMapFactory] AddMapGridInfo: 無効なデータが追加されようとしました。ID: {data.id}, Info: {(data.info != null ? data.info.name : "null")}");
        }
    }

    /// <summary>
    /// マップグリッド情報リストからランダムに1つ抽出
    /// </summary>
    /// <returns>ランダムに選択されたMapGridInfo、リストが空の場合はnull</returns>
    public MapGridInfo GetRandomMapGridInfo()
    {
        if (mapGridInfoList == null || mapGridInfoList.Count == 0)
            return null;
        int idx = UnityEngine.Random.Range(0, mapGridInfoList.Count);
        var tmp = mapGridInfoList[idx].info;
        tmp.id = idx;
        return tmp;
    }

    /// <summary>
    /// マップグリッド情報リストからIDを指定して抽出
    /// </summary>
    /// /// <param name="id">マップグリッドに紐づけられたID</param>
    /// <returns>ランダムに選択されたMapGridInfo、リストが空の場合はnull</returns>
    public MapGridInfo GetRandomMapGridInfo(int id)
    {
        return mapGridInfoList[id].info;
    }

    /// <summary>
    /// 指定された列数に基づいてダンジョンマップのグリッド情報リストを生成
    /// </summary>
    /// <param name="branchCount">生成する分岐数</param>
    /// <returns>各列ごとのグリッド情報リスト</returns>
    public List<List<MapGridInfo>> GenerateDungeonMap(int branchCount, int floorCount)
    {
        var mapGrids = new List<List<MapGridInfo>>();
        for (int i = 0; i < floorCount; i++)
        {
            var floor = new List<MapGridInfo>();
            for (int j = 0; j < branchCount; j++)
            {
                floor.Add(GetRandomMapGridInfo());
            }
            mapGrids.Add(floor);
        }

        return mapGrids;
    }    
    
    // TODO:GenerateDungeonMapをオーバーライドして永続化したダンジョンマップ情報(Json)から
    // マップグリッド情報リストを生成できるようにする
    // 
    // マップグリッド情報リストさえ生成すればSpawnDungeonMapはそれを読み込むだけで
    // マップを実際に画面上に生成できるため、SpawnDungeonMapの方は修整やオーバーライド不要
   /// <summary>
    /// 指定された列数に基づいてダンジョンマップのグリッド情報リストを生成
    /// </summary>
    /// <param name="branchCount">生成する分岐数</param>
    /// <returns>各列ごとのグリッド情報リスト</returns>
    public List<List<MapGridInfo>> GenerateDungeonMap(DungeonMapJson jsonData)
    {
        var mapGrids = new List<List<MapGridInfo>>();
        var dungeonMapGrid = jsonData.dungeonMapGrid;

        // ダンジョンマップインスタンス化
        for (int i = 0; i < dungeonMapGrid.Count; i++)
        {
            var floor = new List<MapGridInfo>();
            for (int j = 0; j < dungeonMapGrid[i].floorGrids.Count; j++)
            {
                // Idでマップのグリッドを検索する。
                var infoWithId = mapGridInfoList.FirstOrDefault(infoWithId => infoWithId.id == dungeonMapGrid[i][j].mapGridInfoID);
                var gridInfo = infoWithId.info; // 値渡しする

                // もしもId検索で引っかからなかったらエラー（infoWithIdの全てのメンバが0かnullのデフォルト値）
                if (infoWithId.id == 0 && gridInfo == null)
                {
                    throw new System.Exception($"指定されたID ({dungeonMapGrid[i][j].mapGridInfoID}) のマップグリッド情報が見つかりません。");
                }

                // // マップグリッド接続先登録
                // foreach (var upperPos in dungeonMapGrid[i][j].upperMapGridPos)
                // {
                //     gridInfo.upperMapGridList.Add(upperPos);
                // }
                // foreach (var underPos in dungeonMapGrid[i][j].underMapGridPos)
                // {
                //     gridInfo.underMapGridList.Add(underPos);
                // }
           
                floor.Add(gridInfo);
            }
            mapGrids.Add(floor);
        }

        return mapGrids;
    }    

    /// <summary>
    /// ダンジョンマップ(マップグリッド)を画面上に配置・生成する処理
    /// <param name="mapGridInfos">スタートマスとボスマス以外のダンジョンマップ情報</param>
    /// <param name="startGridInfo">スタートマスのダンジョンマップ情報</param>
    /// <param name="bossGridInfo">ボスマスのダンジョンマップ情報</param>
    /// <param name="mapGridPrefab">マップグリッドのPrefab</param>
    /// <param name="mapGridCanvas">マップグリッドを配置するCanvas</param>
    /// <param name="xOffset">横方向のマップグリッド間の隙間</param>
    /// <param name="yOffset">縦方向のマップグリッド間の隙間</param>
    /// <param name="isNewDungeon">セーブデータから読み込まれたものではない、今回初めて生成されるマップか？</param>
    /// <returns>マップグリッド配列（ダンジョンマップ）</returns>
    public List<List<MapGrid>> SpawnDungeonMap(
        List<List<MapGridInfo>> mapGridInfos,
        MapGridInfo startGridInfo,
        MapGridInfo bossGridInfo,
        MapGrid mapGridPrefab,
        Canvas mapGridCanvas,
        float xOffset,
        float yOffset,
        bool isNewDungeon,
        DungeonMapJson jsonData = null
        )
    {
        List<List<MapGrid>> mapGrids = null;
        if (mapGridInfos == null || mapGridPrefab == null)
        {
            Debug.LogWarning("mapGrids or mapGridPrefab is null.");
            return null;
        }
   

        for (int row = 0; row < mapGridInfos.Count; row++)
        {
            // 新規で生成されたダンジョンではないなら、スタートマスとボスマスの情報がmapGridInfosに含まれてしまっている為、インスタンス生成をスキップ
            if (!isNewDungeon && (row == 0 || row == mapGridInfos.Count - 1))
            {
                // TODO:スタートマスとボスマスはそれぞれ別のmapGridInfoListからIdを指定してインスタンス生成できるようにしたい（現状はスタートマスもボスマスも1つずつしか用意する予定はない為、
                // ここの処理はスキップして、本ループの下に記載された処理にて決められたスタートマスとボスマスを登録している）
                continue;
            }
            var rowList = mapGridInfos[row];
            for (int col = 0; col < rowList.Count; col++)
            {
                // 初期配置を中央揃えに（Y座標はCanvas中央を0として上方向に配置）
                float totalWidth = (rowList.Count - 1) * xOffset;
                float x = col * xOffset - totalWidth / 2f;
                float y = (row - (isNewDungeon ? 0 : 1)) * yOffset;
                Vector3 position = new (x, y, 0);
                // MapGridのインスタンスを生成
                MapGrid gridInstance = Object.Instantiate(mapGridPrefab, mapGridCanvas.transform);
                // 座標を設定
                gridInstance.transform.localPosition = position;
                // グリッド情報をセット
                gridInstance.SetInfo(rowList[col]);

                // mapGrids にインスタンスを登録
                if (mapGrids == null)
                {
                    mapGrids = new List<List<MapGrid>>(mapGridInfos.Count);
                    for (int i = 0; i < mapGridInfos.Count; i++)
                    {
                        mapGrids.Add(new List<MapGrid>(mapGridInfos[i].Count));
                    }
                }
                mapGrids[row].Add(gridInstance);
#if UNITY_EDITOR
                gridInstance.SetDebugView_GridPosition(col, row);
#endif
            }
        }

        // もしもセーブデータからダンジョンマップを生成した場合、新規生成の場合後で登録するスタートマスとボスマスの情報を
        // 含めてMapGridInfoリストインスタンスを生成する為、一旦削除する。
        // 
        // TODO：もしも今後スタートマスとボスマスの種類を増やす場合は、MapGridInfoリストのスタートマスとボスマスの情報を
        // 無視する処理にしてしまっている為、このあたりの処理を書き直す必要がある。
        if (!isNewDungeon)
        {
            // mapGridsの先頭と最後の要素を削除する
            if (mapGrids != null && mapGrids.Count > 1)
            {
                mapGrids.RemoveAt(mapGrids.Count - 1); // 最後の要素を削除
                mapGrids.RemoveAt(0);                 // 先頭の要素を削除
            }
        }

        // スタートマスとボスマスを生成

        // 1. 必要な開始・ボス用座標の計算
        float startBossX = 0f;

        // 2. スタートマス生成（最初の列の下に配置）
        float startY = -yOffset;
        Vector3 startPosition = new Vector3(startBossX, startY, 0f);
        MapGrid startGridInstance = Object.Instantiate(mapGridPrefab, mapGridCanvas.transform);
        startGridInstance.transform.localPosition = startPosition;

        // スタートマス用のMapGridInfoを設定
        startGridInstance.SetInfo(startGridInfo);

        // 3. ボスマス生成（最後の列の上に配置）
        int rowCount = mapGridInfos.Count;
        float bossY = (rowCount - (isNewDungeon ? 0 : 2)) * yOffset;
        Vector3 bossPosition = new Vector3(startBossX, bossY, 0f);
        MapGrid bossGridInstance = Object.Instantiate(mapGridPrefab, mapGridCanvas.transform);
        bossGridInstance.transform.localPosition = bossPosition;

        // ボスマス用のMapGridInfoを設定
        bossGridInstance.SetInfo(bossGridInfo);

        // mapGridsリストへ登録
        if (mapGrids != null)
        {
            // スタートマス（最初のフロアの下＝row: -1的な扱いなので先頭に追加）
            mapGrids.Insert(0, new List<MapGrid>{startGridInstance});

            // ボスマス（最後のフロアの上＝最後に追加）
            mapGrids.Add(new List<MapGrid>{bossGridInstance});
        }

        // もしもセーブデータから生成したダンジョンマップだったら、マップ間の道を生成する。
        if (!isNewDungeon && jsonData != null)
        {
            for (int row = 0; row < mapGridInfos.Count - 1; row++)
            {
                var currentRow = mapGridInfos[row];
                for (int col = 0; col < currentRow.Count; col++)
                {
                    // マス間の接続を行う（セーブデータから復元した場合、upperLayer・underLayerの情報を使って再接続する）
                    var currentGridInfo = currentRow[col];
                    // 上階層への接続
                    foreach (var upperPos in jsonData.dungeonMapGrid[row][col].upperMapGridPos)
                    {
                        mapGrids[row][col].SetUpperLayer(mapGrids[upperPos.row][upperPos.col]);
                    }
                    // 下階層への接続
                    foreach (var underPos in jsonData.dungeonMapGrid[row][col].underMapGridPos)
                    {
                        mapGrids[row][col].SetUnderLayer(mapGrids[underPos.row][underPos.col]);
                    }
                }
            }
        }

        // すべてのマップグリッドで線分を描画 
        foreach (var row in mapGrids)
        {
            foreach (var grid in row)
            {
                if (grid != null)
                {
                    grid.DrawLinesToUpperLayers();
                }
            }
        }

        return mapGrids;
    }

    /// <summary>
    /// マス間を縦方向のみ（同じ縦＋斜め1マス上）でランダムに接続する
    /// </summary>
    /// <param name="mapGrids">マップグリッド配列（SpawnDungeonMapで生成した物を想定）</param>
    public void ConnectRandomGridCells(ref List<List<MapGrid>> mapGrids)
    {
        if (mapGrids.Count <= 1)
        {
            return;
        }

        int rowCount = mapGrids.Count;

        // スタートマスと２階層目のマスを全て接続
        MapGrid startGrid = mapGrids[0][0];
        for (int col = 0; col < mapGrids[1].Count; col++)
        {
            MapGrid firstGrid = mapGrids[1][col];
            startGrid.SetUpperLayer(firstGrid);
            firstGrid.SetUnderLayer(startGrid);
        }

        
        // ボスマスとボス一歩前のマスを全て接続
        MapGrid bossGrid = mapGrids[^1][0];
        for (int col = 0; col < mapGrids[^2].Count; col++)
        {
            MapGrid lastGrid = mapGrids[^2][col];
            lastGrid.SetUpperLayer(bossGrid);
            bossGrid.SetUnderLayer(lastGrid);
        }

        // スタートマスとボスマス以外のマスの接続
        for (int row = 1; row < mapGrids.Count - 2; row++)
        {
            List<MapGrid> currentRow = mapGrids[row];
            for (int col = 0; col < currentRow.Count; col++)
            {
                MapGrid fromGrid = currentRow[col];

                // 縦＋斜め右上・左上(col-1, col, col+1)にいる次行のマスとランダムで接続
                List<int> candidateCols = new List<int>();
                for (int offset = -1; offset <= 1; offset++)
                {
                    int toCol = col + offset;
                    if (toCol >= 0 && toCol < mapGrids[row + 1].Count)
                    {
                        candidateCols.Add(toCol);
                    }
                }
        

                List<int> connectedCols = new List<int>();
                // ランダム接続を行う
                foreach (int toCol in candidateCols)
                {
                    // 真上のグリッドとは確定で接続し、左右への分岐は50%の確率で接続する。
                    if (toCol == col || Random.value < 0.5f)
                    {
                        MapGrid toGrid = mapGrids[row + 1][toCol];
                        fromGrid.SetUpperLayer(toGrid);
                        toGrid.SetUnderLayer(fromGrid);
                        connectedCols.Add(toCol);
                    }
                }
            }
        }

        // すべてのマップグリッドで線分を描画 
        foreach (var row in mapGrids)
        {
            foreach (var grid in row)
            {
                if (grid != null)
                {
                    grid.DrawLinesToUpperLayers();
                }
            }
        }
    }

    /// <summary>
    /// マップグリッドの位置情報整理
    /// </summary>
    /// <param name="mapGrids">マップグリッド配列（SpawnDungeonMapで生成した物を想定）</param>
    public void OrganizeMapGridPos(ref List<List<MapGrid>> mapGrids)
    {
        for (int i = 0; i < mapGrids.Count; i++)
        {
            for (int j = 0; j < mapGrids[i].Count; j++)
            {
                if (mapGrids[i][j] != null)
                {
                    mapGrids[i][j].pos.row = i;
                    mapGrids[i][j].pos.col = j;
                }
            }
        }
    }
}
