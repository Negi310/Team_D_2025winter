using System;
using System.Collections.Generic;

public class RearPresenter : IDisposable
{
    private readonly RearState _state;
    private readonly RearModel _model;
    private readonly SessionContext _sessionContext;
    private readonly SeaUIManager _view;
    private readonly EventPool _eventPool;

    public RearPresenter(RearState state, RearModel model, SessionContext sessionContext, SeaUIManager view, EventPool eventPool)
    {
        _state = state;
        _model = model;
        _sessionContext = sessionContext;
        _view = view;
        _eventPool = eventPool;

        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
        
        _view.OnEventSelected += HandleEventSelected;
    }

    private void HandleEntered()
    {
        _view.Show();
        // Stateに入ったと同時にターンを1つ進める
        _sessionContext.IncrementTurn();

        // イベントを3つ自動生成してViewに表示
        EventData[] generatedEvents = _model.GenerateAllEvents(_eventPool);
    }
    
    private void HandleExited()
    {
        _view.Hide();
    }

    private void HandleEventSelected(EventData selectedEvent)
    {
        // 1. パラメータに反映し、SessionContextの鮭を上書き保存
        SalmonData updatedSalmon = _model.ApplyEventResult(_sessionContext.CurrentSalmon, selectedEvent);
        _sessionContext.UpdateCurrentSalmon(updatedSalmon);

        // 2. 次のステート(Conversation)へ、選ばれた会話を渡す
        var payload = new ConversationInitPayload(selectedEvent);
        _state.TransitionCheck(payload);
    }
    
    private void HandleEventHovered(EventData choice)
    {
        

        // 計算した結果をViewに渡して表示させる
        //_view.ShowPreview(predictedGains);
    }

    public void Dispose()
    {        
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        
        _view.OnEventSelected -= HandleEventSelected;
    }
}