using UnityEngine;

public class ConversationContext
{
    public ConversationEvent MasterData { get; set; }
    
    // 現在の進行状態
    public int CurrentIndex { get; set; } = 0;
    
    // UIが現在文字送り中かどうか（Presenterがスキップ判定に使う）
    public bool IsTyping { get; set; } = false;
}
