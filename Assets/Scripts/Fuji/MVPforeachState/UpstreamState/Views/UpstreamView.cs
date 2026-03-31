using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks; // ★追加
using DG.Tweening;             // ★追加

public class UpstreamView : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private CameraFollowY _cameraFollow;
    
    private VisualElement _staminaBar;
    //private ProgressBar _staminaBar;
    //private VisualElement _progressFill;     // 伸びるバーの部分
    //private VisualElement _progressBackground;
    private Label _comboLabel;
    private Label _distanceLabel;
    private Label _countdownLabel; // ★追加

    private void Awake()
    {
        var root = _uiDocument.rootVisualElement;
        _staminaBar = root.Q<VisualElement>("stamina-bar-fill");
        // UI ToolkitのProgressBarは、内部的にこれらのクラス名を持った要素で作られています
        //_progressFill = _staminaBar.Q(className: "unity-progress-bar__progress");
        //_progressBackground = _staminaBar.Q(className: "unity-progress-bar__background");

        // （おまけ）背景色をコードから直接指定することも可能（例: 暗いグレー）
        //if (_progressBackground != null)
        //{
            //_progressBackground.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.8f));
        //}
        _staminaBar = root.Q<VisualElement>("stamina-bar-fill");
        _comboLabel = root.Q<Label>("combo-label");
        _distanceLabel = root.Q<Label>("distance-label");
        _countdownLabel = root.Q<Label>("countdown-label");
        if (_countdownLabel != null) _countdownLabel.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.None;
    }
        

  public void Init(float maxStamina)
    {
        //_staminaBar.lowValue = 0f;
        //_staminaBar.highValue = maxStamina;
        UpdateStamina(maxStamina, maxStamina);
        UpdateCombo(0);
        _cameraFollow.ResetCamera();
        _staminaBar.style.backgroundColor = new StyleColor(Color.green);
    }

    public void UpdateStamina(float current, float max)
    {
        float percent = Mathf.Clamp01(current / max) * 100f;
        //_staminaBar.value = current;
        //float percent = current / max;

        _staminaBar.style.width = new Length(percent, LengthUnit.Percent);

        // 2. スタミナ残量に応じて色を変える演出（緑 → 黄 → 赤）
        float colorPercent = current / max;

        if (colorPercent > 0.5f)
        {
            _staminaBar.style.backgroundColor = new StyleColor(Color.green);
        }
        else if (colorPercent > 0.2f)
        {
            _staminaBar.style.backgroundColor = new StyleColor(Color.yellow);
        }
        else
        {
            _staminaBar.style.backgroundColor = new StyleColor(Color.red);
        }
    }

    public void UpdateCombo(int combo)
    {
        _comboLabel.text = $"{combo} COMBO!";
    }

    public void UpdateDistance(float distance)
    {
        _distanceLabel.text = $"{Mathf.FloorToInt(distance)}m";
    }
    
    public async UniTask PlayCountdownAsync()
    {
        if (_countdownLabel == null)
        {
            await UniTask.Delay(1000); // UIがセットされていなければ1秒待機するだけ
            return;
        }

        _countdownLabel.style.display = DisplayStyle.Flex;
        string[] texts = { "3", "2", "1", "GO!" };

        foreach (var t in texts)
        {
            _countdownLabel.text = t;
            
            // 少しポップに弾けるアニメーション
            float scale = 1.5f;
            DOTween.To(() => scale, x => 
            {
                scale = x;
                _countdownLabel.style.scale = new StyleScale(new Vector2(scale, scale));
            }, 1f, 0.5f).SetEase(Ease.OutBack);

            await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
        }

        _countdownLabel.style.display = DisplayStyle.None;
    }

    public void Show() => _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    public void Hide() => _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
}