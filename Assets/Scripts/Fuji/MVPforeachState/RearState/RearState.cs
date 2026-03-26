public class RearState : GameState
{
    public void TransitionCheck(IPayload payload)
    {
        RequestTransition<ConversationState>(payload);
    }
    
    public void TransitionCheck(int currentTurn)
    {
        if (currentTurn == 3) RequestTransition<UpstreamState>();
    }
}
