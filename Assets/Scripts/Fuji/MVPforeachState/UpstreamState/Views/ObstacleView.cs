using System.Collections.Generic;
using UnityEngine;

public class ObstaclesView : MonoBehaviour
{
    private PoolManager _poolManager;

    [Header("Prefabs")]
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private GameObject _treePrefab;
    [SerializeField] private GameObject _drifterPrefab;
    [SerializeField] private GameObject _rivalPrefab;
    [SerializeField] private GameObject _driftwoodPrefab;

    private readonly Dictionary<FixedObstacleData, GameObject> _fixedObs = new();
    private readonly Dictionary<DrifterData, GameObject> _drifters = new();

    public void Init(PoolManager poolManager) => _poolManager = poolManager;

    public void SpawnFixedObstacle(FixedObstacleData data)
    {
        GameObject prefab = data.Type == FixedObstacleType.Rock ? _rockPrefab : _treePrefab;
        GameObject obj = _poolManager.Get(prefab);
        obj.transform.position = new Vector3(data.Position.x, data.Position.y, data.Position.y * 0.01f);
        if (!obj.TryGetComponent<FixedObstacleView>(out var view))
        {
            view = obj.AddComponent<FixedObstacleView>();
        }
        view.Setup(data);
        _fixedObs.Add(data, obj);
    }

    public void SpawnDrifter(DrifterData data)
    {
        GameObject prefab = data.Type switch
        {
            DrifterType.RivalSalmon => _rivalPrefab,
            DrifterType.Fish => _drifterPrefab,
            DrifterType.Driftwood => _driftwoodPrefab,
            _ => _rivalPrefab
        };
        GameObject obj = _poolManager.Get(prefab);
        obj.transform.position = new Vector3(data.Position.x, data.Position.y, 0f);
        if (!obj.TryGetComponent<DrifterView>(out var view))
        {
            view = obj.AddComponent<DrifterView>();
        }
        view.Setup(data);
        _drifters.Add(data, obj);
    }

    // 毎フレームPresenterから呼ばれる
    public void UpdateDrifterTransforms()
    {
        foreach (var kvp in _drifters)
        {
            kvp.Value.transform.position = new Vector3(kvp.Key.Position.x, kvp.Key.Position.y, 0f);
        }
    }

    public void DespawnFixedObstacle(FixedObstacleData data)
    {
        if (_fixedObs.TryGetValue(data, out GameObject obj))
        {
            _poolManager.Release(obj);
            _fixedObs.Remove(data);
        }
    }

    public void DespawnDrifter(DrifterData data)
    {
        if (_drifters.TryGetValue(data, out GameObject obj))
        {
            _poolManager.Release(obj);
            _drifters.Remove(data);
        }
    }
    
    public void ClearAll()
        {
            foreach (var obj in _fixedObs.Values)
            {
                if (obj != null) _poolManager.Release(obj);
            }
            _fixedObs.Clear();
    
            foreach (var obj in _drifters.Values)
            {
                if (obj != null) _poolManager.Release(obj);
            }
            _drifters.Clear();
        }
}