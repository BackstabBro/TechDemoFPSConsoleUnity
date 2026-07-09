using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class RootBootStrapper : IInitializable, IStartable, IDisposable
{
    private readonly GameData _gameData;
    private readonly GlobalParameters _globalData;
    private readonly PlayerParameters _playerData;
    private readonly TimeParameters _timeData;
    private readonly GameTasksRepository _gameTasksData;
    private readonly CSVConfigs _CSVConfigs;
    private readonly InputService _inputService;
    private readonly GameStateService _gameStateService;
    private readonly IReadOnlyList<IGameState> _gameStates;
    private readonly ConsoleCommandService _consoleCommandService;
    private readonly IReadOnlyList<IConsoleCommand> _consoleCommands;
    private readonly ConsoleView _consoleView;
    private readonly ConsoleAutocomplete _consoleAutocomplete;
    private readonly SaveLoadService _saveLoadService;

    public RootBootStrapper(
        GameData gameData,
        PlayerParameters playerData,
        GlobalParameters globalData,
        TimeParameters timeData,        
        CSVConfigs gameConfigsRegistry,
        GameStateService gameStateService,
        IReadOnlyList<IGameState> gameStates,
        ConsoleCommandService consoleCommandService,
        IReadOnlyList<IConsoleCommand> consoleCommands,
        ConsoleView consoleView,
        ConsoleAutocomplete consoleAutocomplete,
        InputService inputService,
        SaveLoadService saveLoadService,
        GameTasksRepository gameTasksData)
    {
        _gameData = gameData;
        _globalData = globalData;
        _playerData = playerData;
        _timeData = timeData;
        _CSVConfigs = gameConfigsRegistry;
        _gameStateService = gameStateService;
        _gameStates = gameStates;

        _consoleCommandService = consoleCommandService;
        _consoleView = consoleView;
        _consoleAutocomplete = consoleAutocomplete;
        _consoleCommands = consoleCommands;
        _saveLoadService = saveLoadService;
        _gameTasksData = gameTasksData;

        _inputService = inputService;
    }




    public void Initialize()
    {
        _inputService.Init();
        _gameData.Init();
        _consoleCommandService.Init();

        _saveLoadService.LoadSettings();

        _CSVConfigs.Startup();
        _gameTasksData.SetDefaultValues();
        _saveLoadService.Init();


        Debug.Log($"[{GetType().Name}] init complete");
    }

    public void Start()
    {
        _consoleView.Init();
        _consoleAutocomplete.Init();
        _consoleView.Startup();

        Debug.Log($"[{GetType().Name}] start complete");
    }

    public void Dispose()
    {
        _consoleView.CleanUp();

        Debug.Log($"[{GetType().Name}] dispose complete");
    }
}
