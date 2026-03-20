public interface IStateChangable
{
    void ChangeState<TState>(object payload = null) where TState : GameState;
}