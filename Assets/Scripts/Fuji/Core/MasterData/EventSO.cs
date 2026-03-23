using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "Scriptable Objects/EventSO")]
public class EventSO : ScriptableObject
{
    [Header("基本設定")]
    public string EventTitle;
    public TargetStatType MainStat; 
    
    [Header("パラメータ変動設定")]
    [Tooltip("確定で上がる基礎量")]
    public int BaseGainAmount;
    
    [Tooltip("ランダムに選ばれたパラメータに加算される値のブレ幅（Min, Max）")]
    public Vector2Int RandomBonusRange; 

    [Header("演出設定")]
    [Tooltip("この特訓を選んだ時に流れる会話イベント")]
    public ConversationEvent LinkedConversation;
}
