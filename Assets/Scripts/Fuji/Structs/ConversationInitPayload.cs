// 【Rear ➡ Conversation】選んだ特訓イベント（とそれに紐づく会話）を渡す
public struct ConversationInitPayload : IPayload
{
    public EventData EventData { get; }
    
    public ConversationInitPayload(EventData eventData)
    {
        EventData = eventData;
    }
}