using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class ScreenTransitionView : MonoBehaviour,IScreenTransitionView
{
    [SerializeField] private Material _fadeMaterial;
    [SerializeField] private string _fadeParamName = "_Radius";

    // フェードイン
    public async UniTask FadeInAsync(float duration)
    {   
        await DOTween.To(() => _fadeMaterial.GetFloat(_fadeParamName),x => _fadeMaterial.SetFloat(_fadeParamName, x),1f, duration).SetEase(Ease.OutQuad).ToUniTask();
    }

    // フェードアウト
    public async UniTask FadeOutAsync(float duration)
    {
        await DOTween.To(() => _fadeMaterial.GetFloat(_fadeParamName),x => _fadeMaterial.SetFloat(_fadeParamName, x),0f, duration).SetEase(Ease.OutQuad).ToUniTask();
    }
}