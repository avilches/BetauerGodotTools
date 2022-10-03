using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public interface IStateMachine<out TStateBuilder, TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        public void AddState(IState<TStateKey, TTransitionKey> state);
        public TStateBuilder CreateState(TStateKey stateKey);
        public void AddListener(IStateMachineListener<TStateKey> machineListener);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public bool IsState(TStateKey state);
        public IState<TStateKey, TTransitionKey> CurrentState { get; }
        public void Enqueue(TTransitionKey name);
        public Task Execute(float delta);
        public bool Available { get; }
        public string? Name { get; }
        public void AddOnEnter(Action<TStateKey, TStateKey> e);
        public void AddOnAwake(Action<TStateKey, TStateKey> e);
        public void AddOnSuspend(Action<TStateKey, TStateKey> e);
        public void AddOnExit(Action<TStateKey, TStateKey> e);
        public void AddOnTransition(Action<TStateKey, TStateKey> e);
        public void AddOnExecuteStart(Action<float, TStateKey> e);
        public void AddOnExecuteEnd(Action<TStateKey> e);
        public void RemoveOnEnter(Action<TStateKey, TStateKey> e);
        public void RemoveOnAwake(Action<TStateKey, TStateKey> e);
        public void RemoveOnSuspend(Action<TStateKey, TStateKey> e);
        public void RemoveOnExit(Action<TStateKey, TStateKey> e);
        public void RemoveOnTransition(Action<TStateKey, TStateKey> e);
        public void RemoveOnExecuteStart(Action<float, TStateKey> e);
        public void RemoveOnExecuteEnd(Action<TStateKey> e);
    }
}