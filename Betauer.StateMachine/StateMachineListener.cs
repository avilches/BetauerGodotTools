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

    public class StateMachineListenerAction<TStateKey> : IStateMachineListener<TStateKey> {
        private LinkedList<Action<TStateKey, TStateKey>>? _onEnter;
        private LinkedList<Action<TStateKey, TStateKey>>? _onAwake;
        private LinkedList<Action<TStateKey, TStateKey>>? _onSuspend;
        private LinkedList<Action<TStateKey, TStateKey>>? _onTransition;
        private LinkedList<Action<float, TStateKey>>? _onExecuteStart;
        private LinkedList<Action<TStateKey>>? _onExecuteEnd;
        public StateMachineListenerAction<TStateKey> AddOnEnter(Action<TStateKey, TStateKey> onEnter) {
            _onEnter ??= new LinkedList<Action<TStateKey, TStateKey>>();
            _onEnter.AddLast(onEnter);
            return this;
        }

        public StateMachineListenerAction<TStateKey> AddOnAwake(Action<TStateKey, TStateKey> onAwake) {
            _onAwake ??= new LinkedList<Action<TStateKey, TStateKey>>();
            _onAwake.AddLast(onAwake);
            return this;
        }

        public StateMachineListenerAction<TStateKey> AddOnSuspend(Action<TStateKey, TStateKey> onSuspend) {
            _onSuspend ??= new LinkedList<Action<TStateKey, TStateKey>>();
            _onSuspend.AddLast(onSuspend);
            return this;
        }

        public StateMachineListenerAction<TStateKey> AddOnTransition(Action<TStateKey, TStateKey> onTransition) {
            _onTransition ??= new LinkedList<Action<TStateKey, TStateKey>>();
            _onTransition.AddLast(onTransition);
            return this;
        }

        public StateMachineListenerAction<TStateKey> AddOnExecuteStart(Action<float, TStateKey> onExecuteStart) {
            _onExecuteStart ??= new LinkedList<Action<float, TStateKey>>();
            _onExecuteStart.AddLast(onExecuteStart);
            return this;
        }

        public StateMachineListenerAction<TStateKey> AddOnExecuteEnd(Action<TStateKey> onExecuteEn) {
            _onExecuteEnd ??= new LinkedList<Action<TStateKey>>();
            _onExecuteEnd.AddLast(onExecuteEn);
            return this;
        }

        public virtual void OnEnter(TStateKey state, TStateKey from) {
            if (_onEnter != null) foreach (var action in _onEnter) action(state, from);
        }

        public virtual void OnAwake(TStateKey state, TStateKey from) {
            if (_onAwake != null) foreach (var action in _onAwake) action(state, from);
        }

        public virtual void OnSuspend(TStateKey state, TStateKey to) {
            if (_onSuspend != null) foreach (var action in _onSuspend) action(state, to);
        }

        public virtual void OnExit(TStateKey state, TStateKey to) {
            if (_onTransition != null) foreach (var action in _onTransition) action(state, to);
        }

        public virtual void OnTransition(TStateKey from, TStateKey to) {
            if (_onTransition != null) foreach (var action in _onTransition) action(from, to);
        }

        public virtual void OnExecuteStart(float delta, TStateKey state) {
            if (_onExecuteStart != null) foreach (var action in _onExecuteStart) action(delta, state);
        }

        public virtual void OnExecuteEnd(TStateKey state) {
            if (_onExecuteEnd != null) foreach (var action in _onExecuteEnd) action(state);
        }
    }
}