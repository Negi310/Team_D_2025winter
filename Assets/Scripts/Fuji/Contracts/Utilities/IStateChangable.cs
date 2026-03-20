public interface IStateChangable
{
    void ChangeState<TState>(IPayload payload = null) where TState : GameState;
}