/// <summary>
/// カード効果のインターフェイス
/// </summary>
public abstract class CardEffect{
    /// <summary>
    /// カードの実行
    /// </summary>
    /// <param name="target">カード効果の適用対象</param>
    public abstract void Play(Creature target);
}