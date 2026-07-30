using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Create Creature Data/Player Data")]
public class PlayerData : CreatureData
{
    // 現状特にCreatureDataクラスから追加したいパラメータは無いので
    // CreatureDataクラスのデータを作成する為のラッパークラスのように運用する。
    // (CreatureDataにはCreateAssetMenuを用意していないので)
}
