using VContainer;
using VContainer.Unity;

public class MainMenuDI : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<MainMenuManager>();
        builder.RegisterComponentInHierarchy<LoadWindowManager>();

        builder.RegisterEntryPoint<MainMenuBootstrapper>();
    }
}
