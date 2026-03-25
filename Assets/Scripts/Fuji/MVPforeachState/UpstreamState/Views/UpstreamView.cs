using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UpstreamView : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    private VisualElement _staminaBar;
    private Label _comboLabel;
    private Label _distanceLabel;

    private void Awake()
    {
        var root = _uiDocument.rootVisualElement;
        _staminaBar = root.Q<VisualElement>("stamina-bar-fill");
        _comboLabel = root.Q<Label>("combo-label");
        _distanceLabel = root.Q<Label>("distance-label");
        root.style.display = DisplayStyle.None;
    }
        

  public void Init(float maxStamina)
    {
        UpdateStamina(maxStamina, maxStamina);
        UpdateCombo(0);
    }

    public void UpdateStamina(float current, float max)
    {
        float percent = Mathf.Clamp01(current / max) * 100f;
        _staminaBar.style.width = new Length(percent, LengthUnit.Percent);
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