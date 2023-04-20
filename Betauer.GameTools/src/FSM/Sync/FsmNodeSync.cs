using System;
using Betauer.Core;
using Betauer.Tools.FastReflection;
using Godot;

namespace Betauer.FSM.Sync; 

public partial class FsmNodeSync<TStateKey, TEventKey> : 
    FsmNode<TStateKey>, IFsmSync<TStateKey, TEventKey, StateNodeSync<TStateKey, TEventKey>> 
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    private class RealFSMNodeSync : BaseFsmSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> { 
        private readonly FsmNodeSync<TStateKey, TEventKey> _owner;

        internal RealFSMNodeSync(FsmNodeSync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
            _owner = owner;
        }

        public EventBuilder<FsmNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
            TEventKey transitionKey) {
            return On(_owner, transitionKey);
        }

        public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) {
            return new StateNodeBuilderSync<TStateKey, TEventKey>(stateKey, AddState);
        }
    }

    private readonly RealFSMNodeSync _stateMachine;
    public override IFsmEvents<TStateKey> GetFsmEvents() => _stateMachine;

    public StateNodeSync<TStateKey, TEventKey> CurrentState => (StateNodeSync<TStateKey, TEventKey>)_stateMachine.CurrentState;

        
    public new string? Name => _stateMachine.Name; 
    public double Delta { get; private set; }

    public FsmNodeSync(TStateKey initialState, string? name = null, bool processInPhysics = false) {
        _stateMachine = new RealFSMNodeSync(this, initialState, name ?? GetType().GetTypeName());
        ProcessInPhysics = processInPhysics;
    }
    public bool IsState(TStateKey state) => _stateMachine.IsState(state);

    public override void _EnterTree() {
        base.Name = Name;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
    public EventBuilder<FsmNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
    public void AddEventRule(TEventKey transitionKey, EventRule<TStateKey, TEventKey> eventRule) => _stateMachine.AddEventRule(transitionKey, eventRule);
    public void AddState(StateNodeSync<TStateKey, TEventKey> stateSync) => _stateMachine.AddState(stateSync);
    public void Send(TEventKey eventKey, int weight = 0) => _stateMachine.Send(eventKey, weight);

    public void Execute() => _stateMachine.Execute();

    public void Reset() => _stateMachine.Reset();

    public bool IsInitialized() => _stateMachine.IsInitialized();

    public sealed override void _Input(InputEvent e) {
        CurrentState?.InputHandler._Input(e);
    }

    public sealed override void _UnhandledInput(InputEvent e) {
        CurrentState?.InputHandler._UnhandledInput(e);
    }

    public sealed override void _ShortcutInput(InputEvent e) {
        CurrentState?.InputHandler._ShortcutInput(e);
    }

    public sealed override void _UnhandledKeyInput(InputEvent e) {
        CurrentState?.InputHandler._UnhandledKeyInput(e);
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
        Delta = delta;
        _stateMachine.Execute();
    }
}