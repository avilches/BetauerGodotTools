using System;
using Betauer.DI;
using Betauer.FSM.Sync;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemNodeFsm<TStateKey, TEventKey> : 
    FsmNodeSync<TStateKey, TEventKey>, ILinkableItem, IInjectable 
    where TStateKey : Enum 
    where TEventKey : Enum {
    protected ItemNodeFsm(TStateKey initialState, string? name = null, bool processInPhysics = false) :
        base(initialState, name, processInPhysics) {
        TreeEntered += () => {
            _busy = true;
            Reset();
        };
        TreeExited += () => _busy = false;
    }

    [Inject] public ItemRepository ItemRepository { get; set; }

    protected virtual Item Item { get; set; }
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    public abstract void PostInject();

    // From IPoolLifecycle
    public abstract void OnGet();

    // IItemNode
    public void LinkItem(Item item) {
        Item = item;
    }

    public void RemoveFromWorld() {
        ItemRepository.Remove(Item);
    }

    public abstract Vector2 GlobalPosition { get; set; }
}