using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine {
    public enum ProcessMode {
        /// <summary>
        /// <para>The state machine is updated in the <c>_PhysicsProcess</c> callback.</para>
        /// </summary>
        Physics,

        /// <summary>
        /// <para>The state machine is updated int the <c>_Process</c> callback.</para>
        /// </summary>
        Idle,
    }
    public class StateMachineNode<TStateKey, TTransitionKey> : Node, IStateMachine<TStateKey, TTransitionKey> {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);
        public readonly IStateMachine<TStateKey, TTransitionKey> StateMachine;
        public IState<TStateKey, TTransitionKey> State => StateMachine.State;
        public ProcessMode Mode { get; set; }
        
        private Func<float, Task>? _beforeExecute;
        private Func<float, Task>? _afterExecute;

        public StateMachineNode(TStateKey initialState, ProcessMode mode = ProcessMode.Idle) {
            StateMachine = new StateMachine<TStateKey, TTransitionKey>(initialState);
            Mode = mode;
        }

        public StateMachineNode(TStateKey initialState, string name, ProcessMode mode = ProcessMode.Idle) {
            StateMachine = new StateMachine<TStateKey, TTransitionKey>(initialState, name);
            Mode = mode;
        }

        public StateMachineNode(IStateMachine<TStateKey, TTransitionKey> stateMachine, ProcessMode mode) {
            StateMachine = stateMachine;
            Mode = mode;
        }

        public StateMachineBuilder<StateMachineNode<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateBuilder() {
            return new StateMachineBuilder<StateMachineNode<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            StateMachine.AddState(state);
        }

        public IState<TStateKey, TTransitionKey> FindState(TStateKey name) {
            return StateMachine.FindState(name);
        }

        public void On(TTransitionKey on, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            StateMachine.On(on, transition);
        }

        public void Trigger(TTransitionKey name) {
            StateMachine.Trigger(name);
        }

        public async Task Execute(float delta) {
            var canEnter = await _semaphoreSlim.WaitAsync(0);
            if (!canEnter) {
                return;
            }
            try {
                if (_beforeExecute != null) await _beforeExecute.Invoke(delta);
                await StateMachine.Execute(delta);
                if (_afterExecute != null) await _afterExecute.Invoke(delta);
            } finally {
                _semaphoreSlim.Release();
            }
        }

        public override async void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) {
                await Execute(delta);
            } else {
                SetPhysicsProcess(false);
            }
        }

        public override async void _Process(float delta) {
            if (Mode == ProcessMode.Idle) {
                await Execute(delta);
            } else {
                SetProcess(false);
            }
        }

        public StateMachineNode<TStateKey, TTransitionKey> BeforeExecute(Action<float> beforeExecute) {
            _beforeExecute = async delta => beforeExecute(delta);
            return this;
        }

        public StateMachineNode<TStateKey, TTransitionKey> BeforeExecute(Func<float, Task> beforeExecute) {
            _beforeExecute = beforeExecute;
            return this;
        }

        public StateMachineNode<TStateKey, TTransitionKey> AfterExecute(Action<float> afterExecute) {
            _afterExecute = async delta => afterExecute(delta);
            return this;
        }
        public StateMachineNode<TStateKey, TTransitionKey> AfterExecute(Func<float, Task> afterExecute) {
            _afterExecute = afterExecute;
            return this;
        }
    }
}