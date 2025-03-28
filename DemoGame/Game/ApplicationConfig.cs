using System;
using Betauer.Application;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Screen;
using Betauer.Application.Screen.Resolution;
using Betauer.Application.Settings;
using Betauer.Camera.Control;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.Platform;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game;

[Configuration]
public class ApplicationConfig {
    public static readonly ScreenConfig Config = new(
        Resolutions.FULLHD,
        Window.ContentScaleModeEnum.CanvasItems, // (viewport is blur)
        Window.ContentScaleAspectEnum.Keep,
        Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9),
        true,
        1f);

    [Singleton(Flags = "Autoload")] public DebugOverlayManager DebugOverlayManager => new();

    [Singleton] public GameObjectRepository GameObjectRepository => new();
    [Singleton] public GameLoader GameLoader => new();
    [Singleton] public CameraContainer CameraContainer => new();
    [Singleton] public ScreenSettingsManager ScreenSettingsManager => new(Config);
    [Singleton] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));
}

[Singleton]
public class Settings : IInjectable {
    [Inject] public SettingsContainer SettingsContainer { get; set; }

    // public SaveSetting<bool> PixelPerfect { get; } = Setting.Create("Video/PixelPerfect", false, true);
    public SaveSetting<bool> Fullscreen { get; } = Setting.Create("Video/Fullscreen", true, true);
    public SaveSetting<bool> VSync { get; } = Setting.Create("Video/VSync", true, true);
    public SaveSetting<bool> Borderless { get; } = Setting.Create("Video/Borderless", false, true);
    public SaveSetting<Vector2I> WindowedResolution { get; } = Setting.Create("Video/WindowedResolution", Resolutions.FULLHD.Size, true);

    public void PostInject() {
        SettingsContainer.AddFromInstanceProperties(this);
        SettingsContainer.Load();
    }
}

[Configuration]
[Loader("GameLoader", Tag = GameLoaderTag)]
[Scene.Singleton<BottomBar>(Name = "BottomBarLazy", Flags = "Autoload")]
public class BottomBarResources {
    public const string GameLoaderTag = "bottom-bar";
}

[Configuration]
[Loader("GameLoader", Tag = GameLoaderTag)]
[Preload<Texture2D>("Icon", "res://icon.png")]
[Resource<Theme>("MyTheme", "res://Game/UI/my_theme2.tres")]
[Resource<Theme>("DebugConsoleTheme", "res://Game/UI/DebugConsole.tres")]
[Resource<Theme>("DebugOverlayTheme", "res://Game/UI/DebugOverlay.tres")]
[Resource<Texture2D>("Xbox360Buttons", "res://Game/UI/Console/Xbox 360 Controller Updated.png")]
[Resource<Texture2D>("XboxOneButtons", "res://Game/UI/Console/Xbox One Controller Updated.png")]
[Scene.Transient<RedefineActionButton>(Name = "RedefineActionButton")]
[Scene.Transient<ModalBoxConfirm>(Name = "ModalBoxConfirmFactory")]
[Scene.Singleton<MainMenu>(Name = "MainMenuSceneLazy", Flags = "Autoload")]
[Scene.Singleton<PauseMenu>(Name = "PauseMenuLazy", Flags = "Autoload")]
[Scene.Singleton<SettingsMenu>(Name = "SettingsMenuLazy", Flags = "Autoload")]
[Scene.Singleton<ProgressScreen>(Name = "ProgressScreenLazy", Flags = "Autoload")]
public class MainResources {
    public const string GameLoaderTag = "main";
}

[Singleton]
public class UiActions : SingleJoypadActionsContainer {
    [Inject] DebugOverlayManager DebugOverlayManager { get; set; }

    public UiActions() {
        AddActionsFromProperties(this);
        SetFirstConnectedJoypad();
        EnableAll();
        Start();

        var disabler = DebugOverlayAction.AddOnJustPressed((e) => DebugOverlayManager!.UserHitDebug(e));
        // disabler(); // This will remove the event
    }

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