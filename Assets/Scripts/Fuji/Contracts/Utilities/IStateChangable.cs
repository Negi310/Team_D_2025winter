public interface IStateChangable
{
    void ChangeState<T>() where T : GameState;
}