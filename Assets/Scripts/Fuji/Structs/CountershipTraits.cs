using UnityEngine;

[System.Serializable]
public struct CourtshipTraits 
{
    public SalmonSize Size;
    public SalmonColor Color;
    public SalmonHair Hair;
    public int EyeIndex;
    public int EyebrowIndex;
    public int MouthIndex;

    public CourtshipTraits(SalmonSize size, SalmonColor color, SalmonHair hair, int eyeIndex, int eyebrowIndex, int mouthIndex)
    {
        Size = size;
        Color = color;
        Hair = hair; 
        EyeIndex = eyeIndex;
        EyebrowIndex = eyebrowIndex;
        MouthIndex = mouthIndex;
    }

    public SalmonEyeMale MaleEye => (SalmonEyeMale)Mathf.Clamp(EyeIndex, 0, 4);
    public SalmonEyeFemale FemaleEye => (SalmonEyeFemale)Mathf.Clamp(EyeIndex, 0, 2);
    public SalmonEyebrowMale MaleEyebrow => (SalmonEyebrowMale)Mathf.Clamp(EyebrowIndex, 0, 3);
    public SalmonEyebrowFemale FemaleEyebrow => (SalmonEyebrowFemale)Mathf.Clamp(EyebrowIndex, 0, 3);
    public SalmonMouthMale MaleMouth => (SalmonMouthMale)Mathf.Clamp(MouthIndex, 0, 2);
    public SalmonMouthFemale FemaleMouth => (SalmonMouthFemale)Mathf.Clamp(MouthIndex, 0, 2);

    // ==========================================
    // ★追加: 顔パーツから特徴（Shape）を診断して命名する！
    // ==========================================
    public string GetShapeFeatureName(bool isMale)
    {
        // 眉毛（Eyebrow）の主張が一番強いので、眉をベースに性格を診断
        if (EyebrowIndex == 2) return "オラオラ系"; // Angry（怒り眉）
        if (EyebrowIndex == 3) return "ぴえん系";   // Sadness（困り眉）
        if (EyebrowIndex == 1) return "癒やし系";   // Droppy（タレ眉）

        // 特徴的な目が選ばれている場合（オスのみ）
        if (isMale && EyeIndex == 3) return "サイコ系"; // White（白目）
        if (isMale && EyeIndex == 4) return "メンヘラ系"; // Cry（泣き目）

        // 髪型による診断
        if (Hair == SalmonHair.Long) return isMale ? "ホスト系" : "ギャル系";
        if (Hair == SalmonHair.Short) return "体育会系";

        // 口が開いているか
        if (MouthIndex == 2) return "アホの子系"; // Open（開いた口）

        return "フツウ系";
    }
}