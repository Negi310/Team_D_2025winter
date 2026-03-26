using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UITest : MonoBehaviour
{
    //UI動作テスト用
    #region UI
    //海UI
    [SerializeField] private SeaUIManager seaUIManager;
    //求愛UI
    [SerializeField] private CourtingUIManager courtingUIManager;
    //命名UI
    [SerializeField] private NamingUIManager namingUIManager;
    //プレイヤーのステータス
    [SerializeField] private string playerSpeed;
    [SerializeField] private string playerJump;
    [SerializeField] private string playerStamina;
    [SerializeField] private string playerAttack;
    [SerializeField] private string playerIntelligence;
    [SerializeField] private string playerColor;
    [SerializeField] private string playerSize;
    [SerializeField] private string playerShape;
    //各トレーニングでのステータスの変化量
    [SerializeField] private string increaseSpeed;
    [SerializeField] private string increaseJump;
    [SerializeField] private string increaseStamina;
    [SerializeField] private string increaseAttack;
    [SerializeField] private string increaseIntelligence;
    //川のステータス
    [SerializeField] private string a;
    [SerializeField] private string b;
    [SerializeField] private string c;
    [SerializeField] private string d;
    [SerializeField] private string e;
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
    #endregion
    //Animatorテスト用
    #region Animation
    //立ち絵テスト用
    #region 
    [SerializeField] private SalmonHair salmonHair;
    [SerializeField] private SalmonEyeMale salmonEye;
    [SerializeField] private SalmonEyebrowMale salmonEyebrow;
    [SerializeField] private SalmonMouthMale salmonMouth;
    [SerializeField] private bool isPale;
    #region メス5名
    [SerializeField] private SalmonColor weakestColor;
    [SerializeField] private SalmonColor weakColor;
    [SerializeField] private SalmonColor normalColor;
    [SerializeField] private SalmonColor strongColor;
    [SerializeField] private SalmonColor strongestColor;
    [SerializeField] private SalmonHair weakestHair;
    [SerializeField] private SalmonHair weakHair;
    [SerializeField] private SalmonHair normalHair;
    [SerializeField] private SalmonHair strongHair;
    [SerializeField] private SalmonHair strongestHair;
    [SerializeField] private SalmonEyeFemale weakestEye;
    [SerializeField] private SalmonEyeFemale weakEye;
    [SerializeField] private SalmonEyeFemale normalEye;
    [SerializeField] private SalmonEyeFemale strongEye;
    [SerializeField] private SalmonEyeFemale strongestEye;
    [SerializeField] private SalmonEyebrowFemale weakestEyebrow;
    [SerializeField] private SalmonEyebrowFemale weakEyebrow;
    [SerializeField] private SalmonEyebrowFemale normalEyebrow;
    [SerializeField] private SalmonEyebrowFemale strongEyebrow;
    [SerializeField] private SalmonEyebrowFemale strongestEyebrow;
    [SerializeField] private SalmonMouthFemale weakestMouth;
    [SerializeField] private SalmonMouthFemale weakMouth;
    [SerializeField] private SalmonMouthFemale normalMouth;
    [SerializeField] private SalmonMouthFemale strongMouth;
    [SerializeField] private SalmonMouthFemale strongestMouth;
    #endregion
    #endregion
    //アニメーションとか管理するクラス
    [SerializeField] SalmonAnimationController salmonAnimationController;
    //鮭の画像のベース
    [SerializeField] GameObject salmonBase;
    //鮭の髪の画像
    [SerializeField] GameObject salmonHair1;
    //鮭の髪の画像(塗り)
    [SerializeField] GameObject salmonHair2;
    //鮭の体1
    [SerializeField] GameObject salmonBody1;
    //鮭の体2
    [SerializeField] GameObject salmonBody2;
    //鮭の色
    [SerializeField] SalmonColor salmonColor;
    private int colorTest = 0;
    #endregion
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
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerSpeed,playerJump,playerStamina,playerAttack,playerIntelligence,playerColor,playerSize,playerShape);
            var riverStatusList = forUIStatusBuilder.RiverInformatinListBuild(a,b,c,d,e);
            var increaseStatusList = forUIStatusBuilder.TrainingStatuIcreaceList(increaseSpeed,increaseJump,increaseStamina,increaseAttack,increaseIntelligence);
            seaUIManager.SetUpUI(playerStatusList,riverStatusList,increaseStatusList,seaTurn,riverName,salmonHair,salmonColor,salmonEye,salmonEyebrow,salmonMouth,isPale,new List<string>{"A","B","C"});
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
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerSpeed,playerJump,playerStamina,playerAttack,playerIntelligence,playerColor,playerSize,playerShape);
            var partnerStatusList = forUIStatusBuilder.PartnersListBuild(weakestPer,weakestSuc,weakPer,weakSuc,normalPer,normalSuc,strongPer,strongSuc,strongestPer,strongestSuc);
            var riverStatusList = forUIStatusBuilder.RiverInformatinListBuild(a,b,c,d,e);
            var femaleIllustList = forUIStatusBuilder.FemaleIllustList(weakestHair,weakestEye,weakestColor,weakestEyebrow,weakestMouth,weakHair,weakEye,weakColor,weakEyebrow,weakMouth,normalHair,normalEye,normalColor,normalEyebrow,normalMouth,strongHair,strongEye,strongColor,strongEyebrow,strongMouth,strongestHair,strongestEye,strongestColor,strongestEyebrow,strongestMouth);
            courtingUIManager.SetUpUI(playerStatusList,partnerStatusList,femaleIllustList,SalmonHair.Long,SalmonEyeMale.Normal,SalmonColor.Orange,SalmonEyebrowMale.Normal,SalmonMouthMale.Normal,isPale,riverStatusList,courtingTimes,riverName);
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
            var playerStatusList = forUIStatusBuilder.PlayerStatusListBuild(playerSpeed,playerJump,playerStamina,playerAttack,playerIntelligence,playerColor,playerSize,playerShape);
            namingUIManager.SetUpUI(playerStatusList,salmonHair,salmonEye,salmonColor,salmonEyebrow,salmonMouth,isPale);
        }
        //mキーでNamingUI非表示
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            namingUIManager.Hide();
        }
        //spaceキーでジャンプアニメーション再生
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            salmonAnimationController.JumpAnimation(salmonBase.GetComponent<Animator>(),salmonBody1.GetComponent<Animator>(),salmonBody2.GetComponent<Animator>());
        }
        //enterキーでダメージ時アニメーション再生
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            salmonAnimationController.DamageAnimation(salmonBase);
        }
        //1キーで髪型ロング
        if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
        {
            salmonAnimationController.HairStyleSet(SalmonHair.Long,salmonHair1.GetComponent<SpriteRenderer>(),salmonHair2.GetComponent<SpriteRenderer>());
        }
        //2キーで髪型普通
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
        {
            salmonAnimationController.HairStyleSet(SalmonHair.Normal,salmonHair1.GetComponent<SpriteRenderer>(),salmonHair2.GetComponent<SpriteRenderer>());
        }
        //3キーで髪型ショート
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
        {
            salmonAnimationController.HairStyleSet(SalmonHair.Short,salmonHair1.GetComponent<SpriteRenderer>(),salmonHair2.GetComponent<SpriteRenderer>());
        }
        //4キーで色変更
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
        {
            salmonAnimationController.ColorSet(salmonColor,salmonHair2.GetComponent<SpriteRenderer>(),salmonBody2.GetComponent<SpriteRenderer>());
        }
        //5キーで色順番に変更
        if (Keyboard.current[Key.Digit5].wasPressedThisFrame)
        {
            salmonAnimationController.ColorSet((SalmonColor)colorTest,salmonHair2.GetComponent<SpriteRenderer>(),salmonBody2.GetComponent<SpriteRenderer>());
            colorTest += 1;
            if(colorTest >= 9) colorTest = 0;
        }
    }
}
