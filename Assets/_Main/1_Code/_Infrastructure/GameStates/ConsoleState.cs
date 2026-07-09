using UnityEngine;

public class ConsoleState : IGameState
{
    public GameStates Type => GameStates.Console;
    private readonly InputService _inputService;

    public ConsoleState(InputService inputService)
    {
        _inputService = inputService;
    }



    public void Enter()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inputService.ToggleGameplayMap(false);
        _inputService.ToggleUiMap(false);
    }

    public void Exit()
    {

    }
}
