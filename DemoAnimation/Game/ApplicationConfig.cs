using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace DemoAnimation.Game {
    public static class ApplicationConfig {
        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV1_875,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }

    [Configuration]
    public class Settings {
        [Service]
        public ScreenSettingsManager ScreenSettingsManager =>
            new ScreenSettingsManager(ApplicationConfig.Configuration);
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
        private VerticalAction UiVerticalMotion => new VerticalAction("UI Vertical", "ui_up", "ui_down")
            .SetDeadZone(0.5f)
            .SetAxis(JoystickList.Axis1)
            .AddVerticalCursorKeys()
            .AddVerticalDPadButtons();


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