using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public partial class StateMachineNodeSync<TStateKey, TEventKey> : 
        StateMachineNode<TStateKey>, IStateMachineSync<TStateKey, TEventKey, StateNodeSync<TStateKey, TEventKey>> 
        where TStateKey : Enum 
        where TEventKey : Enum {
        
        private class RealStateMachineNode : BaseStateMachineSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> { 
            private readonly StateMachineNodeSync<TStateKey, TEventKey> _owner;

            internal RealStateMachineNode(StateMachineNodeSync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
                _owner = owner;
            }

            public EventBuilder<StateMachineNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
                TEventKey transitionKey) {
                return On(_owner, transitionKey);
            }

            public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) {
                return new StateNodeBuilderSync<TStateKey, TEventKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNode _stateMachine;
        public override IStateMachineEvents<TStateKey> GetStateMachineEvents() => _stateMachine;

        public StateNodeSync<TStateKey, TEventKey> CurrentState => (StateNodeSync<TStateKey, TEventKey>)_stateMachine.CurrentState;

        
        public string? Name => _stateMachine.Name; 
        public double Delta { get; private set; }

        public StateMachineNodeSync(TStateKey initialState, string? name = null, bool processInPhysics = false) {
            _stateMachine = new RealStateMachineNode(this, initialState, name);
            ProcessInPhysics = processInPhysics;
        }
        public bool IsState(TStateKey state) => _stateMachine.IsState(state);


        public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
        public EventBuilder<StateMachineNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
        public void AddEvent(TEventKey transitionKey, Event<TStateKey, TEventKey> @event) => _stateMachine.AddEvent(transitionKey, @event);
        public void AddState(StateNodeSync<TStateKey, TEventKey> stateSync) => _stateMachine.AddState(stateSync);
        public void Enqueue(TEventKey name) => _stateMachine.Enqueue(name);

        public void Execute() {
            throw new Exception("Don't call directly to execute. Instead, add the node to the tree");
        }

        public override void _Input(InputEvent e) {
            CurrentState?._Input(e);
        }

        public override void _UnhandledInput(InputEvent e) {
            CurrentState?._UnhandledInput(e);
        }

        public override void _PhysicsProcess(double delta) {
            if (ProcessInPhysics) Execute(delta);
            else SetPhysicsProcess(false);
        }

        public override void _Process(double delta) {
            if (!ProcessInPhysics) Execute(delta);
            else SetProcess(false);
        }

        public void Execute(double delta) {
            if (IsQueuedForDeletion()) return;
            Delta = delta;
            _stateMachine.Execute();
        }
    }
}