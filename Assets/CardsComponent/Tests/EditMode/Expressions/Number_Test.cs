using CardEffect.Expressions;
using NUnit.Framework;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// Numberクラスのテスト定義
    /// </summary>
    public class Number_Test
    {
        /// <summary>
        /// スポーン（コピー）したインスタンスが、値までコピーしているか確認するテスト
        /// </summary>
        /// <param name="num">Numberに登録する値</param>
        /// <returns>複製したNumberの値</returns>
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(6, ExpectedResult = 6)]
        [TestCase(-6, ExpectedResult = -6)]
        [TestCase(50, ExpectedResult = 50)]
        [TestCase(2.3f, ExpectedResult = 2.3f)]
        public float SpawnInstanceValueCheck(float num)
        {
            var baseNumberInstance = new Number(num);
            var copyNumberInstance = baseNumberInstance.Spawn();

            return copyNumberInstance.EvaluateToValue().AsT0;
        }
    }
}