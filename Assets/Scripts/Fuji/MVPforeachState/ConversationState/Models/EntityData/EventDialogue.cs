using System;
using UnityEngine;

// 1ターンの会話データ
[Serializable]
public class DialogueTurn
{
    public string SpeakerName;
    
    [TextArea(3, 5)]
    public string Text;
    
    public float TextSpeed = 0.05f; // 文字送りの速度
    public Sprite BackgroundImage;
    //public AudioClip Bgm;
    public string AnimationTrigger; // "FadeIn", "Shake" などの演出指定
}