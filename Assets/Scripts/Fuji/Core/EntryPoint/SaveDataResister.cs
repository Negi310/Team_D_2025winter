using System;
using System.IO;
using UnityEngine;

// ストレージへの読み書き「だけ」を担当するインフラ
public class SaveDataResister
{
    private readonly string _saveFilePath;

    public SaveDataResister()
    {
        // 端末の安全な保存領域（PCならAppData、スマホなら内部ストレージ）
        _saveFilePath = Path.Combine(Application.persistentDataPath, "salmon_savedata.json");
        Debug.Log($"【セーブデータの場所】: {_saveFilePath}");
    }

    public bool TryLoad(out SaveData saveData)
    {
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                saveData = JsonUtility.FromJson<SaveData>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"セーブデータの読み込みに失敗しました: {e.Message}");
            }
        }

        saveData = null;
        return false;
    }

    public void Save(SaveData saveData)
    {
        try
        {
            // trueを渡すと、見やすい形式(PrettyPrint)でJSON出力される
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"セーブデータの保存に失敗しました: {e.Message}");
        }
    }
    
    public SaveData CreateInitialData()
    {
        var defaultStats = new UpstreamStats(speed: 10f, jump: 10f, stamina: 10f, attack: 10f, intelligence: 10f);
        var defaultTraits = new CourtshipTraits(SalmonSize.Normal, SalmonColor.Orange, SalmonHair.Normal, 0, 0, 0);
        var initialSalmon = new SalmonData(defaultStats, defaultTraits) { Name = "初代" };
        var initialRiver = RiverGenerator.GenerateRiver(1);
        
        return new SaveData
        {
            Generation = 1,
            Turn = 0,
            Salmon = initialSalmon,
            River =  initialRiver,
            LastSavedStateName = nameof(NameState) // 最初はオープニング会話から
        };
    }

    // メモリ上のSessionContextから、保存用のSaveDataを生成（スナップショット）する
    public SaveData CreateFromContext(SessionContext context, string currentStateName)
    {
        return new SaveData
        {
            Generation = context.CurrentGeneration,
            Turn = context.CurrentTurn,
            Salmon = context.CurrentSalmon,
            River = context.CurrentRiver,
            LastSavedStateName = currentStateName
        };
    }
}