using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.Camera;
using Betauer.Camera.Follower;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Betauer.Input.Joypad;
using Godot;
using Pcg;
using Veronenger.Game.Character.Npc;
using Veronenger.Game.Character.Player;
using Veronenger.Game.HUD;
using Veronenger.Game.Items;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;
using Veronenger.Game.Worlds;

namespace Veronenger.Game.Config; 

[Configuration]
public class ApplicationConfig {
	public static readonly ScreenConfiguration Configuration = new(
		FixedViewportStrategy.Instance, 
		Resolutions.FULLHD_DIV2,
		Resolutions.FULLHD_DIV2,
		Window.ContentScaleModeEnum.CanvasItems, // (viewport is blur)
		Window.ContentScaleAspectEnum.Keep,
		Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9), 
		true,
		1f);

	[Singleton] public JsonGameLoader<MySaveGameMetadata> GameObjectLoader() {
		var loader = new JsonGameLoader<MySaveGameMetadata>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		return loader;
	}
	[Singleton] public Random Random => new PcgRandom();
	[Singleton] public DebugOverlayManager DebugOverlayManager => new();
	[Singleton] public GameObjectRepository GameObjectRepository => new();
	[Singleton] public UiActionsContainer UiActionsContainer => new();
	[Singleton] public InputActionsContainer PlayerActionsContainer => new();
	[Singleton] public JoypadPlayersMapping JoypadPlayersMapping => new();
	[Singleton] public GameLoader GameLoader => new();
	[Transient] public StageController StageControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Transient] public StageCameraController StageCameraControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Singleton] public CameraContainer CameraContainer => new();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[Setting<bool>("Settings.Screen.PixelPerfect", SaveAs = "Video/PixelPerfect", Default = false)]
[Setting<bool>("Settings.Screen.Fullscreen", SaveAs = "Video/Fullscreen", Default = true)]
[Setting<bool>("Settings.Screen.VSync", SaveAs = "Video/VSync", Default = true)]
[Setting<bool>("Settings.Screen.Borderless", SaveAs = "Video/Borderless", Default = false)]
[Setting<Resolution>("Settings.Screen.WindowedResolution", SaveAs = "Video/WindowedResolution", DefaultAsString = "1920x1080")]
public class Settings {
	[Singleton] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Configuration);
	[Singleton] public SettingsContainer SettingsContainer => new(new ConfigFileWrapper(AppTools.GetUserFile("settings.ini")));
}

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Preload<Texture2D>("Icon", "res://icon.png")]
[Resource<Theme>("MyTheme", "res://Game/UI/my_theme2.tres")]
[Resource<Theme>("DebugConsoleTheme", "res://Game/UI/DebugConsole.tres")]
[Resource<Texture2D>("Xbox360Buttons", "res://Game/UI/Console/Xbox 360 Controller Updated.png")]
[Resource<Texture2D>("XboxOneButtons", "res://Game/UI/Console/Xbox One Controller Updated.png")]
[Scene.Transient<RedefineActionButton>("RedefineActionButton")]
[Scene.Transient<ModalBoxConfirm>("ModalBoxConfirm")]
[Scene.Transient<PlayerHud>("PlayerHudFactory")]
[Scene.Transient<GameView>("GameSceneFactory")]
[Scene.Singleton<MainMenu>("MainMenuSceneFactory")]
[Scene.Singleton<BottomBar>("BottomBarSceneFactory")]
[Scene.Singleton<PauseMenu>("PauseMenuSceneFactory")]
[Scene.Singleton<SettingsMenu>("SettingsMenuSceneFactory")]
[Scene.Singleton<ProgressScreen>("ProgressScreenFactory")]
public class MainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "game")]
[Resource<Texture2D>("Pickups", "res://Game/Items/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Game/Items/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Game/Character/Player/Player-Leon/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Game/Character/Player/Player-Leon/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Game/Character/Player/Player-Leon/Leon-gun1.png")]
[Scene.Transient<InventorySlot>("InventorySlotResource")]
[Scene.Transient<WorldPlatform>("WorldPlatformFactory")]
[Scene.Transient<PlayerNode>("PlayerNode")]
[Scene.Transient<ZombieNode>("ZombieNode")]
[Scene.Transient<ProjectileTrail>("ProjectileTrail")]
[Scene.Transient<PickableItemNode>("PickableItem")]
public class GameResources {
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

[Configuration]
[InputActionsContainer("UiActionsContainer")]
public class UiActions {
	[AxisAction] 
	private AxisAction UiVertical => AxisAction.Create().Build();

	[AxisAction] 
	private AxisAction UiLateral => AxisAction.Create().Build();

	[InputAction(AxisName = "UiVertical")]
	private InputAction UiUp => InputAction.Create("ui_up")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiVertical")]
	private InputAction UiDown => InputAction.Create("ui_down")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiLateral")]
	private InputAction UiLeft => InputAction.Create("ui_left")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiLateral")]
	private InputAction UiRight => InputAction.Create("ui_right")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction]
	private InputAction UiAccept => InputAction.Create("ui_accept")
		.KeepProjectSettings()
		.Buttons(JoyButton.A)
		.AsGodotInput();

	[InputAction]
	private InputAction UiSelect => InputAction.Create("ui_select")
		.KeepProjectSettings()
		.AsGodotInput();

	[InputAction]
	private InputAction UiCancel => InputAction.Create("ui_cancel")
		.KeepProjectSettings()
		.Buttons(JoyButton.B)
		.AsGodotInput();

	[InputAction]
	private InputAction ControllerSelect => InputAction.Create("select")
		.Keys(Key.Tab)
		.Buttons(JoyButton.Back)
		.AsGodotInput();

	[InputAction]
	private InputAction ControllerStart => InputAction.Create("start")
		.Keys(Key.Escape)
		.Buttons(JoyButton.Start)
		.AsGodotInput();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("UiActionsContainer")]
public class OtherActions {
	[InputAction]
	private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.AsGodotInput();

}