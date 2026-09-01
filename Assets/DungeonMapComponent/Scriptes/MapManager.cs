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
    
    private List<List<MapGridInfo>> mapGrids;
    
    private void Start()
    {
        // 全てのマップグリッド情報をジェネレータに登録
        var generator = new DungeonMapGenerator();
        foreach (var info in availableMapGridInfoList)
        {
            generator.AddMapGridInfo(info);
        }

        mapGrids = generator.GenerateDungeonMap(branchCount, floorCount);
        SpawnMapGrids();
    }

    /// <summary>
    /// マップグリッドを画面上に生成する関数
    /// </summary>
    public void SpawnMapGrids()
    {
        if (mapGrids == null || mapGridPrefab == null)
        {
            Debug.LogWarning("mapGrids or mapGridPrefab is null.");
            return;
        }
   

        for (int col = 0; col < mapGrids.Count; col++)
        {
            var colList = mapGrids[col];
            for (int row = 0; row < colList.Count; row++)
            {
                // 初期配置を中央揃えに（Y座標はCanvas中央を0として上方向に配置）
                float totalWidth = (mapGrids.Count - 1) * xOffset;
                float x = col * xOffset - totalWidth / 2f;
                float y = row * yOffset;
                Vector3 position = new Vector3(x, y, 0);
                // MapGridのインスタンスを生成
                MapGrid gridInstance = Instantiate(mapGridPrefab, mapGridCanvas.transform);
                // 座標を設定
                gridInstance.transform.localPosition = position;
                // グリッド情報をセット
                gridInstance.SetInfo(colList[row]);
            }
        }
    }
}
