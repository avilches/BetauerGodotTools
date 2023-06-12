using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Godot;
using Veronenger.Game.Character.Npc;
using Veronenger.Game.Character.Player;
using Veronenger.Game.HUD;
using Veronenger.Game.Items;
using Veronenger.Game.Worlds;
using WorldPlatform = Veronenger.Game.Worlds.Platform.WorldPlatform;

namespace Veronenger.Game.Character; 

[Configuration]
[Loader("GameLoader", Tag = "game")]
[Resource<Texture2D>("Pickups", "res://Game/Items/Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Game/Items/Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Game/Character/Player/Assets/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Game/Character/Player/Assets/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Game/Character/Player/Assets/Leon-gun1.png")]
[Scene.Transient<InventorySlot>("InventorySlotResource")]
[Scene.Transient<WorldPlatform>("WorldPlatformFactory")]
[Scene.Transient<PlayerNode>("PlayerNode")]
[Scene.Transient<ZombieNode>("ZombieNode")]
[Scene.Transient<ProjectileTrail>("ProjectileTrail")]
[Scene.Transient<PickableItemNode>("PickableItem")]
public class CharacterResources {
}

[Configuration]
[PoolContainer<Node>("PoolNodeContainer")]
public class PoolConfig {
    [Pool] NodePool<PlayerNode> PlayerPool => new("PlayerNode");
    [Pool] NodePool<ZombieNode> ZombiePool => new("ZombieNode");
    [Pool] NodePool<ProjectileTrail> ProjectilePool => new("ProjectileTrail");
    [Pool] NodePool<PickableItemNode> PickableItemPool => new("PickableItem");
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("PlayerActionsContainer")]
public class Actions {

	[AxisAction(SaveAs = "Controls/Lateral")] 
	private AxisAction Lateral => AxisAction.Create().Build();

	[AxisAction] 
	private AxisAction Vertical => AxisAction.Create().Build();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Up")]
	private InputAction Up => InputAction.Create()
		.Keys(Key.Up)
		.Buttons(JoyButton.DpadUp)
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Down")]
	private InputAction Down => InputAction.Create()
		.Keys(Key.Down)
		.Buttons(JoyButton.DpadDown)
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Left")]
	private InputAction Left => InputAction.Create()
		.Keys(Key.Left)
		.Buttons(JoyButton.DpadLeft)
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Right")]
	private InputAction Right => InputAction.Create()
		.Keys(Key.Right)
		.Buttons(JoyButton.DpadRight)
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();


	[InputAction(SaveAs = "Controls/Jump")]
	private InputAction Jump => InputAction.Create()
		.Keys(Key.Space)
		.Buttons(JoyButton.A)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Attack")]
	private InputAction Attack => InputAction.Create()
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction()]
	private InputAction Drop => InputAction.Create()
		.Keys(Key.G)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/NextItem")]
	private InputAction NextItem => InputAction.Create()
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/PrevItem")]
	private InputAction PrevItem => InputAction.Create()
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Float")]
	private InputAction Float => InputAction.Create()
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Pausable()
		.AsExtendedUnhandled();
}
