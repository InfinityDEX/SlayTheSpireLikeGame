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
        /// テスト側でも参照を持ち、デッキの準備と中身の確認に使う
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
            deckManagerInstance.Init(cardDataLibrary, deckInfo, null, null);
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
        /// 指定したIDのカードでデッキを作る
        /// </summary>
        /// <param name="cardIds">デッキに入れるカードIDの並び</param>
        private void SetUpDeck(params int[] cardIds)
        {
            deckInfo.SetDeckData(cardIds.Select(cardDataLibrary.SearchCardData).ToList());
        }

        /// <summary>
        /// デッキのカードIDを取得する（IDの昇順）
        /// </summary>
        /// <returns>デッキのカードID配列</returns>
        private int[] DeckCardIds()
        {
            return deckInfo.DeckList.Select(cardData => cardData.CardID).ToArray();
        }

        // ※カードの追加・削除や、デッキに無いカードを指定したときの例外は
        //   DeckInfomationの責任なので、DeckInfomation_Testで確認する。
        //   ここではDeckManagerが担当する「強化先のカードをライブラリから探してデッキに反映する」処理だけを確認する

        /// <summary>
        /// ライブラリから強化先のカードを探して、デッキのカードが強化されることを確認
        ///
        /// 同じIDのカードが複数あっても、強化されるのは1枚だけ
        /// </summary>
        [Test]
        public void UpgradeCardOnlyOne()
        {
            SetUpDeck(SimpleCardDataFactory.OtherCardId, SimpleCardDataFactory.CardId, SimpleCardDataFactory.CardId);

            deckManagerInstance.UpgradeCard(cardDataLibrary.SearchCardData(SimpleCardDataFactory.CardId));

            Assert.That(DeckCardIds(), Is.EqualTo(new[] {
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.UpgradedCardId }));
        }

        /// <summary>
        /// nullを強化しようとしたら例外が出て、デッキが変化しないことを確認
        ///
        /// DeckManagerは強化先を探すために引数のカードデータを読むので、DeckManager側でもnullを弾く
        /// </summary>
        [Test]
        public void UpgradeNullCard()
        {
            SetUpDeck(SimpleCardDataFactory.CardId);

            Assert.Throws<ArgumentNullException>(() => deckManagerInstance.UpgradeCard(null));

            Assert.That(DeckCardIds(), Is.EqualTo(new[] { SimpleCardDataFactory.CardId }));
        }
    }
}
