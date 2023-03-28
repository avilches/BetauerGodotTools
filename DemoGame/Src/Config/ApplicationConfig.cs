using System;
using System.Threading.Tasks;
using Betauer.Application;
using Betauer.Application.Lifecycle;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Core.Pool;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Transient;
using Veronenger.UI;
using Veronenger.Worlds;
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

	[Singleton] public DebugOverlayManager DebugOverlayManager => new();
	[Singleton] public InputActionsContainer InputActionsContainer => new();

	// All 
	[Singleton] public PoolContainer<IPoolLifecycle> PoolContainer => new();
	[Singleton] public GameLoaderContainer ResourceLoaderContainer => new();

}

[Configuration]
public class Settings {
	[Singleton] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Configuration);
	[Singleton] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));

	// [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
	[Singleton("Settings.Screen.PixelPerfect")]
	public ISetting<bool> PixelPerfect => Setting<bool>.Persistent("Video", "PixelPerfect", false);

	// [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
	[Singleton("Settings.Screen.Fullscreen")]
	public ISetting<bool> Fullscreen => Setting<bool>.Persistent("Video", "Fullscreen", true);

	// [Setting(Section = "Video", Name = "VSync", Default = false)]
	[Singleton("Settings.Screen.VSync")]
	public ISetting<bool> VSync => Setting<bool>.Persistent("Video", "VSync", false);

	// [Setting(Section = "Video", Name = "Borderless", Default = false)]
	[Singleton("Settings.Screen.Borderless")]
	public ISetting<bool> Borderless => Setting<bool>.Persistent("Video", "Borderless", false);

	// [Setting(Section = "Video", Name = "WindowedResolution")]
	[Singleton("Settings.Screen.WindowedResolution")]
	public ISetting<Resolution> WindowedResolution => Setting<Resolution>.Persistent("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
}

public class GameLoaderContainer : ResourceLoaderContainer {
	public Task<TimeSpan> LoadMainResources() => LoadResources("main");
	public Task<TimeSpan> LoadGameResources() => LoadResources("game");
	public void UnloadGameResources() => UnloadResources("game");
} 

[Configuration]
public class Resources {
	[Singleton] Texture2D Icon => Load<Texture2D>("res://icon.png");
	
	[Resource<Texture2D>("game", "res://Assets/Weapons/metalbar.png")] IFactory<Texture2D> MetalbarSprite;
	[Resource<Texture2D>("game", "res://Assets/Weapons/slowgun.png")] IFactory<Texture2D> SlowGunSprite;
	[Resource<Texture2D>("game", "res://Characters/Player-Leon/Leon-knife.png")] IFactory<Texture2D> LeonKnifeAnimationSprite;
	[Resource<Texture2D>("game", "res://Characters/Player-Leon/Leon-metalbar.png")] IFactory<Texture2D> LeonMetalbarAnimationSprite;
	
	[Resource<Texture2D>("main", "res://Assets/UI/Consoles/Xbox 360 Controller Updated.png")] IFactory<Texture2D> Xbox360Buttons;
	[Resource<Texture2D>("main", "res://Assets/UI/Consoles/Xbox One Controller Updated.png")] IFactory<Texture2D> XboxOneButtons;
	[Resource<Theme>("main", "res://Assets/UI/my_theme.tres")] IFactory<Theme> MyTheme;
	[Resource<Theme>("main", "res://Assets/UI/DebugConsole.tres")] IFactory<Theme> DebugConsoleTheme;
}

[Configuration]
public class Scenes {
	[Factory.Transient] SceneFactory<RedefineActionButton> RedefineActionButton => new("main", "res://Scenes/UI/RedefineActionButton.tscn");
	[Factory.Transient] SceneFactory<ModalBoxConfirm> ModalBoxConfirm => new("main", "res://Scenes/Menu/ModalBoxConfirm.tscn");

	[Scene.Singleton<HUD>("main", "res://Scenes/UI/HUD.tscn")] IFactory<HUD> HudResource;
	[Scene.Singleton<MainMenu>("main", "res://Scenes/Menu/MainMenu.tscn")] IFactory<MainMenu> MainMenuResource;
	[Scene.Singleton<BottomBar>("main", "res://Scenes/Menu/BottomBar.tscn")] IFactory<BottomBar> BottomBarResource;
	[Scene.Singleton<PauseMenu>("main", "res://Scenes/Menu/PauseMenu.tscn")] IFactory<PauseMenu> PauseMenuResource;
	[Scene.Singleton<SettingsMenu>("main", "res://Scenes/Menu/SettingsMenu.tscn")] IFactory<SettingsMenu> SettingsMenuResource;
	[Scene.Transient<WorldScene>("game", "res://Worlds/World3.tscn")] IFactory<WorldScene> World3;
	[Scene.Transient<PlayerNode>("game", "res://Scenes/Player.tscn")] IFactory<PlayerNode> Player;
	[Scene.Transient<ZombieNode>("game", "res://Scenes/Zombie2.tscn")] IFactory<ZombieNode> ZombieNode;
	[Scene.Transient<ProjectileTrail>("game", "res://Scenes/ProjectileTrail.tscn")] IFactory<ProjectileTrail> ProjectileTrail;
	[Scene.Transient<PickableItemNode>("game", "res://Scenes/PickableItem.tscn")] IFactory<PickableItemNode> PickableItem;

	[Singleton] IPool<ZombieNode> ZombiePool => new PoolFromNodeFactory<ZombieNode>();
	[Singleton] IPool<ProjectileTrail> ProjectilePool => new PoolFromNodeFactory<ProjectileTrail>();
	[Singleton] IPool<PickableItemNode> PickableItemPool => new PoolFromNodeFactory<PickableItemNode>();
}

[Configuration]
public class UiActions {
	[Singleton]
	private InputAction UiUp => InputAction.Create("ui_up")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private InputAction UiDown => InputAction.Create("ui_down")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private AxisAction UiVertical => AxisAction.Create(nameof(UiVertical), nameof(UiUp), nameof(UiDown));

	[Singleton]
	private InputAction UiLeft => InputAction.Create("ui_left")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private InputAction UiRight => InputAction.Create("ui_right")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private AxisAction UiLateral => AxisAction.Create(nameof(UiLateral), nameof(UiLeft), nameof(UiRight));

	[Singleton]
	private InputAction UiAccept => InputAction.Create("ui_accept")
		.KeepProjectSettings()
		.Buttons(JoyButton.A)
		.AsGodotInput();

	[Singleton]
	private InputAction UiSelect => InputAction.Create("ui_select")
		.KeepProjectSettings()
		.AsGodotInput();

	[Singleton]
	private InputAction UiCancel => InputAction.Create("ui_cancel")
		.KeepProjectSettings()
		.Buttons(JoyButton.B)
		.AsGodotInput();

	[Singleton]
	private InputAction ControllerSelect => InputAction.Create("select")
		.Keys(Key.Tab)
		.Buttons(JoyButton.Back)
		.AsGodotInput();

	[Singleton]
	private InputAction ControllerStart => InputAction.Create("start")
		.Keys(Key.Escape)
		.Buttons(JoyButton.Start)
		.AsGodotInput();
}

[Configuration]
public class Actions {
	[Singleton]
	private InputAction Up => InputAction.Configurable(nameof(Up))
		.Keys(Key.Up)
		.Buttons(JoyButton.DpadUp)
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private InputAction Down => InputAction.Configurable(nameof(Down))
		.Keys(Key.Down)
		.Buttons(JoyButton.DpadDown)
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private InputAction Left => InputAction.Configurable(nameof(Left))
		.Keys(Key.Left)
		.Buttons(JoyButton.DpadLeft)
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private InputAction Right => InputAction.Configurable(nameof(Right))
		.Keys(Key.Right)
		.Buttons(JoyButton.DpadRight)
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[Singleton]
	private AxisAction Lateral => AxisAction.Create(nameof(Lateral), nameof(Left), nameof(Right));

	[Singleton]
	private AxisAction Vertical => AxisAction.Create(nameof(Vertical), nameof(Down), nameof(Up));
	
	[Singleton]
	private InputAction Jump => InputAction.Configurable(nameof(Jump))
		.Keys(Key.Space)
		.Buttons(JoyButton.A)
		.Pausable()
		.ExtendedUnhandled();

	[Singleton]
	private InputAction Attack => InputAction.Configurable(nameof(Attack))
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Pausable()
		.ExtendedUnhandled();

	[Singleton]
	private InputAction NextItem => InputAction.Configurable(nameof(NextItem))
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[Singleton]
	private InputAction PrevItem => InputAction.Configurable(nameof(PrevItem))
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Pausable()
		.ExtendedUnhandled();

	[Singleton]
	private InputAction Float => InputAction.Configurable(nameof(Float))
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Pausable()
		.ExtendedUnhandled();

	[Singleton]
	private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.AsGodotInput();

}