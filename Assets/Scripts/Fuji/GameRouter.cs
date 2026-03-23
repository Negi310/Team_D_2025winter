using System;

public class GameRouter : IDisposable
{
    private readonly GameStateMachine _stateMachine;
    private readonly PresentersFactory _factory;

    private IDisposable _currentPresenters;

    public GameRouter(GameStateMachine stateMachine, PresentersFactory factory)
    {
        _stateMachine = stateMachine;
        _factory = factory;

        _stateMachine.OnStateChanged += HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        // 前のフェーズのPresenterを確実に破棄（UIの隠蔽とメモリ解放）
        _currentPresenters?.Dispose();

        _currentPresenters = _factory.CreatePresentersFor(newState);
    }
    
    public void Dispose()
    {
        _stateMachine.OnStateChanged -= HandleStateChanged;
        _currentPresenters?.Dispose();
    }
}