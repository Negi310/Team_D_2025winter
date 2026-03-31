using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform _target; // 追従するプレイヤー
    [SerializeField] private float _yOffset = 3f; // プレイヤーより少し「上」を映すためのズレ

    [Header("Follow Settings")]
    [SerializeField] private float _smoothTime = 0.15f; // カメラが追いつくまでの時間（滑らかさ）
    [SerializeField] private bool _isOneWayScroll = true; // 下（後ろ）に戻らないようにするかどうか

    private Vector3 _currentVelocity;
    private float _maxYReached = float.MinValue; // 到達した最高のY座標

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 currentPos = transform.position;
        float targetY = _target.position.y + _yOffset;

        // 一方通行スクロール（ノックバック時にカメラが下がるのを防ぐ）
        if (_isOneWayScroll)
        {
            if (targetY > _maxYReached)
            {
                _maxYReached = targetY; // 最高到達点を更新
            }
            else
            {
                targetY = _maxYReached; // 最高到達点より下には行かない
            }
        }

        // XとZは現在のカメラの位置を維持し、Yだけ目標座標へ
        Vector3 targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        // Vector3.SmoothDamp を使うことで、ピタッと止まらず滑らかに追従します
        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref _currentVelocity, _smoothTime);
    }

    // ==========================================
    // 外部（Presenter等）からターゲットをセットする用
    // ==========================================
    public void SetTarget(Transform target)
    {
        _target = target;
        
        // ターゲットが設定された瞬間は、滑らかさ無視で即座にワープさせる
        Vector3 currentPos = transform.position;
        float startY = _target.position.y + _yOffset;
        transform.position = new Vector3(currentPos.x, startY, currentPos.z);
        _maxYReached = startY;
    }
    
    public void ResetCamera()
    {
        if (_target == null) return;

        // SmoothDampの慣性（Velocity）をゼロにする
        _currentVelocity = Vector3.zero;

        // カメラをプレイヤーの初期位置へ瞬間移動させる
        float startY = _target.position.y + _yOffset;
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        
        // ★最高到達点の記憶をリセット！
        _maxYReached = startY; 
    }
}