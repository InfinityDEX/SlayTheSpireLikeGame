using System.Collections.Generic;
using CardEffect;
using OneOf;
using CreaturesComponent;

namespace CardsComponent.Tests.EditMode
{
    ////////////////////////////////////
    //// モック
    ////////////////////////////////////

    ////////////////////////////////////


    ////////////////////////////////////
    //// ヘルパー                       
    ////////////////////////////////////

    public class CardEffectTestHelper
    {
        /// <summary>
        /// テストに渡す引数の種別
        /// </summary>
        public enum TestValType
        {
            Value,
            Action
        }

        /// <summary>
        /// テストの引数生成メソッド
        /// </summary>
        /// <param name="type">引数種別</param>
        /// <param name="value">引数の値</param>
        /// <returns></returns>
        public static OneOf<float, string, CardActionComponent, ValueExpressionComponent> GenerateArgs(TestValType type, float value)
        {
            switch (type)
            {
                case TestValType.Value:
                    return new SimpleNum(value);
                case TestValType.Action:
                    return new SimpleAction(value);
                default:
                    throw new System.ArgumentOutOfRangeException();

            }
        }
    }

    ////////////////////////////////////

    ////////////////////////////////////
    //// スタブ                         
    ////////////////////////////////////
    /// <summary>
    /// 単純な数値を定義するカード効果式クラス（スタブ）
    /// 
    /// Number型はJsonデータからの読み込みとに最適化している為、
    /// テストで使いやすいようシンプルなインスタンス生成ができる型をテスト用に定義
    /// </summary>
    public class SimpleNum : ValueExpressionComponent
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num">値</param>
        public SimpleNum(float num)
        {
            Args.Add(num);
        }

        /// <summary>
        /// 計算メソッド（単純にセットした値を返すだけ）
        /// </summary>
        /// <returns>計算結果</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            return Args[0];
        }

        /// <summary>
        /// コンポーネント複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new SimpleNum(Args[0].AsT0);
        }
    }

    /// <summary>
    /// 単純な数値を定義するカード効果式クラス（スタブ）
    /// 
    /// 重複チェック用に2つ目を定義（内容は上記に定義したものと同じ）
    /// </summary>
    public class SimpleNum2 : SimpleNum
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num">値</param>
        public SimpleNum2(float num)
            : base(num) { }


        /// <summary>
        /// コンポーネント複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new SimpleNum2(Args[0].AsT0);
        }
    }

    /// <summary>
    /// SimpleNumを返すシンプルなカードアクションコンポーネントの実装クラス（スタブ）
    /// </summary>
    public class SimpleAction : CardActionComponent
    {
        protected float num;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num">Executeで返すSimpleNumに設定する値</param>
        public SimpleAction(float num)
        {
            this.num = num;
        }

        /// <summary>
        /// 効果実行
        /// </summary>
        /// <param name="target">効果の対象（設定しても影響なし）</param>
        /// <returns>numの値がセットされたSimpleNum</returns>
        public override ValueExpressionComponent Execute(Creature target)
        {
            return new SimpleNum(num);
        }

        /// <summary>
        /// コンポーネント複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override CardActionComponent Spawn()
        {
            return new SimpleAction(num);
        }
    }

    /// <summary>
    /// SimpleNumを返すシンプルなカードアクションコンポーネントの実装クラス（スタブ）
    /// 
    /// 重複チェック用に2つ目を定義（内容は上記に定義したものと同じ）
    /// </summary>
    public class SimpleAction2 : SimpleAction
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num">Executeで返すSimpleNumに設定する値</param>
        public SimpleAction2(float num)
            : base(num){}

        /// <summary>
        /// コンポーネント複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override CardActionComponent Spawn()
        {
            return new SimpleAction2(num);
        }
    }

    /// <summary>
    /// テスト用のカードデータを生成するファクトリー（スタブ）
    ///
    /// Jsonファイルを読まずに、テストが前提とするカード構成を返す
    /// </summary>
    public class SimpleCardDataFactory : ICardDataFactory
    {
        /// <summary>
        /// 未強化カードのID（強化先: UpgradedCardId）
        /// </summary>
        public const int CardId = 1;

        /// <summary>
        /// CardIdの強化後カードのID
        /// </summary>
        public const int UpgradedCardId = 101;

        /// <summary>
        /// 別の未強化カードのID（強化先: OtherUpgradedCardId）
        /// </summary>
        public const int OtherCardId = 2;

        /// <summary>
        /// OtherCardIdの強化後カードのID
        /// </summary>
        public const int OtherUpgradedCardId = 102;

        /// <summary>
        /// ライブラリに存在しないカードのID
        /// </summary>
        public const int MissingCardId = 999;

        /// <summary>
        /// カードデータリストを生成する
        ///
        /// CardDataは可変なので、テスト間で影響しないように毎回新しいインスタンスを返す
        /// </summary>
        /// <returns>カードデータリスト</returns>
        public List<CardData> CreateCardDataList()
        {
            return new List<CardData>
            {
                new CardData { CardID = CardId, CardName = "Card", UpgradedCardId = UpgradedCardId, IsUpgraded = false },
                new CardData { CardID = UpgradedCardId, CardName = "Card+", IsUpgraded = true },
                new CardData { CardID = OtherCardId, CardName = "OtherCard", UpgradedCardId = OtherUpgradedCardId, IsUpgraded = false },
                new CardData { CardID = OtherUpgradedCardId, CardName = "OtherCard+", IsUpgraded = true },
            };
        }
    }

    ////////////////////////////////////
}