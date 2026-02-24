using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //uiDocument
    [SerializeField] private UIDocument uiDocument;
    //魚のステータス
    [SerializeField] private FishState fishState;
    //川とかの情報
    [SerializeField] private RiverState riverState;
    private VisualElement root;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        //ステータス表示
        StatesUI();
        //川の情報表示
        RiverUI();
        //川のステータス表示非表示のボタンのイベント設定
        var riverStatesButton = root.Q<VisualElement>(className:"next-river-information").Q<Button>();
        riverStatesButton.clicked += () =>
        {
            //クラス変更で革のステータスの表示状況を切り替え
            riverStatesButton.parent.EnableInClassList("is-open",!riverStatesButton.parent.ClassListContains("is-open"));
            //ボタンの文字切り替え
            if(riverStatesButton.text == ">") riverStatesButton.text = "v";
            else riverStatesButton.text = ">";
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ステータス表示の設定
    void StatesUI()
    {
        //VEとかの構造　子VE以降はスクリプトから必要なだけ生成
        //親VE(class:main-salmon-statesbar)
        //|--子VE(class:main-salmon-states)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-states-name-swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(2)(class:main-salmon-states-value)(ステータスの値を表示)
        //:

        //表示用ステータス一覧を取得
        List<(string name, string value, bool toSwim)> stateses = fishState.uiStates;
        //ステータスを表示するやつのベース(上の親VE)
        var statesUIBase = root.Q<VisualElement>(className:"main-salmon-statesbar");

        //ステータスを表示するやつ(上の子VE以下)を表示するステータスの数だけ作成
        for(int i = 0; i < stateses.Count; i++)
        {
            //子VEを作成 クラスも適用
            var statesVE = new VisualElement();
            statesVE.AddToClassList("main-salmon-states");
            //親VEに子VEをAdd
            statesUIBase.Add(statesVE);

            //label(1)の作成 クラス適用(stateses[i].toSwimから川登用か求愛用かを判定) 表示する文字も設定
            var statesNameLabel = new Label(stateses[i].name);
            if(stateses[i].toSwim) statesNameLabel.AddToClassList(className:"main-salmon-states-name-swim");
            else statesNameLabel.AddToClassList(className:"main-salmon-states-name-court");
            //子VEにlabel(1)をAdd
            statesVE.Add(statesNameLabel);

            //label(2)の作成 クラス適用　表示する文字も設定
            var statesValueLabel = new Label(stateses[i].value);
            statesValueLabel.AddToClassList("main-salmon-states-value");
            //子VEにlabel(2)をAdd
            statesVE.Add(statesValueLabel);
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