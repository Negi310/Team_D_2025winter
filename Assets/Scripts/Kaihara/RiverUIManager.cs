using UnityEngine;
using UnityEngine.UIElements;

public class RiverUIManager : MonoBehaviour
{
    //uiDocument
    [SerializeField] private UIDocument uiDocument;
    private VisualElement root;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //uiDocumentのrootVE取得
        root = uiDocument.rootVisualElement;
        //パートナーのVEのリスト作成
        var partnerList = root.Query<VisualElement>(className:"partner-information").ToList();
        //各パートナーのVEにイベント設定
        foreach(VisualElement i in partnerList)
        {
            //ホバー時にイベント発火
            i.RegisterCallback<MouseEnterEvent>(evt =>
            {
                //ホバー時に最前列に
                i.BringToFront();
                //川の情報の方が前になるように
                root.Q<VisualElement>(className:"next-river-information").BringToFront();
                
            });
        }
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
}
