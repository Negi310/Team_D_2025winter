using System;

public class GameStateFactory
{
    public GameState CreateState<T>() where T : GameState
    {
        return typeof(T) switch
        {
            var t when t == typeof(CourtshipState) => (T)(GameState)new CourtshipState(),
            var t when t == typeof(RearState) => (T)(GameState)new RearState(),
            var t when t == typeof(NameState) => (T)(GameState)new NameState(),
            var t when t == typeof(ConversationState) => (T)(GameState)new ConversationState(),
            var t when t == typeof(UpstreamState) => (T)(GameState)new UpstreamState(),
            _ => throw new ArgumentException()
        };
    }
}
