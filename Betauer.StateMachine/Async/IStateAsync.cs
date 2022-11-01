using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public interface IStateAsync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public Task Enter();
        public Task Awake();
        public Task Execute();
        public Command<TStateKey, TTransitionKey> Next(ConditionContext<TStateKey, TTransitionKey> conditionContext);
        public Task Suspend();
        public Task Exit();
    }
}