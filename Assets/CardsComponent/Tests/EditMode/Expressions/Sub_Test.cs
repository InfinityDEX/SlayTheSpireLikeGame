using CardEffect;
using CardEffect.Expressions;
using NUnit.Framework;
using OneOf;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// 減算カード式クラスのテスト定義
    /// </summary>
    public class Sub_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private Sub _subInstance;

        /// <summary>
        /// テストセットアップ
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _subInstance = new Sub();
        }

        /// <summary>
        /// 減算式テスト
        /// </summary>
        [Test]
        public void SubEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftType,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightType)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftType, 3.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightType, 1.0f);

            // 減算式クラスに引数をセット
            _subInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { leftValue, rightValue });
            // 減算式クラスの計算結果が正しいかを検証
            Assert.AreEqual(2.0f, _subInstance.EvaluateToValue().AsT0);
        }
    }
}