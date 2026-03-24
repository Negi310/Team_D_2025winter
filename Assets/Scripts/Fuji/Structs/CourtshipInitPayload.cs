// 【Upstream ➡ Courtship】川をどれだけ進んだかを渡す
public struct CourtshipInitPayload : IPayload
{
    public float DistanceTraveled; // 進行度（相手の生成ランクに影響）
    
    public CourtshipInitPayload(float distanceTraveled)
    {
        DistanceTraveled = distanceTraveled;
    }
}