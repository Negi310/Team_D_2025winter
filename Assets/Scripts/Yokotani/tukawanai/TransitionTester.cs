using UnityEngine;
using Cysharp.Threading.Tasks;

public class TransitionTester : MonoBehaviour
{
    [SerializeField] private ScreenTransitionView transition;
    

    async void Start()
    {
        await UniTask.Delay(1000);

        Debug.Log("FadeOut");
        await transition.FadeOutAsync(2f);

        await UniTask.Delay(1000);

        Debug.Log("FadeIn");
        await transition.FadeInAsync(2f);
    }

}