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
    [SerializeField] private TickProvider tickProvider;
    [SerializeField] private Transform playerTransform; // 鮭のTransform

    [Header("Master Data")]
    [SerializeField] private ConversationEvent conversationEvent; // テスト用の会話データ
    [SerializeField] private ChunkLevelData chunkLevelData;
    [SerializeField] private EventPool eventPool;
    [SerializeField] private MateGenerationSettingsSO mateSetting;
    
    private GameRouter _router;
    private GameStateMachine _stateMachine;
    private SaveDataResister _saveDataResister;
    private SaveData _saveData;

    private void Awake()
    {
        _saveDataResister = new SaveDataResister();
        if (!_saveDataResister.TryLoad(out SaveData saveData))
        {
            saveData = _saveDataResister.CreateInitialData();
            _saveDataResister.Save(saveData);
        }
        
        var sessionContext = new SessionContext(saveData);

        // ロジックを計算するModelの生成
        var logicInstaller = new LogicInstaller();

        var stateCompositeFactory = new StateCompositeFactory(sessionContext, logicInstaller,
            conversationView, treadmillView, obstaclesView,
            courtingUIManager,namingUIManager, seaUIManager,
            eventPool, chunkLevelData, mateSetting, playerTransform, tickProvider);
        // ModelとViewとContextの参照を渡す
        _stateMachine = new GameStateMachine();
        _router = new GameRouter(_stateMachine, stateCompositeFactory, sessionContext, _saveDataResister);
        _saveData = saveData;
    }
    
    private void Start()
    {
        _saveDataResister.ResumeState(_stateMachine, _saveData.LastSavedStateName);
    }

    private void OnDestroy()
    {
        _router?.Dispose();
    }
}
