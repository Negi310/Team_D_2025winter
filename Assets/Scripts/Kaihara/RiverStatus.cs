using UnityEngine;

public class RiverStatus : MonoBehaviour
{
    //現在のターン
    [SerializeField]private int seaTurn;
    //次の川の名前
    [SerializeField]private string riverName;

    public int SeaTurn
    {
        get {return seaTurn;}
    }
    public string RiverName
    {
        get {return riverName;}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
