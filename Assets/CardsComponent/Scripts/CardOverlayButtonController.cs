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
    /// ボタン押下時に処理するイベントを登録
    /// </summary>
    /// <param name="action">オーバーレイボタン押下時に発火する処理</param>
    public void RegisterOnClickListener(UnityAction action)
    {
        cardOverlayButton.onClick.AddListener(action);
    }
}
