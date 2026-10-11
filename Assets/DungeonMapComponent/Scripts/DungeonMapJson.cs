using System.Collections.Generic;

/// <summary>
/// マップグリッドJsonEntityクラス
///
/// マップグリッド（マス）1つ分の情報
/// </summary>
[System.Serializable]
public class MapGridJsonEntity
{
    public MapGridJsonEntity()
    {
        UpperMapGridPos = new();
        LowerMapGridPos = new();
    }
    public int mapGridInfoID; // マップグリッド情報のID
    // マップグリッドの位置
    [System.Serializable]
    public struct MapGridPos
    {
        public int Row;
        public int Col;
    }
    public MapGridPos Pos;

    /// <summary>
    /// このマップグリッドと繋がっている一つ上層のマップグリッドの座標
    /// </summary>
    public List<MapGridPos> UpperMapGridPos;

    /// <summary>
    /// このマップグリッドと繋がっている一つ下層のマップグリッドの座標
    /// </summary>
    public List<MapGridPos> LowerMapGridPos;
}

/// <summary>
/// フロア1つにあるマップグリッドのリストJsonEntityクラス
///
/// List<MapGridJson>のラッパークラス（DungeonMapJson用。JsonUtilityで扱う際に二重リストそのままだとコンバート出来なくなる仕様への対処）
/// </summary>
[System.Serializable]
public class DungeonFloorJson
{
    public DungeonFloorJson()
    {
        FloorGrids = new();
    }

    /// <summary>
    /// 各フロアのマップグリッド情報のリスト
    /// </summary>
    public List<MapGridJsonEntity> FloorGrids;

    /// <summary>
    /// 添字演算子のオーバーライド
    ///
    /// 元々List<List<MapGridJson>型で定義したかったデータを、
    /// JsonUtilityの仕様でコンバートできない症状を解消するために本クラスは定義している。
    /// コードで使用する際はList<List<MapGridJson>の方が直感的に
    /// ダンジョンマップの構造に沿っているため（dungeonJson[floorNum][Column]のような記法）
    /// DungeonMapJson側で二次配列のように扱えるようにしたいので、
    /// 演算子をオーバーライドすることで上記のような記法で本クラスを認識せず利用できる。
    /// </summary>
    /// <param name="index">取得したいfloorGrids内のMapGridのインデックス</param>
    /// <returns>指定したMapGrid実体</returns>
    public MapGridJsonEntity this[int index]
    {
        get { return FloorGrids[index]; }
    }
}

/// <summary>
/// ダンジョンマップJsonEntityクラス
///
/// ダンジョン全体の情報
/// </summary>
[System.Serializable]
public class DungeonMapJson
{
    public DungeonMapJson()
    {
        DungeonMapGrid = new();
    }

    // ダンジョンマップグリッド情報
    public List<DungeonFloorJson> DungeonMapGrid;

    // 移動経路
    public List<MapGridJsonEntity.MapGridPos> MovePath;

    // 現在の位置
    public MapGridJsonEntity.MapGridPos CurrentGridPos;
}