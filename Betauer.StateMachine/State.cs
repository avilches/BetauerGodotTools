using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public interface IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        public TStateKey Key { get; }
        public Task Enter(TStateKey from);
        public Task Awake(TStateKey from);

        public Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(
            ExecuteContext<TStateKey, TTransitionKey> executeContext);

        public Task Suspend(TStateKey to);
        public Task Exit(TStateKey to);
    }

    public abstract class BaseState<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        public TStateKey Key { get; }

        protected BaseState(TStateKey key) {
            Key = key;
        }

        public virtual async Task Enter(TStateKey from) {
        }

        public virtual async Task Awake(TStateKey from) {
        }

        public virtual async Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(
            ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            return executeContext.None();
        }

        public virtual async Task Suspend(TStateKey to) {
        }

        public virtual async Task Exit(TStateKey to) {
        }
    }

    public class State<TStateKey, TTransitionKey> : BaseState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        private readonly Func<TStateKey, Task>? _enter;
        private readonly Func<TStateKey, Task>? _awake;

        private readonly
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>?
            _execute;

        private readonly Func<TStateKey, Task>? _suspend;
        private readonly Func<TStateKey, Task>? _exit;

        public State(TStateKey key,
            Func<TStateKey, Task>? enter = null,
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>?
                execute = null,
            Func<TStateKey, Task>? exit = null,
            Func<TStateKey, Task>? suspend = null,
            Func<TStateKey, Task>? awake = null) :
            base(key) {
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
        }

        public override async Task Enter(TStateKey from) {
            if (_enter != null) {
                await _enter.Invoke(from);
            }
        }

        public override async Task Awake(TStateKey from) {
            if (_awake != null) {
                await _awake.Invoke(from);
            }
        }

        public override async Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(
            ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            if (_execute != null) {
                return await _execute.Invoke(executeContext);
            }
            return executeContext.None();
        }

        public override async Task Suspend(TStateKey to) {
            if (_suspend != null) {
                await _suspend.Invoke(to);
            }
        }

        public override async Task Exit(TStateKey to) {
            if (_exit != null) {
                await _exit.Invoke(to);
            }
        }
    }

    public class StateBuilder<T, TStateKey, TTransitionKey, TParent> where T : IStateMachine<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        private Func<TStateKey, Task>? _enter;

        private Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>?
            _execute;

        private Func<TStateKey, Task>? _exit;
        private Func<TStateKey, Task>? _suspend;
        private Func<TStateKey, Task>? _awake;
        private List<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>? _events;
        private readonly TStateKey _key;
        private readonly TParent _parent;
        private readonly T _stateMachine;

        internal StateBuilder(T stateMachine, TStateKey key, TParent parent) {
            _stateMachine = stateMachine;
            _key = key;
            _parent = parent;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> On(
            TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??=
                new List<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>();
            _events.Add(
                new Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>(transitionKey,
                    transition));
            return this;
        }

        /*
         * Enter
         */
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Func<TStateKey, Task> enter) {
            _enter = enter;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Func<Task> enter) {
            _enter = async from => await enter();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Action<TStateKey> enter) {
            _enter = async from => enter(from);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Action enter) {
            _enter = async from => enter();
            return this;
        }

        /*
         * Awake
         */

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Func<TStateKey, Task> awake) {
            _awake = awake;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Func<Task> awake) {
            _awake = async from => await awake();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Action<TStateKey> awake) {
            _awake = async from => awake(from);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Action awake) {
            _awake = async from => awake();
            return this;
        }


        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<Task<ExecuteTransition<TStateKey, TTransitionKey>>> execute) {
            _execute = delta => execute();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>
                execute) {
            _execute = execute;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = async delta => execute();
            ;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = async delta => execute(delta);
            ;
            return this;
        }

        /*
         * Suspend
         */
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Func<TStateKey, Task> suspend) {
            _suspend = suspend;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Func<Task> suspend) {
            _suspend = async to => await suspend();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Action<TStateKey> suspend) {
            _suspend = async to => suspend(to);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Action suspend) {
            _suspend = async to => suspend();
            return this;
        }

        /*
         * Exit
         */
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Func<TStateKey, Task> exit) {
            _exit = exit;
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Func<Task> exit) {
            _exit = async to => await exit();
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Action<TStateKey> exit) {
            _exit = async to => exit(to);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Action exit) {
            _exit = async to => exit();
            return this;
        }

        public TParent End() {
            return _parent;
        }

        internal IState<TStateKey, TTransitionKey> Build() {
            IState<TStateKey, TTransitionKey> state =
                new State<TStateKey, TTransitionKey>(_key, _enter, _execute, _exit, _suspend, _awake);
            _stateMachine.AddState(state);
            _events?.ForEach(tuple => _stateMachine.On(state.Key, tuple.Item1, tuple.Item2));
            return state;
        }
    }
}