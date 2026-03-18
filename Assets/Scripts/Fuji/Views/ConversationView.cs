using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening; // DOTweenを使用

public class ConversationView : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private Animator _characterAnimator;

    // Presenterへ通知するイベント
    public event Action OnNextButtonClicked;
    public event Action OnIntroAnimationFinished;
    public event Action OnTypingCompleted;

    // UI Toolkitの要素
    private VisualElement _rootContainer;
    private Label _speakerNameLabel;
    private Label _dialogueLabel;
    private VisualElement _backgroundElement;
    private Button _nextButton;

    // 文字送り用
    private CancellationTokenSource _typingCts;
    private string _currentSpeaker;
    private string _currentFullText;

    private void Awake()
    {
        // 1. UI Toolkitの要素を取得 (Query)
        var root = _uiDocument.rootVisualElement;
        _rootContainer = root.Q<VisualElement>("ConversationContainer"); // ウィンドウ全体
        _speakerNameLabel = root.Q<Label>("SpeakerName");
        _dialogueLabel = root.Q<Label>("DialogueText");
        _backgroundElement = root.Q<VisualElement>("Background");
        _nextButton = root.Q<Button>("NextButton");

        // ボタンのクリックイベントをPresenterへ横流しする
        _nextButton.clicked += () => OnNextButtonClicked?.Invoke();

        // 初期状態は透明にしておく
        _rootContainer.style.opacity = 0f;
    }

    public void SetupEnvironment(Sprite bg, AudioClip bgm)
    {
        if (bg != null)
        {
            // UI Toolkitでの背景画像セットアップ
            _backgroundElement.style.backgroundImage = new StyleBackground(bg);
        }
        // BGMの処理は省略（AudioSource等で再生）
    }

    // --- DOTweenを使ったUI演出 ---
    public void ShowUI()
    {
        // UI Toolkitのopacity(透明度)をDOTweenでアニメーション
        DOTween.To(
            () => _rootContainer.style.opacity.value,
            x => _rootContainer.style.opacity = x,
            1f, // 目標値 (不透明)
            0.5f // かける秒数
        ).SetEase(Ease.OutQuad);
    }

    public void HideUI()
    {
        DOTween.To(
            () => _rootContainer.style.opacity.value,
            x => _rootContainer.style.opacity = x,
            0f, // 目標値 (透明)
            0.5f
        ).SetEase(Ease.InQuad);
    }

    // --- UniTaskを使ったアニメーション待機 ---
    public void PlayIntroAnimation(string triggerName)
    {
        PlayIntroAnimationAsync(triggerName).Forget();
    }

    private async UniTaskVoid PlayIntroAnimationAsync(string triggerName)
    {
        if (!string.IsNullOrEmpty(triggerName) && _characterAnimator != null)
        {
            _characterAnimator.SetTrigger(triggerName);
            // 登場アニメーションの尺分待機（本来はAnimation Event推奨）
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        }
        
        // 待機が終わったらPresenterに通知
        OnIntroAnimationFinished?.Invoke();
    }

    // --- UniTaskを使った文字送り演出 ---
    public void StartTyping(string speaker, string text, float speed)
    {
        _currentSpeaker = speaker;
        _currentFullText = text;

        _speakerNameLabel.text = speaker;
        _dialogueLabel.text = "";

        // 前回の文字送りが残っていればキャンセル
        _typingCts?.Cancel();
        _typingCts?.Dispose();
        _typingCts = new CancellationTokenSource();

        TypeTextAsync(speed, _typingCts.Token).Forget();
    }

    private async UniTaskVoid TypeTextAsync(float speed, CancellationToken token)
    {
        try
        {
            foreach (char c in _currentFullText)
            {
                _dialogueLabel.text += c;
                // 設定された速度ごとに待機
                await UniTask.Delay(TimeSpan.FromSeconds(speed), cancellationToken: token);
            }

            // 最後まで表示しきったらPresenterへ通知
            OnTypingCompleted?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // スキップされてキャンセルされた場合はここで処理が止まる
            // 全文表示は SkipTyping() 側で行うため、ここでは何もしない
        }
    }

    public void SkipTyping()
    {
        // 実行中の文字送り(UniTask)をキャンセル
        _typingCts?.Cancel();

        // 瞬時に全文を表示
        _dialogueLabel.text = _currentFullText;

        // Presenterへ文字送り完了（スキップ完了）を通知
        OnTypingCompleted?.Invoke();
    }

    private void OnDestroy()
    {
        _typingCts?.Cancel();
        _typingCts?.Dispose();
    }
}