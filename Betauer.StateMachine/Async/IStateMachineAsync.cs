using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public interface IStateMachineAsync<TStateKey, TEventKey, TStateSync> : IStateMachine<TStateKey, TEventKey, TStateSync> 
        where TEventKey : Enum where TStateKey : Enum {
        
        public bool Available { get; }
        public Task Execute();
        
    }
}