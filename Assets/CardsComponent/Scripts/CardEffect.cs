using System;
using System.Collections.Generic;
using OneOf;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace CardEffect
{
    /// <summary>
    /// ゲームの状態を変化させるコンポーネント
    ///
    /// 本クラスの派生クラスのインスタンスをライブラリクラスで管理し、
    /// Prototype Patternで作成することで、ライブラリクラスに登録した
    /// インスタンスからパラメータを少ししたうえで自己複製できるようにする。
    /// </summary>
    public abstract class CardActionComponent
    {
        /// <summary>
        /// 自己複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public abstract CardActionComponent Spawn();

        /// <summary>
        /// カード処理で使用する引数のリスト。
        /// 扱う値は処理によって異なるので、ValueExpressionかCardActionModuleの共用型(OneOfを使用)を入れられるようにしている。
        /// </summary>
        protected List<OneOf<CardActionComponent, ValueExpressionComponent>> Args { get; private set; } = new();

        /// <summary>
        /// カード処理で使用する引数をセットする
        /// </summary>
        /// <param name="args">引数リスト</param>
        public void SetArgs(List<OneOf<CardActionComponent, ValueExpressionComponent>> args)
        {
            Args = args;
        }

        /// <summary>
        /// 効果の実行
        /// </summary>
        /// <param name="target">カード効果の適用対象</param>
        /// <returns>処理の結果何かしらの結果を出力できるならここで返す(例：手札を全部捨てる効果で、何枚捨てたか？等)</returns>
        public abstract ValueExpressionComponent Execute(Creature target);
    }

    /// <summary>
    /// ゲームの状態から「値」を計算して返すコンポーネントのインターフェース（例：手札の枚数、敵の数、かかっているバフデバフの値等）
    /// </summary>
    public abstract class ValueExpressionComponent
    {
        /// <summary>
        /// 自己複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public abstract ValueExpressionComponent Spawn();

        /// <summary>
        /// このコンポーネントは末端ノードか？(末端ノード：Evaluateを実行しても自分自身の複製を出す(何も計算しない)ノード。NumberとかString等、
        /// それ自身が何かの情報として扱える))
        /// </summary>
        public virtual bool IsLeaf { get { return false;} } // デフォルトはfalse

        /// <summary>
        /// 式の引数を表す変数
        /// </summary>
        protected List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> Args { get; set; } = new();

        /// <summary>
        /// 計算処理
        /// </summary>
        /// <returns>計算結果</returns>
        public abstract OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate();

        /// <summary>
        /// カード処理で使用する引数をセットする
        /// </summary>
        /// <param name="args">引数リスト</param>
        public void SetArgs(List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> args)
        {
            Args = args;
        }

        /// <summary>
        /// 末端ノードまでEvaluateしてfloatまたはstringの値を返す
        /// </summary>
        /// <returns>計算結果(評価結果がfloatまたはstring)</returns>
        public OneOf<float, string> EvaluateToValue()
        {
            OneOf<float, string, CardActionComponent, ValueExpressionComponent> result = this.Evaluate();


            /// プリミティブ型だった場合は値をそのまま返す。
            if (result.IsT0)
            {
                return result.AsT0;
            }
            else if (result.IsT1)
            {
                return result.AsT1;
            }

            if (result.IsT2)
            {
                // TODO:一旦現状のカードデザイン案では使用しないので未定義
                // return result.AsT2.Execute(null).EvaluateToValue();
                throw new System.NotImplementedException("CardActionComponentとしてのEvaluateToValueは未定義です。");
            }

            if (result.IsT3)
            {
                // さらにネストされている場合は再帰的に計算する
                return result.AsT3.EvaluateToValue();
            }


            // ここは通らないはずだが、構文エラーを抑えるために記載
            throw new System.InvalidOperationException("Evaluate did not return a float or string value.");
        }
    }

    /// <summary>
    /// プリミティブ型のそのものを表すValueExpressionComponentの派生クラス
    ///
    /// Number等クラスそのものがプリミティブな値を表すクラスを実装する際はこのクラスから派生して定義
    /// </summary>
    public abstract class PrimitiveValueExpressionComponent : ValueExpressionComponent
    {
        /// <summary>
        /// このコンポーネントは末端ノードか？(末端ノード：Evaluateを実行しても自分自身の複製を出す(何も計算しない)ノード。NumberとかString等、
        /// それ自身が何かの情報として扱える))
        /// </summary>
        public override sealed bool IsLeaf { get { return true;} } // このクラスを継承するクラスはプリミティブ型そのものの定義である為、それ以上は計算不能のはず
    }
}