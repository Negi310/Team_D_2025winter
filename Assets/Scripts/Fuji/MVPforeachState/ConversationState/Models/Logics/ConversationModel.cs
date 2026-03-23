public class ConversationModel
{
    // 次のターンが存在するか判定する
    public bool HasNextTurn(ConversationContext context)
    {
        if (context.MasterData == null || context.MasterData.turns == null) return false;
        return context.CurrentIndex < context.MasterData.turns.Count - 1;
    }

    // 次のターンへ進める
    public void MoveToNextTurn(ConversationContext context) => context.CurrentIndex++;

    // 現在のターンのデータを取得する
    public DialogueTurn GetCurrentTurn(ConversationContext context)
    {
        return context.MasterData.turns[context.CurrentIndex];
    }
}