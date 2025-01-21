using Betauer.Application;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform;

[Singleton]
public class PlatformBus : GodotBus<IMulticastEvent>;
public interface IMulticastEvent;

public enum PlatformCommandType { SpawnZombie }
public record struct PlatformCommand(PlatformCommandType Type) : IMulticastEvent;
public record struct PlayerDropEvent(PickableGameObject Item, Vector2 GlobalPosition, Vector2 DropVelocity) : IMulticastEvent;

public record struct PlayerInventoryChangeEvent(Inventory Inventory, PlayerInventoryChangeEvent.EventType Type, PickableGameObject PickableGameObject) : IMulticastEvent {
    public enum EventType { Add, Remove, Equip, Unequip, SlotAmountUpdate, Refresh }
}
public record struct PlayerHealthEvent(PlayerNode PlayerNode, float FromHealth, float ToHealth, float Max) : IMulticastEvent;
public record struct PlayerAttackEvent(PlayerNode Player, NpcNode NpcNode, WeaponGameObject Weapon) : IMulticastEvent;