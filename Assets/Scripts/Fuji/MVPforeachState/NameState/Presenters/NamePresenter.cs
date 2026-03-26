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
        _view.Show();
        
        var us = _sessionContext.CurrentSalmon.UpstreamStats;
        var ct = _sessionContext.CurrentSalmon.CourtshipTraits;

        var playerStatusList = _builder.PlayerStatusListBuild(
            us.Speed.ToString("F1"), us.Jump.ToString("F1"), us.Stamina.ToString("F1"), us.Attack.ToString("F1"), us.Intelligence.ToString("F1"),
            ct.Size.ToString(), ct.Color.ToString(), ct.GetShapeFeatureName(isMale: true));
        
        // ★修正: 引数の最後に ct.Size を追加
        _view.SetUpUI(playerStatusList, ct.Hair, ct.MaleEye, ct.Color, ct.MaleEyebrow, ct.MaleMouth, false, ct.Size);
    }
    
    private void HandleExited()
    {
        _view.Hide();
    }

    private void HandleDecided(string inputName)
    {
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