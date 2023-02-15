using System;

namespace Betauer.StateMachine; 

public class EventBuilder<TBuilder, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum 
    where TBuilder : class {
        
    private readonly TBuilder _builder;
    private readonly Action<EventBuilder<TBuilder, TStateKey, TEventKey>> _onBuild;

    internal readonly TEventKey EventKey;
    internal Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? Execute;
    internal Command<TStateKey, TEventKey> Result;

    internal EventBuilder(TBuilder builder, TEventKey eventKey, Action<EventBuilder<TBuilder, TStateKey, TEventKey>> onBuild) {
        _builder = builder;
        EventKey = eventKey;
        _onBuild = onBuild;
    }

    public TBuilder Send(TEventKey e, int weight = 0) => SetResult(Command<TStateKey, TEventKey>.CreateSendEvent(e, weight));
    
    public TBuilder Set(TStateKey state) => SetResult(Command<TStateKey, TEventKey>.CreateSet(state));
    public TBuilder Push(TStateKey state) => SetResult(Command<TStateKey, TEventKey>.CreatePush(state));
    public TBuilder PopPush(TStateKey state) => SetResult(Command<TStateKey, TEventKey>.CreatePopPush(state));
    public TBuilder Pop() => SetResult(Command<TStateKey, TEventKey>.CreatePop());
    public TBuilder Stay() => SetResult(Command<TStateKey, TEventKey>.CreateStay());

    public TBuilder Then(Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
        Execute = execute;
        _onBuild(this);
        return _builder;
    }

    private TBuilder SetResult(Command<TStateKey, TEventKey> command) {
        Result = command;
        _onBuild(this);
        return _builder;
    }
}