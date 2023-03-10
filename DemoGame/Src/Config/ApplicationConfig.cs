using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.DI.Factory;
using Betauer.Input;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Transient;
using Veronenger.UI;
using static Betauer.Loader.LoadTools;
using static Godot.ResourceLoader;

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

	[Service] public DebugOverlayManager DebugOverlayManager => new();
	[Service] public InputActionsContainer InputActionsContainer => new();

}

[Configuration]
public class Settings {
	[Service] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Configuration);
	[Service] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));

	// [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
	[Service("Settings.Screen.PixelPerfect")]
	public ISetting<bool> PixelPerfect => Setting<bool>.Persistent("Video", "PixelPerfect", false);

	// [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
	[Service("Settings.Screen.Fullscreen")]
	public ISetting<bool> Fullscreen => Setting<bool>.Persistent("Video", "Fullscreen", true);

	// [Setting(Section = "Video", Name = "VSync", Default = false)]
	[Service("Settings.Screen.VSync")]
	public ISetting<bool> VSync => Setting<bool>.Persistent("Video", "VSync", false);

	// [Setting(Section = "Video", Name = "Borderless", Default = false)]
	[Service("Settings.Screen.Borderless")]
	public ISetting<bool> Borderless => Setting<bool>.Persistent("Video", "Borderless", false);

	// [Setting(Section = "Video", Name = "WindowedResolution")]
	[Service("Settings.Screen.WindowedResolution")]
	public ISetting<Resolution> WindowedResolution => Setting<Resolution>.Persistent("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
}

[Configuration]
public class Resources {
	[Service] Texture2D Icon => Load<Texture2D>("res://icon.png");
	[Service] Texture2D LeonKnifeAnimationSprite => Load<Texture2D>("res://Characters/Player-Leon/Leon-knife.png");
	[Service] Texture2D LeonMetalbarAnimationSprite => Load<Texture2D>("res://Characters/Player-Leon/Leon-metalbar.png");
	
	[Service] Texture2D Xbox360Buttons => Load<Texture2D>("res://Assets/UI/Consoles/Xbox 360 Controller Updated.png");
	[Service] Texture2D XboxOneButtons => Load<Texture2D>("res://Assets/UI/Consoles/Xbox One Controller Updated.png");
	[Service] Theme MyTheme => Load<Theme>("res://Assets/UI/my_theme.tres");
	[Service] Theme DebugConsoleTheme => Load<Theme>("res://Assets/UI/DebugConsole.tres");
}

[Configuration]
public class Scenes {
	[Factory] private IFactory<ZombieNode> ZombieNode => new SceneFactory<ZombieNode>("res://Scenes/Zombie2.tscn");
	[Factory] private IFactory<RedefineActionButton> RedefineActionButton => new SceneFactory<RedefineActionButton>("res://Scenes/UI/RedefineActionButton.tscn");
	[Factory] private IFactory<Node> World3 => new SceneFactory<Node>("res://Worlds/World3.tscn");
	[Factory] private IFactory<PlayerNode> Player => new SceneFactory<PlayerNode>("res://Scenes/Player.tscn");
	[Factory] private IFactory<ModalBoxConfirm> ModalBoxConfirm => new SceneFactory<ModalBoxConfirm>("res://Scenes/Menu/ModalBoxConfirm.tscn");
	[Factory] private IFactory<ProjectileTrail> ProjectileTrail => new SceneFactory<ProjectileTrail>("res://Scenes/ProjectileTrail.tscn");

	[Service] HUD HudScene => Instantiate<HUD>("res://Scenes/UI/HUD.tscn");
	[Service] MainMenu MainMenuScene => Instantiate<MainMenu>("res://Scenes/Menu/MainMenu.tscn");
	[Service] BottomBar BottomBarScene => Instantiate<BottomBar>("res://Scenes/Menu/BottomBar.tscn");
	[Service] PauseMenu PauseMenuScene => Instantiate<PauseMenu>("res://Scenes/Menu/PauseMenu.tscn");
	[Service] SettingsMenu SettingsMenuScene => Instantiate<SettingsMenu>("res://Scenes/Menu/SettingsMenu.tscn");
}

[Configuration]
public class UiActions {
	[Service]
	private InputAction UiUp => InputAction.Create("ui_up")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private InputAction UiDown => InputAction.Create("ui_down")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private AxisAction UiVertical => AxisAction.Create(nameof(UiVertical), nameof(UiUp), nameof(UiDown));

	[Service]
	private InputAction UiLeft => InputAction.Create("ui_left")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private InputAction UiRight => InputAction.Create("ui_right")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private AxisAction UiLateral => AxisAction.Create(nameof(UiLateral), nameof(UiLeft), nameof(UiRight));

	[Service]
	private InputAction UiAccept => InputAction.Create("ui_accept")
		.KeepProjectSettings()
		.Buttons(JoyButton.A)
		.AsGodotInput();

	[Service]
	private InputAction UiSelect => InputAction.Create("ui_select")
		.KeepProjectSettings()
		.AsGodotInput();

	[Service]
	private InputAction UiCancel => InputAction.Create("ui_cancel")
		.KeepProjectSettings()
		.Buttons(JoyButton.B)
		.AsGodotInput();

	[Service]
	private InputAction ControllerSelect => InputAction.Create("select")
		.Keys(Key.Tab)
		.Buttons(JoyButton.Back)
		.AsGodotInput();

	[Service]
	private InputAction ControllerStart => InputAction.Create("start")
		.Keys(Key.Escape)
		.Buttons(JoyButton.Start)
		.AsGodotInput();
}

[Configuration]
public class Actions {
	[Service]
	private InputAction Up => InputAction.Configurable(nameof(Up))
		.Keys(Key.Up)
		.Buttons(JoyButton.DpadUp)
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private InputAction Down => InputAction.Configurable(nameof(Down))
		.Keys(Key.Down)
		.Buttons(JoyButton.DpadDown)
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private InputAction Left => InputAction.Configurable(nameof(Left))
		.Keys(Key.Left)
		.Buttons(JoyButton.DpadLeft)
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private InputAction Right => InputAction.Configurable(nameof(Right))
		.Keys(Key.Right)
		.Buttons(JoyButton.DpadRight)
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Service]
	private AxisAction Lateral => AxisAction.Create(nameof(Lateral), nameof(Left), nameof(Right));

	[Service]
	private AxisAction Vertical => AxisAction.Create(nameof(Vertical), nameof(Down), nameof(Up));
	
	[Service]
	private InputAction Jump => InputAction.Configurable(nameof(Jump))
		.Keys(Key.Space)
		.Buttons(JoyButton.A)
		.Pausable()
		.ExtendedUnhandled();

	[Service]
	private InputAction Attack => InputAction.Configurable(nameof(Attack))
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Pausable()
		.ExtendedUnhandled();

	[Service]
	private InputAction NextItem => InputAction.Configurable(nameof(NextItem))
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[Service]
	private InputAction PrevItem => InputAction.Configurable(nameof(PrevItem))
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[Service]
	private InputAction Float => InputAction.Configurable(nameof(Float))
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Pausable()
		.ExtendedUnhandled();

	[Service]
	private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.AsGodotInput();

}