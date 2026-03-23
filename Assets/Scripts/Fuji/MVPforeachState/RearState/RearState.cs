public class RearState : GameState
{
    public void TransitionCheck(IPayload payload)
    {
        RequestTransition<ConversationState>(payload);
    }
    
    public void TransitionCheck()
    {
        RequestTransition<UpstreamState>();
    }
}
