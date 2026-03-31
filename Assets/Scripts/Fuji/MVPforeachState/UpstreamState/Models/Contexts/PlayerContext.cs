using UnityEngine;

public class UpstreamPlayerContext
{
    public float MaxStamina { get; set; }
    public float CurrentStamina { get; set; }
    public bool IsDead { get; set; }
    public int ComboCount { get; set; }
    public Vector2 Position { get; set; }
    public bool IsRunning { get; set; }

    public UpstreamPlayerContext()
    {
        IsDead = false;
        IsRunning = false;
    }
}