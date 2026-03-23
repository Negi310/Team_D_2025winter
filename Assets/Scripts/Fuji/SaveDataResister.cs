using UnityEngine;

public class SaveDataResister
{
    public bool TryLoad(out SaveData saveData)
    {
        // TODO: 実際のロード処理（ファイル読み込み等）
        // 今回はロード失敗（セーブデータなし）をシミュレート
        saveData = null;
        return false; 
    }

    public void Save(SaveData saveData)
    {
        // TODO: 実際のセーブ処理
    }
}
