using System;
using System.Collections.Generic;
using UnityEngine;

public class CourtshipPresenter : IDisposable
{
    private readonly CourtshipState _state;
    private readonly IBreedingCalculator _breedingCalculator;
    private readonly ICourtshipEvaluatable _courtshipEvaluatable;
    private readonly IMateGeneratable _mateGeneratable;
    private readonly ForUIStatusBuilder _builder;
    private readonly SessionContext _sessionContext;
    private readonly CourtshipContext _courtshipContext;
    private readonly CourtingUIManager _view;
    private readonly CourtshipInitPayload _payload;
    private readonly MateGenerationSettingsSO _mateSettings;
    
    private int _remainTimes = 3;
    private bool _isProcessingClick = false; 
    private bool _isPale = false;
    
    // ★修正: 型に SalmonSize を追加
    private List<(SalmonHair, SalmonEyeFemale, SalmonColor, SalmonEyebrowFemale, SalmonMouthFemale, SalmonSize)> _femaleIllustList;

    public CourtshipPresenter(CourtshipState state, IBreedingCalculator breedingCalculator,
        ICourtshipEvaluatable courtshipEvaluatable, IMateGeneratable mateGeneratable,
        ForUIStatusBuilder builder, SessionContext sessionContext, CourtshipContext courtshipContext,
        CourtingUIManager view, IPayload payload, MateGenerationSettingsSO mateSettings)
    {
        _state = state; _breedingCalculator = breedingCalculator;
        _courtshipEvaluatable = courtshipEvaluatable; _mateGeneratable = mateGeneratable;
        _builder = builder; _sessionContext = sessionContext;
        _courtshipContext = courtshipContext; _view = view; _mateSettings = mateSettings;
        _payload = payload is CourtshipInitPayload p ? p : new CourtshipInitPayload();
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
    }

    private void HandleEntered()
    {
        _remainTimes = 3;
        _isPale = false;
        _isProcessingClick = false;

        _view.OnPartnerClicked -= HandlePartnerClicked;
        _view.OnPartnerClicked += HandlePartnerClicked;

        _courtshipContext.LastRunScore = _payload.DistanceTraveled;
        _courtshipContext.Candidates = _mateGeneratable.GenerateCandidates(_payload.DistanceTraveled, _mateSettings);

        // ★修正: リスト構築時に Size も追加する
        _femaleIllustList = new List<(SalmonHair, SalmonEyeFemale, SalmonColor, SalmonEyebrowFemale, SalmonMouthFemale, SalmonSize)>();
        foreach (var candidate in _courtshipContext.Candidates)
        {
            var t = candidate.CourtshipTraits;
            _femaleIllustList.Add((t.Hair, t.FemaleEye, t.Color, t.FemaleEyebrow, t.FemaleMouth, t.Size));
        }

        _sessionContext.UpdateCurrentRiver(RiverGenerator.GenerateRiver(_sessionContext.CurrentGeneration + 1));
        
        UpdateUI();
        _view.Show();
    }

    private void HandlePartnerClicked(int index)
    {
        if (_remainTimes <= 0 || _isProcessingClick) return; 
        _isProcessingClick = true;

        _courtshipContext.SelectedMate = _courtshipContext.Candidates[index];
        bool isSuccess = _courtshipEvaluatable.EvaluateCourtship(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);

        if (isSuccess)
        {
            SalmonData child = _breedingCalculator.GenerateChild(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);
            _sessionContext.AdvanceToNextGeneration(child);
            _state.TransitionCheck(true);
        }
        else
        {
            _remainTimes--;
            _isPale = true;

            if (_remainTimes <= 0)
            {
                SalmonData child = _breedingCalculator.GenerateChild(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);
                _sessionContext.AdvanceToNextGeneration(child);
                _state.TransitionCheck(true);
            }
            else
            {
                UpdateUI();
                _isProcessingClick = false; 
            }
        }
    }

    private void UpdateUI()
    {
        var us = _sessionContext.CurrentSalmon.UpstreamStats;
        var ct = _sessionContext.CurrentSalmon.CourtshipTraits; 
        
        var playerStatusList = _builder.PlayerStatusListBuild(
            us.Speed.ToString("F1"), us.Jump.ToString("F1"), us.Stamina.ToString("F1"), us.Attack.ToString("F1"), us.Intelligence.ToString("F1"),
            ct.Size.ToString(), ct.Color.ToString(), ct.GetShapeFeatureName(isMale: true)); 

        var c = _courtshipContext.Candidates;
        var partnerStatusList = new List<(string personality, string successRate)>();
        
        for (int i = 0; i < 5; i++)
        {
            float rate = _courtshipEvaluatable.CalculateSuccessRate(_sessionContext.CurrentSalmon, c[i]);
            string successRateStr = Mathf.RoundToInt(rate * 100).ToString();
            partnerStatusList.Add((c[i].CourtshipTraits.GetShapeFeatureName(isMale: false), successRateStr)); 
        }

        var riverStatusList = _builder.RiverInformatinListBuild(
            _sessionContext.CurrentRiver.DisplayDanger.ToString("F1"), _sessionContext.CurrentRiver.DisplayComplexity.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayMeandering.ToString("F1"), _sessionContext.CurrentRiver.DisplayRichness.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayToughness.ToString("F1")
        );

        // ★修正: 引数に ct.Size を追加
        _view.SetUpUI(
            playerStatusList, partnerStatusList, _femaleIllustList, 
            ct.Hair, ct.MaleEye, ct.Color, ct.MaleEyebrow, ct.MaleMouth, 
            _isPale, ct.Size, riverStatusList, _remainTimes, _sessionContext.CurrentRiver.RiverName
        );
    }

    private void HandleExited()
    {
        _view.Hide();
        _view.OnPartnerClicked -= HandlePartnerClicked;
    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
        _view.OnPartnerClicked -= HandlePartnerClicked;
    }
}