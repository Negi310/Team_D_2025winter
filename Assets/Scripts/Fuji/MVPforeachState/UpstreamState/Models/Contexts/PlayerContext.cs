public class UpstreamPlayerContext
{
    public float MaxStamina { get; }
    public float CurrentStamina { get; set; }
    public float DistanceTraveled { get; set; }
    public bool IsDead { get; set; }

    public UpstreamPlayerContext(float initialStamina)
    {
        MaxStamina = initialStamina;
        CurrentStamina = initialStamina;
        DistanceTraveled = 0f;
        IsDead = false;
    }
}