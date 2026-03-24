public class NameState : GameState
{
    public void TransitionCheck(bool isFinished)
    {
        if (isFinished) RequestTransition<RearState>();
    }
}