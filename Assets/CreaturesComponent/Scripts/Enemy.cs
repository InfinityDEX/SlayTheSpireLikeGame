using UnityEngine;

/// <summary>
/// エネミー実体クラス
/// 
/// 全てのエネミーはボスかノーマルかにかかわらず
/// 本クラスを継承して作成する。
/// </summary>
public class Enemy : Creature
{
    /// <summary>
    /// エネミーの行動処理
    /// </summary>
    /// <returns>行動が完了したかどうか</returns>
    public virtual bool Action()
    {
        // ここに敵の行動を書く
        Debug.Log("行動終了");
        return true;
    }    

    /// <summary>
    /// 次の行動を表すアイコンを更新する
    /// </summary>
    public virtual void RefreshActionIcon() {}
}
