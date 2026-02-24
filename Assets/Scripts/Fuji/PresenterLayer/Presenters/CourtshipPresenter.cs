using System;

public class CourtshipPresenter : IDisposable
{
    private readonly CourtshipState _state;

    // コンストラクタで「自分の担当State」を直接もらう
    public CourtshipPresenter(CourtshipState state)
    {
        _state = state;
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
        //bool isSuccess = _model.EvaluateCourtship();
        
        //_state.TransitionCheck(isSuccess);
    }
    
    public void Dispose()
    {
        // Routerによって殺される時の最終的な後始末
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
    }
}
