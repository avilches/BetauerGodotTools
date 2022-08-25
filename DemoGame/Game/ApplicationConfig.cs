using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Veronenger.Game {
    public static class ApplicationConfig {
        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Mode2d,
            SceneTree.StretchAspect.Keep,
            Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }

    [Configuration]
    public class Settings {
        [Service] public ScreenSettingsManager ScreenSettingsManager => new ScreenSettingsManager(ApplicationConfig.Configuration);
        [Service] public SettingsContainer SettingsContainer => new SettingsContainer(AppTools.GetUserFile("settings.ini"));

        // [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
        [Service("Settings.Screen.PixelPerfect")]
        public ISetting<bool> PixelPerfect => Setting<bool>.Save("Video", "PixelPerfect", false);

        // [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
        [Service("Settings.Screen.Fullscreen")]
        public ISetting<bool> Fullscreen =>  Setting<bool>.Save("Video", "Fullscreen", true);

        // [Setting(Section = "Video", Name = "VSync", Default = false)]
        [Service("Settings.Screen.VSync")]
        public ISetting<bool> VSync =>  Setting<bool>.Save("Video", "VSync", false);

        // [Setting(Section = "Video", Name = "Borderless", Default = false)]
        [Service("Settings.Screen.Borderless")]
        public ISetting<bool> Borderless =>  Setting<bool>.Save("Video", "Borderless", false);

        // [Setting(Section = "Video", Name = "WindowedResolution")]
        [Service("Settings.Screen.WindowedResolution")]
        public ISetting<Resolution> WindowedResolution =>
            Setting<Resolution>.Save("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
    }

    [Configuration]
    public class UiActions {
        [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();
        
        [Service]
        private InputAction UiUp => InputAction.Create("ui_up")
            .KeepProjectSettings()
            .NegativeAxis(1, "ui_down")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction UiDown => InputAction.Create("ui_down")
            .KeepProjectSettings()
            .PositiveAxis(1, "ui_up")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction UiLeft => InputAction.Create("ui_left")
            .KeepProjectSettings()
            .NegativeAxis(0, "ui_right")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction UiRight => InputAction.Create("ui_right")
            .KeepProjectSettings()
            .PositiveAxis(0, "ui_left")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction UiAccept => InputAction.Create("ui_accept").KeepProjectSettings().Build();

        [Service]
        private InputAction UiCancel => InputAction.Create("ui_cancel").KeepProjectSettings().Build();

        [Service]
        private InputAction UiSelect => InputAction.Create("select")
            .Keys(KeyList.Escape)
            .Buttons(JoystickList.XboxB)
            .Build();

        [Service]
        private InputAction UiStart => InputAction.Create("start")
            .Keys(KeyList.Escape)
            .Buttons(JoystickList.Start)
            .Build();
    }

    [Configuration]
    public class Actions {
        [Service]
        private InputAction Up => InputAction.Configurable("up")
            .Keys(KeyList.Up)
            .Buttons(JoystickList.DpadUp)
            .NegativeAxis(1, "down")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction Down => InputAction.Configurable("down")
            .Keys(KeyList.Down)
            .Buttons(JoystickList.DpadDown)
            .PositiveAxis(1, "up")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction Left => InputAction.Configurable("left")
            .Keys(KeyList.Left)
            .Buttons(JoystickList.DpadLeft)
            .NegativeAxis(0, "right")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction Right => InputAction.Configurable("right")
            .Keys(KeyList.Right)
            .Buttons(JoystickList.DpadRight)
            .PositiveAxis(0, "left")
            .DeadZone(0.5f)
            .Build();

        [Service]
        private InputAction Jump => InputAction.Configurable("Jump")
            .Keys(KeyList.Space)
            .Buttons(JoystickList.XboxA)
            .Build();

        [Service]
        private InputAction Attack => InputAction.Configurable("Attack")
            .Keys(KeyList.C)
            .Buttons(JoystickList.XboxX)
            .Build();

        [Service]
        private InputAction PixelPerfectInputAction => InputAction.Create("PixelPerfect")
            .Keys(KeyList.F9)
            .Build();

    }
}