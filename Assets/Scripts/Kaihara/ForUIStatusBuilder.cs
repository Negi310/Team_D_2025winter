using System.Collections.Generic;

public class ForUIStatusBuilder
{
    //プレイヤーのステータスの値をリストにまとめて返す(UIManagerへの受け渡し用)
    public List<string> PlayerStatusListBuild(string speed, string jump, string stamina, string attack, string intelligence, string size, string color, string shape)
    {
        //UIに渡すステータス一式をリストにする
        var uiStatuses =  new List<string>
        {
            speed,jump,stamina,attack,intelligence,size,color,shape
        };
        //↑のリストを返す
        return uiStatuses;
    }
    //パートナーの情報をリストにまとめて返す(UIManagerへの受け渡し用)
    //↓でまとめたやつをさらにまとめる
    //弱い->ちょっと弱い->普通->ちょっと強い->強いの順
    public List<(string personality,string successRate)> PartnersListBuild(string weakestPer,string weakestSuc,string weakPer,string weakSuc,string normalPer,string normalSuc,string strongPer,string strongSuc,string strongestPer,string strongestSuc)
    {
        var weakest = PartnerInfomationTapleBuild(weakestPer,weakestSuc);
        var weak = PartnerInfomationTapleBuild(weakPer,weakSuc);
        var normal = PartnerInfomationTapleBuild(normalPer,normalSuc);
        var strong = PartnerInfomationTapleBuild(strongPer,strongSuc);
        var strongest = PartnerInfomationTapleBuild(strongestPer,strongestSuc);
        var partnerStatusList = new List<(string personality,string successRate)>
        {
            weakest,weak,normal,strong,strongest
        };
        return partnerStatusList;
    }
    //一体分のデータをタプルにまとめる
    private (string personality,string successRate) PartnerInfomationTapleBuild(string Personality,string SuccessRate)
    {
        (string personality,string successRate) partnerTaple = (Personality,SuccessRate);
        return partnerTaple;
    }

    //川のデータをリストにまとめる
    public List<string> RiverInformatinListBuild(string a,string b,string c,string d, string e)
    {
        List<string> riverList = new List<string>
        {
            a ,b ,c ,d ,e
        };
        return riverList;
    }
    //各トレーニングのステータスの増減量をリストに
    public List<string> TrainingStatuIcreaceList(string speed, string jump, string stamina, string attack, string intelligence)
    {
        return new List<string>
        {
            speed,jump,stamina,attack,intelligence
        };
    }
    //メスの立ち絵のパーツの情報とかをリストに
    public List<(SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size)> FemaleIllustList(SalmonHair weakestHair,SalmonEyeFemale weakestEye, SalmonColor weakestColor,SalmonEyebrowFemale weakestEyebrow,SalmonMouthFemale weakestMouth,SalmonSize weakestSize,SalmonHair weakHair,SalmonEyeFemale weakEye, SalmonColor weakColor,SalmonEyebrowFemale weakEyebrow,SalmonMouthFemale weakMouth,SalmonSize weakSize,SalmonHair normalHair,SalmonEyeFemale normalEye, SalmonColor normalColor,SalmonEyebrowFemale normalEyebrow,SalmonMouthFemale normalMouth,SalmonSize normalSize,SalmonHair strongHair,SalmonEyeFemale strongEye, SalmonColor strongColor,SalmonEyebrowFemale strongEyebrow,SalmonMouthFemale strongMouth,SalmonSize strongSize,SalmonHair strongestHair,SalmonEyeFemale strongestEye, SalmonColor strongestColor,SalmonEyebrowFemale strongestEyebrow,SalmonMouthFemale strongestMouth,SalmonSize strongestSize)
    {
        (SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size) weakest = (weakestHair,weakestEye,weakestColor,weakestEyebrow,weakestMouth,weakestSize);
        (SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size) weak = (weakHair,weakEye,weakColor,weakEyebrow,weakMouth,weakSize);
        (SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size) normal = (normalHair,normalEye,normalColor,normalEyebrow,normalMouth,normalSize);
        (SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size) strong = (strongHair,strongEye,strongColor,strongEyebrow,strongMouth,strongSize);
        (SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size) strongest = (strongestHair,strongestEye,strongestColor,strongestEyebrow,strongestMouth,strongestSize);
        return new List<(SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth,SalmonSize size)>{weakest,weak,normal,strong,strongest};
    }

    //ランダムイベントの名前をリストに
    public List<string> RandomEventNameList(string eventName1,string eventName2,string eventName3)
    {
        return new List<string>{eventName1,eventName2,eventName3};
    }
}