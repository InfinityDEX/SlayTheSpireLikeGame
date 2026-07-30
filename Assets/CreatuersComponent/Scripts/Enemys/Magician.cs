using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : Enemy
{
    public override void Action()
    {
        Debug.Log("魔法発動");
        // プレイヤーに10点のダメージを与える
        player.Damage(10);
    }
}
