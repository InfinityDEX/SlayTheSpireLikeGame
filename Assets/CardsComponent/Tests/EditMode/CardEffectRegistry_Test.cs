using CardEffect;
using NUnit.Framework;
using System;

namespace CardsComponent.Tests.EditMode
{
    public class CardEffectRegistry_Test
    {
        /// <summary>
        /// 重複したIDもインスタンスクラスも無ければ登録できることを確認するテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void NonDuplicationRegistration(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType)
        {
            int entriesCount = 0;
            Assert.DoesNotThrow(() =>
                {
                    switch (registeredInstanceType)
                    {
                        case CardEffectTestHelper.TestValType.Value:
                            CardEffectRegistry<ValueExpressionComponent> cardValueComponentRegistry = new();
                            cardValueComponentRegistry.Register(1, new SimpleNum(0));
                            cardValueComponentRegistry.Register(2, new SimpleNum2(0));

                            entriesCount = cardValueComponentRegistry.EntriesCount;
                            break;
                        case CardEffectTestHelper.TestValType.Action:
                            CardEffectRegistry<CardActionComponent> cardActionComponentRegistry = new();
                            cardActionComponentRegistry.Register(1, new SimpleAction(0));
                            cardActionComponentRegistry.Register(2, new SimpleAction2(0));

                            entriesCount = cardActionComponentRegistry.EntriesCount;
                            break;
                        default:
                            Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                            break;
                    }
                });

            Assert.That(entriesCount, Is.EqualTo(2));
        }

        /// <summary>
        /// Nullを登録しようとしたらNullエラーが出ることを確認するテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void NullRegistration(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType)
        {
            ArgumentNullException ex = null;
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    CardEffectRegistry<ValueExpressionComponent> cardValueComponentRegistry = new();
                    ex = Assert.Throws<ArgumentNullException>(() => cardValueComponentRegistry.Register(1, null));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    CardEffectRegistry<CardActionComponent> cardActionComponentRegistry = new();
                    ex = Assert.Throws<ArgumentNullException>(() => cardActionComponentRegistry.Register(1, null));
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }
            // エラーパラメータ（Nullだった要素）としてinstanceが出力されていることを確認
            Assert.That(ex.ParamName, Is.EqualTo("instance"));
            // エラーメッセージにnullが記載されていることを確認
            Assert.That(ex.Message, Does.Contain("null"));
        }

        /// <summary>
        /// 重複するIDを登録しようとするとエラーになることを確認するテスト
        /// </summary>
        /// <param name="id">登録するID</param>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void DuplicationIDRegistration(
            [Values(1, -3, 0, 99)] int id,
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType)
        {
            ArgumentException ex = null;
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    CardEffectRegistry<ValueExpressionComponent> cardValueComponentRegistry = new();
                    cardValueComponentRegistry.Register(id, new SimpleNum(0));
                    ex = Assert.Throws<ArgumentException>(() => cardValueComponentRegistry.Register(id, new SimpleNum2(0)));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    CardEffectRegistry<CardActionComponent> cardActionComponentRegistry = new();
                    cardActionComponentRegistry.Register(id, new SimpleAction(0));
                    ex = Assert.Throws<ArgumentException>(() => cardActionComponentRegistry.Register(id, new SimpleAction2(0)));
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }
            // エラーメッセージに登録しようとしたIDが記載されていることを確認
            Assert.That(ex.Message, Does.Contain(id.ToString()));
        }

        /// <summary>
        /// 重複するインスタンスを登録しようとするとエラーになることを確認するテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void DuplicationInstanceRegistration(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType)
        {
            ArgumentException ex = null;
            Type registerInstanceType = null;
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    CardEffectRegistry<ValueExpressionComponent> cardValueComponentRegistry = new();
                    cardValueComponentRegistry.Register(1, new SimpleNum(0));

                    registerInstanceType = typeof(SimpleNum);
                    ex = Assert.Throws<ArgumentException>(() => cardValueComponentRegistry.Register(2, new SimpleNum(0)));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    CardEffectRegistry<CardActionComponent> cardActionComponentRegistry = new();
                    cardActionComponentRegistry.Register(1, new SimpleAction(0));

                    registerInstanceType = typeof(SimpleAction);
                    ex = Assert.Throws<ArgumentException>(() => cardActionComponentRegistry.Register(2, new SimpleAction(0)));
                    break;
                default: 
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }

            // エラーメッセージに登録しようとしたインスタンスのクラス名が記載されていることを確認
            Assert.That(ex.Message, Does.Contain(registerInstanceType.Name));
        }
    }
}
