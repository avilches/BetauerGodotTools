using System;

namespace Betauer.StateMachine {
    public interface IState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events { get; }
    }
}