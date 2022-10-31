using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public interface IStateAsync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public Task Enter();
        public Task Awake();
        public Task Execute();
        public Context<TStateKey, TTransitionKey>.Response Next(Context<TStateKey, TTransitionKey> context);
        public Task Suspend();
        public Task Exit();
    }
}