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
    private readonly TreadmillContext _treadmillContext;
    private readonly ObstacleContext _obstacleContext;
    private readonly Transform _playerTransform;
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
        TreadmillContext treadmillContext,
        ObstacleContext obstacleContext,
        Transform playerTransform, 
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
        _treadmillContext = treadmillContext;
        _obstacleContext = obstacleContext;
        _playerTransform = playerTransform;
        _tickProvider = tickProvider;
        _availablePresets = data.AvailableChunkPresets;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
    }

    // Dispatcherから毎フレーム呼ばれる
    public void Tick(float deltaTime)
    {
        // 鮭のY座標を勝手に覗き見して、頭脳に報告する
        var result = _model.UpdatePlayerPosition(_treadmillContext, _availablePresets, _playerTransform.position.y);
        if (result.SpawnedChunk == null || result.DespawnedChunk == null) return;
        _treadmillView.SpawnChunkVisually(result.SpawnedChunk);
        _treadmillView.DespawnChunkVisually(result.DespawnedChunk);
        _path.AddChunkSplines(_treadmillContext, result.SpawnedChunk.Preset, result.SpawnedChunk.Position);
        _path.RemoveOldestChunkSplines(_treadmillContext);
        SpawnObstaclesForChunk(result.SpawnedChunk);
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
        }
        //SalmonMove salmon = new SalmonMove();
        //salmon.Init(SessionContext.CurrentSalmon.UpstreamStats);
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
        var newDrifters = _obstacleModel.GenerateDrifters(chunk, _treadmillContext.GlobalLeftBank, _treadmillContext.GlobalRightBank, 2, 5.0f);
        foreach (var drifter in newDrifters)
        {
            _obstacleContext.ActiveDrifters.Add(drifter);
            _obstaclesView.SpawnDrifter(drifter);
        }
    }
}
