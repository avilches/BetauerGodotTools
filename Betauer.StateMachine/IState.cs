using System;

namespace Betauer.StateMachine; 

public interface IState<TStateKey, TEventKey> 
    where TStateKey : Enum where TEventKey : Enum {
        
    public TStateKey Key { get; }
    public bool TryGetEventRule(TEventKey eventKey, out EventRule<TStateKey, TEventKey> result);
    public void EvaluateConditions(CommandContext<TStateKey, TEventKey> ctx, out Command<TStateKey, TEventKey> command);
}