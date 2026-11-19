using CardEffect;
using CardEffect.Expressions;
using NUnit.Framework;
using OneOf;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// 乗算カード式クラスのテスト定義
    /// </summary>
    public class Mul_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private Mul _mulInstance;

        /// <summary>
        /// テストセットアップ
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mulInstance = new Mul();
        }

        /// <summary>
        /// 乗算式テスト
        /// </summary>
        [Test]
        public void MulEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftType,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightType)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftType, 2.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightType, 3.0f);

            // 乗算式クラスに引数をセット
            _mulInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { leftValue, rightValue });
            // 乗算式クラスの計算結果が正しいかを検証
            Assert.AreEqual(6.0f, _mulInstance.EvaluateToValue().AsT0);
        }
    }
}