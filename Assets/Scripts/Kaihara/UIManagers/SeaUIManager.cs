using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SeaUIManager : MonoBehaviour
{
    public Action<EventData> OnEventSelected;
    
    //uiDocument
    [SerializeField]private UIDocument uiDocument;
    //root
    private VisualElement root;
    //プレイヤーのUIのlabel(3)のリスト
    private List<Label> playerUIList = new List<Label>(); 
    //プレイヤーのUIのlabel(4)のリスト
    private List<Label> statusIncreaceUIList = new List<Label>();
    //トレーニングのステータスの上昇量のリスト
    private List<List<string>> statusIncreaceList = new List<List<string>>();
     //川のステータスのひ孫VEのリスト
    private List<VisualElement> riverUIList = new List<VisualElement>();
    //川のステータスのひ孫VEのデフォルトの横幅を保存
    private float defaultRiverStatusValueWidth;
    //川の名前表示のlabel
    private Label riverNameLabel;
    //ターン数のlabel
    private Label turnUILabel;
    //川のステータスの最大値
    private const float riverStatusMax = 20;

    void Awake()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        //初期設定
        InitPlayerUI();
        InitRiverUI();
        InitTrainingButton();
        //VEの横幅取得のためレイアウト確定後に実行
        root.RegisterCallbackOnce<GeometryChangedEvent>(evt =>
        {
            //川UIのひ孫VEの横幅取得
            defaultRiverStatusValueWidth = Mathf.Floor(riverUIList[0].resolvedStyle.width / riverUIList[0].parent.resolvedStyle.width * 100);
        });
    }
    //UIの表示
    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }
    //UIの非表示
    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }

    //プレイヤーのステータスのUIの初期設定
    void InitPlayerUI()
    {
        //VEとかの構造
        //親VE(class:main-salmon-statusbar)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-status_status-information)(ステータスについての解説)
        //|  |--label(2)(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(3)(class:main-salmon-status_value)(ステータスの値を表示)
        //|  |--label(4)(class:main-salmon-status_increace)(ステータスの上昇値を表示)
        //:
        
        //いったん子VEのリストを生成
        var childVEList = root.Q<VisualElement>(className:"main-salmon-statusbar").Children().ToList();
        
        foreach(VisualElement childVE in childVEList)
        {
            //label(3)のリストに追加
            playerUIList.Add(childVE.Q<Label>(className:"main-salmon-status_value"));
            //label(4)のリストに追加
            var label4 = childVE.Q<Label>(className:"main-salmon-status_increace");
            if(label4 != null) statusIncreaceUIList.Add(label4);
            //子VEの数に応じて最小サイズを決定(子VEがn個なら100/n*0.9%)
            childVE.style.minWidth = new Length(100 / childVEList.Count * 0.9f , LengthUnit.Percent);
        }
    }

    //川のステータスのUIの初期設定
    void InitRiverUI()
    {
        //川の情報の処理
        //直接関係はないが求愛回数のlabel(class:remaining-court-times)も川の処理で一緒に処理する
        //VEの構造
        //親VE(class:next-river-information)
        //|--Label(1)(クラスなし) (川の名前を表示)
        //|--Button(クラスなし) (川の詳細情報用のVEの表示非表示切り替え)
        //|--子VE(クラスなし) (この下に川の詳細情報を表示)
        //   |孫VE--(クラスなし)(ステータスの種類ごとに一つ)
        //   :    |--label(2)(class:next-river-information_status_name)(ステータスの名前表示)
        //        |--ひ孫VE(class:next-river-information_status_value)(ステータスの値表示)

        //親VEを一時保存
        var parentVE = root.Q<VisualElement>(className:"next-river-information");
        //ターン数表示のlabel保存
        turnUILabel = root.Q<Label>(className:"turn-ui");
        //label(1)を保存
        riverNameLabel = parentVE.Children().OfType<Label>().First();
        //孫VEリストを一時保存
        var groundchildrenList = parentVE.Children().OfType<VisualElement>().First().Children().OfType<VisualElement>().ToList();
        //ひ孫VEのリストを生成
        foreach(VisualElement groundchildVE in groundchildrenList)
        {
            riverUIList.Add(groundchildVE.Q<VisualElement>(className:"next-river-information_status_value"));
        }
        //Buttonのイベント設定
        var riverstatusButton = root.Q<VisualElement>(className:"next-river-information").Q<Button>();
        riverstatusButton.clicked += () =>
        {
            //クラス変更で川のステータスの表示状況を切り替え
            riverstatusButton.parent.EnableInClassList("is-open",!riverstatusButton.parent.ClassListContains("is-open"));
            //ボタンの文字切り替え
            if(riverstatusButton.text == ">") riverstatusButton.text = "v";
            else riverstatusButton.text = ">";
        };
    }
    void InitTrainingButton()
    {
        //トレーニングを選択するタブの処理
        //VE構造
        //親VE(class:training-tab)(training-buttonで表示)
        //|--button1(class:training-tab_training-button)(ステータスごとに一つ クリックで対応するステータスのトレーニングのイベントに移行)
        //:
        //|--button2(class:training-tab_cancel-button)(タブの非表示)

        //Button(class:training-button)(上のタブの表示用ボタン)

        //親VEを取得
        var parentVE = root.Q<VisualElement>(className:"training-tab");

        //トレーニングの表示のボタンにイベント設定
        var trainingTabOpenButton = root.Q<Button>(className:"training-button");
        trainingTabOpenButton.clicked += () =>
        {
            parentVE.AddToClassList("is-open");
        };
        //button2に非表示イベント設定
        var trainingCancelButon = parentVE.Q<Button>(className:"training-tab_cancel-button");
        trainingCancelButon.clicked += () =>
        {
            parentVE.RemoveFromClassList("is-open");
        };

    }

    //内容更新
    public void SetUpUI(List<string> playerStatusList,List<string> riverStatusList,int turn,string riverName)
    {
        SetUpStatusUI(playerStatusList);
        SetUpRiverUI(turn,riverName,riverStatusList);
    }
    //ステータス表示の設定
    void SetUpStatusUI(List<string> playerStatusList)
    {
        //VEとかの構造
        //親VE(class:main-salmon-statusbar)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(1)(class:main-salmon-status_status-information)(ステータスについての解説)
        //|  |--label(2)(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--label(3)(class:main-salmon-status_value)(ステータスの値を表示)
        //:

        //label(3)の表示内容を更新
        for(int i = 0; i < playerUIList.Count; i++)
        {
            playerUIList[i].text = playerStatusList[i];
        }
    }
    void SetUpRiverUI(int turn,string riverName,List<string> riverStatus)
    {
        //川の情報の処理
        //直接関係はないがターン数のlabel(class:turn-ui)も川の処理で一緒に処理する
        //VEの構造
        //親VE(class:next-river-information)
        //|--Label(1)(クラスなし) (川の名前を表示)
        //|--Button(クラスなし) (川の詳細情報用のVEの表示非表示切り替え)
        //|--子VE(クラスなし) (この下に川の詳細情報を表示)
        //   |孫VE--(クラスなし)(ステータスの種類ごとに一つ)
        //   :    |--label(2)(class:next-river-information_status_name)(ステータスの名前表示)
        //        |--ひ孫VE(class:next-river-information_status_value)(ステータスの値表示)

        //現在のターンを表示
        turnUILabel.text = "ターン\n" + turn;
        //川の名前表示
        riverNameLabel.text = "NEXT>" + riverName;
        //ひ孫VEの内容を更新
        for(int i = 0; i < riverStatus.Count; i++)
        {
            riverUIList[i].style.width = new Length(defaultRiverStatusValueWidth * float.Parse(riverStatus[i]) / riverStatusMax, LengthUnit.Percent);
        }
    }


}