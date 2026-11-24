using CardEffect.Expressions;
using NUnit.Framework;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// 手札カウントクラスのテスト
    /// </summary>
    public class HandCount_Test
    {
        /// <summary>
        /// テスト対象
        /// </summary>
        private HandCount handCountInstance;

        /// <summary>
        /// HandCountに渡すためのデッキ情報インスタンス
        /// </summary>
        private DeckInfomation deckInfo;

        /// <summary>
        /// セットアップ
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            deckInfo = new DeckInfomation();
            handCountInstance = new(deckInfo);
        }

        /// <summary>
        /// 山札を増やす
        /// </summary>
        /// <param name="count">増やす枚数</param>
        private void AddDrawPileCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 中身は見ないのでハリボテ
                var card = new CardData();

                // カードを追加
                deckInfo.AddCard(card);
            }

            // デッキを遊べる状況（バトルできる）にセットアップ
            deckInfo.ResetPlayedDeck();
        }

        /// <summary>
        /// 手札を増やす
        /// </summary>
        /// <param name="count">増やす枚数</param>
        private void AddHandCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // カードを追加
                deckInfo.DrawCard();
            }
        }

        /// <summary>
        /// 手札の枚数が確認できる
        /// </summary>
        /// <param name="handCount">手札枚数</param>
        [Test]
        public void ReadableHandCount(
            [Values(0, 1, 5, 7)] int handCount) // 実行すると山札・手札・捨札の枚数がそれぞれ変わるように調整
        {
            // まず10枚デッキ（山札）にカードを追加する
            AddDrawPileCount(10);

            // 三枚引いて捨てる
            AddHandCount(3);
            deckInfo.DiscardHandCardAll();

            // 手札を増やす
            AddHandCount(handCount);

            // 手札にあるカードの枚数を返すことを確認（山札の枚数とも捨札の枚数とも一致しない）
            Assert.That(handCountInstance.EvaluateToValue().AsT0, Is.EqualTo(handCount));

            // コピー作成
            var copyHandCount = handCountInstance.Spawn();

            // 別のインスタンスであることを確認
            Assert.That(copyHandCount, Is.Not.SameAs(handCountInstance));
            // 複製しても結果が変わらないことを確認する
            Assert.That(copyHandCount.EvaluateToValue().AsT0, Is.EqualTo(handCount));
        }

        /// <summary>
        /// 手札の枚数が変動しても変化した値で出力されることを確認
        /// </summary>
        /// <param name="handCount">手札枚数</param>
        [Test]
        public void ChangeHandCount(
            [Values(0, 1, 3, 5)] int handCount) // 実行すると山札・手札・捨札の枚数がそれぞれ変わるように調整
        {
            // まず15枚デッキ（山札）にカードを追加する
            AddDrawPileCount(15);

            // 3枚引いて捨てる
            AddHandCount(3);
            deckInfo.DiscardHandCardAll();

            // 4枚引く
            int firstHandCount = 4;
            AddHandCount(firstHandCount);

            // 手札にあるカードの枚数を返すことを確認（山札の枚数とも捨札の枚数とも一致しない）
            Assert.That(handCountInstance.EvaluateToValue().AsT0, Is.EqualTo(firstHandCount));

            // 手札を増やす
            AddHandCount(handCount);

            // 先ほど見た手札の枚数より手札を増やした分だけ結果も増加することを確認
            Assert.That(handCountInstance.EvaluateToValue().AsT0, Is.EqualTo(firstHandCount + handCount));

            // 手札を全部捨てる（手札0枚）
            deckInfo.DiscardHandCardAll();

            // 手札は0枚と出力されることを確認
            Assert.That(handCountInstance.EvaluateToValue().AsT0, Is.Zero);
        }
    }
}
