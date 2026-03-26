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

    // 今回生成されたイベント群（0-4:特訓, 5-7:ランダム）を保持する
    private EventData[] _currentEvents;

    public RearPresenter(RearState state, RearModel model, ForUIStatusBuilder builder, SessionContext sessionContext, SeaUIManager view, EventPool eventPool)
    {
        _state = state;
        _model = model;
        _builder = builder;
        _sessionContext = sessionContext;
        _view = view;
        _eventPool = eventPool;

        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
        
        // ★ イベントの購読変更
        _view.OnEventButtonClicked += HandleEventClicked;
    }

    private void HandleEntered()
    {
        _view.Show();
        _sessionContext.IncrementTurn();
        _state.TransitionCheck(_sessionContext.CurrentTurn);
        // 8つのイベント（固定5 + ランダム3）を生成して保存
        _currentEvents = _model.GenerateAllEvents(_eventPool);

        // 鮭の現在のステータスを文字列リスト化（順番: Speed, Jump, Stamina, Attack, Intelligence）
        var currentSalmon = _sessionContext.CurrentSalmon;
        var stats = currentSalmon.UpstreamStats;
        var traits = currentSalmon.CourtshipTraits;
        var playerStatsStr = new List<string> {
            Mathf.FloorToInt(stats.Speed).ToString(),
            Mathf.FloorToInt(stats.Jump).ToString(),
            Mathf.FloorToInt(stats.Stamina).ToString(),
            Mathf.FloorToInt(stats.Attack).ToString(),
            Mathf.FloorToInt(stats.Intelligence).ToString(),
            Mathf.FloorToInt(traits.Size).ToString(),
            Mathf.FloorToInt(traits.ColorValue).ToString(),
            Mathf.FloorToInt(traits.ShapeValue).ToString()
        };

        // UI表示用の上昇量（イベント0〜4のBaseGainを抜き出す）
        var baseIncreases = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            baseIncreases.Add(_currentEvents[i].BaseModifier.Value.ToString());
        }

        // ランダムイベント名のリスト
        var randomEventNames = new List<string>();
        for (int i = 5; i < 8; i++)
        {
            randomEventNames.Add(_currentEvents[i].Title);
        }

        // （※RiverStatusや外見など、必要に応じてコンテキストから実データを渡してください）
        var riverStatus = _builder.RiverInformatinListBuild(
            _sessionContext.CurrentRiver.DisplayDanger.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayComplexity.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayMeandering.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayRichness.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayToughness.ToString("F1")
        );

        // ★ Viewのセットアップを呼び出し
        _view.SetUpUI(
            playerStatsStr, riverStatus, baseIncreases, _sessionContext.CurrentTurn, 
        );
    }
    
    private void HandleExited()
    {
        _view.Hide();
    }

    // ★ ボタンが押されたときの処理
    private void HandleEventClicked(int eventIndex)
    {
        // 配列から対応するイベントを取り出す
        EventData selectedEvent = _currentEvents[eventIndex];

        // 1. パラメータに反映し、SessionContextの鮭を上書き保存
        SalmonData updatedSalmon = _model.ApplyEventResult(_sessionContext.CurrentSalmon, selectedEvent);
        _sessionContext.UpdateCurrentSalmon(updatedSalmon);

        // 2. 次のステート(Conversation)へ、選ばれた会話を渡す
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