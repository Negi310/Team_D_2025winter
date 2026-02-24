/// <summary>
/// 1回のプレイ（セッション）の状態を保持するクラス。
/// MonoBehaviourを継承しない、純粋なC#クラス。
/// </summary>
public class SessionContext
{
    public int CurrentGeneration { get; private set; }
    public SalmonData CurrentSalmon { get; private set; }
    public int CurrentTurn { get; private set; }

    // 新しいゲームを始める時の初期化
    public SessionContext(SaveData data)
    {
        CurrentGeneration = data.Generation;
        CurrentTurn = data.Turn;
        CurrentSalmon = data.Salmon;
    }

    // 次の世代へ引き継ぐメソッド（カプセル化により不正な上書きを防ぐ）
    public void AdvanceToNextGeneration(SalmonData childSalmon)
    {
        CurrentGeneration++;
        CurrentSalmon = childSalmon;
        CurrentTurn = 1; // 世代が変わるのでターンはリセット
    }
    
    public void IncrementTurn() => CurrentTurn++;
    
    // 育成フェーズでのパラメータ更新用
    public void UpdateCurrentSalmon(SalmonData updatedSalmon)
    {
        CurrentSalmon = updatedSalmon;
    }
}