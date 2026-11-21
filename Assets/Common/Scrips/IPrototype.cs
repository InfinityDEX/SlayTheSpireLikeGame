/// <summary>
/// 自己複製を行えるクラスであることを定義するインターフェース（Prototype パターン）
/// </summary>
/// <typeparam name="T">複製して返す型（通常は実装するクラス自身、またはその基底クラス）</typeparam>
public interface IPrototype<T>
{
    /// <summary>
    /// 自己複製メソッド
    /// </summary>
    /// <returns>複製したインスタンス</returns>
    T Spawn();
}