using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace DemoAnimation {
    public static class ApplicationConfig {
        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV1_875,
            Resolutions.FULLHD_DIV1_875,
            Window.ContentScaleModeEnum.Viewport,
            Window.ContentScaleAspectEnum.Keep,
            Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }

    [Configuration]
    public class Settings {
        [Service]
        public ScreenSettingsManager ScreenSettingsManager =>
            new ScreenSettingsManager(ApplicationConfig.Configuration);
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
        private InputAction ControllerSelect => InputAction.Create("select")
            .Keys(Key.Tab)
            .Buttons(JoyButton.Select)
            .Build();

        [Service]
        private InputAction ControllerStart => InputAction.Create("start")
            .Keys(Key.Escape)
            .Buttons(JoyButton.Start)
            .Build();
        
        [Service]
        private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
            .Keys(Key.F9)
            .Build();

    }

}