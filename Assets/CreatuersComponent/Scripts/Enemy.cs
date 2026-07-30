using UnityEngine;

public class Enemy : Creature
{
    [Header("プレイヤー")]
    [SerializeField]
    private Creature playerInspector;

    public Creature player { get; private set;}

    public void Awake()
    {
        player = playerInspector;
    }

    public virtual void Action()
    {
        // ここに敵の行動を書く
        Debug.Log("行動終了");
    }    
}
