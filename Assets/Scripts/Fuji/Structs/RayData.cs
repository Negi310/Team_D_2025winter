public struct RayData
{
    public float LeftDistance;
    public float RightDistance;
    public float ForwardRockDistance;
    public float ForwardNextRockDistance;
    public float ForwardSalmonDistance;
    public float TotalWidth => LeftDistance + RightDistance;
}