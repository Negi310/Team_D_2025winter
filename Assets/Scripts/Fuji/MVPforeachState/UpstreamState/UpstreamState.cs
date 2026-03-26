using UnityEngine;

public class UpstreamState : GameState
{
    public void TransitionCheck(IPayload payload, bool isDead)
    {
        if (payload is CourtshipInitPayload courtshipInitPayload)
        {
            if (isDead) RequestTransition<CourtshipState>(courtshipInitPayload);
            else if (courtshipInitPayload.DistanceTraveled > 1000) RequestTransition<CourtshipState>(courtshipInitPayload);
        }
        
    }
}