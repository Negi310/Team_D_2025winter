using System.Collections.Generic;

public class ObstacleManagerContext
{
    public List<FixedObstacleData> ActiveFixedObstacles { get; } = new();
    public List<DrifterData> ActiveDrifters { get; } = new();
}