using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async; 

public interface IStateMachineAsync<TStateKey, TEventKey, TStateSync> : 
    IStateMachine<TStateKey, TEventKey, TStateSync> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public bool Available { get; }
    public Task Execute();
        
}