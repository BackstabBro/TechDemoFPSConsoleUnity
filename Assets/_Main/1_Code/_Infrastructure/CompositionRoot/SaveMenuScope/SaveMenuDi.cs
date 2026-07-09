using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SaveMenuDi : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<SaveMenuController>();

        builder.RegisterEntryPoint<SaveMenuBootstrap>();
    }
}
