using System;
using System.Threading.Tasks;
using Betauer.Core;
using Godot;

namespace Betauer.FSM.Async; 

public partial class FsmNodeAsync<TStateKey, TEventKey> : 
    FsmNode<TStateKey>, IFsmAsync<TStateKey, TEventKey, StateNodeAsync<TStateKey, TEventKey>> 
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    private class RealFSMNodeAsync : BaseFsmAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> {
        private readonly FsmNodeAsync<TStateKey, TEventKey> _owner;
            
        internal RealFSMNodeAsync(FsmNodeAsync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
            _owner = owner;
        }

        public EventBuilder<FsmNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
            TEventKey transitionKey) {
            return On(_owner, transitionKey);
        }

        public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) {
            return new StateNodeBuilderAsync<TStateKey, TEventKey>(stateKey, AddState);
        }
    }

    private readonly RealFSMNodeAsync _stateMachine;
    public override IFsmEvents<TStateKey> GetFsmEvents() => _stateMachine;

    public StateNodeAsync<TStateKey, TEventKey> CurrentState => (StateNodeAsync<TStateKey, TEventKey>)_stateMachine.CurrentState;
    private Exception? _exception = null;
    public bool Available => _stateMachine.Available;
    public new string? Name => _stateMachine.Name; 
    public double Delta { get; private set; }

    public FsmNodeAsync(TStateKey initialState, string? name = null, bool processInPhysics = false) {
        _stateMachine = new RealFSMNodeAsync(this, initialState, name ?? GetType().Name);
        ProcessInPhysics = processInPhysics;
    }
    public bool IsState(TStateKey state) => _stateMachine.IsState(state);

    public override void _EnterTree() {
        base.Name = Name;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
    public EventBuilder<FsmNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
    public void AddEventRule(TEventKey transitionKey, EventRule<TStateKey, TEventKey> eventRule) => _stateMachine.AddEventRule(transitionKey, eventRule);
    public void AddState(StateNodeAsync<TStateKey, TEventKey> state) => _stateMachine.AddState(state);
    public void Send(TEventKey eventKey, int weight = 0) => _stateMachine.Send(eventKey, weight);

    public Task Execute() {
        throw new Exception("Don't call directly to execute. Instead, add the node to the tree");
    }

    public void Reset() => _stateMachine.Reset();
    
    public bool IsInitialized() => _stateMachine.IsInitialized();

    public sealed override void _Input(InputEvent e) {
        if (Available) CurrentState?.InputHandler._Input(e);
    }

    public sealed override void _UnhandledInput(InputEvent e) {
        if (Available) CurrentState?.InputHandler._UnhandledInput(e);
    }

    public sealed override void _ShortcutInput(InputEvent e) {
        if (Available) CurrentState?.InputHandler._ShortcutInput(e);
    }

    public sealed override void _UnhandledKeyInput(InputEvent e) {
        if (Available) CurrentState?.InputHandler._UnhandledKeyInput(e);
    }
    public sealed override void _PhysicsProcess(double delta) {
        if (ProcessInPhysics) Execute(delta);
        else SetPhysicsProcess(false);
    }

    public sealed override void _Process(double delta) {
        if (!ProcessInPhysics) Execute(delta);
        else SetProcess(false);
    }

    private void Execute(double delta) {
        if (IsQueuedForDeletion()) return;
        if (_exception != null) {
            var e = _exception;
            _exception = null;
            throw e;
        }
        if (!Available) return;
        Delta = delta;
        _stateMachine.Execute().OnException(e => _exception = e, true);
    }
}