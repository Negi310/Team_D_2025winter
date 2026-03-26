using System.Text;

public class ConversationModel
{
    // 次のターンが存在するか判定する
    public bool HasNextTurn(ConversationContext context)
    {
        if (context.MasterData == null || context.MasterData.LinkedConversation.turns == null) return false;
        return context.CurrentIndex < context.MasterData.LinkedConversation.turns.Count - 1;
    }

    // 次のターンへ進める
    public void MoveToNextTurn(ConversationContext context) => context.CurrentIndex++;

    // 現在のターンのデータを取得する
    public DialogueTurn GetCurrentTurn(ConversationContext context)
    {
        return context.MasterData.LinkedConversation.turns[context.CurrentIndex];
    }
    
    public string GenerateResultMessage(EventData eventData)
    {
        StringBuilder sb = new StringBuilder();

        // 基礎上昇（Base）のチェック
        if (eventData.BaseModifier.Value > 0)
        {
            string statNameJP = TranslateStatName(eventData.BaseModifier.TargetStatName);
            sb.AppendLine($"{statNameJP}が {eventData.BaseModifier.Value} 上がった！");
        }

        // ボーナス上昇（Bonus）のチェック（Noneじゃない、かつ0より大きい場合）
        if (eventData.BonusModifier.TargetStatName != "None" && eventData.BonusModifier.Value > 0)
        {
            string statNameJP = TranslateStatName(eventData.BonusModifier.TargetStatName);
            sb.AppendLine($"{statNameJP}がさらに {eventData.BonusModifier.Value} 上がった！");
        }

        return sb.ToString().TrimEnd(); // 最後の改行を消して返す
    }

    // 英語の内部データをプレイヤー向けの日本語に変換する辞書代わり
    public string TranslateStatName(string engName)
    {
        switch (engName)
        {
            case "Speed": return "推進力";
            case "Jump": return "跳躍力";
            case "Stamina": return "持続力";
            case "Attack": return "攻撃力";
            case "Intelligence": return "賢さ";
            default: return engName; // 想定外の文字列が来たらそのまま出す
        }
    }
}