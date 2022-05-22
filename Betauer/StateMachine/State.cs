using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.DI;
using Godot;

namespace Betauer.StateMachine {
    public interface IState {
        public IState? Parent { get; }

        public string Name { get; }

        public NextState Execute(Context context);

        public void Start(Context context);

        public void End();
    }

    public abstract class BaseState : IState {
        public string Name { get; }

        public IState? Parent { get; }

        protected BaseState(IState? parent, string name) {
            Name = name;
            Parent = parent;
        }

        public abstract NextState Execute(Context context);

        public virtual void Start(Context context) {
        }

        public virtual void End() {
        }
    }

    public class StateBuilder<T, TParent> where T : IStateMachine {
        private Func<Context, Task>? _startFunc;
        private Func<Context, NextState>? _execute;
        private Action? _end;
        private readonly string _name;
        private Queue<StateBuilder<T, StateBuilder<T, TParent>>>? _pending;
        private readonly TParent _parent;

        internal StateBuilder(string name, TParent parent) {
            _name = name;
            _parent = parent;
        }

        public StateBuilder<T, TParent> Enter(Action<Context> start) {
            _startFunc = async context => start(context);
            return this;
        }

        public StateBuilder<T, TParent> Enter(Func<Context, Task> start) {
            _startFunc = start;
            return this;
        }

        public StateBuilder<T, StateBuilder<T, TParent>> State(string name) {
            _pending ??= new Queue<StateBuilder<T, StateBuilder<T, TParent>>>();
            var stateBuilder = new StateBuilder<T, StateBuilder<T, TParent>>(name, this);
            _pending.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public StateBuilder<T, TParent> Execute(Func<Context, NextState> execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder<T, TParent> Exit(Action end) {
            _end = end;
            return this;
        }

        public TParent End() {
            return _parent;
        } 

        internal IState Build(T stateMachine, IState? parent) {
            IState state = new State(parent, _name, _startFunc, _execute, _end);
            stateMachine.AddState(state);
            if (_pending != null) {
                while (_pending.Count > 0) {
                    _pending.Dequeue().Build(stateMachine, state);
                }
            }
            return state;
        }
    }

    public class State : IState {
        private readonly Func<Context, Task>? _start;
        private readonly Func<Context, NextState>? _execute;
        private readonly Action? _end;
        public string Name { get; }
        public IState? Parent { get; }

        public State(IState? parent, string name, Func<Context, Task>? start = null, Func<Context, NextState>? execute = null, Action? end = null) {
            Parent = parent;
            Name = name;
            _start = start;
            _execute = execute;
            _end = end;
        }

        public NextState Execute(Context context) {
            if (_execute != null) {
                // Console.WriteLine("Execute "+Name);
                return _execute.Invoke(context);
            }
            return context.Current();
        }

        public async void Start(Context context) {
            if (_start != null) {
                // Console.WriteLine("Start "+Name);
                await _start.Invoke(context);
            }
        }

        public void End() {
            if (_end != null) {
                // Console.WriteLine("End " + Name);
                _end.Invoke();
            }
        }
    }
}