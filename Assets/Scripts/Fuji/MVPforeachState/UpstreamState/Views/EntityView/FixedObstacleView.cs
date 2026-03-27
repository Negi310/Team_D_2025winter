using UnityEngine;

public class FixedObstacleView : MonoBehaviour
{
    public FixedObstacleData Data { get; private set; }

    public void Setup(FixedObstacleData data)
    {
        Data = data;
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = true;
    }
}