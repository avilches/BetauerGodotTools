using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.Settings;
using Betauer.Camera;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Joypad;
using Godot;
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.HUD;
using Veronenger.Game.Platform.Items;
using Veronenger.Game.Platform.World;

namespace Veronenger.Game.Platform;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<PlatformGameView>(Name = "PlatformGameView")]
public class PlatformMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "platform")]
[Resource<Texture2D>("Pickups", "res://Game/Platform/Items/Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Game/Platform/Items/Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-gun1.png")]
[Scene.Transient<PlatformWorld>(Name = "PlatformWorldFactory")]
[Scene.Transient<PlayerHud>(Name = "PlayerHud")]
[Scene.Transient<InventorySlot>(Name = "InventorySlot")]
[Scene.Transient<PlayerNode>(Name = "Player")]
[Scene.NodePool<PickableItemNode>(Name = "PickableItemPool")]
[Scene.NodePool<ProjectileTrail>(Name = "ProjectilePool")]
[Scene.NodePool<ZombieNode>(Name = "ZombiePool")]
public class PlatformGameResources {
	[Transient<PlatformHud>(Name = "PlatformHudFactory")] PlatformHud PlatformHud => new PlatformHud();
}

public interface IPlatformSaveObject : ISaveObject {
}

[Configuration]
public class PlatformGameConfig {
	[Singleton] public JsonGameLoader<PlatformSaveGameMetadata> PlatformGameObjectLoader() {
		var loader = new JsonGameLoader<PlatformSaveGameMetadata>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		loader.Scan<IPlatformSaveObject>();
		return loader;
	}
	
	[Transient] public StageController StageControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Transient] public StageCameraController StageCameraControllerFactory => new(CollisionLayerConstants.LayerStageArea);

    [Singleton] public PlatformMultiPlayerContainer PlatformMultiPlayerContainer => new();
}


[Singleton]
public class PlatformMultiPlayerContainer : MultiPlayerContainer<PlatformPlayerActions>, IInjectable {
    
    [Inject] public SettingsContainer SettingsContainer { get; set; }
    
    public void PostInject() {
        ConfigureSaveSettings(SettingsContainer);
    }
}

public class PlatformPlayerActions : PlayerActionsContainer {

    public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Controls/Lateral").Build();

    public AxisAction Vertical { get; } = AxisAction.Create("Vertical").SaveAs("Controls/Vertical").Build();

    public InputAction Up { get; } = InputAction.Create("Up")
        .AxisName("Vertical")
        .SaveAs("Controls/Up")
        .Keys(Key.Up)
        .Buttons(JoyButton.DpadUp)
        .NegativeAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Down { get; } = InputAction.Create("Down")
        .AxisName("Vertical")
        .SaveAs("Controls/Down")
        .Keys(Key.Down)
        .Buttons(JoyButton.DpadDown)
        .PositiveAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Left { get; } = InputAction.Create("Left")
        .AxisName("Lateral")
        .SaveAs("Controls/Left")
        .Keys(Key.Left)
        .Buttons(JoyButton.DpadLeft)
        .NegativeAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Right { get; } = InputAction.Create("Right")
        .AxisName("Lateral")
        .SaveAs("Controls/Right")
        .Keys(Key.Right)
        .Buttons(JoyButton.DpadRight)
        .PositiveAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Jump { get; } = InputAction.Create("Jump")
        .SaveAs("Controls/Jump")
        .Keys(Key.Space)
        .Buttons(JoyButton.A)
        .Build(true);

    public InputAction Attack { get; } = InputAction.Create("Attack")
        .SaveAs("Controls/Attack")
        .Keys(Key.C)
        .Mouse(MouseButton.Left)
        .Buttons(JoyButton.B)
        .Build();

    public InputAction Drop { get; } = InputAction.Create("Drop")
        .SaveAs("Controls/Drop")
        .Keys(Key.G)
        .Build();

    public InputAction NextItem { get; } = InputAction.Create("NextItem")
        .SaveAs("Controls/NextItem")
        .Keys(Key.E)
        .Buttons(JoyButton.RightShoulder)
        .Build();

    public InputAction PrevItem { get; } = InputAction.Create("PrevItem")
        .SaveAs("Controls/PrevItem")
        .Keys(Key.Q)
        .Buttons(JoyButton.LeftShoulder)
        .Build();

    public InputAction Float { get; } = InputAction.Create("Float")
        .SaveAs("Controls/Float")
        .Keys(Key.F)
        .Buttons(JoyButton.Y)
        .Build();
}