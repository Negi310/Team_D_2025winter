using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class CircleClose : MonoBehaviour
{
    public Material mat;
    float radius = 1f;

    void Start()
    {
        radius = 1f;
        mat.SetFloat("_Radius", radius);
    }

    void Update()
    {
        radius -= Time.deltaTime * 0.2f;
        radius = Mathf.Clamp01(radius);
        mat.SetFloat("_Radius", radius);
    }
}