using System;
using System.Threading.Tasks;

namespace Betauer.FSM.Async; 

public interface IFsmAsync<TStateKey, TEventKey, TStateSync> : 
    IFsm<TStateKey, TEventKey, TStateSync> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public bool Available { get; }
    public Task Execute();
        
}