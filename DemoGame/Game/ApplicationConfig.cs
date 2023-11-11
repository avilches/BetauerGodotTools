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
using Betauer.Input.Attributes;
using Betauer.Input.Joypad;
using Godot;
using Pcg;
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

	[Singleton] public Random Random => new PcgRandom();
	[Singleton] public DebugOverlayManager DebugOverlayManager => new();
	[Singleton] public GameObjectRepository GameObjectRepository => new();
	[Singleton] public UiActionsContainer UiActionsContainer => new();
	[Singleton] public GameLoader GameLoader => new();
	[Singleton] public CameraContainer CameraContainer => new();
	[Singleton] public JoypadPlayersMapping JoypadPlayersMapping => new();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[Setting<bool>("Settings.Screen.PixelPerfect", SaveAs = "Video/PixelPerfect", Default = false)]
[Setting<bool>("Settings.Screen.Fullscreen", SaveAs = "Video/Fullscreen", Default = true)]
[Setting<bool>("Settings.Screen.VSync", SaveAs = "Video/VSync", Default = true)]
[Setting<bool>("Settings.Screen.Borderless", SaveAs = "Video/Borderless", Default = false)]
[Setting<Resolution>("Settings.Screen.WindowedResolution", SaveAs = "Video/WindowedResolution", DefaultAsString = "1920x1080")]
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
[Scene.Transient<RedefineActionButton>("RedefineActionButton")]
[Scene.Transient<ModalBoxConfirm>("ModalBoxConfirmFactory")]
[Scene.Singleton<MainMenu>("MainMenuSceneLazy")]
[Scene.Singleton<BottomBar>("BottomBarLazy")]
[Scene.Singleton<PauseMenu>("PauseMenuLazy")]
[Scene.Singleton<SettingsMenu>("SettingsMenuLazy")]
[Scene.Singleton<ProgressScreen>("ProgressScreenLazy")]
public class MainResources {
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