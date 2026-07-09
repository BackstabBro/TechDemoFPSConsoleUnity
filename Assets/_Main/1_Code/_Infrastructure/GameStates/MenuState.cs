using UnityEngine;

public class MenuState : IGameState
{
    public GameStates Type => GameStates.Menu;
    private readonly InputService _inputService;

    public MenuState(InputService inputService)
    {
        _inputService = inputService;
    }



    public void Enter()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inputService.ToggleGameplayMap(true);
        _inputService.ToggleUiMap(true);
    }

    public void Exit()
    {

    }
}
