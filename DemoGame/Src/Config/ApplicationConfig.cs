using System;
using System.Threading.Tasks;
using Betauer.Application;
using Betauer.Application.Lifecycle;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Transient;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Config; 

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

	[Singleton] public DebugOverlayManager DebugOverlayManager => new();
	[Singleton] public InputActionsContainer InputActionsContainer => new();
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
	[Singleton] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));
}

[Singleton(Name = "MyGameLoader")]
public class GameLoaderContainer : ResourceLoaderContainer {
	public Task<TimeSpan> LoadMainResources() => LoadResources("main");
	public Task<TimeSpan> LoadGameResources() => LoadResources("game");
	public void UnloadGameResources() => UnloadResources("game");
} 

[Configuration]
[Loader("MyGameLoader", Tag = "main")]
[Preload<Texture2D>("Icon", "res://icon.png", Lazy = true)]
[Resource<Theme>("MyTheme", "res://Assets/UI/my_theme2.tres")]
[Resource<Texture2D>("Xbox360Buttons", "res://Assets/UI/Consoles/Xbox 360 Controller Updated.png")]
[Resource<Texture2D>("XboxOneButtons", "res://Assets/UI/Consoles/Xbox One Controller Updated.png")]
[Resource<Theme>("DebugConsoleTheme", "res://Assets/UI/DebugConsole.tres")]
[Scene.Transient<RedefineActionButton>("RedefineActionButton", "res://Scenes/UI/RedefineActionButton.tscn")]
[Scene.Transient<ModalBoxConfirm>("ModalBoxConfirm", "res://Scenes/Menu/ModalBoxConfirm.tscn")]
[Scene.Singleton<HUD>("HudResource", "res://Scenes/UI/HUD.tscn")]
[Scene.Singleton<MainMenu>("MainMenuResource", "res://Scenes/Menu/MainMenu.tscn")]
[Scene.Singleton<BottomBar>("BottomBarResource", "res://Scenes/Menu/BottomBar.tscn")]
[Scene.Singleton<PauseMenu>("PauseMenuResource", "res://Scenes/Menu/PauseMenu.tscn")]
[Scene.Singleton<SettingsMenu>("SettingsMenuResource", "res://Scenes/Menu/SettingsMenu.tscn")]
public class MainResources {
}

[Configuration]
[Loader("MyGameLoader", Tag = "game")]
[Resource<Texture2D>("Pickups", "res://Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Characters/Player-Leon/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Characters/Player-Leon/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Characters/Player-Leon/Leon-gun1.png")]
[Scene.Transient<WorldScene>("World3", "res://Worlds/World3.tscn")]
[Scene.Transient<PlayerNode>("Player", "res://Scenes/Player.tscn")]
[Scene.Transient<ZombieNode>("ZombieNode", "res://Scenes/Zombie2.tscn")]
[Scene.Transient<ProjectileTrail>("ProjectileTrail", "res://Scenes/ProjectileTrail.tscn")]
[Scene.Transient<PickableItemNode>("PickableItem", "res://Scenes/PickableItem.tscn")]
public class GameResources {
}

[Configuration]
[PoolContainer("PoolContainer")]
public class PoolConfig {
	[NodePool<ZombieNode>] NodePool<ZombieNode> ZombiePool => new();
	[NodePool<ProjectileTrail>] NodePool<ProjectileTrail> ProjectilePool => new();
	[NodePool<PickableItemNode>] NodePool<PickableItemNode> PickableItemPool => new();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("InputActionsContainer")]
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
		.ExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Attack")]
	private InputAction Attack => InputAction.Create()
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Pausable()
		.ExtendedUnhandled();

	[InputAction(SaveAs = "Controls/NextItem")]
	private InputAction NextItem => InputAction.Create()
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[InputAction(SaveAs = "Controls/PrevItem")]
	private InputAction PrevItem => InputAction.Create()
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Float")]
	private InputAction Float => InputAction.Create()
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Pausable()
		.ExtendedUnhandled();
}

[Configuration]
[InputActionsContainer("InputActionsContainer")]
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
[InputActionsContainer("InputActionsContainer")]
public class OtherActions {
	[InputAction]
	private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.AsGodotInput();

}