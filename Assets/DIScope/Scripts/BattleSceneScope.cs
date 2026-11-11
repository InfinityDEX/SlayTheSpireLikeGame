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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<BattleManager>(battleManager);
        builder.RegisterComponent<EnemyManager>(enemyManager);
        builder.RegisterComponent<CardDragHandler>(cardDragHandler);
        builder.RegisterComponent<AudioController>(audioController);
    }
}
