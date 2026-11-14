using UnityEngine;
using VContainer;
using VContainer.Unity;

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
        builder.RegisterComponent<BattleManager>(battleManager);
        builder.RegisterComponent<EnemyManager>(enemyManager);
        builder.RegisterComponent<CardDragHandler>(cardDragHandler);
        builder.RegisterComponent<AudioController>(audioController);
        builder.RegisterComponent<BattleRewardManager>(battleRewardManager);
        builder.RegisterComponent<HUDHealthBarController>(hUDHealthBarController);
        builder.RegisterComponent<HUDCoinCounter>(hUDCoinCounter);
        builder.RegisterComponent<HUDFloorNumberCounter>(hUDFloorNumberCounter);
        builder.RegisterComponent<DrawPailGUI>(drawCard);
        builder.RegisterComponent<DiscardPailGUI>(discardCard);
    }
}   
