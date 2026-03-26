using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitionView _transition;

    [SerializeField]
    private GameStateFadeSetting[] _fadeSettings;

    public GameState CurrentState { get; private set; }

    private bool _isTransitioning;

    public async UniTask ChangeState(GameState nextState, Action executeStateChangeLogic)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        var setting = GetSetting(nextState);

        await _transition.FadeOutAsync(setting.fadeOutTime);

        // 画面が真っ暗な裏側で、MVPのステート切り替え処理を一瞬で行う
        executeStateChangeLogic?.Invoke();

        Debug.Log($"State Transitioned to : {nextState.GetType().Name}");

        await _transition.FadeInAsync(setting.fadeInTime);

        _isTransitioning = false;
    }

    private GameStateFadeSetting GetSetting(GameState state)
    {
        var setting = _fadeSettings.FirstOrDefault(s => s.state != null && s.state.GetType() == state.GetType());
        
        if (setting == null)
        {
            return new GameStateFadeSetting { fadeOutTime = 1f, fadeInTime = 1f };
        }
        return setting;
    }

/*    void ExecuteStateLogic(GameState state)
    {
        switch (state)
        {
            case GameState.Title:
                Debug.Log("タイトル処理");
                break;

            case GameState.Upstream:
                Debug.Log("川登り");　　　　　　　　　　　　遷移時になんかしたかったらこの辺使ってもらっても
                break;
        }
    }*/
}