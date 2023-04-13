using System;

namespace Betauer.FSM; 

public interface IState<TStateKey, TEventKey> 
    where TStateKey : Enum where TEventKey : Enum {
        
    public TStateKey Key { get; }
    public bool TryGetEventRule(TEventKey eventKey, out EventRule<TStateKey, TEventKey> result);
    public Command<TStateKey, TEventKey> EvaluateConditions(CommandContext<TStateKey, TEventKey> ctx);
}