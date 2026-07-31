using UnityEngine;

public class Enemy : Creature
{
    public virtual bool Action()
    {
        // ここに敵の行動を書く
        Debug.Log("行動終了");
        return true;
    }    
}
