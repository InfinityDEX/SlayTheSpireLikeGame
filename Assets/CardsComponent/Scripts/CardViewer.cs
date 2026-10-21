using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// カードプレビュー画面
/// </summary>
public class CardViewer : MonoBehaviour
{
    [field: Header("カードのプレハブ"), SerializeField]
    private Card cardPrefab;

    [field: Header("カードを配置するRectTransform"), SerializeField]
    private Transform cardParentTransform;

    private List<Card> cardInstancePool;

    /// <summary>
    ///　ビュアーを表示する
    /// </summary>
    public void EnableViewer(List<CardData> cardDatas)
    {
        // 山札等を順番が通り表示すると順番どおり表示されてしまいゲーム性が損なわれる為、
        // まず渡されたカードデータをID順でソートする
        List<CardData> sortedCardDatas = new();
        // 元のカードデータリストの順番を書き換えないようにディープコピー
        foreach (var cardData in cardDatas)
        {
            // CardDataの方は元からメモリ節約の為ベースデータの参照なのでディープコピー不要
            sortedCardDatas.Add(cardData);
        }
        // 昇順ソート
        sortedCardDatas.Sort((cardDataA, cardDataB) => cardDataA.CardID.CompareTo(cardDataB.CardID));

        // 現在生成し用としているカードのインデックス
        int instancedCardIdx = 0;
        // カード実体の生成
        foreach (var cardData in sortedCardDatas)
        {
            // カードプールの用意
            cardInstancePool ??= new();

            // 既に過去に生成したインスタンスを流用できる場合は使い、
            // 足りなくなったら新しくインスタンス化する
            if (cardInstancePool.Count > instancedCardIdx)
            {
                // インスタンスの再利用及びセットアップ
                var cardObject = cardInstancePool[instancedCardIdx];
                cardObject.gameObject.SetActive(true);
                cardObject.Setup(cardData, -1);
            }
            else
            {
                // インスタンス生成及びセットアップ
                var cardObject = Instantiate(cardPrefab, cardParentTransform);
                cardInstancePool.Add(cardObject);
                cardObject.Setup(cardData, -1);
            }

            instancedCardIdx++;
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    ///　ビュアーを非表示にする
    /// </summary>
    public void DisableViewer()
    {
        // 全てのカードインスタンスを非アクティブ化する(再表示時に再利用)
        foreach(Transform child in cardParentTransform.transform)
        {
            child.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
