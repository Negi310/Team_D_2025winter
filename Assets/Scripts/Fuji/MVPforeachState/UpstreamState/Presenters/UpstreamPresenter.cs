using System;
using UnityEngine;

public class UpstreamPresenter : ITickable, IDisposable
{
    private readonly UpstreamState _state;
    private readonly SalmonPlayer _player; // 抽象化されたインターフェースで持つ！
    private readonly UpstreamView _view;
    private readonly TickProvider _tickProvider;
    private readonly UpstreamPlayerContext _playerCtx;
    private readonly SessionContext _sessionCtx;

    // その他のモデル群（TreadmillPresenterの中身をここに合わせて統合してもOKです）

    public UpstreamPresenter(
        UpstreamState state, SalmonPlayer player, UpstreamView view,
        UpstreamPlayerContext playerCtx, SessionContext sessionCtx, TickProvider tickProvider)
    {
        _state = state;
        _player = player;
        _view = view;
        _playerCtx = playerCtx; _sessionCtx = sessionCtx;
        _tickProvider = tickProvider;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
    }

    private void HandleEntered()
    {
        _tickProvider.Register(this);
        _view.Init(_playerCtx.MaxStamina);
        _view.Show();

        _player.OnStaminaChanged += (st) => _view.UpdateStamina(st, _playerCtx.MaxStamina);
        _player.OnComboChanged += _view.UpdateCombo;
        _player.OnDied += () => _state.TransitionCheck(); 

        _player.Init(_playerCtx, _sessionCtx.CurrentSalmon);
    }

    public void Tick(float deltaTime)
    {
        if (_playerCtx.IsDead) return;

        // --- ここで地形生成(Treadmill)や障害物(Obstacle)のロジックを回す ---

        // プレイヤーの更新（内部でRayを飛ばし、スタミナを計算する）
        _player.Tick(deltaTime);

        _view.UpdateDistance(_playerCtx.Position.y);
        
        if (_playerCtx.Position.y >= 1000f) _state.TransitionCheck();
    }

    private void HandleExited() => _view.Hide();

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _player.Dispose();
    }
}