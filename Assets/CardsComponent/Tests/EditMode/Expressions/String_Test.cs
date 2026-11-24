using CardEffect;
using CardEffect.Expressions;
using NUnit.Framework;
using OneOf;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode.Expressions
{
    /// <summary>
    /// Stringクラスのテスト定義
    /// </summary>
    public class String_Test
    {
        /// <summary>
        /// スポーン（コピー）したインスタンスが、値までコピーしているか確認するテスト
        /// </summary>
        /// <param name="str">Stringに登録する値</param>
        [Test]
        public void SpawnInstanceValueCheck(
            [Values("HogeHoge", "HUgaHuga", "BuzzBuZz", "くぁｗせｄｒｆｔｇｙふじこｌｐ", "./:.,=;.", "12312108.0123f")]string str)
        {
            var baseStringInstance = new String(str);

            // 設定した文字列と同じ文字列が返ることを確認
            Assert.That(baseStringInstance.EvaluateToValue().AsT1, Is.EqualTo(str));
            
            // プリミティブ値であることを確認
            Assert.That(baseStringInstance.IsLeaf, Is.True);

            // コピー作成
            var copyString = baseStringInstance.Spawn();

            // 別のインスタンスであることを確認
            Assert.That(copyString, Is.Not.SameAs(baseStringInstance));
            // 複製しても結果が変わらないことを確認
            Assert.That(copyString.EvaluateToValue().AsT1, Is.EqualTo(str));
        }

        /// <summary>
        /// 数値が渡されたら例外になることを確認
        /// </summary>
        /// <param name="num">Stringに登録する値</param>
        [Test]
        public void SetNumberThrows(
            [Values(0, 2, 4.5f, 541.45f)] float num)
        {
            var baseStringInstance = new String("");
            // 数値を設定する
            baseStringInstance.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> { num });

            // InvalidOperationException例外が吐き出されることを確認
            Assert.Throws<System.InvalidOperationException>(() => baseStringInstance.EvaluateToValue());
        }
    }
}