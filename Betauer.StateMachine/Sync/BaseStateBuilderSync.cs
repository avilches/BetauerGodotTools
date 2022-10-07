using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Sync {
    public abstract class BaseStateBuilderSync<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TBuilder : class {
        
        protected Action<TStateKey>? EnterFunc;
        protected Action<TStateKey>? AwakeFunc;
        protected Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response>? ExecuteFunc;
        protected Action<TStateKey>? SuspendFunc;
        protected Action<TStateKey>? ExitFunc;
        protected EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateSync<TStateKey, TTransitionKey>> OnBuild;

        protected BaseStateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TTransitionKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateSync<TStateKey, TTransitionKey> Build() {
            IStateSync<TStateKey, TTransitionKey> stateSync = CreateState();
            OnBuild(stateSync);
            return stateSync;
        }

        protected abstract IStateSync<TStateKey, TTransitionKey> CreateState();

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
        // public TBuilder Enter(Func<TStateKey, Task> enter) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // async () => {}
        // public TBuilder Enter(Func<Task> enter) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // (state) => {}
        public TBuilder Enter(Action<TStateKey> enter) {
            EnterFunc = enter;
            return this as TBuilder;
        }

        
        
        
        // () => {}
        public TBuilder Enter(Action enter) {
            EnterFunc = (_) => enter();
            return this as TBuilder;
        }

        
        
        
        /*
         * Awake
         */
        // async (state) => {}
        // public TBuilder Awake(Func<TStateKey, Task> awake) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // async () => {}
        // public TBuilder Awake(Func<Task> awake) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // (state) => {}
        public TBuilder Awake(Action<TStateKey> enter) {
            AwakeFunc = enter;
            
            
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Awake(Action enter) {
            AwakeFunc = (_) => enter();
            
            
            
            return this as TBuilder;
        }


        /*
         * Execute
         */
        // async (context) => { return context...() }
        public TBuilder Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>> execute) {
            throw new NotSupportedException("Use StateBuilderAsync instead");
            
        }

        // (context) => { return context...() }
        public TBuilder Execute(
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response> execute) {
            ExecuteFunc = execute;
            return this as TBuilder;
        }

        /*
         * Suspend
         */
        // async (state) => {}
        // public TBuilder Suspend(Func<TStateKey, Task> suspend) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // async () => {}
        // public TBuilder Suspend(Func<Task> suspend) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // (state) => {}
        public TBuilder Suspend(Action<TStateKey> enter) {
            SuspendFunc = enter;
            
            
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Suspend(Action enter) {
            SuspendFunc = (_) => enter();

            
            return this as TBuilder;
        }

        /*
         * Exit
         */
        // async (state) => {}
        // public TBuilder Exit(Func<TStateKey, Task> exit) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // async () => {}
        // public TBuilder Exit(Func<Task> exit) {
            // throw new NotSupportedException("Use StateBuilderAsync instead");
            
        // }

        // (state) => {}
        public TBuilder Exit(Action<TStateKey> enter) {
            ExitFunc = enter;
            return this as TBuilder;
        }

        // () => {}
        public TBuilder Exit(Action enter) {
            ExitFunc = (_) => enter();
            return this as TBuilder;
        }
    }
}