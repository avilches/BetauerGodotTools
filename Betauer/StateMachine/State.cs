using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.DI;
using Godot;

namespace Betauer.StateMachine {
    public interface IState {
        public string Name { get; }
        public Task Enter(Context context);
        public Task Suspend(Context context);
        public Task Awake(Context context);
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
            return context.None();
        }
        public virtual async Task Exit() {
        }

        public virtual async Task Suspend(Context context) {
        }

        public virtual async Task Awake(Context context) {
        }
    }

    public class State : BaseState {
        private readonly Func<Context, Task>? _enter;
        private readonly Func<Context, Task<StateChange>>? _execute;
        private readonly Func<Task>? _exit;
        private readonly Func<Context, Task>? _suspend;
        private readonly Func<Context, Task>? _awake;
        public string Name { get; }
        public IState? Parent { get; }

        public State(IState[] parents, string name, 
            Func<Context, Task>? enter = null, Func<Context, Task<StateChange>>? execute = null, Func<Task>? exit = null, 
            Func<Context, Task>? suspend = null, Func<Context, Task>? awake = null) : base(parents, name) {
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            
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
            return context.None();
        }

        public override async Task Exit() {
            if (_exit != null) {
                await _exit.Invoke();
            }
        }

        public override async Task Suspend(Context context) {
            if (_suspend != null) {
                await _suspend.Invoke(context);
            }
        }

        public override async Task Awake(Context context) {
            if (_awake != null) {
                await _awake.Invoke(context);
            }
        }
    }

    public class StateBuilder<T, TParent> where T : IStateMachine {
        private Func<Context, Task>? _enter;
        private Func<Context, Task<StateChange>>? _execute;
        private Func<Task>? _exit;
        private Func<Context, Task>? _suspend;
        private Func<Context, Task>? _awake;
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

        public StateBuilder<T, TParent> Suspend(Action<Context> suspend) {
            _suspend = async context => suspend(context);
            return this;
        }

        public StateBuilder<T, TParent> Suspend(Func<Context, Task> suspend) {
            _suspend = suspend;
            return this;
        }

        public StateBuilder<T, TParent> Awake(Action<Context> awake) {
            _awake = async context => awake(context);
            return this;
        }

        public StateBuilder<T, TParent> Awake(Func<Context, Task> awake) {
            _awake = awake;
            return this;
        }

        public TParent End() {
            return _parent;
        } 

        internal IState Build(T stateMachine, IState[] parents) {
            IState state = new State(parents, _name, _enter, _execute, _exit, _suspend, _awake);
            stateMachine.AddState(state);
            return state;
        }
    }
}