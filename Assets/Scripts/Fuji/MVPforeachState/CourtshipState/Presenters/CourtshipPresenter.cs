using System;

public class CourtshipPresenter : IDisposable
{
    private readonly CourtshipState _state;
    private readonly IBreedingCalculator _breedingCalculator;
    private readonly ICourtshipEvaluatable _courtshipEvaluatable;
    private readonly IMateGeneratable _mateGeneratable;
    private readonly SessionContext _sessionContext;
    private readonly CourtshipPhaseContext _courtshipContext;
    private readonly CourtingUIManager _view;
    private readonly CourtshipInitPayload _payload;

    // コンストラクタで「自分の担当State」を直接もらう
    public CourtshipPresenter(CourtshipState state, IBreedingCalculator breedingCalculator,
        ICourtshipEvaluatable courtshipEvaluatable, IMateGeneratable mateGeneratable,
        SessionContext sessionContext, CourtshipPhaseContext courtshipContext,
        CourtingUIManager view, CourtshipInitPayload payload)
    {
        _state = state;
        _breedingCalculator = breedingCalculator;
        _courtshipEvaluatable = courtshipEvaluatable;
        _mateGeneratable = mateGeneratable;
        _sessionContext = sessionContext;
        _courtshipContext = courtshipContext;
        _view = view;
        _payload = payload;
        
        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
    }

    private void HandleEntered()
    {
        
    }

    private void HandleExited()
    {

    }

    private void HandleDecided()
    {
        bool isSuccess = _courtshipEvaluatable.EvaluateCourtship(_sessionContext.CurrentSalmon, _courtshipContext.SelectedMate);

        _state.TransitionCheck(isSuccess);
    }

    public void Dispose()
    {
        // Routerによって殺される時の最終的な後始末
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
    }
}
