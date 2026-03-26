[System.Serializable]
public struct UpstreamStats 
{
    public float Speed;
    public float Jump;
    public float Stamina;
    public float Attack;
    public float Intelligence;

    public UpstreamStats(float speed, float jump, float stamina, float attack, float intelligence)
    {
        Speed = speed;
        Jump = jump;
        Stamina = stamina;
        Attack = attack;
        Intelligence = intelligence;
    }
}