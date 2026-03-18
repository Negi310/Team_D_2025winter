using System;

public class ConversationPresenter : IDisposable
{
    private readonly ConversationState _state; // ライフサイクルと画面遷移の窓口
    private readonly ConversationModel _model;      // ロジック
    private readonly ConversationContext _context;        // 変数
    private readonly ConversationView _view;        // 画面描画と非同期のUI処理

    public ConversationPresenter(
        ConversationState state, 
        ConversationModel model, 
        ConversationContext context, 
        ConversationView view)
    {
        _state = state;
        _model = model;
        _context = context;
        _view = view;

        // State(ステートマシン)のイベントを購読
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;

        // View(UI)のイベントを購読
        _view.OnNextButtonClicked += HandleDecided;
        _view.OnIntroAnimationFinished += ShowCurrentTurn; // アニメ完了で会話開始
        _view.OnTypingCompleted += HandleTypingCompleted;  // 文字送り完了（スキップ含む）
    }

    // Stateに入った時（会話イベント開始）
    private void HandleEntered()
    {
        _view.ShowUI();
        _view.SetupEnvironment(_context.MasterData.backgroundImage, _context.MasterData.bgm);
        
        // アニメーションを指示するだけ（待機はViewの中で勝手にやってくれる）
        _view.PlayIntroAnimation(_context.MasterData.introAnimationTrigger);
    }

    // 「次へ」ボタンが押された時
    private void HandleDecided()
    {
        if (_context.IsTyping)
        {
            // 文字送り中なら、Viewにスキップ（全文即時表示）を指示
            _view.SkipTyping();
        }
        else
        {
            // 文字送りが終わっていれば、Modelに「次があるか」を聞く
            if (_model.HasNextTurn(_context))
            {
                _model.MoveToNextTurn(_context);
                ShowCurrentTurn();
            }
            else
            {
                // 次がなければ、Stateに「会話終わったよ」と報告して遷移を任せる
                _state.TransitionCheck(true); 
            }
        }
    }

    // Stateから抜ける時（会話イベント終了）
    private void HandleExited()
    {
        _view.HideUI();
    }

    // ルーターによって破棄される時の後始末
    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;

        _view.OnNextButtonClicked -= HandleDecided;
        _view.OnIntroAnimationFinished -= ShowCurrentTurn;
        _view.OnTypingCompleted -= HandleTypingCompleted;
    }
    
    // 現在のターンを表示する
    private void ShowCurrentTurn()
    {
        _context.IsTyping = true; // Data(変数)を更新
        var turn = _model.GetCurrentTurn(_context);
        
        // Viewに文字送りを指示
        _view.StartTyping(turn.speakerName, turn.text, _context.MasterData.textSpeed);
    }

    // Viewから「文字送りが終わった（またはスキップされた）」と報告が来た時
    private void HandleTypingCompleted()
    {
        _context.IsTyping = false; // Data(変数)を更新して次のクリックに備える
    }
}