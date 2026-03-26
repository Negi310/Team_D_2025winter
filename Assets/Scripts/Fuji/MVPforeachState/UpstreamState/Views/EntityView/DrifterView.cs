using UnityEngine;

public class DrifterView : MonoBehaviour
{
    public DrifterData Data { get; private set; }
    public void Setup(DrifterData data) => Data = data;
}