using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.DI;
using Godot;

namespace Betauer.StateMachine {
    public interface IState {
        public IState[] Parents { get; }
        public string Name { get; }
        public Task Enter(Context context);
        public Task<StateChange> Execute(Context context);
        public Task Exit();
    }

    public abstract class BaseState : IState {
        public string Name { get; }
        
        public IState[] Parents { get; }
        protected BaseState(IState[] parents, string name) {
            Name = name;
            Parents = parents;
        }
        public virtual async Task Enter(Context context) {
        }
        public virtual async Task<StateChange> Execute(Context context) {
            return context.Repeat();
        }
        public virtual async Task Exit() {
        }
    }

    public class State : BaseState {
        private readonly Func<Context, Task>? _enter;
        private readonly Func<Context, Task<StateChange>>? _execute;
        private readonly Func<Task>? _exit;
        public string Name { get; }
        public IState? Parent { get; }

        public State(IState[] parents, string name, Func<Context, Task>? enter = null, Func<Context, Task<StateChange>>? execute = null, Func<Task>? exit = null) : base(parents, name) {
            _enter = enter;
            _execute = execute;
            _exit = exit;
        }

        public override async Task Enter(Context context) {
            if (_enter != null) {
                await _enter.Invoke(context);
            }
        }

        public override async Task<StateChange> Execute(Context context) {
            if (_execute != null) {
                return await _execute.Invoke(context);
            }
            return context.Repeat();
        }

        public override async Task Exit() {
            if (_exit != null) {
                await _exit.Invoke();
            }
        }
    }

    public class StateBuilder<T, TParent> where T : IStateMachine {
        private Func<Context, Task>? _enter;
        private Func<Context, Task<StateChange>>? _execute;
        private Func<Task>? _exit;
        private readonly string _name;
        private readonly TParent _parent;

        internal StateBuilder(string name, TParent parent) {
            _name = name;
            _parent = parent;
        }

        public StateBuilder<T, TParent> Enter(Action<Context> enter) {
            _enter = async context => enter(context);
            return this;
        }

        public StateBuilder<T, TParent> Enter(Func<Context, Task> enter) {
            _enter = enter;
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<Context, Task<StateChange>> execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<Context, StateChange> execute) {
            _execute = async context => execute(context);;
            return this;
        }

        public StateBuilder<T, TParent> Exit(Action exit) {
            _exit = async () => exit();
            return this;
        }
        public StateBuilder<T, TParent> Exit(Func<Task> exit) {
            _exit = exit;
            return this;
        }

        public TParent End() {
            return _parent;
        } 

        internal IState Build(T stateMachine, IState[] parents) {
            IState state = new State(parents, _name, _enter, _execute, _exit);
            stateMachine.AddState(state);
            return state;
        }
    }
}