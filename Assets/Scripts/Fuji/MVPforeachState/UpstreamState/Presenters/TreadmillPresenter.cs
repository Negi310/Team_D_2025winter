using System;
using UnityEngine;
using System.Collections.Generic;

public class TreadmillPresenter: IDisposable, ITickable
{
    private readonly UpstreamState _state;
    private readonly TreadmillModel _model;
    private readonly TreadmillView _view;
    private readonly TreadmillContext _context;
    private readonly Transform _playerTransform;
    private readonly TickProvider _tickProvider;
    private readonly IReadOnlyList<ChunkPreset> _availablePresets;

    public TreadmillPresenter(
        UpstreamState state,
        TreadmillModel model, 
        TreadmillView view,
        TreadmillContext context,
        Transform playerTransform, 
        TickProvider tickProvider,
        ChunkLevelData data)
    {
        _state = state;
        _model = model;
        _view = view;
        _context = context;
        _playerTransform = playerTransform;
        _tickProvider = tickProvider;
        _availablePresets = data.AvailableChunkPresets;

        // 郵便屋に自分を登録し、毎フレームTickを呼んでもらう
        _tickProvider.Register(this);
    }

    // Dispatcherから毎フレーム呼ばれる
    public void Tick(float deltaTime)
    {
        // 鮭のY座標を勝手に覗き見して、頭脳に報告する
        var result = _model.UpdatePlayerPosition(_context, _availablePresets, _playerTransform.position.y);
        if (result.SpawnedChunk == null || result.DespawnedChunk == null) return;
        _view.SpawnChunkVisually(result.SpawnedChunk);
        _view.DespawnChunkVisually(result.DespawnedChunk);
    }
    
    private void HandleEntered()
    {
        var chunk1 = _model.SpawnNextChunk(_context, _availablePresets);
        var chunk2 = _model.SpawnNextChunk(_context, _availablePresets);
        _view.SpawnChunkVisually(chunk1);
        _view.SpawnChunkVisually(chunk2);
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
