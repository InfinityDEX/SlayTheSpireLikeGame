using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardOverlayButtonController : MonoBehaviour
{
    [field: SerializeField, Header("カードオーバーレイボタン")]
    private Button cardOverlayButton;

    /// <summary>
    /// カードオーバーレイボタンを有効化
    /// </summary>
    public void EnableButton()
    {
        cardOverlayButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// カードオーバーレイボタンを無効化
    /// </summary>
    public void DisableButton()
    {
        cardOverlayButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// カードオーバーレイボタンを活性化(押せる状態にする)
    /// </summary>
    public void ActiveButton()
    {
        cardOverlayButton.interactable = true;
    }

    /// <summary>
    /// カードオーバーレイボタンを非活性化(表示しても押せない状態にする)
    /// </summary>
    public void InActiveButton()
    {
        cardOverlayButton.interactable = false;
    }

    /// <summary>
    /// ボタン押下時に処理するイベントを全て削除
    /// </summary>
    public void ResetOnClickListener()
    {
        cardOverlayButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// ボタン押下時に処理するイベントを登録（既存の処理は全て削除）
    /// </summary>
    /// <param name="action">オーバーレイボタン押下時に発火する処理</param>
    public void RegisterOnClickListener(UnityAction action)
    {
        ResetOnClickListener(); // 一度イベントをすべて削除する
        cardOverlayButton.onClick.AddListener(action);
    }

    /// <summary>
    /// ボタン押下時に処理するイベントを複数登録（既存の処理は全て削除）
    /// </summary>
    /// <param name="actions">オーバーレイボタン押下時に発火する処理のリスト</param>
    public void RegisterOnClickListener(List<UnityAction> actions)
    {
        ResetOnClickListener(); // 一度イベントをすべて削除する
        foreach (var action in actions)
        {
            cardOverlayButton.onClick.AddListener(action);
        }
    }
}
