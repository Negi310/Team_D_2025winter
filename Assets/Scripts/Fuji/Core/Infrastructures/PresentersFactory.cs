using System;
using System.Collections.Generic;
using UnityEngine;

public class StateCompositeFactory
{
    // 内部の辞書はベースクラス（GameState）を扱うが、外からは絶対に見せない
    private readonly Dictionary<Type, Func<GameState, IPayload, IDisposable>> _registry = new();

    public StateCompositeFactory(
        SessionContext context, LogicInstaller logic,
        ConversationView conversationView, TreadmillView treadmillView,
        ObstaclesView obstaclesView, SalmonMove salmonMove,
        UpstreamView upstreamView,
        CourtingUIManager courtingUIManager, NamingUIManager namingUIManager,
        SeaUIManager seaUIManager, TitleView titleView,
        EventPool eventPool, ChunkLevelData chunkLevelData,
        MateGenerationSettingsSO _mateSetting,
        TickProvider tickProvider, string initialStateName)
    {
        Register<TitleState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            
            composite.Add(new TitlePresenter(state, titleView, initialStateName));
            //composite.Add(new PlayerStatsPresenter(state));
            //...

            return composite;
        });
        
        // ジェネリクスのおかげで、引数の state は最初から「CourtshipState」として確定している！
        Register<CourtshipState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            
            var courtshipContext = new CourtshipContext();
            composite.Add(new CourtshipPresenter(state, logic.BreedingCalculator, logic.CourtshipEvaluator,
                logic.MateGeneratable, logic.ForUIStatusBuilder, context, courtshipContext, courtingUIManager, payload, _mateSetting));
            //composite.Add(new PlayerStatsPresenter(state));
            //...

            return composite;
        });

        Register<RearState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            
            composite.Add(new RearPresenter(state, logic.RearModel, logic.ForUIStatusBuilder, context, seaUIManager, eventPool));
            //...
            
            return composite;
        });
        
        Register<ConversationState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            var conversationContext = new ConversationContext();
            //conversationContext.MasterData = conversationEvent; // 会話イベントのマスターデータをContextにセット
            composite.Add(new ConversationPresenter(state, logic.ConversationModel, context, conversationContext, conversationView, payload));
            //composite.Add(new OtherStateSpecificPresenter(state));
            //...
            
            return composite;
        });
        
        Register<NameState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            composite.Add(new NamePresenter(state, logic.NameModel, logic.ForUIStatusBuilder, namingUIManager, context));
            //composite.Add(new OtherStateSpecificPresenter(state));
            //...
            
            return composite;
        });
        
        Register<UpstreamState>((state, payload) =>
        {
            var composite = new CompositeDisposable();
            var playerContext = new UpstreamPlayerContext();
            var treadmillContext = new TreadmillContext();
            var obstacleContext = new ObstacleContext();
            var salmonPlayer = new SalmonPlayer(salmonMove, logic.UpstreamGameModel);
            composite.Add(new TreadmillPresenter(state, logic.TreadmillModel, logic.PoolManager, logic.RiverPath,
                logic.ObstacleModel, treadmillView, obstaclesView, context, playerContext, treadmillContext, obstacleContext,
                tickProvider, chunkLevelData));
            composite.Add(salmonPlayer);
            composite.Add(new UpstreamPresenter(state, salmonPlayer, logic.SplineMathModel, upstreamView, playerContext, treadmillContext, context, tickProvider));
            //composite.Add(new OtherStateSpecificPresenter(state));
            //...
            
            return composite;
        });
    }
    
    private void Register<TState>(Func<TState, IPayload, IDisposable> factoryMethod) where TState : GameState
    {
        // 辞書に登録する際、内部で自動的にキャストをラップする。
        // TState で登録されているため、ここに違う型の State が渡ってくることは構造上あり得ない
        _registry[typeof(TState)] = (GameState state, IPayload payload) => 
        {
            return factoryMethod.Invoke((TState)state, payload);
        };
    }

    // ルーターから呼ばれるメソッド（ここは実行時なのでベースクラスを受け取る）
    public IDisposable CreatePresentersFor(GameState state, IPayload payload)
    {
        Type stateType = state.GetType();
        
        if (_registry.TryGetValue(stateType, out var factoryMethod))
        {
            return factoryMethod.Invoke(state, payload) as IDisposable;
        }

        return null;
    }
}
