using UnityEngine;
using VContainer;
using VContainer.Unity;
using AudioComponent;
using BattleComponent;
using CardsComponent;
using CreaturesComponent;
using GameHUDComponent;
using VisualEffectsComponent;

namespace DIScope
{
    public class BattleSceneScope : LifetimeScope
    {
        [field: SerializeField, Header("バトルマネージャー")]
        private BattleManager battleManager;

        [field: SerializeField, Header("エネミーマネージャー")]
        private EnemyManager enemyManager;

        [field: SerializeField, Header("カードドラッグコントローラー")]
        private CardDragHandler cardDragHandler;

        [field: SerializeField, Header("オーディオコントローラー")]
        private AudioController audioController;

        [field: SerializeField, Header("バトルリワードマネージャー")]
        private BattleRewardManager battleRewardManager;

        [field: SerializeField, Header("カードデータライブラリ")]
        private CardDataLibrary cardDataLibrary;

        [field: SerializeField, Header("デッキマネージャー")]
        private DeckManager deckManager;

        [field: SerializeField, Header("エナジーマネージャー")]
        private EnergyManager energyManager;

        [field: SerializeField, Header("ビジュアルエフェクトライブラリ")]
        public VisualEffectLibrary VisualEffectLibrary { get; private set; }

        [field: SerializeField, Header("ダメージポップアップジェネレーター")]
        public DamagePopupGenerator DamagePopupGenerator { get; private set; }

        [field: SerializeField, Header("HUD 体力バーコントローラー")]
        private HUDHealthBarController hUDHealthBarController;

        [field: SerializeField, Header("HUD コインカウンター")]
        private HUDCoinCounter hUDCoinCounter;

        [field: SerializeField, Header("HUD フロアカウンター")]
        private HUDFloorNumberCounter hUDFloorNumberCounter;

        [field: SerializeField, Header("山札カウンター")]
        private DrawPailGUI drawCard;

        [field: SerializeField, Header("捨て札カウンター")]
        private DiscardPailGUI discardCard;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerManager>(Lifetime.Singleton);
            builder.Register<DeckInfomation>(Lifetime.Singleton);
            builder.Register<CardActionDataFactory>(Lifetime.Singleton).As<ICardDataFactory>();
            builder.RegisterComponent(battleManager);
            builder.RegisterComponent(enemyManager);
            builder.RegisterComponent(cardDragHandler);
            builder.RegisterComponent(audioController);
            builder.RegisterComponent(battleRewardManager);
            builder.RegisterComponent(cardDataLibrary);
            builder.RegisterComponent(deckManager);
            builder.RegisterComponent(energyManager);
            builder.RegisterComponent(VisualEffectLibrary);
            builder.RegisterComponent(DamagePopupGenerator);
            builder.RegisterComponent(hUDHealthBarController);
            builder.RegisterComponent(hUDCoinCounter);
            builder.RegisterComponent(hUDFloorNumberCounter);
            builder.RegisterComponent(drawCard);
            builder.RegisterComponent(discardCard);
        }
    }   
}
