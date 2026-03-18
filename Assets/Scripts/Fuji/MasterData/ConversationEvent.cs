using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConversationEvent", menuName = "Scriptable Objects/ConversationEvent")]
public class ConversationEvent : ScriptableObject
{
    public Sprite backgroundImage;
    public AudioClip bgm;
    public float textSpeed = 0.05f; // 文字が流れる速度
    public string introAnimationTrigger = "Enter"; // 最初の登場アニメーション
    public List<DialogueTurn> turns;
}
