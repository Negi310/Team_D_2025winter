using UnityEngine;

public class DrifterView : MonoBehaviour
{
    public DrifterData Data { get; private set; }
    public void Setup(DrifterData data)
    {
        Data = data;
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = true;
        gameObject.SetActive(true); // 非表示にされていた場合も復活
    }
}