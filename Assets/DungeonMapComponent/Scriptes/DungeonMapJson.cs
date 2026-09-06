using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapGridJson
{
    public MapGridJson()
    {
        upperMapGridPos = new();
        underMapGridPos = new();
    }
    public int mapGridInfoID; // マップグリッド情報のID
    // マップグリッドの位置
    [System.Serializable]
    public struct MapGridPos
    {
        public int row;
        public int col;
    }
    public MapGridPos pos;

    public List<MapGridPos> upperMapGridPos;
    public List<MapGridPos> underMapGridPos;
}

[System.Serializable]
public class DungeonMapJson
{
    public DungeonMapJson()
    {
        dungeonMapGrid = new();
    }
    public List<MapGridJson> dungeonMapGrid;
}