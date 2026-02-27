using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SeaUIManager : MonoBehaviour
{
    //uiDocument
    [SerializeField] private UIDocument uiDocument;
    //魚のステータス
    [SerializeField] private FishStatus fishState;
    //川とかの情報
    [SerializeField] private RiverStatus riverState;
    private VisualElement root;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        //ステータス表示
        StatusUI();
        //川の情報表示
        RiverUI();
        //川のステータス表示非表示のボタンのイベント設定
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //ステータス表示の設定
    void StatusUI()
    {
        //VEとかの構造　子VE以降はスクリプトから必要なだけ生成
        //親VE(class:main-salmon-statusbar)
        //|--子VE(class:main-salmon-status)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-status_)(ステータスについての解説)
        //|  |--label(2)(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(3)(class:main-salmon-status_value)(ステータスの値を表示)
        //:

        //表示用ステータス一覧を取得
        List<(string name, string value, bool toSwim)> statuses = fishState.uiStatus;
        //ステータスを表示するやつのベース(上の親VE)
        var statusUIBase = root.Q<VisualElement>(className:"main-salmon-statusbar");

        //ステータスを表示するやつ(上の子VE以下)を表示するステータスの数だけ作成
        for(int i = 0; i < statuses.Count; i++)
        {
            //子VEを作成 クラスも適用
            var statusVE = new VisualElement();
            statusVE.AddToClassList("main-salmon-status");
            //親VEに子VEをAdd
            statusUIBase.Add(statusVE);

            //label(1)の作成 クラス適用
            var statusInformationLabel = new Label();
            statusInformationLabel.AddToClassList("main-salmon-status_status-information");
            //ホバーの判定を消去(虚空にホバーしたら出てくるので)
            statusInformationLabel.pickingMode = PickingMode.Ignore;
            //子VEにlabel(1)をAdd
            statusVE.Add(statusInformationLabel);

            //label(2)の作成 クラス適用(statuses[i].toSwimから川登用か求愛用かを判定) 表示する文字も設定
            var statusNameLabel = new Label(statuses[i].name);
            if(statuses[i].toSwim) statusNameLabel.AddToClassList(className:"main-salmon-status_name--swim");
            else statusNameLabel.AddToClassList(className:"main-salmon-status_name--court");
            //子VEにlabel(2)をAdd
            statusVE.Add(statusNameLabel);

            //label(3)の作成 クラス適用　表示する文字も設定
            var statusValueLabel = new Label(statuses[i].value);
            statusValueLabel.AddToClassList("main-salmon-status_value");
            //子VEにlabel(3)をAdd
            statusVE.Add(statusValueLabel);
        }
    }
    void RiverUI()
    {
        //現在のターンを表示
        var turnUI = root.Q<Label>(className:"turn-ui");
        turnUI.text = "ターン\n" + riverState.SeaTurn;
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
        riverUI.Children().OfType<Label>().First().text = "NEXT>" + riverState.RiverName;
    }
}