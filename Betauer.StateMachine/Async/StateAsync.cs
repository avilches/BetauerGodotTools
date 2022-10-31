using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public class StateAsync<TStateKey, TTransitionKey> : IStateAsync<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events { get; }

        private readonly Func<Task>? _enter;
        private readonly Func<Task>? _awake;
        private readonly Tuple<Func<bool>, Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response>>[]? _conditions;
        private readonly Func<Task>? _execute;
        private readonly Func<Task>? _suspend;
        private readonly Func<Task>? _exit;

        public StateAsync(
            TStateKey key,
            Func<Task>? enter,
            Tuple<Func<bool>, Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response>>[] conditions,
            Func<Task>? execute,
            Func<Task>? exit,
            Func<Task>? suspend,
            Func<Task>? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events) {

            Key = key;
            _enter = enter;
            _conditions = conditions;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            Events = events;
        }

        public Task Enter() {
            return _enter != null ? _enter.Invoke() : Task.CompletedTask;
        }

        public Task Awake() {
            return _awake != null ? _awake.Invoke() : Task.CompletedTask;
        }

        public Task Execute() {
            return _execute != null ? _execute.Invoke() : Task.CompletedTask;
        }

        public Context<TStateKey, TTransitionKey>.Response Next(Context<TStateKey, TTransitionKey> ctx) {
            var span = _conditions.AsSpan();
            for (var i = 0; i < span.Length; i++) {
                var condition = span[i];
                if (condition.Item1()) {
                    return condition.Item2(ctx);
                }
            }
            return ctx.None();
        }

        public Task Suspend() {
            return _suspend != null ? _suspend.Invoke() : Task.CompletedTask;
        }

        public Task Exit() {
            return _exit != null ? _exit.Invoke() : Task.CompletedTask;
        }
    }
}