using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine {
    public class StateBuilder<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {
        private Func<TStateKey, Task>? _enter;

        private Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>?
            _execute;

        private Func<TStateKey, Task>? _exit;
        private Func<TStateKey, Task>? _suspend;
        private Func<TStateKey, Task>? _awake;
        private EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        private readonly TStateKey _key;
        private readonly Action<IState<TStateKey, TTransitionKey>> _onBuild;

        internal StateBuilder(TStateKey key, Action<IState<TStateKey, TTransitionKey>> onBuild) {
            _onBuild = onBuild;
            _key = key;
        }

        public StateBuilder<TStateKey, TTransitionKey> On(
            TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??= new EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _events.Add(transitionKey, transition);
            return this;
        }

        /*
         * Enter
         */
        // async (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Enter(Func<TStateKey, Task> enter) {
            _enter = enter;
            return this;
        }

        // async () => {}
        public StateBuilder<TStateKey, TTransitionKey> Enter(Func<Task> enter) {
            _enter = from => enter();
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Enter(Action<TStateKey> enter) {
            _enter = from => {
                enter(from);
                return Task.CompletedTask;
            };
            return this;
        }

        // () => {}
        public StateBuilder<TStateKey, TTransitionKey> Enter(Action enter) {
            _enter = from => {
                enter();
                return Task.CompletedTask;
            };
            return this;
        }

        /*
         * Awake
         */

        // async (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Awake(Func<TStateKey, Task> awake) {
            _awake = awake;
            return this;
        }

        // async () => {}
        public StateBuilder<TStateKey, TTransitionKey> Awake(Func<Task> awake) {
            _awake = from => awake();
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Awake(Action<TStateKey> awake) {
            _awake = from => {
                awake(from);
                return Task.CompletedTask;
            };
            return this;
        }

        // () => {}
        public StateBuilder<TStateKey, TTransitionKey> Awake(Action awake) {
            _awake = from => {
                awake();
                return Task.CompletedTask;
            };
            return this;
        }


        /*
         * Execute
         */
        // async (context) => { return context...() }
        public StateBuilder<TStateKey, TTransitionKey> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteTransition<TStateKey, TTransitionKey>>>
                execute) {
            _execute = execute;
            return this;
        }
        
        // (context) => { return context...() }
        public StateBuilder<TStateKey, TTransitionKey> Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> execute) {
            _execute = (ctx) => Task.FromResult(execute(ctx));
            return this;
        }

        /*
         * Suspend
         */
        // async (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Suspend(Func<TStateKey, Task> suspend) {
            _suspend = suspend;
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Suspend(Func<Task> suspend) {
            _suspend = to => suspend();
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Suspend(Action<TStateKey> suspend) {
            _suspend = from => {
                suspend(from);
                return Task.CompletedTask;
            };
            return this;
        }

        // () => {}
        public StateBuilder<TStateKey, TTransitionKey> Suspend(Action suspend) {
            _suspend = from => {
                suspend();
                return Task.CompletedTask;
            };
            return this;
        }

        /*
         * Exit
         */
        // async (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Exit(Func<TStateKey, Task> exit) {
            _exit = exit;
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Exit(Func<Task> exit) {
            _exit = to => exit();
            return this;
        }

        // (state) => {}
        public StateBuilder<TStateKey, TTransitionKey> Exit(Action<TStateKey> exit) {
            _exit = from => {
                exit(from);
                return Task.CompletedTask;
            };
            return this;
        }

        // () => {}
        public StateBuilder<TStateKey, TTransitionKey> Exit(Action exit) {
            _exit = from => {
                exit();
                return Task.CompletedTask;
            };
            return this;
        }

        public IState<TStateKey, TTransitionKey> Build() {
            IState<TStateKey, TTransitionKey> state =
                new State<TStateKey, TTransitionKey>(_key, _enter, _execute, _exit, _suspend, _awake, _events);
            _onBuild(state);
            return state;
        }
    }
}