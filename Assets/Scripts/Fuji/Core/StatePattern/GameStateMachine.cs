using System;
using UnityEngine;

public class GameStateMachine : IStateChangable
{
    // ルーターに「状態が変わったこと」だけを知らせる一斉放送
    public event Action<GameState, IPayload> OnStateChanged;

    private GameState _currentState;
    private readonly GameStateFactory _factory;

    public GameStateMachine()
    {
        _factory = new GameStateFactory();
    }
    
    void IStateChangable.ChangeState<T>(IPayload payload)
    {
        GameState nextState = _factory.CreateState<T>();
        nextState.Init(this);
        _currentState?.Exit();
        _currentState = nextState;
        OnStateChanged?.Invoke(_currentState, payload);
        _currentState?.Enter();
        Debug.Log(nextState.ToString());
    }
}
