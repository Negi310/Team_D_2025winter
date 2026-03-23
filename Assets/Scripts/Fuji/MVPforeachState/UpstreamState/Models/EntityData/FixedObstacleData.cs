using UnityEngine;

public class FixedObstacleData
{
    public FixedObstacleType Type { get; }
    public Vector2 Position { get; }
    public float Radius { get; }

    public FixedObstacleData(FixedObstacleType type, Vector2 position, float radius)
    {
        Type = type;
        Position = position;
        Radius = radius;
    }
}