using UnityEngine;

// アプリケーションの起点,ゲーム全体の初期化を行う
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private ConversationView conversationView;
    [SerializeField] private TreadmillView treadmillView;
    [SerializeField] private CourtingUIManager courtingUIManager;
    [SerializeField] private NamingUIManager namingUIManager;
    [SerializeField] private SeaUIManager seaUIManager;
    [SerializeField] private ObstaclesView obstaclesView;
    [SerializeField] private SalmonMove salmonMove;
    [SerializeField] private UpstreamView upstreamView;
    [SerializeField] private TitleView titleView;
    [SerializeField] private TickProvider tickProvider;

    [Header("Master Data")]
    [SerializeField] private ConversationEvent conversationEvent; // テスト用の会話データ
    [SerializeField] private ChunkLevelData chunkLevelData;
    [SerializeField] private EventPool eventPool;
    [SerializeField] private MateGenerationSettingsSO mateSetting;
    
    private GameRouter _router;
    private SaveData _saveData;

    private void Awake()
    {
        var saveDataResister = new SaveDataResister();
        if (!saveDataResister.TryLoad(out SaveData saveData))
        {
            saveData = saveDataResister.CreateInitialData();
            saveDataResister.Save(saveData);
        }
        
        _saveData = saveData;
        var sessionContext = new SessionContext(saveData);

        // ロジックを計算するModelの生成
        var logicInstaller = new LogicInstaller();

        var stateCompositeFactory = new StateCompositeFactory(sessionContext, logicInstaller,
            conversationView, treadmillView, obstaclesView, salmonMove, upstreamView,
            courtingUIManager,namingUIManager, seaUIManager, titleView,
            eventPool, chunkLevelData, mateSetting, tickProvider, _saveData.LastSavedStateName);
        // ModelとViewとContextの参照を渡す
        var stateMachine = new GameStateMachine();
        _router = new GameRouter(stateMachine, stateCompositeFactory, sessionContext, saveDataResister);
        ((IStateChangable)stateMachine).ChangeState<TitleState>();
    }

    private void OnDestroy()
    {
        _router?.Dispose();
    }
}
