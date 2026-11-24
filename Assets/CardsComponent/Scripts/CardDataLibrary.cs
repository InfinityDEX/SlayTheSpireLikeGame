using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace CardsComponent
{
    /// <summary>
    /// カードライブラリクラス
    /// </summary>
    public class CardDataLibrary : MonoBehaviour
    {
        /// <summary>
        /// カードリスト
        /// </summary>
        private List<CardData> cardList = new();

        private ICardDataFactory cardDataFactory;

        [Inject]
        public void Init(ICardDataFactory cardDataFactory)
        {
            this.cardDataFactory = cardDataFactory;

            // CardDataListの初期化
            cardList = cardDataFactory.CreateCardDataList();
        }

        /// <summary>
        /// カードデータの検索
        /// </summary>
        /// <param name="id">カードID</param>
        /// <returns>カードデータ</returns>
        /// <exception cref="KeyNotFoundException">渡されたIDのカードデータがカードリストに無かった</exception>
        public CardData SearchCardData(int id)
        {
            var ret = cardList.Find(cardData => cardData.CardID == id);
            if (ret == null)
                throw new KeyNotFoundException($"指定されたカードID[{id}]を持つカード情報はカードリストに登録されていません。");
            return ret;
        }

        /// <summary>
        /// ライブラリ内にある未強化のカードデータをランダムに取得
        /// </summary>
        /// <param name="getCardCount">取得したいカードデータの数</param>
        /// <returns>カードデータ</returns>
        public List<CardData> GetRandomCards(int getCardCount)
        {
            // 未強化のカードデータだけピックアップ
            var unUpgradeCardList = cardList.Where(card => !card.IsUpgraded).ToList();
            // 取得したいカード数分、重複しないカードをランダムに選択してリストで返す
            if (getCardCount >= unUpgradeCardList.Count)
            {
                // 取得枚数が未強化カード数以上なら、全カードをシャッフルして返す
                return unUpgradeCardList.OrderBy(_ => Random.value).ToList();
            }
            var shuffled = unUpgradeCardList.OrderBy(_ => Random.value).ToList();
            return shuffled.Take(getCardCount).ToList();
        }
    }
}
