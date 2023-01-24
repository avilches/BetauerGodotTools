using System;
using System.Threading.Tasks;
using Betauer.Core;
using Godot;

namespace Betauer.StateMachine.Async; 

public partial class StateMachineNodeAsync<TStateKey, TEventKey> : 
    StateMachineNode<TStateKey>, IStateMachineAsync<TStateKey, TEventKey, StateNodeAsync<TStateKey, TEventKey>> 
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    private class RealStateMachineNodeAsync : BaseStateMachineAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> {
        private readonly StateMachineNodeAsync<TStateKey, TEventKey> _owner;
            
        internal RealStateMachineNodeAsync(StateMachineNodeAsync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
            _owner = owner;
        }

        public EventBuilder<StateMachineNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
            TEventKey transitionKey) {
            return On(_owner, transitionKey);
        }

        public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) {
            return new StateNodeBuilderAsync<TStateKey, TEventKey>(stateKey, AddState);
        }
    }

    private readonly RealStateMachineNodeAsync _stateMachine;
    public override IStateMachineEvents<TStateKey> GetStateMachineEvents() => _stateMachine;

    public StateNodeAsync<TStateKey, TEventKey> CurrentState => (StateNodeAsync<TStateKey, TEventKey>)_stateMachine.CurrentState;
    private Exception? _exception = null;
    public bool Available => _stateMachine.Available;
    public string? Name => _stateMachine.Name; 
    public double Delta { get; private set; }

    public StateMachineNodeAsync(TStateKey initialState, string? name = null, bool processInPhysics = false) {
        _stateMachine = new RealStateMachineNodeAsync(this, initialState, name);
        ProcessInPhysics = processInPhysics;
    }
    public bool IsState(TStateKey state) => _stateMachine.IsState(state);

    public override void _EnterTree() {
        base.Name = Name;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
    public EventBuilder<StateMachineNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
    public void AddEvent(TEventKey transitionKey, Event<TStateKey, TEventKey> @event) => _stateMachine.AddEvent(transitionKey, @event);
    public void AddState(StateNodeAsync<TStateKey, TEventKey> state) => _stateMachine.AddState(state);
    public void Send(TEventKey name, int weight = 0) => _stateMachine.Send(name, weight);

    public Task Execute() {
        throw new Exception("Don't call directly to execute. Instead, add the node to the tree");
    }

    public override void _Input(InputEvent e) {
        if (Available) CurrentState?.InputHandler._Input(e);
    }

    public override void _UnhandledInput(InputEvent e) {
        if (Available) CurrentState?.InputHandler._UnhandledInput(e);
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
        if (_exception != null) {
            var e = _exception;
            _exception = null;
            throw e;
        }
        if (!Available) return;
        Delta = delta;
        CurrentState.InputHandler._InputBatch();
        CurrentState.InputHandler._UnhandledInputBatch();
        _stateMachine.Execute().OnException(e => _exception = e, true);
    }
}