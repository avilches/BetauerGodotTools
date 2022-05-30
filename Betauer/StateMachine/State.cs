using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public interface IState<TStateKey, TTransitionKey> {
        public TStateKey Key { get; }
        public Task Enter();
        public Task Suspend();
        public Task Awake();
        public Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(ExecuteContext<TStateKey, TTransitionKey> executeContext);
        public Task Exit();
    }

    public abstract class BaseState<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey> {
        public TStateKey Key { get; }
        
        protected BaseState(TStateKey key) {
            Key = key;
        }
        public virtual async Task Enter() {
        }
        public virtual async Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            return executeContext.None();
        }
        public virtual async Task Exit() {
        }

        public virtual async Task Suspend() {
        }

        public virtual async Task Awake() {
        }

    }

    public class State<TStateKey, TTransitionKey> : BaseState<TStateKey, TTransitionKey> {
        private readonly Func<Task>? _enter;
        private readonly Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>? _execute;
        private readonly Func<Task>? _exit;
        private readonly Func<Task>? _suspend;
        private readonly Func<Task>? _awake;

        public State(TStateKey key,
            Func<Task>? enter = null, 
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>? execute = null, 
            Func<Task>? exit = null,
            Func<Task>? suspend = null, 
            Func<Task>? awake = null) :
            base(key) {
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

        public override async Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            if (_execute != null) {
                return await _execute.Invoke(executeContext);
            }
            return executeContext.None();
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

    public class StateBuilder<T, TStateKey, TTransitionKey, TParent> where T : IStateMachine<TStateKey, TTransitionKey> {
        private Func<Task>? _enter;
        private Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>? _execute;
        private Func<Task>? _exit;
        private Func<Task>? _suspend;
        private Func<Task>? _awake;
        private Queue<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>? _events;
        private readonly TStateKey _key;
        private readonly TParent _parent;

        internal StateBuilder(TStateKey key, TParent parent) {
            _key = key;
            _parent = parent;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> On(
            TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??= new Queue<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>();
            _events.Enqueue(new Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>(transitionKey, transition));
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Action enter) {
            _enter = async () => enter();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Func<Task> enter) {
            _enter = enter;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Func<Task<ExecuteTransition<TStateKey, TTransitionKey>>> execute) {
            _execute = delta => execute();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>> execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Func<ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = async delta => execute();;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = async delta => execute(delta);;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Action exit) {
            _exit = async () => exit();
            return this;
        }
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Func<Task> exit) {
            _exit = exit;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Action suspend) {
            _suspend = async () => suspend();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Func<Task> suspend) {
            _suspend = suspend;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Action awake) {
            _awake = async () => awake();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Func<Task> awake) {
            _awake = awake;
            return this;
        }

        public TParent End() {
            return _parent;
        } 

        internal IState<TStateKey, TTransitionKey> Build(T stateMachine) {
            IState<TStateKey, TTransitionKey> state = new State<TStateKey, TTransitionKey>(_key, _enter, _execute, _exit, _suspend, _awake);
            stateMachine.AddState(state);
            if (_events != null)
                while (_events.Count > 0) {
                    var tuple = _events.Dequeue();
                    stateMachine.On(state.Key, tuple.Item1, tuple.Item2);
                }
            return state;
        }
    }
}