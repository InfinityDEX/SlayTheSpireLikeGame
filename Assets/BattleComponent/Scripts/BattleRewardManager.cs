using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRewardManager : MonoBehaviour
{
    [field: Header("カードプレハブ"), SerializeField]
    private Card cardPrefab;

    [field: Header("カード選択カーソル"), SerializeField]
    private GameObject cardSelectingCursor;

    [field: Header("カードを配置するRectTransform"), SerializeField]
    private Transform cardParentTransform;

    /// <summary>
    /// リワードカード一覧
    /// </summary>
    private List<Card> rewardCardList;

    /// <summary>
    /// リワードカードのセレクターカーソル
    /// </summary>
    private int selectingRewardCardIdx = 0;

    private void Awake()
    {
        if(rewardCardList == null)
        {
            rewardCardList = new();
        }
    }

    private void Update()
    {
        // カーソルの座標更新
        if (rewardCardList.Count > 0)
        {
            for (int i = 0; i < rewardCardList.Count; i++)
            {
                var mouseHoverChecker = rewardCardList[i].GetComponent<UIMouseHoverChecker>();
                if (mouseHoverChecker.IsHovered)
                {
                    selectingRewardCardIdx = i;
                }
            }

            Vector2 cursorPos = rewardCardList[selectingRewardCardIdx].transform.position;
            cardSelectingCursor.transform.position = cursorPos;
        }
    }

    /// <summary>
    /// リワード画面の表示
    /// </summary>
    /// <param name="cardDatas"></param>
    public void EnableRewardView(List<CardData> rewaeds)
    {
        gameObject.SetActive(true);
        foreach (var rewardCard in rewaeds)
        {
            AddRewardCard(rewardCard);
        }
    }

    /// <summary>
    /// 報酬カード追加
    /// </summary>
    /// <param name="rewardCardData">報酬となるカード</param>
    public void AddRewardCard(CardData rewardCardData)
    {
        Card rewardCard = Instantiate(cardPrefab, cardParentTransform);
        rewardCard.Setup(rewardCardData, -1);
        rewardCard.AddComponent<UIMouseHoverChecker>();
        if(rewardCardList == null)
        {
            rewardCardList = new();
        }
        rewardCardList.Add(rewardCard);
    }

    /// <summary>
    /// 「カード取得ボタン」を押したときに行う処理(Buttonから呼び出し)
    /// </summary>
    public void OnPushCardGetButton()
    {
        var deckManager = DeckManager.Instance;
        deckManager.AddCard(rewardCardList[selectingRewardCardIdx].ThisCardData);
        deckManager.SaveDeck();
        StartCoroutine(LoadMapSceneAfterDelay(2.0f));
    }

    /// <summary>
    /// 「カードを取得しないボタン」を押したときに行う処理(Buttonから呼び出し)
    /// </summary>
    public void OnPushCardNotSelectButton()
    {
        StartCoroutine(LoadMapSceneAfterDelay(2.0f));
    }

    /// <summary>
    /// マップシーンをディレイで呼び出すメソッド
    /// </summary>
    /// <param name="delay">ディレイ秒数</param>
    /// <returns></returns>
    private IEnumerator LoadMapSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}
