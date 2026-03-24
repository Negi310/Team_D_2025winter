using UnityEngine;

public class LogicInstaller
{
    public ConversationModel ConversationModel { get; }
    public PoolManager PoolManager { get; }
    public TreadmillModel TreadmillModel { get; }
    public RiverDirector RiverDirector { get; }
    public ForUIStatusBuilder ForUIStatusBuilder { get; }
    public ICourtshipEvaluatable CourtshipEvaluator { get; }
    public IBreedingCalculator BreedingCalculator { get; }
    public IMateGeneratable MateGeneratable { get; }
    public NameModel NameModel { get; }
    public RearModel RearModel { get; }
    public RiverPath RiverPath { get; }
    public ObstacleModel ObstacleModel { get; }
    public SplineMathModel SplineMathModel { get; }

    public LogicInstaller()
    {
        ConversationModel = new ConversationModel();
        PoolManager = new PoolManager(new GameObject("PoolRoot").transform);
        RiverDirector = new RiverDirector();
        TreadmillModel = new TreadmillModel(RiverDirector);
        ForUIStatusBuilder = new ForUIStatusBuilder();
        CourtshipEvaluator = new CourtshipEvaluator();
        BreedingCalculator = new BreedCalculatable();
        MateGeneratable = new MateGeneratable();
        NameModel = new NameModel();
        RearModel = new RearModel();
        RiverPath = new RiverPath();
        SplineMathModel = new SplineMathModel();
        ObstacleModel = new ObstacleModel(SplineMathModel);
    }
}
