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
        [field: SerializeField, Header("オーディオコントローラー")]
        private AudioController audioController;

        [field: SerializeField, Header("バトルマネージャー")]
        private BattleManager battleManager;

        [field: SerializeField, Header("バトルリワードマネージャー")]
        private BattleRewardManager battleRewardManager;

        [field: SerializeField, Header("カードデータライブラリ")]
        private CardDataLibrary cardDataLibrary;

        [field: SerializeField, Header("カードドラッグコントローラー")]
        private CardDragHandler cardDragHandler;

        [field: SerializeField, Header("ダメージポップアップジェネレーター")]
        private DamagePopupGenerator damagePopupGenerator;

        [field: SerializeField, Header("デッキマネージャー")]
        private DeckManager deckManager;

        [field: SerializeField, Header("捨て札カウンター")]
        private DiscardPailGUI discardCard;

        [field: SerializeField, Header("山札カウンター")]
        private DrawPailGUI drawCard;

        [field: SerializeField, Header("エネミーマネージャー")]
        private EnemyManager enemyManager;

        [field: SerializeField, Header("エナジーマネージャー")]
        private EnergyManager energyManager;

        [field: SerializeField, Header("手札の見た目制御コンポーネント")]
        private HandView handView;

        [field: SerializeField, Header("HUD コインカウンター")]
        private HUDCoinCounter hUDCoinCounter;

        [field: SerializeField, Header("HUD フロアカウンター")]
        private HUDFloorNumberCounter hUDFloorNumberCounter;

        [field: SerializeField, Header("HUD 体力バーコントローラー")]
        private HUDHealthBarController hUDHealthBarController;

        [field: SerializeField, Header("ビジュアルエフェクトライブラリ")]
        private VisualEffectLibrary visualEffectLibrary;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CardActionDataFactory>(Lifetime.Singleton).As<ICardDataFactory>();
            builder.Register<DeckInfomation>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton);
            builder.RegisterComponent(audioController);
            builder.RegisterComponent(battleManager);
            builder.RegisterComponent(battleRewardManager);
            builder.RegisterComponent(cardDataLibrary);
            builder.RegisterComponent(cardDragHandler);
            builder.RegisterComponent(damagePopupGenerator);
            builder.RegisterComponent(deckManager);
            builder.RegisterComponent(discardCard);
            builder.RegisterComponent(drawCard);
            builder.RegisterComponent(enemyManager);
            builder.RegisterComponent(energyManager);
            builder.RegisterComponent(handView);
            builder.RegisterComponent(hUDCoinCounter);
            builder.RegisterComponent(hUDFloorNumberCounter);
            builder.RegisterComponent(hUDHealthBarController);
            builder.RegisterComponent(visualEffectLibrary);
        }
    }   
}
