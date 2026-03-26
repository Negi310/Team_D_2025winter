using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameStateMachine : IStateChangable
{
    // ルーターに「状態が変わったこと」だけを知らせる一斉放送
    public event Action<GameState, IPayload> OnStateChanged;

    private GameState _currentState;
    private readonly GameStateManager _transitionManager;
    private readonly GameStateFactory _factory;

    public GameStateMachine(GameStateManager transitionManager)
    {
        _factory = new GameStateFactory();
        _transitionManager = transitionManager;
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
        //_transitionManager.ChangeState(nextState).Forget();
    }
}
