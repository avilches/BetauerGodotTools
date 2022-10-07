using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public class StateAsync<TStateKey, TTransitionKey> : IStateAsync<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events { get; }

        private readonly Func<TStateKey, Task>? _enter;
        private readonly Func<TStateKey, Task>? _awake;
        private readonly Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>>? _execute;
        private readonly Func<TStateKey, Task>? _suspend;
        private readonly Func<TStateKey, Task>? _exit;

        public StateAsync(TStateKey key,
            Func<TStateKey, Task>? enter,
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>>? execute,
            Func<TStateKey, Task>? exit,
            Func<TStateKey, Task>? suspend,
            Func<TStateKey, Task>? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events) {
            Key = key;
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            Events = events;
        }


        public Task Enter(TStateKey from) {
            return _enter != null ? _enter.Invoke(from) : Task.CompletedTask;
        }

        public Task Awake(TStateKey from) {
            return _awake != null ? _awake.Invoke(from) : Task.CompletedTask;
        }

        public Task<ExecuteContext<TStateKey, TTransitionKey>.Response> Execute(
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