using System;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Bus;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.Loader;
using Godot;
using Veronenger.Controller.Character;
using Veronenger.Controller.Menu;
using Veronenger.Controller.UI;
using Veronenger.Managers;

namespace Veronenger {
	public static class ApplicationConfig {
		public static readonly ScreenConfiguration Configuration = new(
			FixedViewportStrategy.Instance, 
			Resolutions.FULLHD_DIV2,
			Resolutions.FULLHD_DIV2,
			Window.ContentScaleModeEnum.CanvasItems, // (viewport is blur)
			Window.ContentScaleAspectEnum.Keep,
			Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9), 
			true,
			1f);
	}

	[Configuration]
	public class Settings {
		[Service] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Configuration);
		[Service] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));

		// [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
		[Service("Settings.Screen.PixelPerfect")]
		public ISetting<bool> PixelPerfect => Setting<bool>.Save("Video", "PixelPerfect", false);

		// [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
		[Service("Settings.Screen.Fullscreen")]
		public ISetting<bool> Fullscreen => Setting<bool>.Save("Video", "Fullscreen", true);

		// [Setting(Section = "Video", Name = "VSync", Default = false)]
		[Service("Settings.Screen.VSync")]
		public ISetting<bool> VSync => Setting<bool>.Save("Video", "VSync", false);

		// [Setting(Section = "Video", Name = "Borderless", Default = false)]
		[Service("Settings.Screen.Borderless")]
		public ISetting<bool> Borderless => Setting<bool>.Save("Video", "Borderless", false);

		// [Setting(Section = "Video", Name = "WindowedResolution")]
		[Service("Settings.Screen.WindowedResolution")]
		public ISetting<Resolution> WindowedResolution => Setting<Resolution>.Save("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
	}

	[Configuration]
	public class Resources {
		
		[Service] 
		public Texture2D Xbox360Buttons => (Texture2D)ResourceLoader.Load("res://Assets/UI/Consoles/Xbox 360 Controller Updated.png");

		[Service]
		public Texture2D XboxOneButtons => (Texture2D)ResourceLoader.Load("res://Assets/UI/Consoles/Xbox One Controller Updated.png");

		[Service]
		public Theme MyTheme => (Theme)ResourceLoader.Load("res://Assets/UI/my_theme.tres");

		[Service]
		public Theme DebugConsoleTheme => (Theme)ResourceLoader.Load("res://Assets/UI/DebugConsole.tres");
		
		[Service]
		[Lazy]
		public RedefineActionButton RedefineActionButton =>
			((PackedScene)ResourceLoader.Load("res://Scenes/UI/RedefineActionButton.tscn"))
			.Instantiate<RedefineActionButton>();

		[Service(Lifetime.Transient)]
		public Node World3 =>
			((PackedScene)ResourceLoader.Load("res://Worlds/World3.tscn"))
			.Instantiate<Node>();
        
		[Service(Lifetime.Transient)]
		public PlayerController Player =>
			((PackedScene)ResourceLoader.Load("res://cenes/Player.tscn"))
			.Instantiate<PlayerController>();

		[Service]
		public MainMenu MainMenuScene =>
			((PackedScene)ResourceLoader.Load("res://Scenes/Menu/MainMenu.tscn"))
			.Instantiate<MainMenu>();

		[Service]
		public BottomBar BottomBarScene =>
			((PackedScene)ResourceLoader.Load("res://Scenes/Menu/BottomBar.tscn"))
			.Instantiate<BottomBar>();

		[Service]
		public PauseMenu PauseMenuScene =>
			((PackedScene)ResourceLoader.Load("res://Scenes/Menu/PauseMenu.tscn"))
			.Instantiate<PauseMenu>();

		[Service]
		public SettingsMenu SettingsMenuScene =>
			((PackedScene)ResourceLoader.Load("res://Scenes/Menu/SettingsMenu.tscn"))
			.Instantiate<SettingsMenu>();

		[Service(Lifetime.Transient)]
		public ModalBoxConfirm ModalBoxConfirm =>
			((PackedScene)ResourceLoader.Load("res://Scenes/Menu/ModalBoxConfirm.tscn"))
			.Instantiate<ModalBoxConfirm>();
		
	}

	[Configuration]
	public class MouseActions {
		[Service]
		private InputAction LMB => InputAction.Create("LMB")
			.Click(MouseButton.Left)
			.Build();

		[Service]
		private InputAction RMB => InputAction.Create("RMB")
			.Click(MouseButton.Right)
			.Build();

		[Service]
		private InputAction MWU => InputAction.Create("MWU")
			.Click(MouseButton.WheelUp)
			.Build();

		[Service]
		private InputAction MWD => InputAction.Create("MWD")
			.Click(MouseButton.WheelDown)
			.Build();
	}

	[Configuration]
	public class UiActions {
		[Service] public InputActionsContainer InputActionsContainer => new();
		
		[Service]
		private InputAction UiUp => InputAction.Create("ui_up")
			.KeepProjectSettings()
			.NegativeAxis(JoyAxis.LeftY, "ui_down")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction UiDown => InputAction.Create("ui_down")
			.KeepProjectSettings()
			.PositiveAxis(JoyAxis.LeftY, "ui_up")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction UiLeft => InputAction.Create("ui_left")
			.KeepProjectSettings()
			.NegativeAxis(JoyAxis.LeftX, "ui_right")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction UiRight => InputAction.Create("ui_right")
			.KeepProjectSettings()
			.PositiveAxis(JoyAxis.LeftX, "ui_left")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction UiAccept => InputAction.Create("ui_accept")
			.KeepProjectSettings()
			.Buttons(JoyButton.A)
			.Build();

		[Service]
		private InputAction UiSelect => InputAction.Create("ui_select")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiFocusNext => InputAction.Create("ui_focus_next")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiFocusPrev => InputAction.Create("ui_focus_prev")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiPageUp => InputAction.Create("ui_page_up")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiPageDown => InputAction.Create("ui_page_down")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiHome => InputAction.Create("ui_home")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiEnd => InputAction.Create("ui_end")
			.KeepProjectSettings()
			.Build();

		[Service]
		private InputAction UiCancel => InputAction.Create("ui_cancel")
			.KeepProjectSettings()
			.Buttons(JoyButton.B)
			.Build();

		[Service]
		private InputAction ControllerSelect => InputAction.Create("select")
			.Keys(Key.Tab)
			.Buttons(JoyButton.Back)
			.Build();

		[Service]
		private InputAction ControllerStart => InputAction.Create("start")
			.Keys(Key.Escape)
			.Buttons(JoyButton.Start)
			.Build();
	}

	[Configuration]
	public class Actions {
		[Service]
		private InputAction Up => InputAction.Configurable("up")
			.Keys(Key.Up)
			.Buttons(JoyButton.DpadUp)
			.NegativeAxis(JoyAxis.LeftY, "down")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction Down => InputAction.Configurable("down")
			.Keys(Key.Down)
			.Buttons(JoyButton.DpadDown)
			.PositiveAxis(JoyAxis.LeftY, "up")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction Left => InputAction.Configurable("left")
			.Keys(Key.Left)
			.Buttons(JoyButton.DpadLeft)
			.NegativeAxis(JoyAxis.LeftX, "right")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction Right => InputAction.Configurable("right")
			.Keys(Key.Right)
			.Buttons(JoyButton.DpadRight)
			.PositiveAxis(JoyAxis.LeftX, "left")
			.DeadZone(0.5f)
			.Build();

		[Service]
		private InputAction Jump => InputAction.Configurable("Jump")
			.Keys(Key.Space)
			.Buttons(JoyButton.A)
			.Build();

		[Service]
		private InputAction Attack => InputAction.Configurable("Attack")
			.Keys(Key.C)
			.Click(MouseButton.Left)
			.Buttons(JoyButton.B)
			.Build();

		[Service]
		private InputAction Float => InputAction.Configurable("Float")
			.Keys(Key.F)
			.Buttons(JoyButton.Y)
			.Build();

		[Service]
		private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
			.Keys(Key.F9)
			.Build();

	}
}