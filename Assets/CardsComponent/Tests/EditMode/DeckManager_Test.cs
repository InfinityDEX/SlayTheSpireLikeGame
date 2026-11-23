using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace CardsComponent.Tests.EditMode
{
    /// <summary>
    /// デッキマネージャーのテスト定義
    /// </summary>
    public class DeckManager_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private DeckManager deckManagerInstance;

        /// <summary>
        /// DeckManagerに注入するカードデータライブラリ
        /// </summary>
        private CardDataLibrary cardDataLibrary;

        /// <summary>
        /// DeckManagerに注入するデッキ情報
        ///
        /// DeckManagerは山札の中身を公開していないので、テスト側で参照を持って中身を確認する
        /// </summary>
        private DeckInfomation deckInfo;

        /// <summary>
        /// DeckManager、CardDataLibraryを登録するゲームオブジェクト
        /// </summary>
        private GameObject go;

        /// <summary>
        /// 本テストスコープ開始時に一度だけ呼び出すセットアップ
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            go = new();
        }

        /// <summary>
        /// セットアップ（毎テスト開始前実行）
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // 依存性注入（DI）
            // 記載をシンプルにする為、VContainerのビルダーやコンテナは使用せず、
            // DI用のメソッドを呼び出して渡す
            cardDataLibrary = go.AddComponent<CardDataLibrary>();
            cardDataLibrary.Init(new SimpleCardDataFactory());

            deckInfo = new DeckInfomation();

            // EditModeではStartが呼ばれないので、DeckData.jsonの読み込みは行われない
            // カードビュワーはテストで使用しないのでnullを渡す
            deckManagerInstance = go.AddComponent<DeckManager>();
            deckManagerInstance.Init(cardDataLibrary, deckInfo, null);
        }

        /// <summary>
        /// テスト用ファイル取り壊し（テストスコープの最後に一度だけ）
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // 使いまわしていたgoを削除
            UnityEngine.Object.DestroyImmediate(go);
        }

        /// <summary>
        /// テスト用ファイル取り壊し（毎テスト開始後実行）
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // テストに使用したコンポーネントは毎回削除する
            UnityEngine.Object.DestroyImmediate(deckManagerInstance);
            UnityEngine.Object.DestroyImmediate(cardDataLibrary);
        }

        /// <summary>
        /// 指定したIDのカードを順番に山札へ追加する
        /// </summary>
        /// <param name="cardIds">追加するカードIDの並び</param>
        private void SetUpDrawPile(params int[] cardIds)
        {
            foreach (var cardId in cardIds)
            {
                deckManagerInstance.AddCard(cardDataLibrary.SearchCardData(cardId));
            }
        }

        /// <summary>
        /// 山札のカードIDを並び順のまま取得する
        /// </summary>
        /// <returns>山札のカードID配列</returns>
        private int[] DrawPileCardIds()
        {
            return deckInfo.DrawPile.Select(cardData => cardData.CardID).ToArray();
        }

        /// <summary>
        /// カードを山札に追加できることを確認
        /// </summary>
        [Test]
        public void AddCard()
        {
            deckManagerInstance.AddCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.CardId));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// 同じIDのカードが複数あっても、除去されるのは1枚だけであることを確認
        /// </summary>
        [Test]
        public void RemoveCardOnlyOne()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId);

            deckManagerInstance.RemoveCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.CardId));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId, SimpleCardDataFactory.OtherCardId }));
        }

        /// <summary>
        /// 山札に無いカードを除去しようとしたら例外が出て、山札が変化しないことを確認
        /// </summary>
        [Test]
        public void RemoveCardNotInDrawPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);

            Assert.Throws<InvalidOperationException>(
                () => deckManagerInstance.RemoveCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.OtherCardId)));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// nullを除去しようとしたら例外が出ることを確認
        /// </summary>
        [Test]
        public void RemoveNullCard()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);

            Assert.Throws<ArgumentNullException>(() => deckManagerInstance.RemoveCard(null));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// 同じIDのカードが複数あっても、強化されるのは最初の1枚だけで、位置も変わらないことを確認
        /// </summary>
        [Test]
        public void UpgradeCardOnlyFirstOne()
        {
            SetUpDrawPile(SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId);

            deckManagerInstance.UpgradeCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.CardId));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] {
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.UpgradedCardId,
                SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// 山札に無いカードを強化しようとしたら例外が出て、山札が変化しないことを確認
        /// </summary>
        [Test]
        public void UpgradeCardNotInDrawPile()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);

            Assert.Throws<InvalidOperationException>(
                () => deckManagerInstance.UpgradeCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.OtherCardId)));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }

        /// <summary>
        /// nullを強化しようとしたら例外が出ることを確認
        /// </summary>
        [Test]
        public void UpgradeNullCard()
        {
            SetUpDrawPile(SimpleCardDataFactory.CardId);

            Assert.Throws<ArgumentNullException>(() => deckManagerInstance.UpgradeCard(null));

            Assert.That(DrawPileCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }
    }
}
