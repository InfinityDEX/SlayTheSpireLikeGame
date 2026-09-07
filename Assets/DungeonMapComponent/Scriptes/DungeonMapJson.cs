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

// List<MapGridJson>のラッパークラス（DungeonMapJson用。JsonUtilityで扱う際に二重リストそのままだとコンバート出来なくなる使用への対処）
[System.Serializable]
public class DungeonFloorJson
{
    public DungeonFloorJson()
    {
        floorGrids = new();
    }

    public List<MapGridJson> floorGrids;

    // DungeonMapJson側で通常の二次配列のように扱えるように配列演算子をオーバーライド
    public MapGridJson this[int index]
    {
        get { return floorGrids[index]; }
    }
}

[System.Serializable]
public class DungeonMapJson
{
    public DungeonMapJson()
    {
        dungeonMapGrid = new();
    }


    public List<DungeonFloorJson> dungeonMapGrid;
}