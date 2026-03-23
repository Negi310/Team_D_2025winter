/// <summary>
/// 求愛フェーズのステート
/// </summary>
public class CourtshipState : GameState
{
    // Presenterに渡す「完了時の窓口」。引数はこのフェーズ専用でいい。
    public void TransitionCheck(bool isSuccess)
    {
        if (isSuccess) RequestTransition<RearState>();
        else return;
    }
}