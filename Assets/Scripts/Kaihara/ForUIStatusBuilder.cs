using System.Collections.Generic;

public class ForUIStatusBuilder
{
    //プレイヤーのステータスの値をリストにまとめて返す(UIManagerへの受け渡し用)
    public List<(string name, string value, bool toSwim)> PlayerStatusTapleListBuild(float jump, float power, float riskhedging, float stamina, float size, float color, float shape)
    {
        //UIに渡すステータス一式を、各ステータスごとに名前(string)・値(string)・用途(bool)のタプルにして、それらをまとめてリストにする
        var uiStatuses =  new List<(string name, string value, bool toSwim)>
        {
            ("ジャンプ",jump.ToString(),true),
            ("パワー",power.ToString(),true),
            ("リスクヘッジ",riskhedging.ToString(),true),
            ("スタミナ",stamina.ToString(),true),
            ("カラー",color.ToString(),false),
            ("サイズ",size.ToString(),false),
            ("シェイプ",shape.ToString(),false)
        };
        //↑のリストを返す
        return uiStatuses;
    }
    //パートナーの情報をリストにまとめて返す(UIManagerへの受け渡し用)
    //パートナーのデータの保存形式不明のため一旦パートナーの相性・性格・成功率を個別に引数で渡す形に(3種類*5体の計15個)
    //弱い->ちょっと弱い->普通->ちょっと強い->強いの順
    //もともとまとめられている場合は不要
    public List<List<(string name,string value)>> PartnerInformationListBuild(string weakestCompatibility, string weakestPersonality, string weakestSuccessRate,string weakCompatibility, string weakPersonality, string weakSuccessRate,string normalCompatibility, string normalPersonality, string normalSuccessRate,string strongCompatibility, string strongPersonality, string strongSuccessRate,string strongestCompatibility, string strongestPersonality, string strongestSuccessRate)
    {
        var partnerStatusList = new List<List<(string name,string value)>>
        {
            new List<(string name,string value)>{("相性",weakestCompatibility),("性格",weakestPersonality),("成功率",weakestSuccessRate)},
            new List<(string name,string value)>{("相性",weakCompatibility),("性格",weakPersonality),("成功率",weakSuccessRate)},
            new List<(string name,string value)>{("相性",normalCompatibility),("性格",normalPersonality),("成功率",normalSuccessRate)},
            new List<(string name,string value)>{("相性",strongCompatibility),("性格",strongPersonality),("成功率",strongSuccessRate)},
            new List<(string name,string value)>{("相性",strongestCompatibility),("性格",strongestPersonality),("成功率",strongestSuccessRate)}
        };
        return partnerStatusList;
    }
}