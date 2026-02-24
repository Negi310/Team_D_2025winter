using System;

// 【Viewの契約】
public interface INurturingHeaderView
{
    // 出力
    void SetBasicInfo(int generation, int turn, string riverName);
    void SetSalmonData(SalmonData data);
    void SetRiverDataSliders(RiverData data); // トグルで出る川のスライダー
    void ShowParamDiscription(string explanation); // パラメータ解説
    
    // 入力
    event Action<string> OnSalmonParamHovered; // パラメータにカーソルが合った時
    event Action OnRiverDataToggleClicked;     // 川データトグルが押された時
}