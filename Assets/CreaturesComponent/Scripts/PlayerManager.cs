using UnityEngine;
using VContainer;
using AudioComponent;
using VisualEffectsComponent;

namespace CreaturesComponent
{
    /// <summary>
    /// プレイヤーマネージャー
    /// </summary>
    public class PlayerManager
    {
        [field: SerializeField, Header("プレイヤー")]
        public Creature Player { get; private set; }

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
        /// ビジュアルエフェクトライブラリ
        /// </summary>
        [Inject]
        private VisualEffectLibrary visualEffectLibrary;

        public void SetPlayer(Creature player)
        {
            Player = player;
            // プレイやーの初期化
            // プレイヤーはカードを通して盤面に干渉する為、プレイヤーマネージャー及びエネミーマネージャーはnullとする。
            // (むしろDIで循環参照となってしまうのでこう記載せざるを得ない)
            Player.Init(audioController, damagePopupGenerator, null, null, visualEffectLibrary);
        }
    }
}
