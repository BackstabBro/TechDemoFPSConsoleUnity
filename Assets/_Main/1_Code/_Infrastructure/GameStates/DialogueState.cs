using UnityEngine;

public class DialogueState : IGameState
{
    public GameStates Type => GameStates.Dialogue;
    private readonly InputService _inputService;

    public DialogueState(InputService inputService)
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
