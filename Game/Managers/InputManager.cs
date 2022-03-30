using Betauer;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class InputManager {
        public readonly LateralAction LateralMotion;
        public readonly LateralAction UiLateralMotion;
        public readonly VerticalAction UiVerticalMotion;
        public readonly VerticalAction VerticalMotion;
        public readonly ActionState Jump;
        public readonly ActionState Attack;
        public readonly ActionState UiAccept;
        public readonly ActionState UiSelect;
        public readonly ActionState UiStart;
        public readonly ActionState UiCancel;
        public readonly ActionState PixelPerfect;

        private readonly ActionList _actionList;

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(InputManager));

        // TODO: subscribe to signal with the mapping preferences on load or on change
        public InputManager() {
            _actionList = new ActionList(-1);

            // Game actions
            LateralMotion = _actionList.AddLateralAction("left", "right");
            VerticalMotion = _actionList.AddVerticalAction("up", "down");
            Jump = _actionList.AddAction("Jump");
            Attack = _actionList.AddAction("Attack");

            PixelPerfect  = _actionList.AddAction("PixelPerfect");

            // UI actions
            UiLateralMotion = _actionList.AddLateralAction("ui_left", "ui_right");
            UiVerticalMotion = _actionList.AddVerticalAction("ui_up", "ui_down");
            UiAccept = _actionList.AddAction("ui_accept");
            UiCancel = _actionList.AddAction("ui_cancel");
            UiSelect = _actionList.AddAction("ui_select");
            UiStart = _actionList.AddAction("ui_start");
            Configure();
        }

        public void Configure() {

            // Game actions
            LateralMotion
                .ClearConfig()
                .SetDeadZone(0.5f)
                .ConfigureAxis(JoystickList.Axis0)
                .AddLateralCursorKeys()
                .AddLateralDPadButtons()
                .Build();

            VerticalMotion
                .ClearConfig()
                .SetDeadZone(0.5f)
                .ConfigureAxis(JoystickList.Axis1)
                .AddVerticalCursorKeys()
                .AddVerticalDPadButtons()
                .Build();

            Jump.ClearConfig()
                .AddKey(KeyList.Space)
                .AddButton(JoystickList.XboxA)
                .Build();

            Attack.ClearConfig()
                .AddKey(KeyList.C)
                .AddButton(JoystickList.XboxX)
                .Build();

            PixelPerfect.ClearConfig()
                .AddKey(KeyList.F9)
                .Build();

            // UI actions
            UiAccept.ClearConfig()
                .AddKey(KeyList.Space)
                .AddKey(KeyList.Enter)
                .AddButton(JoystickList.XboxA)
                .Build();

            UiCancel.ClearConfig()
                .AddKey(KeyList.Escape)
                .AddButton(JoystickList.XboxB)
                .Build();

            UiStart.ClearConfig()
                .AddKey(KeyList.Escape)
                .AddButton(JoystickList.Start)
                .Build();

            UiLateralMotion.ImportFrom(LateralMotion).Build();
            UiVerticalMotion.ImportFrom(VerticalMotion).Build();
        }


        public void Debug(BaseAction baseAction) {
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                // if (_wrapper.IsMotion()) {
                // Logger.Debug($"Axis {_wrapper.Device}[{_wrapper.Axis}]:{_wrapper.GetStrength()} ({_wrapper.AxisValue}) ({action?.Name})");
                // } else if (_wrapper.IsAnyButton()) {
                // Logger.Debug($"Button {_wrapper.Device}[{_wrapper.Button}]:{_wrapper.Pressed} ({_wrapper.Pressure}) ({action?.Name})");
                // } else if (_wrapper.IsAnyKey()) {
                // Logger.Debug($"Key \"{_wrapper.KeyString}\" #{_wrapper.Key} Pressed:{_wrapper.Pressed}/Echo:{_wrapper.Echo} ({action?.Name})");
                // }
                /*
                 * Aqui se comprueba que el JustPressed, Pressed y JustReleased del SALTO SOLO de InputManager coinciden
                 * con las del singleton Input de Godot. Se genera un texto con los 3 resultados y si no coinciden se pinta
                 */
                /*
                 var mine = InputManager.Jump.JustPressed + " " + InputManager.Jump.JustReleased + " " +
                            InputManager.Jump.Pressed;
                 var godot = Input.IsActionJustPressed("ui_select") + " " + Input.IsActionJustReleased("ui_select") +
                             " " +
                             Input.IsActionPressed("ui_select");
                 if (!mine.Equals(godot)) {
                     _logger.Debug("INPUT MISMATCH: Mine : " + mine);
                     _logger.Debug("INPUT MISTMATCH Godot: " + godot);
                 }
                 */
            }
        }
    }
}