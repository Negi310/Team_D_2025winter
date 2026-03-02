using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
public class CourtingUIManager : MonoBehaviour
{
    //uiDocument
    private UIDocument uiDocument;
    //uiのオブジェクト
    private GameObject uiObject;
    
    //UI生成(引数は求愛UIのプレファブ,プレイヤーのステータスのリスト,パートナーの情報のリスト,残り求愛回数,川の名前)
    public void Show(GameObject CourtingUIPrefab, List<(string name, string value, bool toSwim)> playerStatusList, List<List<(string name ,string value)>> partnerStatusList, int courtTimes, string riverName)
    {
        //プレファブからui生成
        uiObject = Instantiate(CourtingUIPrefab);
        //uiDocument保存
        uiDocument = uiObject.GetComponent<UIDocument>();

        //uiDocumentのrootVE取得
        var root = uiDocument.rootVisualElement;

        //パートナーのUIの処理
        PartnerUI(root,partnerStatusList);
        //プレイヤーのuiの処理
        PlayerUI(root, playerStatusList);
        //川の情報UIの処理
        RiverUI(root,courtTimes,riverName);
    }
    
    //UIの非表示
    public void Hide()
    {
        if (uiObject != null)
        {
            Destroy(uiObject);
            uiObject = null;
            uiDocument = null;
        }
    }

    //パートナーの情報表示
    void PartnerUI(VisualElement root,List<List<(string name ,string value)>> partnerStatusList)
    {
        //VE構成
        //親VE(class:partner-information)(パートナー一体につき一つ)
        //|--子VE(1)(class:partner-information_background)(ホバー時に拡大してホバー対象をわかりやすくする)
        //|--image(クラスなし)(パートナーのイラスト表示用)
        //|--子VE(2)(class:partner-information_status)(相性などの情報表示用　ホバー時にのみ見える)
        //   |--孫VE(クラスなし)(情報3項目の並びの設定用 項目数に応じて生成)
        //   :  |--label(1)(class:class:partner-information_status_name)(項目の名前の表示)
        //      |--label(2)(class:class:partner-information_status_value)(項目の値の表示)

        //親VEのリスト作成
        var partnerList = root.Query<VisualElement>(className:"partner-information").ToList();

        //UIの各種設定
        //弱いやつから順に
        foreach(List<(string name ,string value)> partnerStatus in partnerStatusList)
        {
            //表示場所を乱数で決定
            int i = Random.Range(0,partnerList.Count-1);
            var partner = partnerList[i];
            //ホバー時のイベント設定
            partner.RegisterCallback<MouseEnterEvent>(evt =>
            {
                //ホバー時に最前列に
                partner.BringToFront();
                //川の情報の方が前になるように
                root.Q<VisualElement>(className:"next-river-information").BringToFront();
                
            });
            //子VE(2)取得
            var childVE = partner.Q<VisualElement>(className:"partner-information_status");
            //各項目の孫VE以下を生成
            for(int j = 0;j < partnerStatus.Count; j++)
            {
                //孫VEを生成
                var groundchildVE = new VisualElement(){pickingMode = PickingMode.Ignore};
                childVE.Add(groundchildVE);

                //label(1)生成
                var label1 = new Label(partnerStatus[j].name){pickingMode = PickingMode.Ignore};
                label1.AddToClassList("partner-information_status_name");
                groundchildVE.Add(label1);

                //label(2)生成
                var label2 = new Label(partnerStatus[j].value){pickingMode = PickingMode.Ignore};
                label2.AddToClassList("partner-information_status_value");
                groundchildVE.Add(label2);

            }
            //設定の終わった親VEをリストから削除
            partnerList.RemoveAt(i);
        }
    }

    //プレイヤーのステータスのUI
    
    void PlayerUI(VisualElement root, List<(string name, string value, bool toSwim)> playerStatusList)
    {
        //VEとかの構造　子VE以降はスクリプトから必要なだけ生成
        //親VE(class:main-salmon-status)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(2)(class:main-salmon-status_value)(ステータスの値を表示)
        //:

        //親VE取得
        var parentVE = root.Q<VisualElement>(className:"main-salmon-status");
        //ステータスの個数だけ実行
        foreach((string name, string value, bool toSwim) status in playerStatusList)
        {
            //子VE生成し親VEにadd
            var childVE = new VisualElement();
            parentVE.Add(childVE);

            //label(1)生成と処理
            var label1 = new Label(status.name);
            if(status.toSwim) label1.AddToClassList("main-salmon-status_name--swim");
            else label1.AddToClassList("main-salmon-status_name--court");
            childVE.Add(label1);

            //label(2)生成と処理
            var label2 = new Label(status.value);
            label2.AddToClassList("main-salmon-status_value");
            childVE.Add(label2);
        }
    }

    //川の情報のUI
    void RiverUI(VisualElement root,int courtTimes,string riverName)
    {
        //現在のターンを表示
        var courtTimeUI = root.Q<Label>(className:"remaining-court-times");
        courtTimeUI.text = "残り\n" + courtTimes + "回";

        //川の情報の処理
        //VEの構造
        //親VE(class:next-river-information)
        //|--Label (川の名前を表示)
        //|--Button (川の詳細情報用のVEの表示非表示切り替え)
        //|--子VE (子の下に川の詳細情報を表示)
        //   |
        //親VE取得
        var riverUI = root.Q<VisualElement>(className:"next-river-information");
        //川の名前表示
        riverUI.Children().OfType<Label>().First().text = "NEXT>" + riverName;

        //Buttonのイベント設定
        var riverstatusButton = root.Q<VisualElement>(className:"next-river-information").Q<Button>();
        riverstatusButton.clicked += () =>
        {
            //クラス変更で革のステータスの表示状況を切り替え
            riverstatusButton.parent.EnableInClassList("is-open",!riverstatusButton.parent.ClassListContains("is-open"));
            //ボタンの文字切り替え
            if(riverstatusButton.text == ">") riverstatusButton.text = "v";
            else riverstatusButton.text = ">";
        };
    }
}
