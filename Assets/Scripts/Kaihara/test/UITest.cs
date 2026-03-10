using UnityEngine;
using UnityEngine.InputSystem;

public class UITest : MonoBehaviour
{
    //UI動作テスト用
    //海UI
    [SerializeField] private SeaUIManager seaUIManager;
    //求愛UI
    [SerializeField] private CourtingUIManager courtingUIManager;
    //命名UI
    [SerializeField] private NamingUIManager namingUIManager;
    //プレイヤーのステータス
    [SerializeField] private string playerJump;
    [SerializeField] private string playerPower;
    [SerializeField] private string playerRiskhedging;
    [SerializeField] private string playerStamina;
    [SerializeField] private string playerColor;
    [SerializeField] private string playerSize;
    [SerializeField] private string playerShape;
    //パートナーのステータス
    [SerializeField] private string weakestPer;
    [SerializeField] private string weakestSuc;

    [SerializeField] private string weakPer;
    [SerializeField] private string weakSuc;
    
    [SerializeField] private string normalPer;
    [SerializeField] private string normalSuc;

    [SerializeField] private string strongPer;
    [SerializeField] private string strongSuc;

    [SerializeField] private string strongestPer;
    [SerializeField] private string strongestSuc;


    //海フェーズターン数
    [SerializeField] private int seaTurn;
    //川の名前
    [SerializeField] private string riverName;
    //求愛残り回数
    [SerializeField] private int courtingTimes;
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
            seaUIManager.Show();
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerJump,playerPower,playerRiskhedging,playerStamina,playerColor,playerSize,playerShape);
            seaUIManager.SetUpUI(playerStatusList,seaTurn,riverName);
        }
        //kキーでSeaUI非表示
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            seaUIManager.Hide();
        }

        //uキーでCourtingUI表示
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            courtingUIManager.Show();
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerJump,playerPower,playerRiskhedging,playerStamina,playerColor,playerSize,playerShape);
            var partnerStatusList = forUIStatusBuilder.PartnersListBuild(weakestPer,weakestSuc,weakPer,weakSuc,normalPer,normalSuc,strongPer,strongSuc,strongestPer,strongestSuc);
            courtingUIManager.SetUpUI(playerStatusList,partnerStatusList,courtingTimes,riverName);
        }
        //iキーでCourtingUI非表示
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            courtingUIManager.Hide();
        }
        //nキーでNamingUI表示
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            namingUIManager.Show();
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerJump,playerPower,playerRiskhedging,playerStamina,playerColor,playerSize,playerShape);
            namingUIManager.SetUpUI(playerStatusList);
        }
        //mキーでNamingUI非表示
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            namingUIManager.Hide();
        }
    }
}
