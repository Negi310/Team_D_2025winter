using UnityEngine;

public class LogicInstaller
{
    public ConversationModel ConversationModel { get; }
    public ChunkPoolManager ChunkPoolManager { get; }
    public TreadmillModel TreadmillModel { get; }
    public RiverDirector RiverDirector { get; }
    public ForUIStatusBuilder ForUIStatusBuilder { get; }
    public CourtshipEvaluator CourtshipEvaluator { get; }
    public NameModel NameModel { get; }
    public RearModel RearModel { get; }

    public LogicInstaller()
    {
        ConversationModel = new ConversationModel();
        ChunkPoolManager = new ChunkPoolManager(new GameObject("PoolRoot").transform);
        RiverDirector = new RiverDirector();
        TreadmillModel = new TreadmillModel(RiverDirector);
        ForUIStatusBuilder = new ForUIStatusBuilder();
        CourtshipEvaluator = new CourtshipEvaluator();
        NameModel = new NameModel();
        RearModel = new RearModel();
    }
}
