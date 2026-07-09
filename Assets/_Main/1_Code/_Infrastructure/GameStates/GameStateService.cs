using System;
using System.Collections.Generic;
using VContainer;

public class GameStateService
{
    public GameStates CurrentState => _stateStack.Count > 0 ? _stateStack.Peek() : GameStates.Gameplay;
    public event Action<GameStates> OnGameStateChanged;

    private readonly Dictionary<GameStates, IGameState> _statesMap = new();
    private readonly Stack<GameStates> _stateStack = new Stack<GameStates>();

    [Inject]
    public GameStateService(IReadOnlyList<IGameState> states)
    {
        foreach (var state in states)
        {
            _statesMap[state.Type] = state;
        }
    }

    public void Init()
    {
        _stateStack.Clear();
    }


    public void Startup()
    {
        var defaultState = _statesMap[GameStates.Gameplay];
        _stateStack.Push(defaultState.Type);
        defaultState.Enter();
    }

    public void PushState(GameStates newStateType)
    {
        if (_statesMap.TryGetValue(newStateType, out var newState))
        {
            _stateStack.Push(newStateType);
            newState.Enter();

            OnGameStateChanged?.Invoke(CurrentState);
        }
    }

    public void PopState()
    {
        if (_stateStack.Count <= 1) return;

        GameStates discardedStateType = _stateStack.Pop();
        _statesMap[discardedStateType].Exit();

        GameStates previousStateType = _stateStack.Peek();

        _statesMap[previousStateType].Enter();

        OnGameStateChanged?.Invoke(CurrentState);
    }



    public void Clean()
    {
        _stateStack.Clear();
    }


}


public enum GameStates
{
    Gameplay,
    Pause,
    Dialogue,
    Console,
    Cutscene,
    Menu,
    SaveMenu
}