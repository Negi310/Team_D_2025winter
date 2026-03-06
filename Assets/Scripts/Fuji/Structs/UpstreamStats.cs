[System.Serializable]
public struct UpstreamStats 
{
    public float Power;
    public float Jump;
    public float Cautiousness; // 警戒心
    public float Stamina;

    public UpstreamStats(float power, float jump, float cautiousness, float stamina)
    {
        Power = power;
        Jump = jump;
        Cautiousness = cautiousness;
        Stamina = stamina;
    }
}