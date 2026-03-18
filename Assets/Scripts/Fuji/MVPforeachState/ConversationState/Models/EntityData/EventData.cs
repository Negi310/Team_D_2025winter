using System.Collections.Generic;

// ドメイン層：イベントのデータ構造
public class EventData
{
    public string Title { get; }
    public string Description { get; }
    public bool IsSelectionEvent { get; } // 選択肢か、ランダム(強制)か

    // このイベントが実行された時、どのステータスがどれくらい変動するかの定義
    public IReadOnlyList<StatModifier> Modifiers { get; }

    public EventData(string title, string description, bool isSelection, List<StatModifier> modifiers)
    {
        Title = title;
        Description = description;
        IsSelectionEvent = isSelection;
        Modifiers = modifiers;
    }
}