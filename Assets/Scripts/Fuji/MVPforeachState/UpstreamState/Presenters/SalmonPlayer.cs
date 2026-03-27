using System;
using UnityEngine;
using DG.Tweening;

public class SalmonPlayer : IDisposable
{
    public event Action<float> OnStaminaChanged;
    public event Action<int> OnComboChanged;
    public event Action OnDied;

    private readonly SalmonMove _view;
    private readonly UpstreamGameModel _model;
    
    private UpstreamPlayerContext _playerCtx;
    private SalmonData _salmonData;
    private bool _isPlaying, _isJumping, _isInvincible, _isJustAfterJump;
    
    private float _currentDistanceFromCenter;
    
    // ★追加: 小魚によるスタミナ消費軽減バフの残り時間
    private float _staminaBuffTimer; 
    
    private readonly GameSetting _settings;

    public SalmonPlayer(SalmonMove view, UpstreamGameModel model, GameSetting settings)
    {
        _view = view;
        _model = model;
        _settings = settings;
        _view.OnTriggerHit += HandleTriggerHit;
        _view.OnJumpTriggered += HandleJumpTriggered;
    }

    public void Init(UpstreamPlayerContext ctx, SalmonData data)
    {
        _playerCtx = ctx;
        _salmonData = data;
        _isPlaying = true;

        // ★修正: スタミナパラメータを「スタミナ総量」に変換する係数を掛ける
        _playerCtx.MaxStamina = _salmonData.UpstreamStats.Stamina * _settings.StaminaToMaxStaminaRate;
        
        _playerCtx.CurrentStamina = _playerCtx.MaxStamina;
        _playerCtx.IsDead = false;
        _playerCtx.ComboCount = 0;
        _staminaBuffTimer = 0f; 
        
        Color displayColor = ConvertEnumToColor(_salmonData.CourtshipTraits.Color);
        _view.SetBodyColor(displayColor);
        
        _view.ResetPosition(Vector2.zero);
        _view.SetPlayingState(true);
    }

    // ★修正: flowSpeed を受け取るように
    public void Tick(float deltaTime, float distanceFromCenter, float flowSpeed)
    {
        if (!_isPlaying || _playerCtx.IsDead) return;
        _currentDistanceFromCenter = distanceFromCenter;
        
        if (_staminaBuffTimer > 0f) _staminaBuffTimer -= deltaTime;

        RayData sensorData = _view.GetSensorData();
        
        // ★修正: returnを消し、ダメージ判定時も絶対にPositionを更新するように変更！
        if (!_isJumping && !_isInvincible && sensorData.ForwardRockDistance < 0.3f)
        {
            TakeDamage(20f);
        }
        else
        {
            float speedX = _model.CalculateHorizontalSpeed(_salmonData.UpstreamStats, sensorData, _isInvincible);
            float speedY = _model.CalculateForwardSpeed();
            _view.UpdateSpeeds(speedX, speedY);
            
            if (!_isJumping)
            {
                float drain = _model.CalculateDrain(_salmonData.UpstreamStats, sensorData, distanceFromCenter, deltaTime, flowSpeed);
                
                // ★修正: 小魚バフ中のスタミナ消費倍率を設定ファイルから引っ張る
                if (_staminaBuffTimer > 0f) drain *= _settings.FishBuffDrainMultiplier; 
                
                DrainStamina(drain);
            }
        }

        // ★★★ これがサボられるとスプラインの生成が狂うため、毎フレーム必ず実行！ ★★★
        _playerCtx.Position = _view.Position;
    }

    private void HandleJumpTriggered()
    {
        if (!_playerCtx.IsRunning || _isJumping) return;
        _isJumping = true;
        _view.SetJumpingState(true);
        AudioManager.I.PlaySE(SE.Name.Jump);
        RayData sensorData = _view.GetSensorData();
        float cost = _model.CalculateJumpCost(_salmonData.UpstreamStats, sensorData, _currentDistanceFromCenter);
        
        // ★修正: 小魚バフ中はジャンプコストも設定された倍率で軽減
        if (_staminaBuffTimer > 0f) cost *= _settings.FishBuffDrainMultiplier;
        
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
        DOVirtual.DelayedCall(_settings.JustJumpWindow, () => _isJustAfterJump = false);
    }

    private void HandleTriggerHit(GameObject hitObject)
    {
        if (_isInvincible) return;

        // ==========================================
        // ★追加: ジャンプ中の処理（流木に触れたらコンボ＆すり抜け）
        // ==========================================
        if (_isJumping)
        {
            bool isDriftwood = hitObject.TryGetComponent<DrifterView>(out var dView) && dView.Data.Type == DrifterType.Driftwood;
            bool isFallenTree = hitObject.TryGetComponent<FixedObstacleView>(out var fView) && fView.Data.Type == FixedObstacleType.FallenTree;

            if (isDriftwood || isFallenTree)
            {
                AddCombo();
                // コンボが連続で入らないようにColliderだけオフにする
                if (hitObject.TryGetComponent<Collider2D>(out var col)) col.enabled = false;
            }
            return;
        }

        if (hitObject.TryGetComponent<DrifterView>(out var drifterView)) ProcessDrifterHit(drifterView.Data, hitObject);
    }

    private void ProcessDrifterHit(DrifterData drifter, GameObject hitObject)
    {
        if (drifter.Type == DrifterType.Fish)
        {
            _staminaBuffTimer = _settings.FishBuffDuration;
        }
        else if (drifter.Type == DrifterType.RivalSalmon)
        {
            float atk = _salmonData.UpstreamStats.Attack + (_isJustAfterJump ? _settings.JustJumpAttackBonus : 0f);
            var result = _model.EvaluateRivalBattle(atk);

            if (result == BattleResult.Win) { AddCombo(); }
            else if (result == BattleResult.Draw) { DrainStamina(_settings.RivalDrawDamage); }
            else { TakeDamage(_settings.RivalLoseDamage); }
        }
        else if (drifter.Type == DrifterType.Driftwood)
        {
            TakeDamage(_settings.DriftwoodDamage);
        }
    }

    private void TakeDamage(float damage)
    {
        if (_isInvincible) return;
        DrainStamina(damage); // ※ダメージはバフで半減しません
        ResetCombo();
        _isInvincible = true;
        _view.PlayDamageReaction(_settings.InvincibleDuration, () => _isInvincible = false);
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
    
    private Color ConvertEnumToColor(SalmonColor salmonColor)
    {
        // ※ご自身のゲームの仕様に合わせて色（RGB）を調整してください！
        return salmonColor switch
        {
            SalmonColor.Orange => Color.orange, // オレンジ
            SalmonColor.Red => Color.red,
            SalmonColor.Blue => Color.blue,
            SalmonColor.Pink => Color.pink,
            SalmonColor.Green => Color.green,
            SalmonColor.Purple => Color.purple,
            SalmonColor.SkyBlue => Color.skyBlue,
            SalmonColor.Yellow => Color.yellow,
            SalmonColor.YellowGreen => Color.yellowGreen,
            _ => Color.white // デフォルト
        };
    }
    
    public void Stop()
    {
        _isPlaying = false;
        _view.SetPlayingState(false);
    }

    private void AddCombo() { _playerCtx.ComboCount++; OnComboChanged?.Invoke(_playerCtx.ComboCount); }
    private void ResetCombo() { _playerCtx.ComboCount = 0; OnComboChanged?.Invoke(0); }
    
    public void Dispose()
    {
        _view.OnTriggerHit -= HandleTriggerHit;
        _view.OnJumpTriggered -= HandleJumpTriggered;
    }
}