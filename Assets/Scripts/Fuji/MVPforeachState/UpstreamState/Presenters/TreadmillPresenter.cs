using System;
using UnityEngine;
using System.Collections.Generic;

public class TreadmillPresenter: IDisposable, ITickable
{
    private readonly UpstreamState _state;
    private readonly TreadmillModel _model;
    private readonly PoolManager _poolManager;
    private readonly RiverPath _path;
    private readonly ObstacleModel _obstacleModel;
    private readonly TreadmillView _treadmillView;
    private readonly ObstaclesView _obstaclesView;
    private readonly UpstreamPlayerContext _playerContext;
    private readonly TreadmillContext _treadmillContext;
    private readonly ObstacleContext _obstacleContext;
    private readonly TickProvider _tickProvider;
    private readonly IReadOnlyList<ChunkPreset> _availablePresets;

    public TreadmillPresenter(
        UpstreamState state,
        TreadmillModel model,
        PoolManager poolManager,
        RiverPath path,
        ObstacleModel obstacleModel,
        TreadmillView treadmillView,
        ObstaclesView obstaclesView,
        UpstreamPlayerContext playerContext,
        TreadmillContext treadmillContext,
        ObstacleContext obstacleContext,
        TickProvider tickProvider,
        ChunkLevelData data)
    {
        _state = state;
        _model = model;
        _poolManager = poolManager;
        _path = path;
        _obstacleModel = obstacleModel;
        _treadmillView = treadmillView;
        _obstaclesView = obstaclesView;
        _playerContext = playerContext;
        _treadmillContext = treadmillContext;
        _obstacleContext = obstacleContext;
        _tickProvider = tickProvider;
        _availablePresets = data.AvailableChunkPresets;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
    }

    // Dispatcherから毎フレーム呼ばれる
    public void Tick(float deltaTime)
    {
        float playerY = _playerContext.Position.y;
        var result = _model.UpdatePlayerPosition(_treadmillContext, _availablePresets, playerY);
        if (result.SpawnedChunk != null || result.DespawnedChunk != null)
        {
            _treadmillView.SpawnChunkVisually(result.SpawnedChunk);
            _treadmillView.DespawnChunkVisually(result.DespawnedChunk);
            _path.AddChunkSplines(_treadmillContext, result.SpawnedChunk.Preset, result.SpawnedChunk.Position);
            _path.RemoveOldestChunkSplines(_treadmillContext);
            SpawnObstaclesForChunk(result.SpawnedChunk);
        }
        
        var driftersToRemove = new List<DrifterData>();
        foreach (var drifter in _obstacleContext.ActiveDrifters)
        {
            // ★ 引数に _obstacleContext.ActiveFixedObstacles を追加して「前方の岩のリスト」を渡す
            _obstacleModel.UpdateDrifter(drifter, deltaTime, _treadmillContext.GlobalLeftBank, _treadmillContext.GlobalRightBank, _obstacleContext.ActiveFixedObstacles);
            
            if (drifter.Position.y < playerY - 10f || drifter.Position.y > playerY + 40f) // (※上方向の破棄判定も忘れずに)
            {
                driftersToRemove.Add(drifter);
            }
        }

        // 破棄対象の漂流物をプールに返却
        foreach (var oldDrifter in driftersToRemove)
        {
            _obstaclesView.DespawnDrifter(oldDrifter);
            _obstacleContext.ActiveDrifters.Remove(oldDrifter);
        }

        // ==========================================
        // 3. 固定設置物の破棄（通り過ぎた岩・倒木の回収）
        // ==========================================
        var obsToRemove = new List<FixedObstacleData>();
        foreach (var obs in _obstacleContext.ActiveFixedObstacles)
        {
            if (obs.Position.y < playerY - 10f)
            {
                obsToRemove.Add(obs);
            }
        }

        foreach (var oldObs in obsToRemove)
        {
            _obstaclesView.DespawnFixedObstacle(oldObs);
            _obstacleContext.ActiveFixedObstacles.Remove(oldObs);
        }

        // ==========================================
        // 4. Viewに漂流物の「新しい座標」を一斉反映
        // ==========================================
        _obstaclesView.UpdateDrifterTransforms();
    }
    
    private void HandleEntered()
    {
        // 郵便屋に自分を登録し、毎フレームTickを呼んでもらう
        _tickProvider.Register(this);
        _treadmillView.Init(_poolManager);
        _obstaclesView.Init(_poolManager);
        for (int i = 0; i < 2; i++)
        {
            var chunk = _model.SpawnNextChunk(_treadmillContext, _availablePresets);
            _treadmillView.SpawnChunkVisually(chunk);
            _path.AddChunkSplines(_treadmillContext, chunk.Preset, chunk.Position);
            SpawnObstaclesForChunk(chunk);
        }
    }

    private void HandleExited()
    {

    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _tickProvider.Unregister(this);
    }
    
    private void SpawnObstaclesForChunk(RuntimeChunkData chunk)
    {
        // 固定設置物の生成
        var newObs = _obstacleModel.GenerateFixedObstacles(chunk, _treadmillContext.GlobalLeftBank, _treadmillContext.GlobalRightBank, 3, _obstacleContext.ActiveFixedObstacles);
        foreach (var obs in newObs)
        {
            _obstacleContext.ActiveFixedObstacles.Add(obs);
            _obstaclesView.SpawnFixedObstacle(obs);
        }

        // 漂流物の生成
        var newDrifters = _obstacleModel.GenerateDrifters(chunk, _treadmillContext.GlobalLeftBank, _treadmillContext.GlobalRightBank, 2);
        foreach (var drifter in newDrifters)
        {
            _obstacleContext.ActiveDrifters.Add(drifter);
            _obstaclesView.SpawnDrifter(drifter);
        }
    }
}
