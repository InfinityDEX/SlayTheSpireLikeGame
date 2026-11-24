using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CardsComponent.Tests.EditMode
{
    /// <summary>
    /// デッキ情報クラスのテスト
    /// </summary>
    public class DeckInfomation_Test
    {
        /// <summary>
        /// テスト対象
        /// </summary>
        private DeckInfomation deckInfomationInstance;

        /// <summary>
        /// テストで使うカードデータ
        ///
        /// DeckInfomationはカードデータの正しさに責任を持たないので、区別できるだけのデータをスタブから取得する
        /// </summary>
        private List<CardData> cardList;

        /// <summary>
        /// セットアップ
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            deckInfomationInstance = new();
            cardList = new SimpleCardDataFactory().CreateCardDataList();
        }

        /// <summary>
        /// 指定したIDのカードデータを取得する
        /// </summary>
        /// <param name="cardId">カードID</param>
        /// <returns>カードデータ</returns>
        private CardData GetCard(int cardId)
        {
            return cardList.Find(cardData => cardData.CardID == cardId);
        }

        /// <summary>
        /// 指定したIDのカードデータのリストを作る
        /// </summary>
        /// <param name="cardIds">カードIDの並び</param>
        /// <returns>カードデータのリスト</returns>
        private List<CardData> CreateCards(params int[] cardIds)
        {
            return cardIds.Select(GetCard).ToList();
        }

        /// <summary>
        /// 指定したIDのカードでデッキを作り、山札にコピーする（バトル開始時の状態）
        /// </summary>
        /// <param name="cardIds">カードIDの並び（山札にコピーされたあとシャッフルされる）</param>
        private void SetUpDrawPile(params int[] cardIds)
        {
            deckInfomationInstance.SetDeckData(CreateCards(cardIds));
            deckInfomationInstance.ResetPlayedDeck();
        }

        /// <summary>
        /// カードデータのリストをカードIDの配列にする
        /// </summary>
        /// <param name="cardDatas">カードデータのリスト</param>
        /// <returns>カードIDの配列</returns>
        private static int[] ToIds(IEnumerable<CardData> cardDatas)
        {
            return cardDatas.Select(cardData => cardData.CardID).ToArray();
        }

        /// <summary>
        /// 正常なデッキデータ（カードデータリスト）が渡されたら問題なくDeckInfomation内に反映されることを確認
        ///
        /// ※データがある状態で追加されるタイミングは想定していないが、現状は元々あったデータは全て削除する仕様
        /// </summary>
        [Test]
        public void SetDeckDataInDeck()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.OtherCardId));

            var deckDatas = CreateCards(SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.SetDeckData(deckDatas);

            // 元々あったデータは残らない
            Assert.That(ToIds(deckInfomationInstance.DeckList),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId }));

            // 渡したリストを後から書き換えてもデッキは変わらない（コピーして保持している）
            deckDatas.Clear();
            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(2));
        }

        /// <summary>
        /// デッキデータにnullを渡したら例外が出ることを確認
        /// </summary>
        [Test]
        public void SetNullDeckDataInDeckThrows()
        {
            Assert.Throws<ArgumentNullException>(() => deckInfomationInstance.SetDeckData(null));
        }

        /// <summary>
        /// カードデータ単体を追加できることを確認
        /// </summary>
        [Test]
        public void AddCardDataInDeck()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            deckInfomationInstance.AddCard(GetCard(SimpleCardDataFactory.OtherCardId));

            Assert.That(ToIds(deckInfomationInstance.DeckList),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// カードデータを削除できることを確認
        ///
        /// 同じIDのカードが複数あっても、削除されるのは1枚だけ
        /// </summary>
        [Test]
        public void RemoveCardDataInDeck()
        {
            deckInfomationInstance.SetDeckData(CreateCards(
                SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId));

            deckInfomationInstance.RemoveCard(GetCard(SimpleCardDataFactory.CardId));

            Assert.That(ToIds(deckInfomationInstance.DeckList),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// 全てのカードデータをデッキデータから削除できることを確認
        /// </summary>
        [Test]
        public void RemoveAllCardDataInDeck()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DiscardCardById(deckInfomationInstance.HandList[0].cardIdx);

            deckInfomationInstance.RemoveDeck();

            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// バトル後のデッキ状況をリセットできることを確認
        ///
        /// リセット後の山札はデッキのコピーなので、山札からカードを引いてもデッキは減らない
        /// </summary>
        [Test]
        public void ResetPlayedDeck()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DiscardCardById(deckInfomationInstance.HandList[0].cardIdx);

            deckInfomationInstance.ResetPlayedDeck();

            Assert.That(ToIds(deckInfomationInstance.DrawPileList), Is.EqualTo(ToIds(deckInfomationInstance.DeckList)));
            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));

            // 山札とデッキが別のリストになっていることを確認
            deckInfomationInstance.DrawCard();
            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(2));
        }

        /// <summary>
        /// デッキに無いカードデータを削除しようとすると例外になることを確認
        /// </summary>
        [Test]
        public void RemoveCardDataNotInDeckThrows()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            Assert.Throws<InvalidOperationException>(
                () => deckInfomationInstance.RemoveCard(GetCard(SimpleCardDataFactory.OtherCardId)));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// カードデータ削除メソッドにnullを渡すと例外になることを確認
        /// </summary>
        [Test]
        public void RemoveNullCardDataThrows()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            Assert.Throws<ArgumentNullException>(() => deckInfomationInstance.RemoveCard(null));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// カードデータを強化できることを確認
        ///
        /// 同じIDのカードが複数あっても、強化されるのは1枚だけ
        /// </summary>
        [Test]
        public void UpgradeCardDataInDeck()
        {
            deckInfomationInstance.SetDeckData(CreateCards(
                SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId));

            deckInfomationInstance.UpgradeCard(
                GetCard(SimpleCardDataFactory.CardId), GetCard(SimpleCardDataFactory.UpgradedCardId));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.UpgradedCardId }));
        }

        /// <summary>
        /// デッキに無いカードデータを強化しようとすると例外になることを確認
        /// </summary>
        [Test]
        public void UpgradeCardDataNotInDeckThrows()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            Assert.Throws<InvalidOperationException>(() => deckInfomationInstance.UpgradeCard(
                GetCard(SimpleCardDataFactory.OtherCardId), GetCard(SimpleCardDataFactory.OtherUpgradedCardId)));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// デッキ強化メソッドの強化元の引数にnullを渡すと例外になることを確認
        /// </summary>
        [Test]
        public void UpgradeNullCardDataThrows()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            Assert.Throws<ArgumentNullException>(
                () => deckInfomationInstance.UpgradeCard(null, GetCard(SimpleCardDataFactory.UpgradedCardId)));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// デッキ強化メソッドの強化先の引数にnullを渡すと例外になることを確認
        /// （デッキにnullが入らないこと）
        /// </summary>
        [Test]
        public void UpgradeCardNullUpgradeTargetThrows()
        {
            deckInfomationInstance.SetDeckData(CreateCards(SimpleCardDataFactory.CardId));

            Assert.Throws<ArgumentNullException>(
                () => deckInfomationInstance.UpgradeCard(GetCard(SimpleCardDataFactory.CardId), null));

            Assert.That(ToIds(deckInfomationInstance.DeckList), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// カードデータにnullを渡したら例外が出ることを確認
        /// </summary>
        [Test]
        public void AddNullCardDataInDeckThrows()
        {
            Assert.Throws<ArgumentNullException>(() => deckInfomationInstance.AddCard(null));

            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 山札からカードを引くことができることを確認
        ///
        /// ResetPlayedDeckで山札はシャッフルされるので、どのカードが引かれるかは確認しない。
        /// 引いたカードと残りの山札を合わせると、元の山札と同じ構成になることを確認する
        /// </summary>
        [Test]
        public void DrawCardToHandInDrawPile()
        {
            var cardIds = new[] { SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId };
            SetUpDrawPile(cardIds);

            deckInfomationInstance.DrawCard();

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(1));
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(2));
            var drawnAndRemaining = ToIds(deckInfomationInstance.HandList.Select(cardInHand => cardInHand.cardData))
                .Concat(ToIds(deckInfomationInstance.DrawPileList));
            Assert.That(drawnAndRemaining, Is.EquivalentTo(cardIds));
        }

        /// <summary>
        /// 山札が0の時、捨札にカードがあれば捨札のカードをすべて山札に戻してから
        /// カードが引かれることを確認
        /// </summary>
        [Test]
        public void MoveDiscardPileCardToDrawPileOnDrawCardToHandInEmptyDrawPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DiscardHandCardAll();

            deckInfomationInstance.DrawCard();

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(1));
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(1));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 山札にも捨札にもカードがない状態でカードドローメソッドを読んでも例外にならず、手札も増えないことを確認
        /// </summary>
        [Test]
        public void NonDrawCardToHandInEmptyDrawPileAndEmptyDiscardPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();

            // 全てのカードが手札にある状態で引く（例外が出ればテストは失敗する）
            deckInfomationInstance.DrawCard();

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(1));
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 手札から捨札に指定したIDのカードを移動させられることを確認
        /// </summary>
        [Test]
        public void MoveCardToDiscardPileInHand()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();

            // 手札のインデックスは静的なカウンターで採番されるので、実際の値を読んでから使う
            var target = deckInfomationInstance.HandList.Find(
                cardInHand => cardInHand.cardData.CardID == SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DiscardCardById(target.cardIdx);

            Assert.That(ToIds(deckInfomationInstance.HandList.Select(cardInHand => cardInHand.cardData)),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
            Assert.That(ToIds(deckInfomationInstance.DiscardPileList),
                Is.EqualTo(new[] { SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// 手札から捨札に全てのカードを移動させられることを確認
        /// </summary>
        [Test]
        public void MoveCardToDiscardPileInAllHand()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();

            deckInfomationInstance.DiscardHandCardAll();

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(0));
            Assert.That(ToIds(deckInfomationInstance.DiscardPileList),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// 手札に無いIDを指定して手札から捨札にカード移動させようとすると例外になることを確認
        /// </summary>
        [Test]
        public void MoveCardToDiscardPileNotInHandThrows()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();

            // 手札のインデックスは1から採番されるので、-1が手札にあることはない
            Assert.Throws<InvalidOperationException>(() => deckInfomationInstance.DiscardCardById(-1));

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(1));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 捨札のカードを全て山札に戻せることを確認
        ///
        /// 手札のカードは山札に戻らない
        /// </summary>
        [Test]
        public void MoveCardToDrawPileInAllDiscardPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            var target = deckInfomationInstance.HandList.Find(
                cardInHand => cardInHand.cardData.CardID == SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DiscardCardById(target.cardIdx);

            deckInfomationInstance.ReturnDiscardCardsToDrawPile();

            Assert.That(ToIds(deckInfomationInstance.DrawPileList), Is.EqualTo(new[] { SimpleCardDataFactory.OtherCardId }));
            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(2));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 手札と捨札のカードを全て山札に戻せることを確認
        /// </summary>
        [Test]
        public void MoveAllCardsToDrawPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            var target = deckInfomationInstance.HandList.Find(
                cardInHand => cardInHand.cardData.CardID == SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DiscardCardById(target.cardIdx);

            deckInfomationInstance.ReturnAllCardsToDrawPile();

            Assert.That(ToIds(deckInfomationInstance.DrawPileList), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId }));
            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(0));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// 捨札が空っぽでも捨札のカードを全て山札に戻す処理を例外なく実行できることを確認
        /// </summary>
        [Test]
        public void MoveCardToDrawPileInEmptyDiscardPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);

            // 例外が出ればテストは失敗する
            deckInfomationInstance.ReturnDiscardCardsToDrawPile();

            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(2));
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(0));
        }

        /// <summary>
        /// デッキ枚数を見ることができる
        /// </summary>
        [Test]
        public void CanCheckDeckCount()
        {
            deckInfomationInstance.SetDeckData(CreateCards(
                SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.OtherCardId));

            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(3));
        }

        /// <summary>
        /// デッキリストを見ることができる
        ///
        /// IDの昇順で並び、受け取ったリストを書き換えてもデッキは変わらない
        /// </summary>
        [Test]
        public void CanCheckDeckList()
        {
            deckInfomationInstance.SetDeckData(CreateCards(
                SimpleCardDataFactory.OtherUpgradedCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId));

            var deckList = deckInfomationInstance.DeckList;

            Assert.That(ToIds(deckList), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.OtherUpgradedCardId }));

            deckList.Clear();
            Assert.That(deckInfomationInstance.DeckCount, Is.EqualTo(3));
        }

        /// <summary>
        /// デッキ中にある未強化のカードリストを見ることができる
        /// </summary>
        [Test]
        public void CanCheckUnUpgradeDeckList()
        {
            deckInfomationInstance.SetDeckData(CreateCards(
                SimpleCardDataFactory.UpgradedCardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId));

            Assert.That(ToIds(deckInfomationInstance.UnUpgradeCardList),
                Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// 山札の枚数を見ることができる
        /// </summary>
        [Test]
        public void CanCheckDrawPileCount()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(3));

            deckInfomationInstance.DrawCard();
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(2));
        }

        /// <summary>
        /// 山札リストを見ることができる
        ///
        /// IDの昇順で並び、受け取ったリストを書き換えても山札は変わらない
        /// </summary>
        [Test]
        public void CanCheckDrawPileList()
        {
            SetUpDrawPile(SimpleCardDataFactory.OtherUpgradedCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);

            var drawPileList = deckInfomationInstance.DrawPileList;

            Assert.That(ToIds(drawPileList), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.OtherUpgradedCardId }));

            drawPileList.Clear();
            Assert.That(deckInfomationInstance.DrawPileCount, Is.EqualTo(3));
        }

        /// <summary>
        /// 手札の枚数を見ることができる
        /// </summary>
        [Test]
        public void CanCheckHandCount()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();

            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(2));
        }

        /// <summary>
        /// 手札リストを見ることができる
        ///
        /// IDの昇順で並び、受け取ったリストを書き換えても手札は変わらない
        /// </summary>
        [Test]
        public void CanCheckHandList()
        {
            SetUpDrawPile(SimpleCardDataFactory.OtherUpgradedCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();

            var handList = deckInfomationInstance.HandList;

            Assert.That(ToIds(handList.Select(cardInHand => cardInHand.cardData)), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.OtherUpgradedCardId }));

            handList.Clear();
            Assert.That(deckInfomationInstance.HandCount, Is.EqualTo(3));
        }

        /// <summary>
        /// 捨札の枚数を見ることができる
        /// </summary>
        [Test]
        public void CanCheckDiscardPileCount()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DiscardHandCardAll();

            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(2));
        }

        /// <summary>
        /// 捨札リストを見ることができる
        ///
        /// IDの昇順で並び、受け取ったリストを書き換えても捨札は変わらない
        /// </summary>
        [Test]
        public void CanCheckDiscardPileList()
        {
            SetUpDrawPile(SimpleCardDataFactory.OtherUpgradedCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DrawCard();
            deckInfomationInstance.DiscardHandCardAll();

            var discardPileList = deckInfomationInstance.DiscardPileList;

            Assert.That(ToIds(discardPileList), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.OtherUpgradedCardId }));

            discardPileList.Clear();
            Assert.That(deckInfomationInstance.DiscardPileCount, Is.EqualTo(3));
        }

        /// <summary>
        /// デッキシャッフルした時にデッキの枚数や構成カード枚数が変わらないことを確認
        ///
        /// ※ランダムに混ざっているかどうかは内部で呼んでいるRandom.Rangeの責任なので確認しない。
        /// </summary>
        [Test]
        public void CanShuffleDrawPile()
        {
            SetUpDrawPile(
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.UpgradedCardId);
            var before = ToIds(deckInfomationInstance.DrawPileList);

            deckInfomationInstance.Shuffle();

            // 並び順は問わず、中身が同じであることを確認
            Assert.That(ToIds(deckInfomationInstance.DrawPileList), Is.EquivalentTo(before));
        }
    }
}
