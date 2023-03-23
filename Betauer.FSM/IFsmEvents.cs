using System;

namespace Betauer.FSM;

public interface IFsmEvents<TStateKey> {
    public string? Name { get; }
    public event Action<TransitionArgs<TStateKey>>? OnEnter;
    public event Action<TransitionArgs<TStateKey>>? OnAwake;
    public event Action<TransitionArgs<TStateKey>>? OnSuspend;
    public event Action<TransitionArgs<TStateKey>>? OnExit;
    public event Action<TransitionArgs<TStateKey>>? OnTransition;
    public event Action? OnBefore;
    public event Action? OnAfter;
}