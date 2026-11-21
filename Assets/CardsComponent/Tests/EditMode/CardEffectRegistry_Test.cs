using CardEffect;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CardsComponent.Tests.EditMode
{
    public class CardEffectRegistry_Test
    {
        /// <summary>
        /// レジストリを要素を登録した状態で生成する処理
        /// </summary>
        /// <param name="bodies">登録する要素本体</param>
        /// <param name="ids">登録する要素ID</param>
        /// <returns>作成したレジストリ</returns>
        /// <typeparam name="T">登録するクラスの種別</typeparam>
        private CardEffectRegistry<T> GenerateRegistry<T>(List<T> bodies, List<int> ids) where T : class, IPrototype<T>
        {
            CardEffectRegistry<T> result = new();

            for (int i = 0; i < bodies.Count; i++)
            {
                result.Register(ids[i], bodies[i]);
            }

            return result;
        }


        /// <summary>
        /// 重複したIDもインスタンスクラスも無ければ登録できることを確認するテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void NonDuplicationRegistration(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType)
        {
            int entriesCount = 0;
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    var cardValueComponentRegistry = 
                        GenerateRegistry(new List<ValueExpressionComponent> { new SimpleNum(0), new SimpleNum2(0) }, new List<int> { 0, 1 });

                    entriesCount = cardValueComponentRegistry.EntriesCount;
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    var cardActionComponentRegistry = 
                        GenerateRegistry(new List<CardActionComponent> { new SimpleAction(0), new SimpleAction2(0) }, new List<int> { 0, 1 });

                    entriesCount = cardActionComponentRegistry.EntriesCount;
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }

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
                    ex = Assert.Throws<ArgumentNullException>(() =>
                        GenerateRegistry(new List<ValueExpressionComponent> { null }, new List<int> { 0 })
                    );
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    ex = Assert.Throws<ArgumentNullException>(() =>
                        GenerateRegistry(new List<CardActionComponent> { null }, new List<int> { 0 })
                    );
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
                    var valueExpressionRegistry = GenerateRegistry(new List<ValueExpressionComponent> { new SimpleNum(0) }, new List<int> { id });
                    ex = Assert.Throws<ArgumentException>(() => valueExpressionRegistry.Register(id, new SimpleNum2(0)));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    var cardActionRegistry = GenerateRegistry(new List<CardActionComponent> { new SimpleAction(0) }, new List<int> { id });
                    ex = Assert.Throws<ArgumentException>(() => cardActionRegistry.Register(id, new SimpleAction2(0)));
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }

            // エラーメッセージに、登録しようとした要素IDが記載されていることを確認
            Assert.That(ex.Message, Does.Contain($"[{id}]"));
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
                    registerInstanceType = typeof(SimpleNum);
                    var valueExpressionRegistry = GenerateRegistry(new List<ValueExpressionComponent> { new SimpleNum(0) }, new List<int> { 0 });
                    ex = Assert.Throws<ArgumentException>(() => valueExpressionRegistry.Register(1, new SimpleNum(0)));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    registerInstanceType = typeof(SimpleAction);
                    var cardActionRegistry = GenerateRegistry(new List<CardActionComponent> { new SimpleAction(0) }, new List<int> { 0 });
                    ex = Assert.Throws<ArgumentException>(() => cardActionRegistry.Register(1, new SimpleAction(0)));
                    break;
                default: 
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }

            // エラーメッセージに、登録しようとしたインスタンスのクラス名が記載されていることを確認
            Assert.That(ex.Message, Does.Contain(registerInstanceType.Name));
        }

        /// <summary>
        /// レジストリに登録された要素を取得できるかどうかのテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        /// <param name="checkRegistryId">確認する要素のID</param>
        [Test]
        public void GetInstanceFromRegistry(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType,
            [Values(-1, 0, 1, 2)] int checkRegistryId)
        {
            Type getEntryType = null;
            var valueExpressionComponents = new List<ValueExpressionComponent> { new SimpleNum(0), new SimpleNum2(0) };
            var cardActionComponents = new List<CardActionComponent> { new SimpleAction(0), new SimpleAction2(0) };

            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    var cardValueComponentRegistry = GenerateRegistry(valueExpressionComponents, new List<int> { 0, 1 });
                    Assert.That(
                        cardValueComponentRegistry.TryGet(checkRegistryId, out var searchValueExpressionResult),
                        (0 <= checkRegistryId && checkRegistryId <= 1) ? Is.True : Is.False);
                    getEntryType = searchValueExpressionResult?.GetType();
                    break;

                case CardEffectTestHelper.TestValType.Action:
                    var cardActionComponentRegistry = GenerateRegistry(cardActionComponents, new List<int> { 0, 1 });
                    Assert.That(
                        cardActionComponentRegistry.TryGet(checkRegistryId, out var searchCardActionResult),
                        (0 <= checkRegistryId && checkRegistryId <= 1) ? Is.True : Is.False);
                    getEntryType = searchCardActionResult?.GetType();
                    break;

                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }
            // 登録した要素のID以外をcheckRegistryIdに指定した場合はnullを返す
            if (checkRegistryId < 0 || 1 < checkRegistryId)
                Assert.That(getEntryType, Is.Null);
            else
            {
                // 登録した要素のID指定したなら、生成されたインスタンスと型が一致しているインスタンスが返されることを確認する。
                switch (registeredInstanceType)
                {
                    case CardEffectTestHelper.TestValType.Value:
                        Assert.That(getEntryType, Is.EqualTo(valueExpressionComponents[checkRegistryId].GetType()));
                        break;
                    case CardEffectTestHelper.TestValType.Action:
                        Assert.That(getEntryType, Is.EqualTo(cardActionComponents[checkRegistryId].GetType()));
                        break;
                    default:
                        Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                        break;
                }
            }
        }

        /// <summary>
        /// レジストリに登録された要素を空だった時に、要素取得メソッドを実行したらnullが返るかどうかのテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        /// <param name="checkRegistryId">確認する要素のID</param>
        [Test]
        public void GetInstanceFromEmptyRegistry(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType,
            [Values(-1, 0, 1, 2)] int checkRegistryId)

        {
            Type getEntryType = null;
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    var cardValueComponentRegistry = GenerateRegistry(new List<ValueExpressionComponent>(), new List<int>());
                    Assert.That(cardValueComponentRegistry.TryGet(checkRegistryId, out var searchValueExpressionResult), Is.False);
                    getEntryType = searchValueExpressionResult?.GetType();
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    var cardActionComponentRegistry = GenerateRegistry(new List<CardActionComponent>(), new List<int>());
                    Assert.That(cardActionComponentRegistry.TryGet(checkRegistryId, out var searchCardActionResult), Is.False);
                    getEntryType = searchCardActionResult?.GetType();
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }

            // 「レジストリを空の状態で生成するテスト設定だった」か、「取得しようとした要素のIDが登録されていないID」なら
            // 取得結果はNull
            Assert.That(getEntryType, Is.Null);
        }

        /// <summary>
        /// レジストリから取得したインスタンスが、レジストリ内に登録されている本体ではなく
        /// 新規に生成された複製が取得されていることを確認するテスト
        /// </summary>
        /// <param name="registeredInstanceType">登録するインスタンスの型種別</param>
        [Test]
        public void TryGetReturnsCopiedInstance(
            [Values(CardEffectTestHelper.TestValType.Value, CardEffectTestHelper.TestValType.Action)] CardEffectTestHelper.TestValType registeredInstanceType,
            [Values(1, -1, 0, 30, 23.4f)] float componentValue)
        {
            var valueExpressionComponents = new List<ValueExpressionComponent> { new SimpleNum(componentValue) };
            var cardActionComponents = new List<CardActionComponent> { new SimpleAction(componentValue) };
            switch (registeredInstanceType)
            {
                case CardEffectTestHelper.TestValType.Value:
                    var cardValueComponentRegistry = GenerateRegistry(valueExpressionComponents, new List<int> { 0 });
                    Assert.That(cardValueComponentRegistry.TryGet(0, out var searchValueExpressionResult), Is.True);
                    Assert.That(searchValueExpressionResult, Is.Not.Null);

                    // レジストリに登録していたインスタンスそのものを渡しているわけではないことを確認
                    Assert.That(searchValueExpressionResult, Is.Not.SameAs(valueExpressionComponents[0]));
                    // ただし実行時の結果は同じ
                    Assert.That(searchValueExpressionResult.EvaluateToValue(), Is.EqualTo(valueExpressionComponents[0].EvaluateToValue()));
                    break;
                case CardEffectTestHelper.TestValType.Action:
                    var cardActionComponentRegistry = GenerateRegistry(cardActionComponents, new List<int> { 0 });
                    Assert.That(cardActionComponentRegistry.TryGet(0, out var searchCardActionResult), Is.True);
                    Assert.That(searchCardActionResult, Is.Not.Null);

                    // レジストリに登録していたインスタンスそのものを渡しているわけではないことを確認
                    Assert.That(searchCardActionResult, Is.Not.SameAs(cardActionComponents[0]));
                    // ただし実行時の結果は同じ
                    Assert.That(searchCardActionResult.Execute(null).EvaluateToValue(), Is.EqualTo(cardActionComponents[0].Execute(null).EvaluateToValue()));
                    break;
                default:
                    Assert.Fail($"想定外のTestValType:{registeredInstanceType}");
                    break;
            }
        }
    }
}
