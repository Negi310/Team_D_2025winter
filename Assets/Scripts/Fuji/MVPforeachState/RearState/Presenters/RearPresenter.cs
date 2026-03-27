using System;
using System.Collections.Generic;
using UnityEngine;

public class RearPresenter : IDisposable
{
    private readonly RearState _state;
    private readonly RearModel _model;
    private readonly ForUIStatusBuilder _builder;
    private readonly SessionContext _sessionContext;
    private readonly SeaUIManager _view;
    private readonly EventPool _eventPool;

    private EventData[] _currentEvents;

    public RearPresenter(RearState state, RearModel model, ForUIStatusBuilder builder, SessionContext sessionContext, SeaUIManager view, EventPool eventPool)
    {
        _state = state; _model = model; _builder = builder;
        _sessionContext = sessionContext; _view = view; _eventPool = eventPool;

        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
        _view.OnEventButtonClicked += HandleEventClicked;
    }

    private void HandleEntered()
    {
        AudioManager.I.PlayBGM(BGM.Name.Stage_2);
        _view.Show();
        _sessionContext.IncrementTurn();
        _state.TransitionCheck(_sessionContext.CurrentTurn);
        
        _currentEvents = _model.GenerateAllEvents(_eventPool);

        var stats = _sessionContext.CurrentSalmon.UpstreamStats;
        var ct = _sessionContext.CurrentSalmon.CourtshipTraits;

        var playerStatsStr = _builder.PlayerStatusListBuild(
            Mathf.FloorToInt(stats.Speed).ToString(), Mathf.FloorToInt(stats.Jump).ToString(),
            Mathf.FloorToInt(stats.Stamina).ToString(), Mathf.FloorToInt(stats.Attack).ToString(),
            Mathf.FloorToInt(stats.Intelligence).ToString(),
            ct.Color.ToString(), ct.Size.ToString(), ct.GetShapeFeatureName(isMale: true)
        );

        var baseIncreases = new List<string>();
        for (int i = 0; i < 5; i++) baseIncreases.Add(_currentEvents[i].BaseModifier.Value.ToString());

        var randomEventNames = new List<string>();
        for (int i = 5; i < 8; i++) randomEventNames.Add(_currentEvents[i].Title);

        var riverStatus = _builder.RiverInformatinListBuild(
            _sessionContext.CurrentRiver.DisplayDanger.ToString("F1"), _sessionContext.CurrentRiver.DisplayComplexity.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayMeandering.ToString("F1"), _sessionContext.CurrentRiver.DisplayRichness.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayToughness.ToString("F1")
        );

        // ★修正: 引数に ct.Size を追加（isPale の次、randomEventNames の前）
        _view.SetUpUI(
            playerStatsStr, riverStatus, baseIncreases, 
            _sessionContext.CurrentTurn, _sessionContext.CurrentRiver.RiverName,
            ct.Hair, ct.Color, ct.MaleEye, ct.MaleEyebrow, ct.MaleMouth, 
            false, ct.Size, randomEventNames
        );
    }
    
    private void HandleExited() => _view.Hide();

    private void HandleEventClicked(int eventIndex)
    {
        EventData selectedEvent = _currentEvents[eventIndex];
        SalmonData updatedSalmon = _model.ApplyEventResult(_sessionContext.CurrentSalmon, selectedEvent);
        _sessionContext.UpdateCurrentSalmon(updatedSalmon);
        AudioManager.I.PlaySE(SE.Name.Click);
        var payload = new ConversationInitPayload(selectedEvent);
        _state.TransitionCheck(payload);
    }

    public void Dispose()
    {        
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _view.OnEventButtonClicked -= HandleEventClicked;
    }
}