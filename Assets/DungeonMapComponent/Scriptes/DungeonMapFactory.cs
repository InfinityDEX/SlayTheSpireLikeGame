using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonMapFactory
{    
    // マップグリッド情報のリスト
    private List<MapGridInfo> mapGridInfoList = new List<MapGridInfo>();

    /// <summary>
    /// マップグリッド情報をリストに追加
    /// </summary>
    /// <param name="info">追加するMapGridInfo</param>
    public void AddMapGridInfo(MapGridInfo info)
    {
        if (info != null && !mapGridInfoList.Contains(info))
        {
            mapGridInfoList.Add(info);
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
        return mapGridInfoList[idx];
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
    
    /// <summary>
    /// ダンジョンマップ(マップグリッド)を画面上に配置・生成する処理
    /// <param name="mapGridInfos">スタートマスとボスマス以外のダンジョンマップ情報</param>
    /// <param name="startGridInfo">スタートマスのダンジョンマップ情報</param>
    /// <param name="bossGridInfo">ボスマスのダンジョンマップ情報</param>
    /// <param name="mapGridPrefab">マップグリッドのPrefab</param>
    /// <param name="mapGridCanvas">マップグリッドを配置するCanvas</param>
    /// <param name="xOffset">横方向のマップグリッド間の隙間</param>
    /// <param name="yOffset">縦方向のマップグリッド間の隙間</param>
    /// <returns>マップグリッド配列（ダンジョンマップ）</returns>
    public List<List<MapGrid>> SpawnDungeonMap(
        List<List<MapGridInfo>> mapGridInfos,
        MapGridInfo startGridInfo,
        MapGridInfo bossGridInfo,
        MapGrid mapGridPrefab,
        Canvas mapGridCanvas,
        float xOffset,
        float yOffset
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
            var rowList = mapGridInfos[row];
            for (int col = 0; col < rowList.Count; col++)
            {
                // 初期配置を中央揃えに（Y座標はCanvas中央を0として上方向に配置）
                float totalWidth = (rowList.Count - 1) * xOffset;
                float x = col * xOffset - totalWidth / 2f;
                float y = row * yOffset;
                Vector3 position = new Vector3(x, y, 0);
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

        // スタートマスとボスマスを生成

        // 1. 必要な開始・ボス用座標の計算
        float startBossX = 0f;
        // int centerRow = mapGridInfos.Count / 2;
        // float totalWidthForSB = (mapGridInfos.Count - 1) * xOffset;
        // startBossX = centerRow * xOffset - totalWidthForSB / 2f;

        // 2. スタートマス生成（最初の列の下に配置）
        float startY = -yOffset;
        Vector3 startPosition = new Vector3(startBossX, startY, 0f);
        MapGrid startGridInstance = Object.Instantiate(mapGridPrefab, mapGridCanvas.transform);
        startGridInstance.transform.localPosition = startPosition;

        // スタートマス用のMapGridInfoを設定
        startGridInstance.SetInfo(startGridInfo);

        // 3. ボスマス生成（最後の列の上に配置）
        int rowCount = mapGridInfos.Count;
        float bossY = rowCount * yOffset;
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
        return mapGrids;
    }

    /// <summary>
    /// マス間を縦方向のみ（同じ縦＋斜め1マス上）で接続する
    /// </summary>
    /// <param name="mapGrids">マップグリッド配列（SpawnDungeonMapで生成した物を想定）</param>
    public void ConnectGridCells(ref List<List<MapGrid>> mapGrids)
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
                
                // // どこにも繋がらなかった場合は、ランダムで1つだけ必ず接続する
                // if (connectedCols.Count == 0)
                // {
                //     MapGrid toGrid = mapGrids[row + 1][Random.Range(0, candidateCols.Count)];
                //     fromGrid.SetUpperLayer(toGrid);
                //     toGrid.SetUnderLayer(fromGrid);
                // }
            }
        }


        // int centerCol = mapGrids.Count / 2;

        // int colCount = mapGrids.Count;
        // for (int col = 0; col < colCount - 1; col++)
        // {
        //     List<MapGrid> currentCol = mapGrids[col];
        //     List<MapGrid> nextCol = mapGrids[col + 1];

        //     for (int row = 0; row < currentCol.Count; row++)
        //     {
        //         MapGrid fromGrid = currentCol[row];

        //         // 縦＋斜め右上・左上(col-1, col, col+1)にいる次行のマスとランダムで接続
        //         List<int> candidateCols = new List<int>();
        //         for (int offset = -1; offset <= 1; offset++)
        //         {
        //             int toCol = col + offset;
        //             if (toCol >= 0 && toCol < mapGrids.Count)
        //             {
        //                 candidateCols.Add(toCol);
        //             }
        //         }
        

        //         // List<int> connectedCols = new List<int>();
        //         // // ランダム接続を行う
        //         // foreach (int toCol in candidateCols)
        //         // {
        //         //     // if (UnityEngine.Random.value < 0.5f && row < mapGrids[toCol + 1].Count)
        //         //     // {
        //         //         MapGrid toGrid = mapGrids[toCol + 1][row];
        //         //         fromGrid.SetUpperLayer(toGrid);
        //         //         toGrid.SetUnderLayer(fromGrid);
        //         //         connectedCols.Add(toCol);
        //         //     // }
        //         // }
           

        //         // // どこにも繋がらなかった場合は、ランダムで1つだけ必ず接続する
        //         // if (connectedRows.Count == 0 && candidateRows.Count > 0)
        //         // {
        //         //     int guaranteedRow = candidateRows[UnityEngine.Random.Range(0, candidateRows.Count)];
        //         //     MapGrid toGrid = nextCol[guaranteedRow];
        //         //     fromGrid.SetUpperLayer(toGrid);
        //         //     toGrid.SetUnderLayer(fromGrid);
        //         // }
        //     }
        // }

        // // スタートマス（先頭=最下段）と隣のグリッド、ボスマス（末尾=最上段）をそれぞれ接続
        // int startCol = centerCol;
        // if (mapGrids[startCol].Count > 1)
        // {
        //     MapGrid startGrid = mapGrids[startCol][0];
        //     MapGrid firstGrid = mapGrids[startCol][1];
        //     startGrid.SetUpperLayer(firstGrid);
        //     firstGrid.SetUnderLayer(startGrid);
        // }
        // int bossCol = centerCol;
        // if (mapGrids[bossCol].Count > 2)
        // {
        //     MapGrid bossGrid = mapGrids[bossCol][mapGrids[bossCol].Count - 1];
        //     MapGrid lastGrid = mapGrids[bossCol][mapGrids[bossCol].Count - 2];
        //     bossGrid.SetUnderLayer(lastGrid);
        //     lastGrid.SetUpperLayer(bossGrid);
        // }

        // すべてのマップグリッドで線分を描画 
        foreach (var column in mapGrids)
        {
            foreach (var grid in column)
            {
                if (grid != null)
                {
                    grid.DrawLinesToUpperLayers();
                }
            }
        }
    }
}
