using System;
using UnityEngine;
using DG.Tweening;

public class SalmonMove : MonoBehaviour
{
    public event Action<GameObject> OnTriggerHit;
    public event Action OnJumpTriggered;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Transform _visualTransform;

    private RayManager _raySensor;
    private Rigidbody2D _rb;

    private float _speedX, _speedY;
    private bool _isJumping;
    private bool _isPlaying;
    private bool _isTakingDamage; // 被弾中フラグ

    public Vector2 Position => transform.position;

    private void Awake()
    {
        _raySensor = GetComponent<RayManager>();
        _rb = GetComponent<Rigidbody2D>();

        // 物理演算の安定化（コードからも強制設定）
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 壁のすり抜け防止
    }

    public RayData GetSensorData() => _raySensor.MeasureEnvironment();
    public void UpdateSpeeds(float speedX, float speedY) { _speedX = speedX; _speedY = speedY; }

    public void SetJumpingState(bool isJumping)
    {
        _isJumping = isJumping;
        
        if (isJumping)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerJumping");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player"); // 元のレイヤー名に合わせてください
        }
    }

    public void SetPlayingState(bool isPlaying)
    {
        _isPlaying = isPlaying;
        if (!_isPlaying) _rb.linearVelocity = Vector2.zero; // 死んだらピタッと止まる
    }

    // ==========================================
    // 入力の受け付け (Update)
    // ==========================================

    // ==========================================
    // 物理的な移動 (FixedUpdate)
    // ==========================================
    private void Update()
    {
        if (!_isPlaying || _isTakingDamage)
        {
            if (_isTakingDamage) _rb.linearVelocity = Vector2.zero; // 被弾中は通常の移動をキャンセル
            return;
        }
        
        if (Input.GetButtonDown("Jump") && !_isJumping)
        {
            OnJumpTriggered?.Invoke();
        }

        // ★ジャンプ中は横移動の入力を 0 にする
        float inputX = _isJumping ? 0f : Input.GetAxis("Horizontal");
        
        // ★自動で前に進む（縦入力でわずかに加減速できるようにする）
        float inputY = Input.GetAxis("Vertical");
        float finalSpeedY = _speedY + (inputY * 0f); // ※完全自動が良い場合は _speedY だけにする
        //Debug.Log(_speedX + " , " + finalSpeedY);
        // Rigidbodyのvelocityに代入（※Time.deltaTimeは不要です）
        Vector2 velocity = new Vector2(inputX * _speedX, finalSpeedY);
        _rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D other) => OnTriggerHit?.Invoke(other.gameObject);

    public void PlayJumpAnimation(Action onComplete)
    {
        _visualTransform.DOScale(1.5f, 0.4f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void PlayDamageReaction(float duration, Action onComplete)
    {
        _isTakingDamage = true;
        _rb.linearVelocity = Vector2.zero;

        // ★物理エンジンと競合しないよう、transformではなく _rb.DOMoveY を使う
        _visualTransform.DOLocalMoveY(_visualTransform.localPosition.y - 1.0f, 0.2f)
            .SetLoops(2, LoopType.Yoyo) // 行って帰ってくる（元の位置に戻る）
            .SetEase(Ease.OutCubic)
            .OnComplete(() => _isTakingDamage = false); // 被弾アニメが終わったらフラグを戻す
        
        int flashCount = Mathf.FloorToInt(duration / 0.1f) / 2;
        
        _renderer.DOColor(Color.red, 0.1f).SetLoops(flashCount * 2, LoopType.Yoyo)
            .OnComplete(() => 
            { 
                _renderer.color = Color.white; 
                onComplete?.Invoke();
            });
    }
}