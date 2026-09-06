using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Data", menuName = "Create Map /MapGrid Data")]
public class MapGridInfo : ScriptableObject
{
    [Header("グリッド画像")]
    public Sprite gridSprite;

    [Serializable]
    public enum FloorInfo
    {
        StartFloor,
        EnemyFloor, // 敵フロア
        EliteEnemyFloor, // エリート敵フロア
        BossFloor, // ボスフロア
        ShopFloor, // ショップフロア
    }

    [Header("フロア情報")]
    public FloorInfo type;

    public int id;
}
