using UnityEngine;

// アプリケーションの起点,ゲーム全体の初期化を行う
public class GameBootstrapper : MonoBehaviour
{
    
    
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
            saveData = new SaveData
            {
                Generation = 1,
                Turn = 1,
                Salmon = new SalmonData
                (
                    new UpstreamStats
                    {
                        Power = 10,
                        Stamina = 10,
                        Cautiousness = 10,
                        Jump = 10
                    },
                    new CourtshipTraits
                    {
                        Size = 10,
                        ColorValue = 10,
                        ShapeValue = 10,
                    }
                )
            };
        }

        var sessionContext = new SessionContext(saveData); 
        
        // ロジックを計算するModelの生成
        //...

        
        //var presenterFactory = new PresenterFactory();
        
        _stateMachine = new GameStateMachine();
        //_router = new GameRouter(_stateMachine, presenterFactory);
    }

    private void OnDestroy()
    {
        _router?.Dispose();
    }
}