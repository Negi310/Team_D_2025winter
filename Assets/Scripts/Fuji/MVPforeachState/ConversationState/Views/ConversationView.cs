using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening; // DOTweenを使用

public class ConversationView : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    // ★追加: 立ち絵設定用のマネージャー
    [SerializeField] private MaleIllustrationManager _maleIllust;

    // Presenterへ通知するイベント
    public event Action OnNextButtonClicked;
    public event Action OnIntroAnimationFinished;
    public event Action OnTypingCompleted;

    // UI Toolkitの要素
    private VisualElement _root;
    private VisualElement _rootContainer;
    private Label _speakerNameLabel;
    private Label _dialogueLabel;
    private Button _nextButton;
    
    // ★追加: プレイヤー立ち絵のVE
    private VisualElement _playerIllust;

    // 文字送り用
    private CancellationTokenSource _typingCts;
    private string _currentSpeaker;
    private string _currentFullText;

    private void Awake()
    {
        // 1. UI Toolkitの要素を取得 (Query)
        _root = _uiDocument.rootVisualElement;
        _rootContainer = _root.Q<VisualElement>("ConversationContainer"); // ウィンドウ全体
        _speakerNameLabel = _root.Q<Label>("Speaker");
        _dialogueLabel = _root.Q<Label>("DialogueText");
        
        // （構成によってNextButton自体がボタンか、中にボタンがあるかで取得）
        var nextButtonEl = _root.Q<VisualElement>("NextButton");
        _nextButton = nextButtonEl as Button ?? nextButtonEl?.Q<Button>();

        // ボタンのクリックイベントをPresenterへ横流しする
        if (_nextButton != null)
        {
            _nextButton.clicked += () => OnNextButtonClicked?.Invoke();
        }

        // ★追加: 立ち絵のVE取得と初期設定
        _playerIllust = _root.Q<VisualElement>(className:"male-illustration_base");
        Debug.Log(_playerIllust != null);
        if (_maleIllust != null && _playerIllust != null)
        {
            _maleIllust.InitIllust(_playerIllust);
        }

        // 初期状態は透明にしておく
        _rootContainer.style.opacity = 0f;
        _root.style.display = DisplayStyle.None;
    }
    
    public void SetupEnvironment(Sprite bg)
    {
        if (bg != null)
        {
            // ★変更: 背景要素を使わず、直接 _rootContainer の背景に設定する
            _rootContainer.style.backgroundImage = new StyleBackground(bg);
        }
        // BGMの処理は省略（AudioSource等で再生）
    }
    
    // ★追加: プレイヤーのイラストを適用するメソッド（Presenterから呼ぶ）
    public void SetupPlayerIllust(SalmonHair hair, SalmonEyeMale eye, SalmonColor color, SalmonEyebrowMale eyebrow, SalmonMouthMale mouth, bool isPale, SalmonSize size)
    {
        if (_maleIllust != null && _playerIllust != null)
        {
            _maleIllust.SetUpIllust(_playerIllust, hair, eye, color, eyebrow, mouth, isPale, size);
            Debug.Log(_playerIllust != null);
        }
    }

    // --- DOTweenを使ったUI演出 ---
    public void ShowUI()
    {
        _root.style.display = DisplayStyle.Flex;
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
        ).SetEase(Ease.InQuad).OnComplete(() => 
        {
            // ★修正: アニメーションが完全に終わってから非表示にする
            _root.style.display = DisplayStyle.None;
        });
    }

    // --- UniTaskを使ったアニメーション待機 ---
    public void PlayIntroAnimation(string triggerName)
    {
        PlayIntroAnimationAsync(triggerName).Forget();
    }

    private async UniTaskVoid PlayIntroAnimationAsync(string triggerName)
    {
        if (!string.IsNullOrEmpty(triggerName))
        {
            //_characterAnimator.SetTrigger(triggerName);
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
    
    public void EndTyping() => _dialogueLabel.text = "";

    private void OnDestroy()
    {
        _typingCts?.Cancel();
        _typingCts?.Dispose();
    }
}