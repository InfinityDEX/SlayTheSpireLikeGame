using CardEffect;
using CardEffect.Expressions;
using NUnit.Framework;
using OneOf;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// 加算カード式クラスのテスト定義
    /// </summary>
    public class Add_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private Add _addInstance;

        /// <summary>
        /// テストセットアップ
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _addInstance = new Add();
        }

        /// <summary>
        /// 加算式テスト
        /// </summary>
        [Test]
        public void AddEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftValue_Type,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightValue_Type)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftValue_Type, 1.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightValue_Type, 3.0f);

            // 加算式クラスに引数をセット
            _addInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{ leftValue, rightValue });
            // 加算式クラスの計算結果が正しいかを検証
            Assert.AreEqual(4.0f, _addInstance.EvaluateToValue().AsT0);
        }
    }
}