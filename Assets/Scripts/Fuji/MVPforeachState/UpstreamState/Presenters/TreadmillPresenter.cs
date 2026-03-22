using System;
using UnityEngine;
using System.Collections.Generic;

public class TreadmillPresenter: IDisposable, ITickable
{
    private readonly UpstreamState _state;
    private readonly TreadmillModel _model;
    private readonly ChunkPoolManager _chunkPoolManager;
    private readonly RiverPath _path;
    private readonly TreadmillView _view;
    private readonly TreadmillContext _context;
    private readonly Transform _playerTransform;
    private readonly TickProvider _tickProvider;
    private readonly IReadOnlyList<ChunkPreset> _availablePresets;

    public TreadmillPresenter(
        UpstreamState state,
        TreadmillModel model,
        ChunkPoolManager chunkPoolManager,
        RiverPath path,
        TreadmillView view,
        TreadmillContext context,
        Transform playerTransform, 
        TickProvider tickProvider,
        ChunkLevelData data)
    {
        _state = state;
        _model = model;
        _chunkPoolManager = chunkPoolManager;
        _path = path;
        _view = view;
        _context = context;
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
        var result = _model.UpdatePlayerPosition(_context, _availablePresets, _playerTransform.position.y);
        if (result.SpawnedChunk == null || result.DespawnedChunk == null) return;
        _view.SpawnChunkVisually(result.SpawnedChunk);
        _view.DespawnChunkVisually(result.DespawnedChunk);
        _path.AddChunkSplines(_context, result.SpawnedChunk.Preset, result.SpawnedChunk.Position);
        _path.RemoveOldestChunkSplines(_context);
    }
    
    private void HandleEntered()
    {
        // 郵便屋に自分を登録し、毎フレームTickを呼んでもらう
        _tickProvider.Register(this);
        _view.Init(_chunkPoolManager);
        for (int i = 0; i < 2; i++)
        {
            var chunk = _model.SpawnNextChunk(_context, _availablePresets);
            _view.SpawnChunkVisually(chunk);
            _path.AddChunkSplines(_context, chunk.Preset, chunk.Position);
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
}
