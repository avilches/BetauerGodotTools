using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public interface IStateMachineAsync<TStateKey, TTransitionKey, TStateSync> : IStateMachine<TStateKey, TTransitionKey, TStateSync> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        public bool Available { get; }
        public Task Execute(float delta);
        
    }
}