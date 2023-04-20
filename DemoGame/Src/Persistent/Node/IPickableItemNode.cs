using System;
using Godot;

namespace Veronenger.Persistent.Node; 

public interface IPickableItemNode : ILinkableItem {
    void PlayerPicksUp(Func<Vector2> playerPosition, Action onPickup);
}