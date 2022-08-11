using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public interface IStateMachineListener<in TStateKey> {
        public void OnEnter(TStateKey state, TStateKey from);
        public void OnAwake(TStateKey state, TStateKey from);
        public void OnSuspend(TStateKey state, TStateKey to);
        public void OnExit(TStateKey state, TStateKey to);
        public void OnTransition(TStateKey from, TStateKey to);
        public void OnExecuteStart(float delta, TStateKey state);
        public void OnExecuteEnd(TStateKey state);
    }

    public class StateMachineListenerBase<TStateKey> : IStateMachineListener<TStateKey> {
        public virtual void OnEnter(TStateKey state, TStateKey from) {
        }

        public virtual void OnAwake(TStateKey state, TStateKey from) {
        }

        public virtual void OnSuspend(TStateKey state, TStateKey to) {
        }

        public virtual void OnExit(TStateKey state, TStateKey to) {
        }

        public virtual void OnTransition(TStateKey from, TStateKey to) {
        }

        public virtual void OnExecuteStart(float delta, TStateKey state) {
        }

        public virtual void OnExecuteEnd(TStateKey state) {
        }
    }

    public class StateMachineEvents<TStateKey> : IStateMachineListener<TStateKey> {
        public event Action<TStateKey, TStateKey> Enter;
        public event Action<TStateKey, TStateKey> Awake;
        public event Action<TStateKey, TStateKey> Suspend;
        public event Action<TStateKey, TStateKey> Exit;
        public event Action<TStateKey, TStateKey> Transition;
        public event Action<float, TStateKey> ExecuteStart;
        public event Action<TStateKey> ExecuteEnd;

        public virtual void OnEnter(TStateKey state, TStateKey from) {
            Enter?.Invoke(state, from);
        }

        public virtual void OnAwake(TStateKey state, TStateKey from) {
            Awake?.Invoke(state, from);
        }

        public virtual void OnSuspend(TStateKey state, TStateKey to) {
            Suspend?.Invoke(state, to);
        }

        public virtual void OnExit(TStateKey state, TStateKey to) {
            Exit?.Invoke(state, to);
        }

        public virtual void OnTransition(TStateKey from, TStateKey to) {
            Transition?.Invoke(from, to);
        }

        public virtual void OnExecuteStart(float delta, TStateKey state) {
            ExecuteStart?.Invoke(delta, state);
        }

        public virtual void OnExecuteEnd(TStateKey state) {
            ExecuteEnd?.Invoke(state);
        }
    }
}