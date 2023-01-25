using System;

namespace Betauer.StateMachine.Sync; 

public interface IStateSync<TStateKey, TEventKey> : 
    IState<TStateKey, TEventKey> 
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    public void Enter();
    public void Awake();
    public void Execute();
    public Command<TStateKey, TEventKey> Next(ConditionContext<TStateKey, TEventKey> conditionContext);
    public void Suspend();
    public void Exit();
}