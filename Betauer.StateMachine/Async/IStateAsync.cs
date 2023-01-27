using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async; 

public interface IStateAsync<TStateKey, TEventKey> : 
    IState<TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public Task Before();
    public Task Enter();
    public Task Awake();
    public Task Execute();
    public Task Suspend();
    public Task Exit();
    public Task After();
}