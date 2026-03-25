public class RearState : GameState
{
    public void TransitionCheck(IPayload payload)
    {
        RequestTransition<ConversationState>(payload);
    }
    
    public void TransitionCheck(int currentTurn)
    {
        if (currentTurn == 10) RequestTransition<UpstreamState>();
    }
}
