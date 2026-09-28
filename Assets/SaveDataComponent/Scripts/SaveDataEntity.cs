/// <summary>
/// セーブデータエンティティクラス
/// 
/// ダンジョンマップは別に保存
/// それ以外は基本的にこのエンティティを使う
/// </summary>
[System.Serializable]
public class SaveDataEntity
{
    /// <summary>
    /// 最大体力
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// 現在の残り体力
    /// </summary>
    public int currentHealth = 100;
}
