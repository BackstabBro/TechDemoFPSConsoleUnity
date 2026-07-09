using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class PauseService : IDisposable
{
    public event Action<bool> OnPauseActive;
    public event Action OnShowPauseDeathScreen;

    private readonly GameStateService _gameStateService;
    private readonly InputService _inputService;
    private readonly PlayerService _playerService;
    private bool isPauseActive = false;

    [Inject]
    public PauseService(GameStateService gameStateService, InputService inputService, PlayerService playerService)
    {
        _gameStateService = gameStateService;
        _inputService = inputService;
        _playerService = playerService;
    }

    public void Init()
    {
        _inputService.OnPauseStarted += HandlePausePress;
        _playerService.OnPlayerDead += HandlePlayerDeath;
    }

    public void Dispose()
    {
        _inputService.OnPauseStarted -= HandlePausePress;
        _playerService.OnPlayerDead -= HandlePlayerDeath;
    }


    private void HandlePausePress()
    {
        if (_gameStateService.CurrentState == GameStates.Console)
        {
            return;
        }

        if (isPauseActive)
        {
            QuitPause();
        }
        else
        {
            StartPause();
        }
    }

    private void HandlePlayerDeath()
    {
        _inputService.OnPauseStarted -= HandlePausePress;
        isPauseActive = true;
        _gameStateService.PushState(GameStates.Pause);
        OnShowPauseDeathScreen?.Invoke();
    }



    public void QuitPause()
    {
        isPauseActive = false;
        _gameStateService.PopState();
        OnPauseActive?.Invoke(isPauseActive);
    }


    public void RestartGame()
    {
        _gameStateService.PushState(GameStates.Gameplay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void StartPause()
    {
        isPauseActive = true;
        _gameStateService.PushState(GameStates.Pause);
        OnPauseActive?.Invoke(isPauseActive);

    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }




}
