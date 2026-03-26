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
    private readonly TreadmillContext _treadmillCtx;
    private readonly SplineMathModel _splineMath;

    // その他のモデル群（TreadmillPresenterの中身をここに合わせて統合してもOKです）

    public UpstreamPresenter(
        UpstreamState state, SalmonPlayer player, SplineMathModel splineMath, UpstreamView view,
        UpstreamPlayerContext playerCtx, TreadmillContext treadmillContext, SessionContext sessionCtx, TickProvider tickProvider)
    {
        _state = state;
        _player = player;
        _splineMath = splineMath;
        _view = view;
        _playerCtx = playerCtx;
        _treadmillCtx = treadmillContext;
        _sessionCtx = sessionCtx;
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
        _player.OnDied += () =>
        {
            _playerCtx.IsDead = true;
            CourtshipInitPayload payload = new CourtshipInitPayload { DistanceTraveled = _playerCtx.Position.y };
            _state.TransitionCheck(payload, _playerCtx.IsDead);
        };

        _player.Init(_playerCtx, _sessionCtx.CurrentSalmon);
    }

    public void Tick(float deltaTime)
    {
        if (_playerCtx.IsDead) return;

        float distanceFromCenter = 0f;
        if (_treadmillCtx.GlobalCenterLine.Count > 0)
        {
            if (_splineMath.TryGetXAtY(_treadmillCtx.GlobalCenterLine, _playerCtx.Position.y, out float centerX))
            {
                distanceFromCenter = Mathf.Abs(_playerCtx.Position.x - centerX);
            }
        }

        // プレイヤーの更新（内部でRayを飛ばし、スタミナを計算する）
        _player.Tick(deltaTime, distanceFromCenter);

        _view.UpdateDistance(_playerCtx.Position.y);
        
        CourtshipInitPayload payload = new CourtshipInitPayload { DistanceTraveled = _playerCtx.Position.y };
        _state.TransitionCheck(payload, _playerCtx.IsDead);
    }

    private void HandleExited() => _view.Hide();

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _player.Dispose();
    }
}