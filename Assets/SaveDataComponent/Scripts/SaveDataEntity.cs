using System;

/// <summary>
/// セーブデータエンティティクラス
/// 
/// ダンジョンマップは別に保存
/// それ以外は基本的にこのエンティティを使う
/// </summary>
[Serializable]
public class SaveDataEntity
{
    /// <summary>
    /// 最大体力
    /// </summary>
    public int MaxHealth { get; set;}

    /// <summary>
    /// 現在の残り体力
    /// </summary>
    public int CurrentHealth { get; set;}
}
