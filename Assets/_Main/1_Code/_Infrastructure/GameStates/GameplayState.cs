using UnityEngine;

public class GameplayState : IGameState
{
    public GameStates Type => GameStates.Gameplay;
    private readonly InputService _inputService;

    public GameplayState(InputService inputService)
    {
        _inputService = inputService;
    }


    public void Enter()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputService.ToggleGameplayMap(true);
        _inputService.ToggleUiMap(true);
    }

    public void Exit()
    {

    }
}
