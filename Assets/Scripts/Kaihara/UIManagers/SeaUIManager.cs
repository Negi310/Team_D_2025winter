using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SeaUIManager : MonoBehaviour
{
    //uiDocument
    private UIDocument uiDocument;

    //インスタンス化したUIオブジェクト(Hide用に保存)
    private GameObject uiObject;

    //UIの表示(引数:SeaUIのプレファブ,ForUIStatusBuilderで用意したプレイヤーのステータスのリスト,ターン,川の名前)
    public void Show(GameObject SeaUIPrefab, List<(string name, string value, bool toSwim)> playerStatusList,int turn,string riverName)
    {
        //prefabからUI生成
        uiObject = Instantiate(SeaUIPrefab);
        //コンポーネントのUIDocumentを保存
        uiDocument = uiObject.GetComponent<UIDocument>();
        //uiDocumentのrootVE取得
        var root = uiDocument.rootVisualElement;
        //ステータス表示
        StatusUI(root,playerStatusList);
        //川の情報表示
        RiverUI(root,turn,riverName);
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



    //ステータス表示の設定
    void StatusUI(VisualElement root, List<(string name, string value, bool toSwim)> playerStatusList)
    {
        //VEとかの構造　子VE以降はスクリプトから必要なだけ生成
        //親VE(class:main-salmon-statusbar)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-status_)(ステータスについての解説)
        //|  |--label(2)(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(3)(class:main-salmon-status_value)(ステータスの値を表示)
        //:

        //ステータスを表示するやつのベース(上の親VE)
        var statusUIBase = root.Q<VisualElement>(className:"main-salmon-statusbar");

        //ステータスを表示するやつ(上の子VE以下)を表示するステータスの数だけ作成
        foreach((string name, string value, bool toSwim) status in playerStatusList)
        {
            //子VEを作成
            var statusVE = new VisualElement();
            //親VEに子VEをAdd
            statusUIBase.Add(statusVE);

            //label(1)の作成 クラス適用
            var statusInformationLabel = new Label();
            statusInformationLabel.AddToClassList("main-salmon-status_status-information");
            //ホバーの判定を消去
            statusInformationLabel.pickingMode = PickingMode.Ignore;
            //子VEにlabel(1)をAdd
            statusVE.Add(statusInformationLabel);

            //label(2)の作成 クラス適用(status.toSwimから川登用か求愛用かを判定) 表示する文字も設定
            var statusNameLabel = new Label(status.name);
            if(status.toSwim) statusNameLabel.AddToClassList(className:"main-salmon-status_name--swim");
            else statusNameLabel.AddToClassList(className:"main-salmon-status_name--court");
            //子VEにlabel(2)をAdd
            statusVE.Add(statusNameLabel);

            //label(3)の作成 クラス適用　表示する文字も設定
            var statusValueLabel = new Label(status.value);
            statusValueLabel.AddToClassList("main-salmon-status_value");
            //子VEにlabel(3)をAdd
            statusVE.Add(statusValueLabel);
        }
    }
    void RiverUI(VisualElement root,int turn,string riverName)
    {
        //現在のターンを表示
        var turnUI = root.Q<Label>(className:"turn-ui");
        turnUI.text = "ターン\n" + turn;

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