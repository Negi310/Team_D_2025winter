using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitionView _transition;

    [SerializeField]
    private GameStateFadeSetting[] _fadeSettings;

    public GameState CurrentState { get; private set; }

    private bool _isTransitioning;

    public async UniTask ChangeState(GameState nextState)
    {
        if (_isTransitioning) return;

        _isTransitioning = true;

        var setting = GetSetting(nextState);

        await _transition.FadeOutAsync(setting.fadeOutTime);//フェードアウト

        CurrentState = nextState;
        Debug.Log($"State : {CurrentState}");

       //ExecuteStateLogic(nextState);

        await _transition.FadeInAsync(setting.fadeInTime);//フェードイン

        _isTransitioning = false;
    }

    GameStateFadeSetting GetSetting(GameState state)
    {
        return _fadeSettings.First(s => s.state == state);
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