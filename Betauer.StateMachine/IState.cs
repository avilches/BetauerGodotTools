using System;

namespace Betauer.StateMachine;


public enum ConditionStage {
    /// <summary>
    /// The state changed in the previous execution. So, the new state is executed without evaluating conditions at the
    /// beginning. But when the state finishes, all the conditions (Asap, Lazy and Always) should be evaluated to find
    /// if the new state.
    /// 
    /// - If there is new state, the next execution will repeat the StageChanged stage in the new state (when it finishes)
    /// - If the state is the same, the next execution will execute the current state using the SameStateBeforeExecution stage
    /// </summary>
    StageChanged,
    
    /// <summary>
    /// The state is the same as the previous state. But just before to execute the state again, evaluate only
    /// the Condition.Type.Asap and Always conditions. 
    /// </summary>
    SameStateBeforeExecution,
    
    /// <summary>
    /// The state is the same as the previous state and the state is finished, evaluate only
    /// the Condition.Type.Lazy and Always conditions.  
    /// </summary>
    SameStateAfterExecution
}

public interface IState<TStateKey, TEventKey> 
    where TStateKey : Enum where TEventKey : Enum {
        
    public TStateKey Key { get; }
    public bool TryGetEventRule(TEventKey eventKey, out EventRule<TStateKey, TEventKey> result);
    public void EvaluateConditions(CommandContext<TStateKey, TEventKey> ctx, out Command<TStateKey, TEventKey> command, ConditionStage conditionStage);
}