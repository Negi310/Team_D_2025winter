using System;

public class GameRouter : IDisposable
{
    private readonly GameStateMachine _stateMachine;
    private readonly StateCompositeFactory _factory;
    private readonly SessionContext _sessionContext;
    private readonly SaveDataResister _saveDataResister;

    private IDisposable _currentPresenters;

    public GameRouter(GameStateMachine stateMachine, StateCompositeFactory factory, 
                      SessionContext sessionContext, SaveDataResister saveDataResister)   
    {
        _stateMachine = stateMachine;
        _factory = factory;
        _sessionContext = sessionContext;
        _saveDataResister = saveDataResister;

        _stateMachine.OnStateChanged += HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState, IPayload payload)
    {
        // 前のフェーズのPresenterを確実に破棄（UIの隠蔽とメモリ解放）
        _currentPresenters?.Dispose();
        _currentPresenters = _factory.CreatePresentersFor(newState, payload);
        string stateName = newState.GetType().Name;
        SaveData snapshot = _saveDataResister.CreateFromContext(_sessionContext, stateName);
        //_saveDataResister.Save(snapshot);
    }
    
    public void Dispose()
    {
        _stateMachine.OnStateChanged -= HandleStateChanged;
        _currentPresenters?.Dispose();
    }
}