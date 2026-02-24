using System;

public class GameStateFactory
{
    public GameState CreateState<T>() where T : GameState
    {
        return typeof(T) switch
        {
            var t when t == typeof(CourtshipState) => (T)(GameState)new CourtshipState(),
            var t when t == typeof(RearState) => (T)(GameState)new RearState(),
            _ => throw new ArgumentException()
        };
    }
}