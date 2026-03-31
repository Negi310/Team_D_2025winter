public class TitleState : GameState
{
    public void TransitionCheck(string initialStateName)
    {
        if (initialStateName == nameof(UpstreamState)) RequestTransition<UpstreamState>();
        else if (initialStateName == nameof(CourtshipState)) RequestTransition<CourtshipState>();
        else if (initialStateName == nameof(RearState)) RequestTransition<RearState>();
        else if (initialStateName == nameof(NameState)) RequestTransition<NameState>();
        else if (initialStateName == nameof(ConversationState)) RequestTransition<ConversationState>();
        else RequestTransition<UpstreamState>();
        //else RequestTransition<CourtshipState>();
        //else RequestTransition<RearState>();
        //else RequestTransition<NameState>();
        //else RequestTransition<ConversationState>();
    }
}