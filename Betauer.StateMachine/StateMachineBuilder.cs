using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public class StateMachineBuilder<T, TStateKey, TTransitionKey> where T : IStateMachine<TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>> _pendingStateBuilders =
            new Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>>();

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> On(TStateKey stateKey, TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateMachine.On(stateKey, transitionKey, transition);
            return this;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> On(TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateMachine.On(transitionKey, transition);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>> State(TStateKey stateKey) {
            var stateBuilder = new StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>(_stateMachine, stateKey, this);
            _pendingStateBuilders.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> AddListener(IStateMachineListener<TStateKey> machineListener) {
            if (machineListener == null) throw new ArgumentNullException(nameof(machineListener));
            _stateMachine.AddListener(machineListener);
            return this;
        }

        public T Build() {
            while (_pendingStateBuilders.Count > 0) _pendingStateBuilders.Dequeue().Build();
            return _stateMachine;
        }
    }

    public class StateBuilder<T, TStateKey, TTransitionKey, TParent> where T : IStateMachine<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {
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
        // async (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Func<TStateKey, Task> enter) {
            _enter = enter;
            return this;
        }

        // async () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Func<Task> enter) {
            _enter = async from => await enter();
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Action<TStateKey> enter) {
            _enter = async from => enter(from);
            return this;
        }

        // () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Enter(Action enter) {
            _enter = async from => enter();
            return this;
        }

        /*
         * Awake
         */

        // async (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Func<TStateKey, Task> awake) {
            _awake = awake;
            return this;
        }

        // async () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Func<Task> awake) {
            _awake = async from => await awake();
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Action<TStateKey> awake) {
            _awake = async from => awake(from);
            return this;
        }

        // () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Awake(Action awake) {
            _awake = async from => awake();
            return this;
        }


        /*
         * Execute
         */
        // async (context) => { return context...() }
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>
                execute) {
            _execute = execute;
            return this;
        }

        // async () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Func<Task> execute) {
            _execute = async (ctx) => {
                await execute();
                return ctx.None();
            };
            return this;
        }

        // (context) => { return context...() }
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = async (ctx) => execute(ctx);
            return this;
        }

        // () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Execute(Action execute) {
            _execute = async (ctx) => {
                execute();
                return ctx.None();
            };
            return this;
        }

        /*
         * Suspend
         */
        // async (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Func<TStateKey, Task> suspend) {
            _suspend = suspend;
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Func<Task> suspend) {
            _suspend = async to => await suspend();
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Action<TStateKey> suspend) {
            _suspend = async to => suspend(to);
            return this;
        }

        // () => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Suspend(Action suspend) {
            _suspend = async to => suspend();
            return this;
        }

        /*
         * Exit
         */
        // async (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Func<TStateKey, Task> exit) {
            _exit = exit;
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Func<Task> exit) {
            _exit = async to => await exit();
            return this;
        }

        // (state) => {}
        public StateBuilder<T, TStateKey, TTransitionKey, TParent> Exit(Action<TStateKey> exit) {
            _exit = async to => exit(to);
            return this;
        }

        // () => {}
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