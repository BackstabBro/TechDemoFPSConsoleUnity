using UnityEngine;

public class PauseState : IGameState
{
    public GameStates Type => GameStates.Pause;
    private readonly InputService _inputService;

    public PauseState(InputService inputService)
    {
        _inputService = inputService;
    }



    public void Enter()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inputService.ToggleGameplayMap(false);
        _inputService.ToggleUiMap(true);
    }

    public void Exit()
    {

    }
}

