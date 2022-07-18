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
    public class StateMachineNode<TStateKey, TTransitionKey> : Node, IStateMachine<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        public readonly IStateMachine<TStateKey, TTransitionKey> StateMachine;
        public IState<TStateKey, TTransitionKey> CurrentState => StateMachine.CurrentState;
        public ProcessMode Mode { get; set; }
        public bool IsState(TStateKey state) => StateMachine.IsState(state);

        public StateMachineNode(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) :
            this(new StateMachine<TStateKey, TTransitionKey>(initialState, name), mode) {
        }

        public StateMachineNode(IStateMachine<TStateKey, TTransitionKey> stateMachine, ProcessMode mode = ProcessMode.Idle) {
            StateMachine = stateMachine;
            Mode = mode;
        }

        public StateMachineBuilder<StateMachineNode<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateBuilder() {
            return new StateMachineBuilder<StateMachineNode<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this);
        }

        public void AddListener(IStateMachineListener<TStateKey> machineListener) {
            StateMachine.AddListener(machineListener);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            StateMachine.AddState(state);
        }

        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            StateMachine.On(transitionKey, transition);
        }

        public void On(TStateKey stateKey, TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            StateMachine.On(stateKey, transitionKey, transition);
        }

        public void Enqueue(TTransitionKey name) {
            StateMachine.Enqueue(name);
        }

        public async Task Execute(float delta) {
            await StateMachine.Execute(delta);
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
    }
}