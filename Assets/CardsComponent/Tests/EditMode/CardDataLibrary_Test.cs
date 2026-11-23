using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace CardsComponent.Tests.EditMode
{
    /// <summary>
    /// カードデータライブラリのテスト定義
    /// </summary>
    public class CardDataLibrary_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private CardDataLibrary cardDataLibraryInstance;

        /// <summary>
        /// CardDataLibraryを登録するゲームオブジェクト
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
            cardDataLibraryInstance = go.AddComponent<CardDataLibrary>();
            cardDataLibraryInstance.Init(new SimpleCardDataFactory());
        }

        /// <summary>
        /// テスト用ファイル取り壊し（テストスコープの最後に一度だけ）
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // 使いまわしていたgoを削除
            Object.DestroyImmediate(go);
        }

        /// <summary>
        /// テスト用ファイル取り壊し（毎テスト開始後実行）
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // テストに使用したcardDataLibraryは毎回削除する
            Object.DestroyImmediate(cardDataLibraryInstance);
        }

        /// <summary>
        /// ライブラリにあるIDを指定したら、そのIDのカードデータが取得できることを確認
        /// </summary>
        /// <param name="cardId">検索するカードID</param>
        [Test]
        public void SearchCardData(
            [Values(
                SimpleCardDataFactory.CardId,
                SimpleCardDataFactory.UpgradedCardId,
                SimpleCardDataFactory.OtherCardId,
                SimpleCardDataFactory.OtherUpgradedCardId)] int cardId)
        {
            var cardData = cardDataLibraryInstance.SearchCardData(cardId);

            Assert.That(cardData.CardID, Is.EqualTo(cardId));
        }

        /// <summary>
        /// ライブラリに無いIDを指定したら例外が出ることを確認
        /// （nullを返して、呼び出し側にnullが流れないこと）
        /// </summary>
        [Test]
        public void SearchCardDataNotInLibrary()
        {
            Assert.Throws<KeyNotFoundException>(
                () => cardDataLibraryInstance.SearchCardData(SimpleCardDataFactory.MissingCardId));
        }
    }
}
