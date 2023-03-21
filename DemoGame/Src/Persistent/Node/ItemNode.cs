using Betauer.Application.Lifecycle;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemNode : Godot.Node, INodeLifecycle, IItemNode {

    protected ItemRepository ItemRepository;
    protected Item Item;
    private Vector2 _initialPosition;
    private volatile bool _busy = true;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    // From MiniPool
    public abstract void Initialize();
    public abstract void OnGet();

    // IItemNode
    public void OnAddToWorld(ItemRepository itemRepository, Item item) {
        ItemRepository = itemRepository;
        Item = item;
    }

    public void AddToScene(Godot.Node parent, Vector2 initialPosition) {
        _busy = true;
        _initialPosition = initialPosition;
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