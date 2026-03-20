using System;

public abstract class GameState
{
    // StateMachineが監視するための遷移要求イベント（引数に次のStateを渡す）
    public event Action OnEnter;
    public event Action OnExit;
    
    private IStateChangable _stateMachine;
    
    public void Init(IStateChangable stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter() => OnEnter?.Invoke();

    public virtual void Exit() => OnExit?.Invoke();

    // サブクラス（具象State）が遷移したい時に呼ぶメソッド
    protected void RequestTransition<TState>(object payload = null) where TState : GameState
    {
        _stateMachine.ChangeState<TState>(payload);
    }
}