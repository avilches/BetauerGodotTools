using System;
using Betauer.Core.Pool;
using Betauer.StateMachine.Sync;
using Godot;

namespace Veronenger.Persistent;

public abstract partial class ItemStateMachineNodeSync<TStateKey, TEventKey> : 
    StateMachineNodeSync<TStateKey, TEventKey>, IBusyElement 
    where TStateKey : Enum 
    where TEventKey : Enum {
    protected ItemStateMachineNodeSync(TStateKey initialState, string? name = null, bool processInPhysics = false) : base(initialState, name, processInPhysics) {
    }

    protected World World;
    protected Item Item;
    private Vector2 _initialPosition;
    private bool _busy = true;

    public bool IsBusy() => _busy;

    public void OnAddToWorld(World world, Item item) {
        World = world;
        Item = item;
    }

    public void AddToScene(Node node, Vector2 initialPosition) {
        _busy = true;
        _initialPosition = initialPosition;
        Reset(); // the StateMachine
        RequestReady();
        node.AddChild(this);
    }

    public override void _Ready() {
        OnStart(_initialPosition);
    }

    public void RemoveFromScene() {
        if (!_busy) return;
        GetParent().RemoveChild(this);
        OnRemoveFromScene();
        _busy = false;
    }

    public void RemoveFromWorld() {
        World.Remove(Item);
        RemoveFromScene();
    }

    public abstract void OnStart(Vector2 initialPosition);
    public abstract void OnRemoveFromScene();
}