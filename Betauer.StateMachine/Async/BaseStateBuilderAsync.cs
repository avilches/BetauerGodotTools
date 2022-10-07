using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public abstract class BaseStateBuilderAsync<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TBuilder : class {
        
        protected Func<TStateKey, Task>? EnterFunc;
        protected Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>>? ExecuteFunc;
        protected Func<TStateKey, Task>? ExitFunc;
        protected Func<TStateKey, Task>? SuspendFunc;
        protected Func<TStateKey, Task>? AwakeFunc;
        protected EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateAsync<TStateKey, TTransitionKey>> OnBuild;

        protected BaseStateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TTransitionKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateAsync<TStateKey, TTransitionKey> Build() {
            IStateAsync<TStateKey, TTransitionKey> state = CreateState();
            OnBuild(state);
            return state;
        }

        protected abstract IStateAsync<TStateKey, TTransitionKey> CreateState();

        public TBuilder On(
            TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response> transition) {
            Events ??= EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>.Create();
            Events.Add(transitionKey, transition);
            return this as TBuilder;
        }

        /*
         * Enter
         */
        // async (state) => {}
        public TBuilder Enter(Func<TStateKey, Task> enter) {
            EnterFunc = enter;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Enter(Func<Task> enter) {
            EnterFunc = from => enter();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Enter(Action<TStateKey> enter) {
            EnterFunc = from => {
                enter(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Enter(Action enter) {
            EnterFunc = from => {
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
            AwakeFunc = awake;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Awake(Func<Task> awake) {
            AwakeFunc = from => awake();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Awake(Action<TStateKey> awake) {
            AwakeFunc = from => {
                awake(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Awake(Action awake) {
            AwakeFunc = from => {
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
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>> execute) {
            ExecuteFunc = execute;
            return this as TBuilder;
        }

        // (context) => { return context...() }
        public TBuilder Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response> execute) {
            ExecuteFunc = (ctx) => Task.FromResult(execute(ctx));
            return this as TBuilder;
        }

        /*
         * Suspend
         */
        // async (state) => {}
        public TBuilder Suspend(Func<TStateKey, Task> suspend) {
            SuspendFunc = suspend;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Suspend(Func<Task> suspend) {
            SuspendFunc = to => suspend();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Suspend(Action<TStateKey> suspend) {
            SuspendFunc = from => {
                suspend(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Suspend(Action suspend) {
            SuspendFunc = from => {
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
            ExitFunc = exit;
            return this as TBuilder;
        }

        // async () => {}
        public TBuilder Exit(Func<Task> exit) {
            ExitFunc = to => exit();
            return this as TBuilder;
        }

        // (state) => {}
        public TBuilder Exit(Action<TStateKey> exit) {
            ExitFunc = from => {
                exit(from);
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Exit(Action exit) {
            ExitFunc = from => {
                exit();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

    }
}