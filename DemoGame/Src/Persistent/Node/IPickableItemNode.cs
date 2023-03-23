using System;
using Godot;

namespace Veronenger.Persistent.Node; 

public interface IPickableItemNode : IItemNode {
    void BringTo(Func<Vector2> func, Action onPickup);
}