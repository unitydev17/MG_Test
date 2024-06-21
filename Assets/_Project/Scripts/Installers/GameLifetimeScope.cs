using CargoMover;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private Cargo _cargoPrefab;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<Gameplay>();
        builder.RegisterComponentInHierarchy<PlayerController>();
        builder.Register<LevelBuilder>(Lifetime.Scoped);

        builder.Register<PhysicCaster>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

        RegisterFactories(builder);
    }

    private void RegisterFactories(IContainerBuilder builder)
    {
        builder.Register<CargoFactory>(Lifetime.Scoped).WithParameter(typeof(Cargo), _cargoPrefab);
    }
}
