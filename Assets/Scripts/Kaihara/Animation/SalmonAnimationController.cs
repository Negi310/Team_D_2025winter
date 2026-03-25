using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SalmonAnimationController : MonoBehaviour
{
    //鮭の画像関連のオブジェクト構造
    //鮭画像全体用(全体のサイズ調整など)<-これをprefabにしてますのでこれを挙動部分の下に
    //|--髪1(SpriteRenderer付き)
    //|--髪2(SpriteRenderer付き)
    //|--体1(色変更なし)(SpriteRenderer,Animator付き)
    //|--体2(色変更あり)(SpriteRenderer,Animator付き)

    //key髪型　値画像とした辞書
    private Dictionary<SalmonHair,(Sprite hair1,Sprite hair2)> hairDic;
    //髪型の画像
    #region
    [SerializeField] private Sprite hair1Long;
    [SerializeField] private Sprite hair2Long;
    [SerializeField] private Sprite hair1Normal;
    [SerializeField] private Sprite hair2Normal;
    [SerializeField] private Sprite hair1Short;
    [SerializeField] private Sprite hair2Short;
    #endregion
    //色SO
    [SerializeField] private SalmonColorSO salmonColorSO;

    //key色(enum)　値色(Color)とした辞書
    private Dictionary<SalmonColor,Color> colorDic;

    //mpb
    private MaterialPropertyBlock mpb;
    //初期設定
    private void Awake()
    {
        //髪の画像を辞書に
        hairDic = new Dictionary<SalmonHair,(Sprite hair1,Sprite hair2)>()
        {
            {SalmonHair.Long, (hair1Long,hair2Long)},
            {SalmonHair.Normal, (hair1Normal,hair2Normal)},
            {SalmonHair.Short, (hair1Short,hair2Short)}
        };
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

        //mpb 
        mpb = new MaterialPropertyBlock();
    }
    //髪型変更 髪1・2のSpriteRenderを引数に
    public void HairStyleSet(SalmonHair hairStyle,SpriteRenderer hair1Renderer,SpriteRenderer hair2Renderer)
    {
        //画像切り替え
        hair1Renderer.sprite = hairDic[hairStyle].hair1;
        hair2Renderer.sprite = hairDic[hairStyle].hair2;
    }
    //カラー変更 髪と体2のSpriteRendererを引数に
    public void ColorSet(SalmonColor salmonColor,SpriteRenderer hairRenderer,SpriteRenderer bodyRenderer)
    {
        //髪色変更
        hairRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor",colorDic[salmonColor]);
        hairRenderer.SetPropertyBlock(mpb);
        //体色変更
        bodyRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor",colorDic[salmonColor]);
        bodyRenderer.SetPropertyBlock(mpb);

    }
    //ジャンプアニメーション 鮭画像全体用・体1・体2のAnimatorを引数に
    public void JumpAnimation(Animator baseAnimator, Animator body1Animator, Animator body2Animator)
    {
        baseAnimator.SetTrigger("Jump");
        body1Animator.SetTrigger("Jump");
        body2Animator.SetTrigger("Jump");
    }

    //ダメージ時アニメーション用のコルーチン
    private IEnumerator DamageFlash(GameObject baseObject)
    {
        SpriteRenderer[] children = baseObject.GetComponentsInChildren<SpriteRenderer>();

        foreach(var child in children)
        {
            if (child.sharedMaterial.shader.name == "Custom/Color")
            {
                child.GetPropertyBlock(mpb);
                mpb.SetFloat("_FlashAlpha",0);
                child.SetPropertyBlock(mpb);
            }
            else
            {
                Color color = child.color;
                color.a = 0;
                child.color = color;
            }
        }

        yield return new WaitForSeconds(0.1f);
        foreach(var child in children)
        {
            if (child.sharedMaterial.shader.name == "Custom/Color")
            {
                child.GetPropertyBlock(mpb);
                mpb.SetFloat("_FlashAlpha",1);
                child.SetPropertyBlock(mpb);
            }
            else
            {
                Color color = child.color;
                color.a = 1;
                child.color = color;
            }
        }

        yield return new WaitForSeconds(0.1f);
        foreach(var child in children)
        {
            if (child.sharedMaterial.shader.name == "Custom/Color")
            {
                child.GetPropertyBlock(mpb);
                mpb.SetFloat("_FlashAlpha",0);
                child.SetPropertyBlock(mpb);
            }
            else
            {
                Color color = child.color;
                color.a = 0;
                child.color = color;
            }
        }

        yield return new WaitForSeconds(0.1f);
        foreach(var child in children)
        {
            if (child.sharedMaterial.shader.name == "Custom/Color")
            {
                child.GetPropertyBlock(mpb);
                mpb.SetFloat("_FlashAlpha",1);
                child.SetPropertyBlock(mpb);
            }
            else
            {
                Color color = child.color;
                color.a = 1;
                child.color = color;
            }
        }
    }
    //ダメージ時アニメーション 鮭画像全体用のgameobjectを引数に
    public void DamageAnimation(GameObject baseObject)
    {
        StartCoroutine(DamageFlash(baseObject));
    }
}
