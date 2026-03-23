using System.Collections.Generic;

public class ObstacleContext
{
    public List<FixedObstacleData> ActiveFixedObstacles { get; } = new();
    public List<DrifterData> ActiveDrifters { get; } = new();
}