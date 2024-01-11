using System;
using Betauer.Application;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Screen;
using Betauer.Application.Screen.Resolution;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.Camera.Control;
using Betauer.DI.Attributes;
using Betauer.Input;
using Godot;
using Pcg;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game; 

[Configuration]
public class ApplicationConfig {
	public static readonly ScreenConfig Config = new(
		FixedViewportStrategy.Instance, 
		Resolutions.FULLHD,
		Window.ContentScaleModeEnum.CanvasItems, // (viewport is blur)
		Window.ContentScaleAspectEnum.Keep,
		Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9), 
		true,
		1f);

	[Singleton(Flags = "Autoload")] public DebugOverlayManager DebugOverlayManager => new();

	[Singleton] public Random Random => new PcgRandom();
	[Singleton] public GameObjectRepository GameObjectRepository => new();
	[Singleton] public GameLoader GameLoader => new();
	[Singleton] public CameraContainer CameraContainer => new();
	[Singleton] public PlatformMultiPlayerContainer MultiPlayerContainer => new();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[Setting<bool>("Settings.Screen.PixelPerfect", SaveAs = "Video/PixelPerfect", Default = false)]
[Setting<bool>("Settings.Screen.Fullscreen", SaveAs = "Video/Fullscreen", Default = true)]
[Setting<bool>("Settings.Screen.VSync", SaveAs = "Video/VSync", Default = true)]
[Setting<bool>("Settings.Screen.Borderless", SaveAs = "Video/Borderless", Default = false)]
[Setting<Vector2I>("Settings.Screen.WindowedResolution", SaveAs = "Video/WindowedResolution")]
public class Settings {
	[Singleton] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Config);
	[Singleton] public SettingsContainer SettingsContainer => new(new ConfigFileWrapper(AppTools.GetUserFile("settings.ini")));
}

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Preload<Texture2D>("Icon", "res://icon.png")]
[Resource<Theme>("MyTheme", "res://Game/UI/my_theme2.tres")]
[Resource<Theme>("DebugConsoleTheme", "res://Game/UI/DebugConsole.tres")]
[Resource<Theme>("DebugOverlayTheme", "res://Game/UI/DebugOverlay.tres")]
[Resource<Texture2D>("Xbox360Buttons", "res://Game/UI/Console/Xbox 360 Controller Updated.png")]
[Resource<Texture2D>("XboxOneButtons", "res://Game/UI/Console/Xbox One Controller Updated.png")]
[Scene.Transient<RedefineActionButton>(Name = "RedefineActionButton")]
[Scene.Transient<ModalBoxConfirm>(Name = "ModalBoxConfirmFactory")]
[Scene.Singleton<MainMenu>(Name = "MainMenuSceneLazy", Flags = "Autoload")]
[Scene.Singleton<BottomBar>(Name = "BottomBarLazy", Flags = "Autoload")]
[Scene.Singleton<PauseMenu>(Name = "PauseMenuLazy", Flags = "Autoload")]
[Scene.Singleton<SettingsMenu>(Name = "SettingsMenuLazy", Flags = "Autoload")]
[Scene.Singleton<ProgressScreen>(Name = "ProgressScreenLazy", Flags = "Autoload")]
public class MainResources {
	Lazy<MainMenu> a = new(() => new MainMenu());
}

[Singleton]
public class UiActions : UiActionsContainer {
	public AxisAction UiVertical { get; } = new AxisAction("UiVertical");
	public AxisAction UiLateral { get; } = new AxisAction("UiLateral");

	public InputAction UiUp { get; } = InputAction.Create("ui_up")
		.AxisName("UiVertical")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.Build();

	public InputAction UiDown { get; } = InputAction.Create("ui_down")
		.AxisName("UiVertical")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.Build();

	public InputAction UiLeft { get; } = InputAction.Create("ui_left")
		.AxisName("UiLateral")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.Build();

	public InputAction UiRight { get; } = InputAction.Create("ui_right")
		.AxisName("UiLateral")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.Build();

	public InputAction UiAccept { get; } = InputAction.Create("ui_accept")
		.KeepProjectSettings()
		.Buttons(JoyButton.A)
		.Build();

	public InputAction UiSelect { get; } = InputAction.Create("ui_select")
		.KeepProjectSettings()
		.Build();

	public InputAction UiCancel { get; } = InputAction.Create("ui_cancel")
		.KeepProjectSettings()
		.Buttons(JoyButton.B)
		.Build();

	public InputAction ControllerSelect { get; } = InputAction.Create("select")
		.Keys(Key.Tab)
		.Buttons(JoyButton.Back)
		.Build();

	public InputAction ControllerStart { get; } = InputAction.Create("start")
		.Keys(Key.Escape)
		.Buttons(JoyButton.Start)
		.Build();

	public InputAction DebugOverlayAction { get; } = InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.Build();

}