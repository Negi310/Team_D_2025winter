using Cysharp.Threading.Tasks;

public interface IScreenTransitionView
{
    UniTask FadeOutAsync(float duration);
    UniTask FadeInAsync(float duration);
}