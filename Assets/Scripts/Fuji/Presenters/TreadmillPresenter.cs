using System;
using UnityEngine;

public class TreadmillPresenter: IDisposable, ITickable
{
    private readonly UpstreamState _state;
    private readonly RiverTreadmillModel _model;
    private readonly RiverView _view;
    private readonly Transform _playerTransform;
    private readonly TickProvider _tickProvider;

    public TreadmillPresenter(
        UpstreamState state,
        RiverTreadmillModel model, 
        RiverView view, 
        Transform playerTransform, 
        TickProvider tickProvider)
    {
        _state = state;
        _model = model;
        _view = view;
        _playerTransform = playerTransform;
        _tickProvider = tickProvider;

        _model.OnChunkSpawnRequested += HandleSpawn;
        _model.OnChunkDespawnRequested += HandleDespawn;

        // 郵便屋に自分を登録し、毎フレームTickを呼んでもらう
        _tickProvider.Register(this);
    }

    // Dispatcherから毎フレーム呼ばれる
    public void Tick(float deltaTime)
    {
        // 鮭のY座標を勝手に覗き見して、頭脳に報告する（Pull）
        _model.UpdatePlayerPosition(_playerTransform.position.y);
    }

    private void HandleSpawn(RuntimeChunkData data) => _view.SpawnChunkVisually(data);
    private void HandleDespawn(RuntimeChunkData data) => _view.DespawnChunkVisually(data);
    
    private void HandleEntered()
    {
        _model.SpawnNextChunk(); 
        _model.SpawnNextChunk(); 
    }

    private void HandleExited()
    {

    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _model.OnChunkSpawnRequested -= HandleSpawn;
        _model.OnChunkDespawnRequested -= HandleDespawn;
        _tickProvider.Unregister(this);
    }
}
