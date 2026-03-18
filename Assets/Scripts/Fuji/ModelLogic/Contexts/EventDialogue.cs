using System;
using UnityEngine;

// 1ターンの会話データ
[Serializable]
public class DialogueTurn
{
    public string speakerName;
    [TextArea(3, 5)]
    public string text;
    public string animationTriggerName; // 再生するAnimatorのトリガー名
}