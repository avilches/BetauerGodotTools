using System;
using Godot;

namespace Veronenger.Persistent.Node; 

public interface IPickableItemNode : ILinkableItem {
    void BringTo(Func<Vector2> func, Action onPickup);
}