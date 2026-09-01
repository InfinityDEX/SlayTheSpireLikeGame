using System.Collections.Generic;

public class DungeonMapGenerator
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
        for (int i = 0; i < branchCount; i++)
        {
            var floor = new List<MapGridInfo>();
            for (int j = 0; j < floorCount; j++)
            {
                floor.Add(GetRandomMapGridInfo());
            }
            mapGrids.Add(floor);
        }

        return mapGrids;
    }
}
