using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StoreSceneLifetimeScope : LifetimeScope
{
    [field: SerializeField, Header("カード販売マネージャー")]
    private SalesCardManager _salesCardManager;

    [field: SerializeField, Header("カードライブラリ")]
    private CardDataLibrary _scardDataLibrary;

    [field: SerializeField, Header("デッキマネージャー")]
    private DeckManager _deckManager;
    protected override void Configure(IContainerBuilder builder)
    {
        // Monobehaviourの登録
        builder.RegisterComponent(_salesCardManager);
        builder.RegisterComponent(_scardDataLibrary);
        builder.RegisterComponent(_deckManager);
    }
}
