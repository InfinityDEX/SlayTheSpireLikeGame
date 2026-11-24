using System.Collections.Generic;
using UnityEngine;
using VContainer;
using AudioComponent;
using VisualEffectsComponent;

namespace CreaturesComponent
{
    /// <summary>
    /// エネミーマネージャ
    /// 
    /// 現在のバトルに参加している全てのエネミーを管理する
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [field:SerializeField, Header("現在のバトルに参加しているエネミーリスト")]
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

        /// <summary>
        /// オーディオコントローラー
        /// </summary>
        [Inject]
        private AudioController audioController;

        /// <summary>
        /// ダメージポップアップジェネレーター
        /// </summary>
        [Inject]
        private DamagePopupGenerator damagePopupGenerator;

        /// <summary>
        /// プレイヤーマネージャー
        /// </summary>
        [Inject]
        private PlayerManager playerManager;

        /// <summary>
        /// ビジュアルエフェクトライブラリ
        /// </summary>
        [Inject]
        private VisualEffectLibrary visualEffectLibrary;

        /// <summary>
        /// エネミーの追加
        /// </summary>
        /// <param name="enemyCombinationData">エネミーコンビネーションデータ（バトルに参加させる全エネミーの情報）</param>
        public void SetEnemy(EnemyCombinationData enemyCombinationData)
        {
            foreach (var enemyData in enemyCombinationData.enemyCombinations)
            {
                var enemy = Instantiate(enemyData.enemyPrefab, this.transform);
                enemy.Init(audioController, damagePopupGenerator, playerManager, this, visualEffectLibrary);
                enemy.transform.position = enemyData.spawnPosition;
                Enemies.Add(enemy);
            }
        }
    }
}
