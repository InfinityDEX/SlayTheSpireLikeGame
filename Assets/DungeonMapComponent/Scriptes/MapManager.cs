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
    private List<MapGridInfo> availableMapGridInfoList;
    
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

    private MapGrid startGrid;
    
    private MapGrid bossGrid;

    private void Start()
    {
        // 全てのマップグリッド情報をジェネレータに登録
        var generator = new DungeonMapGenerator();
        foreach (var info in availableMapGridInfoList)
        {
            generator.AddMapGridInfo(info);
        }

        // マップのマス配置情報をランダムに生成
        mapGridInfos = generator.GenerateDungeonMap(branchCount, floorCount);
        SpawnMapGrids(); 
    }

    /// <summary>
    /// マップグリッドを画面上に生成する関数
    /// </summary>
    public void SpawnMapGrids()
    {
        if (mapGridInfos == null || mapGridPrefab == null)
        {
            Debug.LogWarning("mapGrids or mapGridPrefab is null.");
            return;
        }
   

        for (int col = 0; col < mapGridInfos.Count; col++)
        {
            var colList = mapGridInfos[col];
            for (int row = 0; row < colList.Count; row++)
            {
                // 初期配置を中央揃えに（Y座標はCanvas中央を0として上方向に配置）
                float totalWidth = (mapGridInfos.Count - 1) * xOffset;
                float x = col * xOffset - totalWidth / 2f;
                float y = row * yOffset;
                Vector3 position = new Vector3(x, y, 0);
                // MapGridのインスタンスを生成
                MapGrid gridInstance = Instantiate(mapGridPrefab, mapGridCanvas.transform);
                // 座標を設定
                gridInstance.transform.localPosition = position;
                // グリッド情報をセット
                gridInstance.SetInfo(colList[row]);

                // mapGrids にインスタンスを登録
                if (mapGrids == null)
                {
                    mapGrids = new List<List<MapGrid>>(mapGridInfos.Count);
                    for (int i = 0; i < mapGridInfos.Count; i++)
                    {
                        mapGrids.Add(new List<MapGrid>(mapGridInfos[i].Count));
                    }
                }
                mapGrids[col].Add(gridInstance);
            }
        }

        // スタートマスとボスマスを生成

        // 1. 必要な開始・ボス用座標の計算
        float startBossX = 0f;
        int centerCol = mapGridInfos.Count / 2;
        float totalWidthForSB = (mapGridInfos.Count - 1) * xOffset;
        startBossX = centerCol * xOffset - totalWidthForSB / 2f;

        // 2. スタートマス生成（最初の列の下に配置）
        float startY = -yOffset;
        Vector3 startPosition = new Vector3(startBossX, startY, 0f);
        MapGrid startGridInstance = Instantiate(mapGridPrefab, mapGridCanvas.transform);
        startGridInstance.transform.localPosition = startPosition;

        // スタートマス用のMapGridInfoを生成
        MapGridInfo startInfo = ScriptableObject.CreateInstance<MapGridInfo>();
        startInfo.type = MapGridInfo.FloorInfo.ShopFloor; // 適宜 FloorInfo を StartFloor などにカスタムしてもよいです
        startGridInstance.SetInfo(startInfo);

        // 3. ボスマス生成（最後の列の上に配置）
        int rowCount = mapGridInfos[centerCol].Count;
        float bossY = (rowCount) * yOffset;
        Vector3 bossPosition = new Vector3(startBossX, bossY, 0f);
        MapGrid bossGridInstance = Instantiate(mapGridPrefab, mapGridCanvas.transform);
        bossGridInstance.transform.localPosition = bossPosition;

        // ボスマス用のMapGridInfoを生成
        MapGridInfo bossInfo = ScriptableObject.CreateInstance<MapGridInfo>();
        bossInfo.type = MapGridInfo.FloorInfo.BossFloor;
        bossGridInstance.SetInfo(bossInfo);

        // mapGridsリストへ登録
        if (mapGrids != null)
        {
            // スタートマス（最初の列の下＝row: -1的な扱いなので先頭に追加）
            mapGrids[centerCol].Insert(0, startGridInstance);

            // ボスマス（最後の列の上＝最後に追加）
            mapGrids[centerCol].Add(bossGridInstance);
        }
    }
}
