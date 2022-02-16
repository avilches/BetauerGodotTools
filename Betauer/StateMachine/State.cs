using System;
using System.Collections.Generic;
using Betauer.DI;
using Godot;

namespace Betauer.StateMachine {
    public interface IState {
        public string Name { get; }

        public NextState Execute(Context context);

        public void Start(Context context);

        public void End();
    }

    public abstract class BaseState : IState {
        public string Name { get; }

        protected BaseState(string name) {
            Name = name;
        }

        public abstract NextState Execute(Context context);

        public virtual void Start(Context context) {
        }

        public virtual void End() {
        }
    }

    public class StateBuilder {
        private Action<Context>? _start;
        private Func<Context, NextState> _execute;
        private Action? _end;
        private readonly string _name;

        internal StateBuilder(string name) {
            _name = name;
        }

        public StateBuilder Enter(Action<Context> start) {
            _start = start;
            return this;
        }

        public StateBuilder Execute(Func<Context, NextState> execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder Exit(Action end) {
            _end = end;
            return this;
        }

        internal IState Build() => new State(_name, _start, _execute, _end);
    }

    public class State : IState {
        private readonly Action<Context>? _start;
        private readonly Func<Context, NextState> _execute;
        private readonly Action? _end;
        public string Name { get; }


        public State(string name, Func<Context, NextState> execute, Action? end = null) :
            this(name, null, execute, end) {
        }

        public State(string name, Action<Context>? start, Func<Context, NextState> execute, Action? end = null) {
            Name = name;
            _start = start;
            _execute = execute;
            _end = end;
        }

        public NextState Execute(Context context) {
            return _execute(context);
        }

        public void Start(Context context) {
            _start?.Invoke(context);
        }

        public void End() {
            _end?.Invoke();
        }
    }
}