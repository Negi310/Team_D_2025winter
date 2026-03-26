using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class FemaleIllustrationManager : MonoBehaviour
{
    #region 画像
    [SerializeField] private List<Texture2D> hairLineList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> hairPaintList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> eyebrowList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> eyeLineList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> eyePaintList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> mouthList = new List<Texture2D>();
    [SerializeField] private List<Texture2D> otherTextureList = new List<Texture2D>();
    
    #endregion

    //色SO
    [SerializeField] private SalmonColorSO salmonColorSO;

    //key色(enum)　値色(Color)とした辞書
    private Dictionary<SalmonColor,Color> colorDic;



    //立ち絵のVEなどの構造  UI Builder側はVE配置と識別用クラスの設定のみ 以下クラスはFemaleSalmonIllust.ussにある　親VEには適宜レイアウト用のussのクラスで配置と大きさを設定
    //親VE(className:"female-illustration_base")
    //|--VE(1)(className:"female-illustration_body-paint") 体の色が変わる部分
    //|--VE(2)(className:"female-illustration_body-base") 体の色が変わらない部分
    //|--VE(3)(className:"female-illustration_mouth") 口 mouthList参照
    //|--VE(4)(className:"female-illustration_eye-paint") 目の色が変わる部分　ないやつもある eyePaintList参照
    //|--VE(5)(className:"female-illustration_eye-line") 目の線画と色が変わらない部分 eyeLineList参
    //|--VE(6)(className:"female-illustration_hair-paint") 髪の塗り　hairLineList参照照
    //|--VE(7)(className:"female-illustration_hair-line") 髪の線画　hairLineList参照
    //|--VE(8)(className:"female-illustration_eyebrow") 眉 eyebrowList参照

    void Awake()
    {
        //enumと色の対応を辞書に
        colorDic = new Dictionary<SalmonColor,Color>()
        {
            {SalmonColor.Orange,salmonColorSO.HairOrange},
            {SalmonColor.Red,salmonColorSO.HairRed},
            {SalmonColor.Blue,salmonColorSO.HairBlue},
            {SalmonColor.Yellow,salmonColorSO.HairYellow},
            {SalmonColor.Green,salmonColorSO.HairGreen},
            {SalmonColor.Pink,salmonColorSO.HairPink},
            {SalmonColor.Purple,salmonColorSO.HairPurple},
            {SalmonColor.SkyBlue,salmonColorSO.HairSkyBlue},
            {SalmonColor.YellowGreen,salmonColorSO.HairYellowGreen}
        };
    }
    //初期設定 23の変わらない部分の画像を設定する
    public void InitIllust(VisualElement playerImage)
    {
        playerImage.Q<VisualElement>(className:"female-illustration_body-base").style.backgroundImage = otherTextureList[0];
        playerImage.Q<VisualElement>(className:"female-illustration_body-paint").style.backgroundImage = otherTextureList[1];
    }
    //変更適用
    public void SetUpIllust(VisualElement playerImage,SalmonHair hair, SalmonEyeFemale eye, SalmonColor color, SalmonEyebrowFemale eyebrow, SalmonMouthFemale mouth) 
    {
        
        //VE8
        playerImage.Q<VisualElement>(className:"female-illustration_eyebrow").style.backgroundImage = eyebrowList[(int)eyebrow];
        //VE7
        playerImage.Q<VisualElement>(className:"female-illustration_hair-line").style.backgroundImage = hairLineList[(int)hair];
        //VE6
        playerImage.Q<VisualElement>(className:"female-illustration_hair-paint").style.backgroundImage = hairPaintList[(int)hair];
        playerImage.Q<VisualElement>(className:"female-illustration_hair-paint").style.unityBackgroundImageTintColor = colorDic[color];
        //目用に補色の番号取得
        int eyeColorNumber = (int) color + 4;
        if(eyeColorNumber >= 8) eyeColorNumber -= 8;
        SalmonColor eyeColor = (SalmonColor) eyeColorNumber;
        //VE5
        playerImage.Q<VisualElement>(className:"female-illustration_eye-line").style.backgroundImage = eyeLineList[(int)eye];
        //VE4
        playerImage.Q<VisualElement>(className:"female-illustration_eye-paint").style.backgroundImage = eyePaintList[(int)eye];
        playerImage.Q<VisualElement>(className:"female-illustration_eye-paint").style.unityBackgroundImageTintColor = colorDic[eyeColor];
        //VE3
        playerImage.Q<VisualElement>(className:"female-illustration_mouth").style.backgroundImage = mouthList[(int)mouth];
        //VE1
        playerImage.Q<VisualElement>(className:"female-illustration_body-paint").style.unityBackgroundImageTintColor = colorDic[color];
    }
}