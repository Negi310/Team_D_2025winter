using UnityEngine;

// アプリケーションの起点,ゲーム全体の初期化を行う
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private ConversationView conversationView;
    [SerializeField] private TreadmillView treadmillView;
    [SerializeField] private CourtingUIManager courtingUIManager;
    [SerializeField] private NamingUIManager namingUIManager;
    [SerializeField] private SeaUIManager seaUIManager;
    [SerializeField] private TickProvider tickProvider;
    [SerializeField] private Transform playerTransform; // 鮭のTransform

    [Header("Master Data")]
    [SerializeField] private ConversationEvent conversationEvent; // テスト用の会話データ
    [SerializeField] private ChunkLevelData chunkLevelData;
    [SerializeField] private EventPool eventPool;
    
    private GameRouter _router;

    private void Awake()
    {
        var saveDataResister = new SaveDataResister();
        if (!saveDataResister.TryLoad(out SaveData saveData))
        {
            saveData = saveDataResister.CreateInitialData();
            saveDataResister.Save(saveData); 
        }
        
        var sessionContext = new SessionContext(saveData);

        // ロジックを計算するModelの生成
        var logicInstaller = new LogicInstaller();

        var stateCompositeFactory = new StateCompositeFactory(sessionContext, logicInstaller,
            conversationView, treadmillView,
            courtingUIManager,namingUIManager, seaUIManager,
            eventPool, chunkLevelData, playerTransform, tickProvider);
        // ModelとViewとContextの参照を渡す
        var stateMachine = new GameStateMachine();
        _router = new GameRouter(stateMachine, stateCompositeFactory, sessionContext, saveDataResister);
        
        saveDataResister.ResumeState(stateMachine, saveData.LastSavedStateName);
    }

    private void OnDestroy()
    {
        _router?.Dispose();
    }
}
