using System;

public abstract class GameState
{
    // StateMachineが監視するための遷移要求イベント（引数に次のStateを渡す）
    public event Action OnEnter;
    public event Action OnExit;
    
    protected readonly IStateChangable _stateMachine;

    public virtual void Enter() => OnEnter?.Invoke();

    public virtual void Exit() => OnExit?.Invoke();

    // サブクラス（具象State）が遷移したい時に呼ぶメソッド
    protected void RequestTransition<T>() where T : GameState
    {
        _stateMachine.ChangeState<T>();
    }
}