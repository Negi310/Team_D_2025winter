using UnityEngine;
using UnityEngine.InputSystem;

public class UITest : MonoBehaviour
{
    //UI動作テスト用
    //海UI
    [SerializeField] private SeaUIManager seaUIManager;
    //求愛UI
    [SerializeField] private CourtingUIManager courtingUIManager;
    //海UIプレファブ
    [SerializeField] private GameObject seaUIPrefab;
    //求愛UIプレファブ
    [SerializeField] private GameObject courtingUIPrefab;
    //プレイヤーのステータス
    [SerializeField] private float playerJump;
    [SerializeField] private float playerPower;
    [SerializeField] private float playerRiskhedging;
    [SerializeField] private float playerStamina;
    [SerializeField] private float playerColor;
    [SerializeField] private float playerSize;
    [SerializeField] private float playerShape;
    //パートナーのステータス
    [SerializeField] private string weakestCom;
    [SerializeField] private string weakestPer;
    [SerializeField] private string weakestSuc;

    [SerializeField] private string weakCom;
    [SerializeField] private string weakPer;
    [SerializeField] private string weakSuc;
    
    [SerializeField] private string normalCom;
    [SerializeField] private string normalPer;
    [SerializeField] private string normalSuc;

    [SerializeField] private string strongCom;
    [SerializeField] private string strongPer;
    [SerializeField] private string strongSuc;

    [SerializeField] private string strongestCom;
    [SerializeField] private string strongestPer;
    [SerializeField] private string strongestSuc;


    //海フェーズターン数
    [SerializeField] private int seaTurn;
    //川の名前
    [SerializeField] private string riverName;
    //求愛残り回数
    [SerializeField] private int CourtingTimes;
    private ForUIStatusBuilder forUIStatusBuilder;
    void Start()
    {
        forUIStatusBuilder = new ForUIStatusBuilder();
    }
    // Update is called once per frame
    void Update()
    {
        //jキーでSeaUI表示
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            seaUIManager.Show(seaUIPrefab,forUIStatusBuilder.PlayerStatusTapleListBuild(playerJump,playerPower,playerRiskhedging,playerStamina,playerColor,playerSize,playerShape),seaTurn,riverName);
        }
        //kキーでSeaUI非表示
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            seaUIManager.Hide();
        }
        //uキーでCourtingUI表示
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            var partnerList = forUIStatusBuilder.PartnerInformationListBuild(weakestCom,weakestPer,weakestSuc,weakCom,weakPer,weakSuc,normalCom,normalPer,normalSuc,strongCom,strongPer,strongSuc,strongestCom,strongestPer,strongestSuc);
            courtingUIManager.Show(courtingUIPrefab,forUIStatusBuilder.PlayerStatusTapleListBuild(playerJump,playerPower,playerRiskhedging,playerStamina,playerColor,playerSize,playerShape),partnerList,seaTurn,riverName);
        }
        //iキーでCourtingUI非表示
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            courtingUIManager.Hide();
        }
    }
}
/*
聞いておきたいこと
(1)UI用のデータのリスト化は呼び出す側でやるのかこちら側でやるのか
(2)仕様書で棒グラフになっているところはどうするのか　棒グラフで表示するのなら最大値の設定などが欲しい(現在値/最大値でグラフの長さを決定する)
(3)UIへのデータの受け渡し時の型　引数を一律でstring型にして呼び出し側で変換してもらうとそちら側の変更で完結するので楽そう？
(4)川やパートナー候補のUIに表示する情報について　プレイヤーのステータスと同様わかり次第ForUIStatusBuilderでリスト化して表示できるようにするので決まり次第連絡ください
　またパートナー候補の情報がどのように管理されているのか　どこまでまとめられているかによってリストにまとめるメソッドの引数を調整
(5)海フェーズのUIで表示する各ステータスの説明文の保存場所　別のところで保存する場合は引数に追加しておきます
*/