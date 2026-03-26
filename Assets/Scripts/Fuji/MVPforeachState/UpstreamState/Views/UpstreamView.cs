using System;
using UnityEngine;
using UnityEngine.UIElements;

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
        root.style.display = DisplayStyle.None;
    }
        

  public void Init(float maxStamina)
    {
        //_staminaBar.lowValue = 0f;
        //_staminaBar.highValue = maxStamina;
        UpdateStamina(maxStamina, maxStamina);
        UpdateCombo(0);
        _cameraFollow.ResetCamera();
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
        _comboLabel.text = combo > 1 ? $"{combo} COMBO!" : "";
    }

    public void UpdateDistance(float distance)
    {
        _distanceLabel.text = $"{Mathf.FloorToInt(distance)}m";
    }

    public void Show() => _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    public void Hide() => _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
}