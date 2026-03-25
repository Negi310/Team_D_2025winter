using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class TitleView : MonoBehaviour
{
    public Action OnStartButtonPressed;
    
    [SerializeField] private UIDocument _uiDocument;

    private VisualElement _root;
    private Button _pressAnyButton;
    private Tween _blinkTween;

    void Awake()
    {
        _root = _uiDocument.rootVisualElement;

        // UXML内で設定した名前（Name）で要素を検索
        // ※UIBuilderでLabelを作成し、名前を "press-any-button" に設定してください
        _pressAnyButton = _root.Q<Button>("start-button");
        _pressAnyButton.clicked += () => OnStartButtonPressed?.Invoke();
        // 初期化時は非表示にしておく
        Hide();
    }
    
    public void Show()
    {
        // 画面全体を表示（Flex）にする
        _root.style.display = DisplayStyle.Flex;

        // DOTweenを使ってUIToolkitの不透明度（Opacity）をアニメーションさせる
        if (_pressAnyButton != null)
        {
            _pressAnyButton.style.opacity = 1f;
            
            // 1.0(完全表示)から0.0(透明)へ、0.8秒かけてループさせる点滅演出
            _blinkTween = DOTween.To(
                () => _pressAnyButton.style.opacity.value, // 現在の値の取得
                x => _pressAnyButton.style.opacity = x,    // 値の代入
                1f,                                             // 目標値
                0.3f                                            // 時間
            ).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }

    public void Hide()
    {
        // アニメーションを安全に停止・破棄する
        if (_blinkTween != null)
        {
            _blinkTween.Kill();
            _blinkTween = null;
        }

        // 画面全体を非表示（None）にする
        if (_root != null)
        {
            _root.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時の安全対策
        if (_blinkTween != null) _blinkTween.Kill();
    }
}