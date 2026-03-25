using System;
using UnityEngine;

public class NamePresenter : IDisposable
{
    private readonly NameState _state;
    private readonly NameModel _model;
    private readonly ForUIStatusBuilder _builder;
    private readonly NamingUIManager _view;
    private readonly SessionContext _sessionContext;

    public NamePresenter(NameState state, NameModel model, ForUIStatusBuilder builder, NamingUIManager view, SessionContext sessionContext)
    {
        _state = state;
        _model = model;
        _builder = builder;
        _view = view;
        _sessionContext = sessionContext;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;

        _view.OnDecideButtonClicked += HandleDecided;
    }
    
    private void HandleEntered()
    {
        // 名前入力UIの初期化
        _view.Show();
        var playerStatusList = _builder.PlayerStatusListBuild(
            _sessionContext.CurrentSalmon.UpstreamStats.Speed.ToString(),
            _sessionContext.CurrentSalmon.UpstreamStats.Jump.ToString(),
            _sessionContext.CurrentSalmon.UpstreamStats.Stamina.ToString(),
            _sessionContext.CurrentSalmon.UpstreamStats.Attack.ToString(),
            _sessionContext.CurrentSalmon.UpstreamStats.Intelligence.ToString(),
            _sessionContext.CurrentSalmon.CourtshipTraits.Size.ToString(),
            _sessionContext.CurrentSalmon.CourtshipTraits.ColorValue.ToString(),
            _sessionContext.CurrentSalmon.CourtshipTraits.ShapeValue.ToString());
        //メソッドの引数追加につきエラーが出るため一旦引数を入れておきます　お手数ですが修正よろしくお願いします　貝原
        _view.SetUpUI(playerStatusList,SalmonHair.Normal,SalmonEyeMale.Normal,SalmonColor.Orange,SalmonEyebrowMale.Normal,SalmonMouthMale.Normal,false);
    }
    
    private void HandleExited()
    {
        _view.Hide();
    }

    private void HandleDecided(string inputName)
    {
        // 名前を適用した新しい鮭データを作成
        SalmonData namedSalmon = _model.ApplyNameToSalmon(_sessionContext.CurrentSalmon, inputName);
        _sessionContext.UpdateCurrentSalmon(namedSalmon);
        _state.TransitionCheck(true);
    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;

        _view.OnDecideButtonClicked -= HandleDecided;
    }
}