using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.HID;
public class CourtingUIManager : MonoBehaviour
{
    //uiDocument
    [SerializeField]private UIDocument uiDocument;
    //rootVisualElement
    private VisualElement root;
    //パートナー候補UIの性格のlabel1,成功率label1,imageのリスト
    private List<(Label personalityLabel,Label successLabel,Image image)> partnerUIList = new List<(Label personalityLabel,Label successLabel,Image image)>();
    //プレイヤーのUIの孫VEのリスト
    private List<VisualElement> playerSwimUIList = new List<VisualElement>();
    //プレイヤーのUIのlabel(2)のリスト
    private List<Label> playerCourtUIList = new List<Label>();
    //川のステータスのひ孫VEのリスト
    private List<VisualElement> riverUIList = new List<VisualElement>();
    //プレイヤーのUIの孫VEのデフォルトの横幅を保存
    private float defaultPlayerStatusValueWidth;
    //川のステータスのひ孫VEのデフォルトの横幅を保存
    private float defaultRiverStatusValueWidth;
    //川の名前表示のlabel
    private Label riverNameLabel;
    //残り求愛回数のlabel
    private Label courtingTimesLabel;

    //プレイヤーのステータスの最大値(いったん20)
    private const float playerStatusMax = 20;
    void Awake()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        //各UIの初期設定
        //パートナー候補UIの初期設定
        InitPartnerUI();
        //プレイヤーUIの初期設定
        InitPlayerUI();
        //川UIの初期設定
        InitRiverUI();
        //VEの横幅取得のためレイアウト確定後に実行
        root.RegisterCallbackOnce<GeometryChangedEvent>(evt =>
        {
            //プレイヤーUIの孫VEの横幅取得
            defaultPlayerStatusValueWidth = Mathf.Floor(playerSwimUIList[0].resolvedStyle.width / playerSwimUIList[0].parent.resolvedStyle.width * 100);
            //川UIのひ孫VEの横幅取得
            defaultRiverStatusValueWidth = Mathf.Floor(riverUIList[0].resolvedStyle.width / riverUIList[0].parent.resolvedStyle.width * 100);
            //非表示
            Hide();
        });
    }

    //UI生成(引数は求愛UIのプレファブ,プレイヤーのステータスのリスト,パートナーの情報のリスト,残り求愛回数,川の名前)
    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }
    
    //UIの非表示
    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }

    //パートナー候補UIの初期設定
    void InitPartnerUI()
    {
        //パートナー候補のUI
        //親VE(class:partner-information)(パートナー一体につき一つ)
        //|--子VE(1)(class:partner-information_background)(ホバー時に拡大してホバー対象をわかりやすくする)
        //|--image(クラスなし)(パートナーのイラスト表示用)
        //|--子VE(2)(class:partner-information_status)(相性などの情報表示用　ホバー時にのみ見える)
        //   |--孫VE(クラスなし)(項目の並びの設定用 性格・成功率の順で配置)
        //   :  |--label(1)(class:partner-information_status_name)(項目の名前の表示)
        //      |--label(2)(class:partner-information_status_value)(項目の値の表示)

        //親VEのリスト生成
        var parentVEList =  root.Query<VisualElement>(className:"partner-information").ToList();
        //パートナー候補の数だけ実行し子VE(2)とimageのリストを生成
        foreach (VisualElement parentVE in parentVEList)
        {
            //孫VEを一旦リストに保存
            var groundChildVEList = parentVE.Q<VisualElement>(className:"partner-information_status").Children().ToList();
            //変更箇所をタプルにまとめる
            (Label personalityLabel,Label successLabel,Image image) partnerTaple = (groundChildVEList[0].Q<Label>(className:"partner-information_status_value"),groundChildVEList[1].Q<Label>(className:"partner-information_status_value"),parentVE.Q<Image>());
            // リストに追加していく
            partnerUIList.Add(partnerTaple);
            //ホバー時のイベント設定
            parentVE.RegisterCallback<MouseEnterEvent>(evt =>
            {
                //ホバー時に最前列に
                parentVE.BringToFront();
                //川の情報の方が前になるように
                root.Q<VisualElement>(className:"next-river-information").BringToFront();
                
            });
        }
    }
    //川の情報のUIの初期設定
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
        //求愛回数表示のlabel保存
        courtingTimesLabel = root.Q<Label>(className:"remaining-court-times");
        //label(1)を保存
        riverNameLabel = parentVE.Children().OfType<Label>().First();
        //孫VEリストを一時保存
        var groundchildrenList = parentVE.Q<VisualElement>().Query<VisualElement>().ToList();
        //ひ孫VEのリストを生成
        foreach(VisualElement groundchildVE in groundchildrenList)
        {
            riverUIList.Add(groundchildVE.Q<VisualElement>(className:"next-river-information_status_value"));
        }
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
    void InitPlayerUI()
    {
        //VEとかの構造
        //親VE(class:main-salmon-status)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--孫VE(class:main-salmon-status_value--swim)(ステータスの値を表示)/label2(class:main-salmon-status_value--court)(求愛ステータスはこっち)
        //:

        //子VEのリスト作成
        var childVEList = root.Q<VisualElement>(className:"main-salmon-status").Children().ToList();
        //ステータスの数だけ実行
        for(int i = 0; i < childVEList.Count; i++)
        {
            //孫VEがあれば孫VEのリストに追加
            if(childVEList[i].Q<VisualElement>(className:"main-salmon-status_value--swim") != null)
            playerSwimUIList.Add(childVEList[i].Q<VisualElement>(className:"main-salmon-status_value--swim"));
            //なければ求愛用なのでlabel(2)をリストに追加
            else  playerCourtUIList.Add(childVEList[i].Q<Label>(className:"main-salmon-status_value--court"));
        }
        
    }
    //表示内容更新
    public void SetUpUI(List<string> playerStatusList,List<(string personality,string successRate)> partnerStatusList,int courtTimes,string riverName)
    {
        //パートナーUIの内容更新
        SetUpPartnerUI(partnerStatusList);
        //プレイヤーUIの内容更新
        SetUpPlayerUI(playerStatusList);
        //川UIの内容更新
        SetUpRiverUI(courtTimes,riverName);
    }


    //パートナーUIの内容更新
    void SetUpPartnerUI(List<(string personality,string successRate)> partnerStatusList)
    {
        //VE構成
        //親VE(class:partner-information)(パートナー一体につき一つ)
        //|--子VE(1)(class:partner-information_background)(ホバー時に拡大してホバー対象をわかりやすくする)
        //|--image(クラスなし)(パートナーのイラスト表示用)
        //|--子VE(2)(class:partner-information_status)(相性などの情報表示用　ホバー時にのみ見える)
        //   |--孫VE(クラスなし)(項目の並びの設定用)
        //   :  |--label(1)(class:class:partner-information_status_name)(項目の名前の表示)
        //      |--label(2)(class:class:partner-information_status_value)(項目の値の表示)

        //UIの各種設定
        //子VE(2)の数だけ(=パートナーの数だけ)実行
        for(int i = 0; i < partnerUIList.Count; i++)
        {
            //性格を表示
            partnerUIList[i].personalityLabel.text = partnerStatusList[i].personality;
            //成功率を表示
            partnerUIList[i].successLabel.text = partnerStatusList[i].successRate + "%";
        }
    }

    //プレイヤーのステータスのUIの内容更新
    
    void SetUpPlayerUI(List<string> playerStatusList)
    {
        //VEとかの構造
        //親VE(class:main-salmon-status)
        //|--子VE(クラスなし)(ステータスごとに一つ存在)
        //|  |--label(class:main-salmon-status_name--swim)(ステータスの名前を表示　求愛用の場合はswim->courtで色変更)
        //|  |--孫VE(class:main-salmon-status_value--swim)(ステータスの値を表示)/label2(class:main-salmon-status_value--court)(求愛ステータスはこっち)
        //:

        //孫VEの数だけ実行
        for(int i = 0; i < playerSwimUIList.Count; i++)
        {
            //現在値と最大値から比率を計算し長さを指定
            playerSwimUIList[i].style.width = new Length(defaultPlayerStatusValueWidth * float.Parse(playerStatusList[i]) / playerStatusMax, LengthUnit.Percent);
        }
        //label(2)の数だけ実行
        for(int i = 0; i < playerCourtUIList.Count; i++)
        {
            //内容更新
            playerCourtUIList[i].text = playerStatusList[i+playerSwimUIList.Count];
        }
    }

    //川の情報のUI
    void SetUpRiverUI(int courtTimes,string riverName)
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
        
        //現在のターンを表示
        courtingTimesLabel.text = "残り\n" + courtTimes + "回";

        //川の名前表示
        riverNameLabel.text = "NEXT>" + riverName;

        
    }
}