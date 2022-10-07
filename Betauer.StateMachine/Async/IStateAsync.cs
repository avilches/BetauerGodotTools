using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public interface IStateAsync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public Task Enter(TStateKey from);
        public Task Awake(TStateKey from);
        public Task<ExecuteContext<TStateKey, TTransitionKey>.Response> Execute(ExecuteContext<TStateKey, TTransitionKey> executeContext);
        public Task Suspend(TStateKey to);
        public Task Exit(TStateKey to);
    }
}