using System;

public class GameStateMachine : IStateChangable
{
    // ルーターに「状態が変わったこと」だけを知らせる一斉放送
    public event Action<GameState> OnStateChanged;
    
    private GameState _currentState;
    private readonly GameStateFactory _factory;

    public GameStateMachine()
    {
        _factory = new GameStateFactory();
        ((IStateChangable)this).ChangeState<RearState>();
    }

    void IStateChangable.ChangeState<T>()
    {
        GameState nextState = _factory.CreateState<T>();
        _currentState?.Exit();
        _currentState = nextState;
        OnStateChanged?.Invoke(_currentState);
        _currentState?.Enter();
    }
}