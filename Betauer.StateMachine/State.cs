using System;
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

        public virtual Task Enter(TStateKey from) {
            return Task.CompletedTask;
        }

        public virtual Task Awake(TStateKey from) {
            return Task.CompletedTask;
        }

        public virtual Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(
            ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            return Task.FromResult(executeContext.None());
        }

        public virtual Task Suspend(TStateKey to) {
            return Task.CompletedTask;
        }

        public virtual Task Exit(TStateKey to) {
            return Task.CompletedTask;
        }
    }

    public class State<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        public TStateKey Key { get; }

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
            Func<TStateKey, Task>? awake = null) {
            Key = key;
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
        }


        public Task Enter(TStateKey from) {
            return _enter != null ? _enter.Invoke(from) : Task.CompletedTask;
        }

        public Task Awake(TStateKey from) {
            return _awake != null ? _awake.Invoke(from) : Task.CompletedTask;
        }

        public Task<ExecuteTransition<TStateKey, TTransitionKey>> Execute(
            ExecuteContext<TStateKey, TTransitionKey> executeContext) {
            return _execute != null ? _execute.Invoke(executeContext) : Task.FromResult(executeContext.None());
        }

        public Task Suspend(TStateKey to) {
            return _suspend != null ? _suspend.Invoke(to) : Task.CompletedTask;
        }

        public Task Exit(TStateKey to) {
            return _exit != null ? _exit.Invoke(to) : Task.CompletedTask;
        }
    }
}