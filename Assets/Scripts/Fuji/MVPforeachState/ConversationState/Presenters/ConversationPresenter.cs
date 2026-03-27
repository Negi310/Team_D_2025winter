using System;

public class ConversationPresenter : IDisposable
{
    private readonly ConversationState _state; // ライフサイクルと画面遷移の窓口
    private readonly ConversationModel _model;      // ロジック
    private readonly SessionContext _sessionContext;
    private readonly ConversationContext _context;        // 変数
    private readonly ConversationView _view;        // 画面描画と非同期のUI処理
    private readonly IPayload _payload;
    
    private string _resultMessage;
    private bool _isShowingResult;

    public ConversationPresenter(
        ConversationState state, 
        ConversationModel model,
        SessionContext sessionContext,
        ConversationContext context, 
        ConversationView view,
        IPayload payload)
    {
        _state = state;
        _model = model;
        _sessionContext = sessionContext;
        _context = context;
        _view = view;
        _payload = payload;
        if (_payload is ConversationInitPayload conversationInitPayload) _context.MasterData = conversationInitPayload.EventData; // 会話イベントのマスターデータをContextにセット

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
        if (_payload is ConversationInitPayload payload)
        {
            _context.MasterData = payload.EventData;
            _context.CurrentIndex = 0;

            _resultMessage = _model.GenerateResultMessage(payload.EventData);
            _isShowingResult = false;
        }

        var ct = _sessionContext.CurrentSalmon.CourtshipTraits;
        var isPale = false;
        AudioManager.I.PlayBGM(BGM.Name.Stage_4);
        _view.ShowUI();
        _view.SetupEnvironment(_context.MasterData.LinkedConversation.backgroundImage);
        _view.SetupPlayerIllust(ct.Hair, ct.MaleEye, ct.Color, ct.MaleEyebrow, ct.MaleMouth, isPale, ct.Size);
        _view.PlayIntroAnimation(_context.MasterData.LinkedConversation.introAnimationTrigger);
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
            else if (!_isShowingResult)
            {
                // 会話は終わったが、リザルトが未表示の場合
                _isShowingResult = true;
                _context.IsTyping = true; // リザルトも文字送りアニメーションさせる

                // 話者名を「システム」等にして、保持していたリザルトメッセージを流し込む
                _view.StartTyping("", _resultMessage, _context.MasterData.LinkedConversation.textSpeed);
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
        _view.EndTyping();
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
        _view.StartTyping(turn.SpeakerName, turn.Text, _context.MasterData.LinkedConversation.textSpeed);
    }

    // Viewから「文字送りが終わった（またはスキップされた）」と報告が来た時
    private void HandleTypingCompleted()
    {
        _context.IsTyping = false; // Data(変数)を更新して次のクリックに備える
    }
}