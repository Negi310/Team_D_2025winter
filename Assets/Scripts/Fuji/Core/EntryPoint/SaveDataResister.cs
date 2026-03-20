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
        var defaultStats = new UpstreamStats(power: 10f, jump: 5f, cautiousness: 5f, stamina: 100f);
        var defaultTraits = new CourtshipTraits(size: 1.0f, colorValue: 0.5f, shapeValue: 0.5f);
        var initialSalmon = new SalmonData(defaultStats, defaultTraits) { Name = "初代" };

        return new SaveData
        {
            Generation = 1,
            Turn = 1,
            Salmon = initialSalmon,
            LastSavedStateName = nameof(ConversationState) // 最初はオープニング会話から
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
            LastSavedStateName = currentStateName
        };
    }
    
    public void ResumeState(GameStateMachine stateMachine, string stateName)
    {
        var sm = (IStateChangable)stateMachine;
        
        if (stateName == nameof(UpstreamState)) sm.ChangeState<UpstreamState>();
        else if (stateName == nameof(CourtshipState)) sm.ChangeState<CourtshipState>();
        else if (stateName == nameof(RearState)) sm.ChangeState<RearState>();
        else sm.ChangeState<ConversationState>();
    }
}