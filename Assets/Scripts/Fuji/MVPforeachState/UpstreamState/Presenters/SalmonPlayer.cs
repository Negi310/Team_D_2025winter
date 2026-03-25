using System;
using UnityEngine;
using DG.Tweening;

public class SalmonPlayer : IDisposable
{
    public event Action<float> OnStaminaChanged;
    public event Action<int> OnComboChanged;
    public event Action OnDied;

    private readonly SalmonMove _view; // インターフェースでView(SalmonMove)を持つ！
    private readonly UpstreamGameModel _model;
    
    private UpstreamPlayerContext _playerCtx;
    private SalmonData _salmonData;
    private bool _isPlaying, _isJumping, _isInvincible, _isJustAfterJump;

    public SalmonPlayer(SalmonMove view, UpstreamGameModel model)
    {
        _view = view; _model = model;
        _view.OnTriggerHit += HandleTriggerHit;
        _view.OnJumpTriggered += HandleJumpTriggered;
    }

    public void Init(UpstreamPlayerContext ctx, SalmonData data)
    {
        _playerCtx = ctx;
        _salmonData = data;
        _isPlaying = true;
        _playerCtx.MaxStamina = _salmonData.UpstreamStats.Stamina;
        _playerCtx.CurrentStamina = _playerCtx.MaxStamina;
        _view.SetPlayingState(true);
    }

    public void Tick(float deltaTime)
    {
        if (!_isPlaying || _playerCtx.IsDead) return;
        RayData sensorData = _view.GetSensorData();
        // Modelから速度を計算し、View(SalmonMove)に注入する
        float speedX = _model.CalculateHorizontalSpeed(_salmonData.UpstreamStats, sensorData);
        float speedY = _model.CalculateForwardSpeed(_salmonData.UpstreamStats);
        _view.UpdateSpeeds(speedX, speedY);

        if (!_isJumping)
        {
            float drain = _model.CalculateRayBasedDrain(_salmonData.UpstreamStats, sensorData, deltaTime);
            DrainStamina(drain);
        }

        _playerCtx.Position = _view.Position;
    }

    private void HandleJumpTriggered()
    {
        if (_isJumping) return;
        _isJumping = true;
        _view.SetJumpingState(true);

        RayData sensorData = _view.GetSensorData();
        float cost = _model.CalculateJumpCost(_salmonData.UpstreamStats, sensorData);
        
        if (sensorData.ForwardRockDistance < 5f) AddCombo(); else ResetCombo();
        DrainStamina(cost);

        _view.PlayJumpAnimation(() => 
        {
            _isJumping = false;
            _view.SetJumpingState(false);
            StartJustAfterJumpWindow();
        });
    }

    private void StartJustAfterJumpWindow()
    {
        _isJustAfterJump = true;
        DOVirtual.DelayedCall(0.5f, () => _isJustAfterJump = false);
    }

    private void HandleTriggerHit(GameObject hitObject)
    {
        if (_isJumping || _isInvincible) return;

        if (hitObject.TryGetComponent<DrifterView>(out var drifterView)) ProcessDrifterHit(drifterView.Data, hitObject);
        else if (hitObject.TryGetComponent<FixedObstacleView>(out var obstacleView)) TakeDamage(20f);
    }

    private void ProcessDrifterHit(DrifterData drifter, GameObject hitObject)
    {
        if (drifter.Type == DrifterType.Fish)
        {
            HealStamina(15f); AddCombo();
            GameObject.Destroy(hitObject);
        }
        else if (drifter.Type == DrifterType.RivalSalmon)
        {
            float atk = _salmonData.UpstreamStats.Attack + (_isJustAfterJump ? 10f : 0f);
            var result = _model.EvaluateRivalBattle(atk);

            if (result == BattleResult.Win) { AddCombo(); }
            else if (result == BattleResult.Draw) { DrainStamina(5f); }
            else { TakeDamage(20f); }
            GameObject.Destroy(hitObject);
        }
        else if (drifter.Type == DrifterType.Driftwood)
        {
            TakeDamage(25f);
            GameObject.Destroy(hitObject);
        }
    }

    private void TakeDamage(float damage)
    {
        DrainStamina(damage); ResetCombo();
        _isInvincible = true;
        Debug.Log($"Damage: {damage}");
        _view.PlayDamageReaction(() => _isInvincible = false);
    }

    private void DrainStamina(float amount)
    {
        _playerCtx.CurrentStamina -= amount;
        OnStaminaChanged?.Invoke(_playerCtx.CurrentStamina);
        if (_playerCtx.CurrentStamina <= 0 && _isPlaying)
        { 
            _isPlaying = false;
            _view.SetPlayingState(false);
            OnDied?.Invoke(); 
        }
    }

    private void HealStamina(float amount)
    {
        _playerCtx.CurrentStamina = Mathf.Min(_playerCtx.CurrentStamina + amount, _playerCtx.MaxStamina);
        OnStaminaChanged?.Invoke(_playerCtx.CurrentStamina);
    }

    private void AddCombo() { _playerCtx.ComboCount++; OnComboChanged?.Invoke(_playerCtx.ComboCount); }
    private void ResetCombo() { _playerCtx.ComboCount = 0; OnComboChanged?.Invoke(0); }
    
    public void Dispose()
    {
        _view.OnTriggerHit -= HandleTriggerHit;
        _view.OnJumpTriggered -= HandleJumpTriggered;
    }
}