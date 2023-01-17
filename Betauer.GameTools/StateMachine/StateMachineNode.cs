using System;
using Godot;

namespace Betauer.StateMachine; 

public abstract partial class StateMachineNode<TStateKey> : Node
    where TStateKey : Enum {
    public bool ProcessInPhysics { get; set; }

    public abstract IStateMachineEvents<TStateKey> GetStateMachineEvents();

    public event Action<TransitionArgs<TStateKey>>? OnEnter {
        add => GetStateMachineEvents().OnEnter += value;
        remove => GetStateMachineEvents().OnEnter -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnAwake {
        add => GetStateMachineEvents().OnAwake += value;
        remove => GetStateMachineEvents().OnAwake -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnSuspend {
        add => GetStateMachineEvents().OnSuspend += value;
        remove => GetStateMachineEvents().OnSuspend -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnExit {
        add => GetStateMachineEvents().OnExit += value;
        remove => GetStateMachineEvents().OnExit -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnTransition {
        add => GetStateMachineEvents().OnTransition += value;
        remove => GetStateMachineEvents().OnTransition -= value;
    }

    public event Action? OnBeforeExecute {
        add => GetStateMachineEvents().OnBeforeExecute += value;
        remove => GetStateMachineEvents().OnBeforeExecute -= value;
    }

    public event Action? OnAfterExecute {
        add => GetStateMachineEvents().OnAfterExecute += value;
        remove => GetStateMachineEvents().OnAfterExecute -= value;
    }
}