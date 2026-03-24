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

    [Header("保存先のマスターデータ (ScriptableObject)")]
    [SerializeField] private ChunkPreset _targetPreset;

    [Tooltip("抽出する点の数（1チャンクにつき何等分するか）")]
    [SerializeField] private int _resolution = 10;

    // インスペクターから右クリック（ContextMenu）で実行できるメソッド
    [ContextMenu("Bake Splines to Preset (スプラインをSOに保存)")]
    public void BakeSplines()
    {
        if (_targetPreset == null)
        {
            Debug.LogError("保存先の ChunkPreset が設定されていません！");
            return;
        }

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
            // t はスプライン上の進行度 (0.0 〜 1.0)
            // 前のチャンクの終点と重複しないよう、(i + 1) を使って 0.1 〜 1.0 の範囲を取得する
            float t = (float)(i + 1) / _resolution;

            // 1. スプラインコンポーネントにおけるローカル座標を取得
            Vector3 splineLocalPos = splineContainer.EvaluatePosition(t);

            // 2. その点をワールド座標に変換（スプラインが子オブジェクトにアタッチされていてもズレないようにする）
            Vector3 worldPos = splineContainer.transform.TransformPoint(splineLocalPos);

            // 3. 最後に「この川プレハブ(root)」から見たローカル座標に変換し直す
            Vector3 chunkLocalPos = transform.InverseTransformPoint(worldPos);

            // X と Y だけを Vector2 として配列に格納する
            points[i] = new Vector2(chunkLocalPos.x, chunkLocalPos.y);
        }

        return points;
    }
}