using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Yarn.Unity;

public class GameplayDI : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {


        builder.Register<DayTimeService>(Lifetime.Singleton);

        builder.Register<DialogueService>(Lifetime.Singleton);

        builder.Register<PlayerService>(Lifetime.Singleton);
        builder.Register<PauseService>(Lifetime.Singleton);
        builder.Register<TaskService>(Lifetime.Singleton);



        builder.RegisterComponentInHierarchy<PlayerView>();
        builder.RegisterComponentInHierarchy<PlayerMovementController>();
        builder.RegisterComponentInHierarchy<PlayerCameraController>();
        builder.RegisterComponentInHierarchy<PlayerInteractController>();
        builder.RegisterComponentInHierarchy<PlayerSoundController>();
        builder.RegisterComponentInHierarchy<HeadBlobber>();

        builder.RegisterComponentInHierarchy<HealthbarUi>();

        InteractSpeaking[] interactables = Object.FindObjectsByType<InteractSpeaking>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        builder.RegisterInstance<IReadOnlyList<InteractSpeaking>>(interactables);

        builder.RegisterBuildCallback(container =>
        {
            foreach (var npc in interactables)
            {
                container.Inject(npc);
            }
        });

        builder.RegisterComponentInHierarchy<InteractSpeaking>();

        builder.RegisterComponentInHierarchy<DialogueRunner>();


        builder.RegisterComponentInHierarchy<Ui_GameTime>();
        builder.RegisterComponentInHierarchy<UiReticle>();
        builder.RegisterComponentInHierarchy<PauseViewController>();


        builder.RegisterComponentInHierarchy<TasksDebugMenu>();


        builder.RegisterComponentInHierarchy<TestPlayerBed>();


        //EntryPoint
        builder.RegisterEntryPoint<GameplayBootStrapper>();
        Debug.Log($"[{GetType().Name}] configure complete");
    }
}
