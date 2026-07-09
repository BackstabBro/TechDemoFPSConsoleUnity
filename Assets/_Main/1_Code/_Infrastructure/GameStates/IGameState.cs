public interface IGameState
{
    GameStates Type { get; }
    void Enter();
    void Exit();
}
