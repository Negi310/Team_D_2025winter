using UnityEngine;

// アプリケーションの起点,ゲーム全体の初期化を行う
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private ConversationView conversationView;
    [SerializeField] private TreadmillView treadmillView;
    [SerializeField] private TickProvider tickProvider;
    
    private GameStateMachine _stateMachine;
    private GameRouter _router;

    private void Awake()
    {
        var saveDataResister = new SaveDataResister();
        SaveData saveData;
        if (saveDataResister.TryLoad(out saveData))
        {

        }
        else
        {
            //saveData = masterData.CreateInitialSaveData();
        }

        var sessionContext = new SessionContext(saveData);

        // ロジックを計算するModelの生成
        var logicInstaller = new LogicInstaller();

        var presentersFactory = new StateCompositeFactory(sessionContext, logicInstaller.ConversationModel, conversationView);
        // ModelとViewとContextの参照を渡す
        _stateMachine = new GameStateMachine();
        _router = new GameRouter(_stateMachine, presentersFactory);
        
        ((IStateChangable)_stateMachine).ChangeState<ConversationState>();
    }

    private void OnDestroy()
    {
        _router?.Dispose();
    }
}
