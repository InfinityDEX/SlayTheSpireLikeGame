using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MapSceneScope : LifetimeScope
{

    [field: SerializeField, Header("ダンジョンマップマネージャー")]
    private DungeonMapManager mapManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<DungeonMapManager>(mapManager);
    }
}
