using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventPool", menuName = "Scriptable Objects/EventPool")]
public class EventPool : ScriptableObject
{
    [Tooltip("パラメータ特化型のイベント（基本5種）")]
    public List<EventSO> ParameterEvents;
    
    [Tooltip("ランダムイベント（3種）")]
    public List<EventSO> RandomEvents;
}
