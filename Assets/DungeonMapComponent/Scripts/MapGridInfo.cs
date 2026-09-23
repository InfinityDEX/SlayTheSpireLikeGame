using System;
using UnityEngine;

/// <summary>
/// マップグリッド情報クラス
/// 
/// マップグリッドの種別やアイコン画像等を定義
/// </summary>
[CreateAssetMenu(fileName = "Map Data", menuName = "Create Map /MapGrid Data")]
public class MapGridInfo : ScriptableObject
{
    [Header("マップグリッド画像ソース")]
    public Sprite gridSprite;

    [Serializable]
    public enum FloorInfo
    {
        StartFloor, // スタートフロア
        EnemyFloor, // エネミーフロア
        EliteEnemyFloor, // エリートエネミーフロア
        BossFloor, // ボスフロア
        ShopFloor, // ショップフロア
    }

    [Header("フロア情報")]
    public FloorInfo type;

    public int id;
}
