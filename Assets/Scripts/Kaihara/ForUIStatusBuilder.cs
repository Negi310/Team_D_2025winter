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
}