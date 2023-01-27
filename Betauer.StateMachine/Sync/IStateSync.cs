using System;

namespace Betauer.StateMachine.Sync; 

public interface IStateSync<TStateKey, TEventKey> : 
    IState<TStateKey, TEventKey> 
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    public void Before();
    public void Enter();
    public void Awake();
    public void Execute();
    public void Suspend();
    public void Exit();
    public void After();
}