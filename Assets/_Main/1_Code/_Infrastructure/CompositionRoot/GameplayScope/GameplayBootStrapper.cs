using System;
using UnityEngine;
using VContainer.Unity;
using Yarn.Unity;

public class GameplayBootStrapper : IInitializable, IStartable, ITickable, IDisposable
{
    private readonly InputService _inputService;
    private readonly GameData _gameData;
    private readonly GameStateService _gameStateService;
    private readonly DayTimeService _dayTimeService;
    private readonly PlayerService _playerService;
    private readonly PauseService _pauseService;
    private readonly DialogueService _dialogueService;
    private readonly TaskService _tasksService;
    private readonly PauseViewController _pauseViewController;
    private readonly DialogueRunner _dialogueRunner;
    private readonly PlayerView _playerView;
    private readonly HealthbarUi _healthbarUi;

    private readonly Ui_GameTime _ui_GameTime;
    private readonly TasksDebugMenu _TasksDebugMenu;

    public GameplayBootStrapper(
        InputService inputService,
        GameStateService stateService,
        GameData gameData,
        DayTimeService gameplayTimeService,
        PlayerView playerView,
        HealthbarUi healthbarUi,
        PlayerService playerService,
        PauseService pauseService,
        DialogueService dialogueService,
        DialogueRunner dialogueRunner,
        TaskService tasksService,
        TasksDebugMenu tasksDebugMenu,
        PauseViewController pauseViewController,
        Ui_GameTime ui_GameTime)
    {
        _inputService = inputService;
        _gameData = gameData;
        _gameStateService = stateService;
        _dayTimeService = gameplayTimeService;
        _playerService = playerService;
        _playerView = playerView;
        _healthbarUi = healthbarUi;
        _pauseService = pauseService;
        _dialogueService = dialogueService;
        _dialogueRunner = dialogueRunner;
        _tasksService = tasksService;
        _TasksDebugMenu = tasksDebugMenu;
        _pauseViewController = pauseViewController;
        _ui_GameTime = ui_GameTime;
    }

    public void Initialize()
    {
        _inputService.ClearGameplayInputs();

        //init POCO
        _gameStateService.Init();
        _dayTimeService.Init();
        _tasksService.Init();

        _dialogueService.Init();        
        _playerService.Init();
        _pauseService.Init();


        Debug.Log($"[{GetType().Name}] init complete");
    }

    public void Start()
    {
        //Startup POCO
        _dayTimeService.Startup();
        _tasksService.Startup();


        //init MONO
        _playerView.Init();
        _healthbarUi.Init();
        _ui_GameTime.Init();
        _pauseViewController.Init();
        _TasksDebugMenu.Init();


        //Startup MONO
        _playerView.Startup();
        _healthbarUi.Startup();
        _ui_GameTime.Startup();
        _pauseViewController.Startup();
        _TasksDebugMenu.Startup();

        _gameStateService.Startup();


        _dialogueRunner.gameObject.SetActive(true);
        Debug.Log($"[{GetType().Name}] start complete");
    }




    public void Tick()
    {
        if (_gameStateService.CurrentState == GameStates.Gameplay || _gameStateService.CurrentState == GameStates.Menu)
        {
            _dayTimeService.TickGameTime();
            _tasksService.UpdateActiveTimers(Time.deltaTime);
        }
    }


    public void Dispose()
    {
        _playerView?.CleanUp();
        _healthbarUi?.CleanUp();
        _ui_GameTime.CleanUp();
        _pauseViewController?.CleanUp();
        _TasksDebugMenu?.CleanUp();


        Debug.Log($"[{GetType().Name}] dispose complete");
    }

}
