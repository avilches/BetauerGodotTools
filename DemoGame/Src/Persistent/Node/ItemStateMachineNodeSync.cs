using System;
using Betauer.Application.Lifecycle;
using Betauer.StateMachine.Sync;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemStateMachineNodeSync<TStateKey, TEventKey> : 
    StateMachineNodeSync<TStateKey, TEventKey>, INodeLifecycle, IItemNode 
    where TStateKey : Enum 
    where TEventKey : Enum {
    protected ItemStateMachineNodeSync(TStateKey initialState, string? name = null, bool processInPhysics = false) : base(initialState, name, processInPhysics) {
    }

    protected ItemRepository ItemRepository;
    protected Item Item;
    private Vector2 _initialPosition;
    private volatile bool _busy = true;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    
    // From INodeLifecycle, called by PoolFromNodeFactory
    public abstract void Initialize();

    // From IPoolLifecycle
    public abstract void OnGet();

    // IItemNode
    public void OnAddToWorld(ItemRepository itemRepository, Item item) {
        ItemRepository = itemRepository;
        Item = item;
    }

    public void AddToScene(Godot.Node parent, Vector2 initialPosition) {
        _busy = true;
        _initialPosition = initialPosition;
        base.Reset(); // the StateMachine
        RequestReady();
        parent.AddChild(this);
    }

    public override void _Ready() {
        OnStart(_initialPosition);
    }

    protected abstract void OnStart(Vector2 initialPosition);

    public void RemoveFromWorld() {
        ItemRepository.Remove(Item);
        RemoveFromScene();
    }

    public void RemoveFromScene() {
        if (!_busy) return;
        GetParent().RemoveChild(this);
        OnRemoveFromScene();
        _busy = false;
    }

    public abstract void OnRemoveFromScene();

    
}