using System;
using UnityEngine;

public class TitlePresenter : IDisposable
{
    private readonly TitleState _state;
    private readonly TitleView _view;
    private readonly string _initialStateName;

    private bool _isTransitioning; // 連打防止フラグ

    public TitlePresenter(
        TitleState state, 
        TitleView view,
        string initialStateName
        )
    {
        _state = state;
        _view = view;
        _initialStateName = initialStateName;

        _state.OnEnter += HandleEntered;
        _state.OnExit += HandleExited;
        
        _view.OnStartButtonPressed += HandleStartButtonPressed;
    }

    private void HandleEntered()
    {
        _isTransitioning = false;
        _view.Show();
    }

    private void HandleStartButtonPressed()
    {
        if (_isTransitioning) return; // 連打防止
        _isTransitioning = true;
        _state.TransitionCheck(_initialStateName);
    }
    
    private void HandleExited()
    {
        _view.Hide();
    }

    public void Dispose()
    {
        _state.OnEnter -= HandleEntered;
        _state.OnExit -= HandleExited;
    }
}