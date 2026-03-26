using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

public class NamingUIManager : MonoBehaviour
{
    public event Action<string> OnDecideButtonClicked;
    
    //uiDocument
    [SerializeField]private UIDocument uiDocument;
    //立ち絵設定
    [SerializeField] private MaleIllustrationManager maleIllust;
    //rootVisualElement
    private VisualElement root;
    //プレイヤーのUIの孫VEのリスト
    private List<VisualElement> playerSwimUIList = new List<VisualElement>();
    //プレイヤーのUIのlabel(2)のリスト
    private List<Label> playerCourtUIList = new List<Label>();
    //プレイヤーのUIの孫VEのデフォルトの横幅を保存
    private float defaultPlayerStatusValueWidth;
    // 名前入力欄
    private TextField _nameInputField; 
    // 決定ボタン
    private Button _decideButton;
    //プレイヤー立ち絵のVE
    private VisualElement playerIllust;     

    //プレイヤーのステータスの最大値(いったん20)
    private const float playerStatusMax = 20;
    void Awake()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        _nameInputField = root.Q<TextField>("TextField");
        _decideButton = root.Q<Button>("Button");
        
        //ボタンが押されたら、入力欄のテキストを取り出してPresenterへ叫ぶ
        _decideButton.clicked += () => OnDecideButtonClicked?.Invoke(_nameInputField.value);
        //UIの初期設定
        InitPlayerUI();
        playerIllust = root.Q<VisualElement>(className:"male-illustration_base");
        maleIllust.InitIllust(playerIllust);
        //VEの横幅取得のためレイアウト確定後に実行
        root.RegisterCallbackOnce<GeometryChangedEvent>(evt =>
        {
            //プレイヤーUIの孫VEの横幅取得
            defaultPlayerStatusValueWidth = Mathf.Floor(playerSwimUIList[0].resolvedStyle.width / playerSwimUIList[0].parent.resolvedStyle.width * 100);
            //上記横幅取得が非表示の時できないためここで非表示 
            Hide();
        });
    }

    //UI表示
    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
        DOTween.To(
            () => root.style.opacity.value,
            x => root.style.opacity = x,
            1f, // 目標値 (不透明)
            0.5f // かける秒数
        ).SetEase(Ease.OutQuad);
    }
    
    //UIの非表示
    public void Hide()
    {
        DOTween.To(
            () => root.style.opacity.value,
            x => root.style.opacity = x,
            0f, // 目標値 (透明)
            0.5f
        ).SetEase(Ease.InQuad).OnComplete(() =>
        {
            root.style.display = DisplayStyle.None;
            _nameInputField.value = string.Empty;
        });
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
    public void SetUpUI(List<string> playerStatusList,SalmonHair hair,SalmonEyeMale eye,SalmonColor color,SalmonEyebrowMale eyebrow,SalmonMouthMale mouth,bool isPale,SalmonSize size)
    {
        SetUpPlayerUI(playerStatusList);
        maleIllust.SetUpIllust(playerIllust,hair,eye,color,eyebrow,mouth,isPale,size);
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
}
