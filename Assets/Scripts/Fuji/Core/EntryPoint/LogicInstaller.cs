using UnityEngine;

public class LogicInstaller
{
    public ConversationModel ConversationModel { get; }
    public PoolManager PoolManager { get; }
    public TreadmillModel TreadmillModel { get; }
    public RiverDirector RiverDirector { get; }
    public ForUIStatusBuilder ForUIStatusBuilder { get; }
    public CourtshipEvaluator CourtshipEvaluator { get; }
    public NameModel NameModel { get; }
    public RearModel RearModel { get; }
    public RiverPath RiverPath { get; }

    public LogicInstaller()
    {
        ConversationModel = new ConversationModel();
        PoolManager = new PoolManager(new GameObject("PoolRoot").transform);
        RiverDirector = new RiverDirector();
        TreadmillModel = new TreadmillModel(RiverDirector);
        ForUIStatusBuilder = new ForUIStatusBuilder();
        CourtshipEvaluator = new CourtshipEvaluator();
        NameModel = new NameModel();
        RearModel = new RearModel();
        RiverPath = new RiverPath();
    }
}
