using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootDI : LifetimeScope
{
    private InputSystem_Actions _actions;
    [SerializeField] private PlayerAudioData _playerAudioData;
    [SerializeField] private CSVConfigs _configsRegistry;

    protected override void Configure(IContainerBuilder builder)
    {
        _actions = new InputSystem_Actions();
        builder.RegisterInstance(_actions);
        builder.RegisterInstance(_configsRegistry);
        builder.RegisterInstance(_playerAudioData);

        builder.Register<SaveLoadService>(Lifetime.Singleton);

        builder.Register<GameData>(Lifetime.Singleton);        
        builder.Register<GlobalParameters>(Lifetime.Singleton);
        builder.Register<PlayerParameters>(Lifetime.Singleton);
        builder.Register<TimeParameters>(Lifetime.Singleton);
        builder.Register<GameTasksRepository>(Lifetime.Singleton);
        

        builder.Register<InputService>(Lifetime.Singleton);

        builder.Register<GameStateService>(Lifetime.Singleton);
        builder.Register<IGameState, GameplayState>(Lifetime.Singleton);
        builder.Register<IGameState, PauseState>(Lifetime.Singleton);
        builder.Register<IGameState, ConsoleState>(Lifetime.Singleton);
        builder.Register<IGameState, DialogueState>(Lifetime.Singleton);
        builder.Register<IGameState, MenuState>(Lifetime.Singleton);

        //Console
        builder.Register<ConsoleCommandService>(Lifetime.Singleton);
        builder.Register<IConsoleCommand, Set_Command>(Lifetime.Singleton);
        builder.Register<IConsoleCommand, Get_Command>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<ConsoleView>();
        builder.RegisterComponentInHierarchy<ConsoleAutocomplete>();

        //Entry point
        builder.RegisterEntryPoint<RootBootStrapper>();
        Debug.Log($"[{GetType().Name}] configure complete");
    }
}
