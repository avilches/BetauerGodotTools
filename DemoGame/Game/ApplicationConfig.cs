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
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }

    [Configuration]
    public class Settings {
        [Service] public ScreenSettingsManager ScreenSettingsManager => new ScreenSettingsManager();
        [Service] public SettingsContainer SettingsContainer => new SettingsContainer();

        // [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
        [Service("ScreenSettings.PixelPerfect")]
        public Setting<bool> PixelPerfect => new Setting<bool>("Video", "PixelPerfect", false);

        // [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
        [Service("ScreenSettings.Fullscreen")]
        public Setting<bool> Fullscreen => new Setting<bool>("Video", "Fullscreen", true);

        // [Setting(Section = "Video", Name = "VSync", Default = false)]
        [Service("ScreenSettings.VSync")] public Setting<bool> VSync => new Setting<bool>("Video", "VSync", false);

        // [Setting(Section = "Video", Name = "Borderless", Default = false)]
        [Service("ScreenSettings.Borderless")]
        public Setting<bool> Borderless => new Setting<bool>("Video", "Borderless", false);

        // [Setting(Section = "Video", Name = "WindowedResolution")]
        [Service("ScreenSettings.WindowedResolution")]
        public Setting<Resolution> WindowedResolution =>
            new Setting<Resolution>("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
    }

    [Configuration]
    public class Actions {
        [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();

        // Game actions
        [Service]
        private LateralAction LateralMotion => new LateralAction("Game Lateral", "left", "right")
            .SetDeadZone(0.5f)
            .SetAxis(JoystickList.Axis0)
            .AddLateralCursorKeys()
            .AddLateralDPadButtons();

        [Service]
        private LateralAction UiLateralMotion => new LateralAction("UI Lateral", "ui_left", "ui_right")
            .SetDeadZone(0.5f)
            .SetAxis(JoystickList.Axis0)
            .AddLateralCursorKeys()
            .AddLateralDPadButtons();

        [Service]
        private InputAction UiLeft => InputAction.Create("left")
            .Keys(KeyList.Left)
            .Buttons(JoystickList.DpadLeft).Build();

        [Service]
        private InputAction UiRight => InputAction.Create("right")
            .Keys(KeyList.Right)
            .Buttons(JoystickList.DpadRight)
            .Build();

        [Service]
        private VerticalAction VerticalMotion => new VerticalAction("Game Vertical", "up", "down")
            .SetDeadZone(0.5f)
            .SetAxis(JoystickList.Axis1)
            .AddVerticalCursorKeys()
            .AddVerticalDPadButtons();

        [Service]
        private VerticalAction UiVerticalMotion => new VerticalAction("UI Vertical", "ui_up", "ui_down")
            .SetDeadZone(0.5f)
            .SetAxis(JoystickList.Axis1)
            .AddVerticalCursorKeys()
            .AddVerticalDPadButtons();

        [Service]
        private InputAction Jump => InputAction.Create("Jump")
            .Keys(KeyList.Space)
            .Buttons(JoystickList.XboxA)
            .Configurable()
            .Build();

        [Service]
        private InputAction Attack => InputAction.Create("Attack")
            .Keys(KeyList.C)
            .Buttons(JoystickList.XboxX)
            .Configurable()
            .Build();

        [Service]
        private InputAction PixelPerfectInputAction => InputAction.Create("PixelPerfect")
            .Keys(KeyList.F9)
            .Build();

        // UI actions
        [Service]
        private InputAction UiAccept => InputAction.Create("ui_accept")
            .Keys(KeyList.Space)
            .Keys(KeyList.Enter)
            .Buttons(JoystickList.XboxA)
            .Build();

        [Service]
        private InputAction UiCancel => InputAction.Create("ui_cancel")
            .Keys(KeyList.Escape)
            .Buttons(JoystickList.XboxB)
            .Build();

        [Service]
        private InputAction UiSelect => InputAction.Create("ui_select")
            .Keys(KeyList.Escape)
            .Buttons(JoystickList.XboxB)
            .Build();

        [Service]
        private InputAction UiStart => InputAction.Create("ui_start")
            .Keys(KeyList.Escape)
            .Buttons(JoystickList.Start)
            .Build();
    }
}