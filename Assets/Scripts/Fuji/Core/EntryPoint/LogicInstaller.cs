using UnityEngine;

public class LogicInstaller
{
    public ConversationModel ConversationModel { get; }
    public ChunkPoolManager ChunkPoolManager { get; }
    public TreadmillModel TreadmillModel { get; }
    public RiverDirector RiverDirector { get; }

    public LogicInstaller()
    {
        ConversationModel = new ConversationModel();
        ChunkPoolManager = new ChunkPoolManager(new GameObject("PoolRoot").transform);
        RiverDirector = new RiverDirector();
        TreadmillModel = new TreadmillModel(RiverDirector);
    }
}
