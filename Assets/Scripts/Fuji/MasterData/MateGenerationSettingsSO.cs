using UnityEngine;

[CreateAssetMenu(fileName = "MateGenerationSettings", menuName = "Salmon/MateGenerationSettings")]
public class MateGenerationSettingsSO : ScriptableObject 
{
    [System.Serializable]
    public struct TierSetting 
    {
        public string tierName; // "妥協枠", "強い" など
        [Tooltip("基礎量に乗算する係数の最小値")] public float minMultiplier;
        [Tooltip("基礎量に乗算する係数の最大値")] public float maxMultiplier;
        [Tooltip("このTierの基準となる求愛難易度（基礎値）")] public float baseCourtshipDifficulty;
    }

    [Header("相手の5段階ティア設定")]
    public TierSetting veryWeak;
    public TierSetting weak;
    public TierSetting compromise;
    public TierSetting normal;
    public TierSetting strong;

    // パラメータ割り振りの偏り（ランダム性）のルールなどもここに定義すると良いです
}