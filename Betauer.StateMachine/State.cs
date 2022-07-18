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
}