using System;
using Godot;

namespace Veronenger.Persistent.Node; 

public interface IPickableItemNode : ILinkableItem {
    void FlyingPickup(Func<Vector2> func, Action onPickup);
}