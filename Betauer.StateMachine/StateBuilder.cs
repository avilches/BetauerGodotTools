using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public abstract class BaseStateBuilder<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TBuilder : class {
        protected Func<TStateKey, Task>? _enter;
        protected Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>?
            _execute;
        protected Func<TStateKey, Task>? _exit;
        protected Func<TStateKey, Task>? _suspend;
        protected Func<TStateKey, Task>? _awake;
        protected EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        protected readonly TStateKey _key;
        protected readonly Action<IState<TStateKey, TTransitionKey>> _onBuild;

        public BaseStateBuilder(TStateKey key, Action<IState<TStateKey, TTransitionKey>> onBuild) {
            _onBuild = onBuild;
            _key = key;
        }

        public TBuilder On(
            TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??=
                new EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _events.Add(transitionKey, transition);
            return this as TBuilder;
        }

        /*
         * Enter
         */
        // async (state) => {}
        public TBuilder Enter(Func<TStateKey, Task> enter) {
            _enter = enter;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Enter(Func<Task> enter) {
            _enter = from => enter();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Enter(Action<TStateKey> enter) {
            _enter = from => {
                enter(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Enter(Action enter) {
            _enter = from => {
                enter();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Awake
         */

        // async (state) => {}
        public TBuilder Awake(Func<TStateKey, Task> awake) {
            _awake = awake;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Awake(Func<Task> awake) {
            _awake = from => awake();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Awake(Action<TStateKey> awake) {
            _awake = from => {
                awake(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Awake(Action awake) {
            _awake = from => {
                awake();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }


        /*
         * Execute
         */
        // async (context) => { return context...() }
        public TBuilder Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>
                execute) {
            _execute = execute;
            return this as TBuilder;
        }

        // (context) => { return context...() }
        public TBuilder Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = (ctx) => Task.FromResult(execute(ctx));
            return this as TBuilder;
        }

        /*
         * Suspend
         */
        // async (state) => {}
        public TBuilder Suspend(Func<TStateKey, Task> suspend) {
            _suspend = suspend;
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Suspend(Func<Task> suspend) {
            _suspend = to => suspend();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Suspend(Action<TStateKey> suspend) {
            _suspend = from => {
                suspend(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Suspend(Action suspend) {
            _suspend = from => {
                suspend();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Exit
         */
        // async (state) => {}
        public TBuilder Exit(Func<TStateKey, Task> exit) {
            _exit = exit;
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Exit(Func<Task> exit) {
            _exit = to => exit();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Exit(Action<TStateKey> exit) {
            _exit = from => {
                exit(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Exit(Action exit) {
            _exit = from => {
                exit();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        public IState<TStateKey, TTransitionKey> Build() {
            IState<TStateKey, TTransitionKey> state = CreateState();
            _onBuild(state);
            return state;
        }

        protected abstract IState<TStateKey, TTransitionKey> CreateState();

    }

    public class StateBuilder<TStateKey, TTransitionKey> : BaseStateBuilder<StateBuilder<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        protected override IState<TStateKey, TTransitionKey> CreateState() {
            return new State<TStateKey, TTransitionKey>(_key, _enter, _execute, _exit, _suspend, _awake, _events);
        }

        public StateBuilder(TStateKey key, Action<IState<TStateKey, TTransitionKey>> onBuild) : base(key, onBuild) {
        }
    }
}