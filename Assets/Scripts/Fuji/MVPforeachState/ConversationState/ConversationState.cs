public class ConversationState : GameState
{
    public void TransitionCheck(bool isFinished)
    {
        if (isFinished) RequestTransition<RearState>(); 
    }
}
