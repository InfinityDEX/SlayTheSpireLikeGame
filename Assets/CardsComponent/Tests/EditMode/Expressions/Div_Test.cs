using CardEffect;
using CardEffect.Expressions;
using NUnit.Framework;
using OneOf;
using System;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// 除算カード式クラスのテスト定義
    /// </summary>
    public class Div_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private Div _divInstance;

        /// <summary>
        /// テストセットアップ
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _divInstance = new Div();
        }

        /// <summary>
        /// 除算式テスト
        /// </summary>
        [Test]
        public void DivEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftType,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightType)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftType, 10.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightType, 2.0f);

            // 除算式クラスに引数をセット
            _divInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { leftValue, rightValue });
            // 除算式クラスの計算結果が正しいかを検証
            Assert.AreEqual(5.0f, _divInstance.EvaluateToValue().AsT0);
        }

        /// <summary>
        /// ゼロを割る処理のテスト
        /// </summary>
        [Test]
        public void DivZeroByXEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftType,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightType)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftType, 0.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightType, 2.0f);

            // 除算式クラスに引数をセット
            _divInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { leftValue, rightValue });
            // 除算式クラスの計算結果が正しいかを検証
            Assert.AreEqual(0.0f, _divInstance.EvaluateToValue().AsT0);
        }

        /// <summary>
        /// ゼロ除算テスト
        /// </summary>
        [Test]
        public void DivXByZeroEvaluate(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType leftType,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType rightType)
        {
            var leftValue = CardEffectTestHelper.GenerateArgs(leftType, 10.0f);
            var rightValue = CardEffectTestHelper.GenerateArgs(rightType, 0.0f);

            // 除算式クラスに引数をセット
            _divInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { leftValue, rightValue });
            // ゼロ除算を行ったとき、DivideByZeroExceptionが投げられるか？
            Assert.Throws<DivideByZeroException>(() => _divInstance.EvaluateToValue());
        }
    }
}