using System.Collections.Generic;

// ドメイン層：イベントのデータ構造
public class EventData
{
    public string Title { get; }
    public StatModifier BaseModifier { get; }
    public StatModifier BonusModifier { get; }
    public ConversationEvent LinkedConversation { get; } // 決定後に流す会話

    public EventData(string title, StatModifier baseModifier, StatModifier bonusModifier, ConversationEvent linkedConversation)
    {
        Title = title;
        BaseModifier = baseModifier;
        BonusModifier = bonusModifier;
        LinkedConversation = linkedConversation;
    }
}