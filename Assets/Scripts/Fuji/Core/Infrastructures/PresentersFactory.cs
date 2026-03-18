using System;
using System.Collections.Generic;

public class StateCompositeFactory
{
    // 内部の辞書はベースクラス（GameState）を扱うが、外からは絶対に見せない
    private readonly Dictionary<Type, Func<GameState, IDisposable>> _registry = new();

    public StateCompositeFactory(
        SessionContext context,
        ConversationModel conversationModel, ConversationView conversationView)
    {
        // ジェネリクスのおかげで、引数の state は最初から「CourtshipState」として確定している！
        Register<CourtshipState>(state =>
        {
            var composite = new CompositeDisposable();
            
            //composite.Add(new CourtshipMenuPresenter(state));
            //composite.Add(new PlayerStatsPresenter(state));
            //...

            return composite;
        });

        Register<RearState>(state =>
        {
            var composite = new CompositeDisposable();
            
            //composite.Add(new NurturingActionPresenter(state));
            //...
            
            return composite;
        });
        
        Register<ConversationState>(state =>
        {
            var composite = new CompositeDisposable();
            var conversationContext = new ConversationContext();
            composite.Add(new ConversationPresenter(state, conversationModel, conversationContext, conversationView));
            //composite.Add(new OtherStateSpecificPresenter(state));
            //...
            
            return composite;
        });
        
        Register<UpstreamState>(state =>
        {
            var composite = new CompositeDisposable();
            //var upstreamContext = new UpstreamContext();
            //composite.Add(new UpstreamPresenter(state, context));
            //composite.Add(new OtherStateSpecificPresenter(state));
            //...
            
            return composite;
        });
    }
    
    private void Register<TState>(Func<TState, IDisposable> factoryMethod) where TState : GameState
    {
        // 辞書に登録する際、内部で自動的にキャストをラップする。
        // TState で登録されているため、ここに違う型の State が渡ってくることは構造上あり得ない
        _registry[typeof(TState)] = (GameState state) => 
        {
            return factoryMethod.Invoke((TState)state);
        };
    }

    // ルーターから呼ばれるメソッド（ここは実行時なのでベースクラスを受け取る）
    public IDisposable CreatePresentersFor(GameState state)
    {
        Type stateType = state.GetType();

        if (_registry.TryGetValue(stateType, out var factoryMethod))
        {
            return factoryMethod.Invoke(state);
        }

        return null;
    }
}
