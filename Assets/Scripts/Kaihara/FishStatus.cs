using System;
using System.Collections.Generic;
using UnityEngine;

public class FishStatus : MonoBehaviour
{
    //魚のステータス
    [SerializeField]
    private int jump;
    [SerializeField]
    private int riskhedging;
    [SerializeField]
    private int stamina;
    [SerializeField]
    private int power;
    [SerializeField]
    private string color;
    [SerializeField]
    private string size;

    //UIに渡すステータス一式(名前、値、用途のタプルをリストに)(ui用なので値は文字列、外部からは読み取り専用)
    public List<(string name,string value,bool toSwim)> uiStatus{get;private set;}



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //UIに渡すステータス一式を、各ステータスごとに名前(string)・値(string)・用途(bool)のタプルにして、それらをまとめてリストにする
        uiStatus =  new List<(string name, string value, bool toSwim)>
    {
        ("ジャンプ",jump.ToString(),true),
        ("パワー",power.ToString(),true),
        ("リスクヘッジ",riskhedging.ToString(),true),
        ("スタミナ",stamina.ToString(),true),
        ("カラー",color,false),
        ("サイズ",size,false)
    };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
