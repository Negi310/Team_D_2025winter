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
    public UpstreamGameModel UpstreamGameModel { get; }

    public LogicInstaller(GameSetting setting)
    {
        ConversationModel = new ConversationModel();
        PoolManager = new PoolManager(new GameObject("PoolRoot").transform);
        RiverDirector = new RiverDirector(setting);
        TreadmillModel = new TreadmillModel(RiverDirector);
        ForUIStatusBuilder = new ForUIStatusBuilder();
        CourtshipEvaluator = new CourtshipEvaluator(setting);
        BreedingCalculator = new BreedCalculatable();
        MateGeneratable = new MateGeneratable();
        NameModel = new NameModel();
        RearModel = new RearModel();
        RiverPath = new RiverPath();
        SplineMathModel = new SplineMathModel();
        ObstacleModel = new ObstacleModel(SplineMathModel, setting);
        UpstreamGameModel = new UpstreamGameModel(setting);
    }
}
