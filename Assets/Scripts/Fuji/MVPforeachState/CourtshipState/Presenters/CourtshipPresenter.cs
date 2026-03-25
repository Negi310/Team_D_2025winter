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
    }

    private void HandleEntered()
    {
        _view.OnPartnerClicked += HandlePartnerClicked;

        // 1. 候補の生成 (Modelへの依頼)
        _courtshipContext.LastRunScore = _payload.DistanceTraveled;
        _courtshipContext.Candidates = _mateGeneratable.GenerateCandidates(_payload.DistanceTraveled, _mateSettings);

        var us = _sessionContext.CurrentSalmon.UpstreamStats;
        var ct = _sessionContext.CurrentSalmon.CourtshipTraits;
        
        // 2. プレイヤーのステータス整形
        var playerStatusList = _builder.PlayerStatusListBuild(
            us.Speed.ToString(), us.Jump.ToString(), us.Stamina.ToString(), us.Attack.ToString(), us.Intelligence.ToString(),
            ct.Size.ToString(), ct.ColorValue.ToString(), ct.ShapeValue.ToString());

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
            c[0].CourtshipTraits.Size.ToString(), rates[0],
            c[1].CourtshipTraits.Size.ToString(), rates[1],
            c[2].CourtshipTraits.Size.ToString(), rates[2],
            c[3].CourtshipTraits.Size.ToString(), rates[3],
            c[4].CourtshipTraits.Size.ToString(), rates[4]
        );

        // 4. 川のステータス情報（仮）
        var riverStatusList = _builder.RiverInformatinListBuild("1", "1", "1", "1", "1");
        int remainTimes = 3; 
        string riverName = "激流の川"; 

        // 5. Viewへ全データを流し込み、表示させる（ホバー時の表示/非表示はUSSに任せる）
        //_view.SetUpUI(playerStatusList, partnerStatusList, riverStatusList, 
        _view.Show();
    }

    private void HandlePartnerClicked(int index)
    {
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

        // 次のステートへ遷移
        _view.Hide();
        _state.TransitionCheck(isSuccess);
    }

    private void HandleExited()
    {
        _view.OnPartnerClicked -= HandlePartnerClicked;
    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
    }
}
