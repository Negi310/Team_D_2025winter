using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ゲーム実行中には一切動かない、エディタ上でのみ使うベイク用ツール
public class ChunkSplineBaker : MonoBehaviour
{
    [Header("スプラインコンポーネント")]
    [SerializeField] private SplineContainer _leftBankSpline;
    [SerializeField] private SplineContainer _rightBankSpline;
    [SerializeField] private SplineContainer _centerSpline;
    
    //[SerializeField] private Transform 

    [Header("保存先のマスターデータ (ScriptableObject)")]
    [SerializeField] private ChunkPreset _targetPreset;

    [Tooltip("抽出する点の数（1チャンクにつき何等分するか）")]
    [SerializeField] private int _resolution = 10;

    // インスペクターから右クリック（ContextMenu）で実行できるメソッド
    [ContextMenu("Bake Splines to Preset (スプラインをSOに保存)")]
    public void BakeSplines()
    {
        // 各スプラインから点を抽出し、配列に保存する
        if (_leftBankSpline != null)
            _targetPreset.LocalLeftBank = ExtractPoints(_leftBankSpline);

        if (_rightBankSpline != null)
            _targetPreset.LocalRightBank = ExtractPoints(_rightBankSpline);

        if (_centerSpline != null)
            _targetPreset.LocalCenterLine = ExtractPoints(_centerSpline);
        
        // ★最重要: Unityエディタに変更を検知させ、ディスク（ファイル）に保存させる
#if UNITY_EDITOR
        EditorUtility.SetDirty(_targetPreset);
        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>【Bake完了】</color> {_targetPreset.name} にスプラインデータを焼き付けました！");
#endif
    }

    private Vector2[] ExtractPoints(SplineContainer splineContainer)
    {
        var points = new Vector2[_resolution];

        for (int i = 0; i < _resolution; i++)
        {
            // t を 0.0 (始点) 〜 1.0 (終点) になるように計算
            // 例: resolutionが10なら、0/9, 1/9, ... 9/9 となる
            float t = (float)i / (_resolution - 1);

            // インスペクターのローカル座標をそのまま取得（複雑な変換は一切しない）
            Vector3 localPos = splineContainer.EvaluatePosition(t);
            
            points[i] = new Vector2(localPos.x, localPos.y);
        }

        return points;
    }
}