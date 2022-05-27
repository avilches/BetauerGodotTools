using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public interface IState {
        public string Name { get; }
        public Task Enter();
        public Task Suspend();
        public Task Awake();
        public Task<Transition> Execute(float delta);
        public Task Exit();
    }

    public abstract class BaseState : IState {
        public string Name { get; }
        
        protected BaseState(string name) {
            Name = name;
        }
        public virtual async Task Enter() {
        }
        public virtual async Task<Transition> Execute(float delta) {
            return Transition.None();
        }
        public virtual async Task Exit() {
        }

        public virtual async Task Suspend() {
        }

        public virtual async Task Awake() {
        }
    }

    public class State : BaseState {
        private readonly Func<Task>? _enter;
        private readonly Func<float, Task<Transition>>? _execute;
        private readonly Func<Task>? _exit;
        private readonly Func<Task>? _suspend;
        private readonly Func<Task>? _awake;

        public State(string name, 
            Func<Task>? enter = null, Func<float, Task<Transition>>? execute = null, Func<Task>? exit = null, 
            Func<Task>? suspend = null, Func<Task>? awake = null) : base(name) {
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            
        }

        public override async Task Enter() {
            if (_enter != null) {
                await _enter.Invoke();
            }
        }

        public override async Task<Transition> Execute(float delta) {
            if (_execute != null) {
                return await _execute.Invoke(delta);
            }
            return Transition.None();
        }

        public override async Task Exit() {
            if (_exit != null) {
                await _exit.Invoke();
            }
        }

        public override async Task Suspend() {
            if (_suspend != null) {
                await _suspend.Invoke();
            }
        }

        public override async Task Awake() {
            if (_awake != null) {
                await _awake.Invoke();
            }
        }
    }

    public class StateBuilder<T, TParent> where T : IStateMachine {
        private Func<Task>? _enter;
        private Func<float, Task<Transition>>? _execute;
        private Func<Task>? _exit;
        private Func<Task>? _suspend;
        private Func<Task>? _awake;
        private readonly string _name;
        private readonly TParent _parent;

        internal StateBuilder(string name, TParent parent) {
            _name = name;
            _parent = parent;
        }

        public StateBuilder<T, TParent> Enter(Action enter) {
            _enter = async () => enter();
            return this;
        }

        public StateBuilder<T, TParent> Enter(Func<Task> enter) {
            _enter = enter;
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<Task<Transition>> execute) {
            _execute = delta => execute();
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<float, Task<Transition>> execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<Transition> execute) {
            _execute = async delta => execute();;
            return this;
        }

        public StateBuilder<T, TParent> Execute(Func<float, Transition> execute) {
            _execute = async delta => execute(delta);;
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

        public StateBuilder<T, TParent> Suspend(Action suspend) {
            _suspend = async () => suspend();
            return this;
        }

        public StateBuilder<T, TParent> Suspend(Func<Task> suspend) {
            _suspend = suspend;
            return this;
        }

        public StateBuilder<T, TParent> Awake(Action awake) {
            _awake = async () => awake();
            return this;
        }

        public StateBuilder<T, TParent> Awake(Func<Task> awake) {
            _awake = awake;
            return this;
        }

        public TParent End() {
            return _parent;
        } 

        internal IState Build(T stateMachine) {
            IState state = new State(_name, _enter, _execute, _exit, _suspend, _awake);
            stateMachine.AddState(state);
            return state;
        }
    }
}