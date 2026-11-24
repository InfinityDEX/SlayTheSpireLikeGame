using UnityEngine;

namespace CreaturesComponent
{
    /// <summary>
    /// エネミー組み合わせデータ管理クラス
    /// 
    /// このクラスが持つエネミー組み合わせデータを基に
    /// バトルシーンにエネミー群を生成する。
    /// シングルトン
    /// </summary>
    public class EnemyCombinationsDataManager : MonoBehaviour
    {
        /// <summary>
        /// エネミー組み合わせデータ
        /// </summary>
        private EnemyCombinationData enemyCombinationData;

        /// <summary>
        /// エネミー組み合わせデータのセット
        /// </summary>
        /// <param name="data">セットするデータ</param>
        public void SetData(EnemyCombinationData data)
        {
            enemyCombinationData = data;
        }

        /// <summary>
        /// エネミー組み合わせデータの取得
        /// </summary>
        /// <returns>エネミー組み合わせデータ</returns>
        public EnemyCombinationData GetData()
        {
            return enemyCombinationData;
        }
    }
}
