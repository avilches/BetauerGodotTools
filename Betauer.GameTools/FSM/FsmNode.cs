using System;
using Godot;

namespace Betauer.FSM; 

public abstract partial class FsmNode<TStateKey> : Node
    where TStateKey : Enum {
    public bool ProcessInPhysics { get; set; }

    public abstract IFsmEvents<TStateKey> GetFsmEvents();

    public event Action<TransitionArgs<TStateKey>>? OnEnter {
        add => GetFsmEvents().OnEnter += value;
        remove => GetFsmEvents().OnEnter -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnAwake {
        add => GetFsmEvents().OnAwake += value;
        remove => GetFsmEvents().OnAwake -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnSuspend {
        add => GetFsmEvents().OnSuspend += value;
        remove => GetFsmEvents().OnSuspend -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnExit {
        add => GetFsmEvents().OnExit += value;
        remove => GetFsmEvents().OnExit -= value;
    }

    public event Action<TransitionArgs<TStateKey>>? OnTransition {
        add => GetFsmEvents().OnTransition += value;
        remove => GetFsmEvents().OnTransition -= value;
    }

    public event Action? OnBefore {
        add => GetFsmEvents().OnBefore += value;
        remove => GetFsmEvents().OnBefore -= value;
    }

    public event Action? OnAfter {
        add => GetFsmEvents().OnAfter += value;
        remove => GetFsmEvents().OnAfter -= value;
    }
}