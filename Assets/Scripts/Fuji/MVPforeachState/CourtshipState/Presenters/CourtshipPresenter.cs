using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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
    
    private SalmonHair _maleHair;
    private SalmonEyeMale _maleEye;
    private SalmonColor _maleColor;
    private SalmonEyebrowMale _maleEyebrow;
    private SalmonMouthMale _maleMouth;
    private bool _isPale = false;
    
    // 女の子のイラスト情報（UI更新で顔が変わらないように保持）
    private List<(SalmonHair, SalmonEyeFemale, SalmonColor, SalmonEyebrowFemale, SalmonMouthFemale)> _femaleIllustList;

    // コンストラクタで「自分の担当State」を直接もらう
    public CourtshipPresenter(CourtshipState state, IBreedingCalculator breedingCalculator,
        ICourtshipEvaluatable courtshipEvaluatable, IMateGeneratable mateGeneratable,
        ForUIStatusBuilder builder,
        SessionContext sessionContext, CourtshipContext courtshipContext,
        CourtingUIManager view, IPayload payload, MateGenerationSettingsSO mateSettings)
    {
        _state = state;
        _breedingCalculator = breedingCalculator;
        _courtshipEvaluatable = courtshipEvaluatable;
        _mateGeneratable = mateGeneratable;
        _builder = builder;
        _sessionContext = sessionContext;
        _courtshipContext = courtshipContext;
        _view = view;
        _payload = payload is CourtshipInitPayload ? (CourtshipInitPayload)payload : default;
        _mateSettings = mateSettings;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
        
        _view.OnPartnerClicked += HandlePartnerClicked;
    }

    private void HandleEntered()
    {
        UpdateUI();
        _view.Show();
    }

    private void HandlePartnerClicked(int index)
    {
        if (_remainTimes <= 0)
        {
            _state.TransitionCheck(false);
        }
        // 決定した相手を保持
        _courtshipContext.SelectedMate = _courtshipContext.Candidates[index];

        // ==========================================
        // ★クリック時（決定時）のみ、乱数による交配処理を行う
        // ==========================================
        bool isSuccess = _courtshipEvaluatable.EvaluateCourtship(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);

        if (isSuccess)
        {
            // 成功なら新世代（子供）を生成し、SessionContext（全体データ）を更新
            SalmonData child = _breedingCalculator.GenerateChild(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);
            _sessionContext.AdvanceToNextGeneration(child);
        }
        else
        {
            _remainTimes--;
        }
        
        _state.TransitionCheck(isSuccess);
    }

    private void UpdateUI()
    {
        _isPale = false;

        // 1. プレイヤーの見た目をランダム決定
        _maleHair = (SalmonHair)Random.Range(0, 3);
        _maleEye = (SalmonEyeMale)Random.Range(0, 3);
        _maleColor = (SalmonColor)Random.Range(0, 9);
        _maleEyebrow = (SalmonEyebrowMale)Random.Range(0, 3);
        _maleMouth = (SalmonMouthMale)Random.Range(0, 3);
        
        _femaleIllustList = new List<(SalmonHair, SalmonEyeFemale, SalmonColor, SalmonEyebrowFemale, SalmonMouthFemale)>();
        for (int i = 0; i < 5; i++)
        {
            _femaleIllustList.Add((
                (SalmonHair)Random.Range(0, 3), 
                (SalmonEyeFemale)Random.Range(0, 3), 
                (SalmonColor)Random.Range(0, 9), 
                (SalmonEyebrowFemale)Random.Range(0, 3), 
                (SalmonMouthFemale)Random.Range(0, 3)
            ));
        }
        // 1. 候補の生成 (Modelへの依頼)
        _courtshipContext.LastRunScore = _payload.DistanceTraveled;
        _courtshipContext.Candidates = _mateGeneratable.GenerateCandidates(_payload.DistanceTraveled, _mateSettings);

        var us = _sessionContext.CurrentSalmon.UpstreamStats;
        var ct = _sessionContext.CurrentSalmon.CourtshipTraits;
        
        // 2. プレイヤーのステータス整形
        var playerStatusList = _builder.PlayerStatusListBuild(
            us.Speed.ToString("F1"), us.Jump.ToString("F1"), us.Stamina.ToString("F1"), us.Attack.ToString("F1"), us.Intelligence.ToString("F1"),
            ct.Size.ToString("F1"), ct.ColorValue.ToString("F1"), ct.ShapeValue.ToString("F1"));

        // ==========================================
        // 3. ★ステート開始時に、5匹全員の成功率を事前計算する
        // ==========================================
        var c = _courtshipContext.Candidates;
        string[] rates = new string[5];
        for (int i = 0; i < 5; i++)
        {
            // 確率計算ロジックを実行し、文字列にして配列にストック
            float rate = _courtshipEvaluatable.CalculateSuccessRate(_sessionContext.CurrentSalmon, c[i]);
            rates[i] = Mathf.RoundToInt(rate * 100).ToString();
        }

        // 事前計算した実データをそのままBuilderに渡す
        var partnerStatusList = _builder.PartnersListBuild(
            c[0].CourtshipTraits.Size.ToString("F1"), rates[0],
            c[1].CourtshipTraits.Size.ToString("F1"), rates[1],
            c[2].CourtshipTraits.Size.ToString("F1"), rates[2],
            c[3].CourtshipTraits.Size.ToString("F1"), rates[3],
            c[4].CourtshipTraits.Size.ToString("F1"), rates[4]
        );

        // 4. 川のステータス情報（仮）
        _sessionContext.UpdateCurrentRiver(RiverGenerator.GenerateRiver(_sessionContext.CurrentGeneration + 1));
        var riverStatusList = _builder.RiverInformatinListBuild(
            _sessionContext.CurrentRiver.DisplayDanger.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayComplexity.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayMeandering.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayRichness.ToString("F1"), 
            _sessionContext.CurrentRiver.DisplayToughness.ToString("F1")
        );
        
        string riverName = _sessionContext.CurrentRiver.RiverName;
        
        _view.SetUpUI(playerStatusList, riverStatusList, 
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
    }
}
