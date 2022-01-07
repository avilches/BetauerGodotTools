using Betauer;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class InputManager {
        public readonly DirectionInput LateralMotion;
        public readonly DirectionInput VerticalMotion;
        public readonly ActionState Jump;
        public readonly ActionState Attack;

        private readonly ActionList _actionList;

        private static Logger _logger = LoggerFactory.GetLogger(typeof(InputManager));

        public InputManager() {
            _actionList = new ActionList(-1);
            LateralMotion = _actionList.AddDirectionalMotion("Lateral");
            VerticalMotion = _actionList.AddDirectionalMotion("Vertical");

            Jump = _actionList.AddAction("Jump");
            Attack = _actionList.AddAction("Attack");
        }


        public void ConfigureMapping() {
            // TODO: subscribe to signal with the mapping preferences on load or on change
            LateralMotion
                .ClearConfig()
                .ConfigureAxis(JoystickList.Axis0)
                .AddLateralCursorKeys()
                .AddVerticalDPadButtons();

            VerticalMotion
                .ClearConfig()
                .ConfigureAxis(JoystickList.Axis1)
                .AddVerticalCursorKeys()
                .AddVerticalDPadButtons();

            Jump.ClearConfig()
                .AddKey(KeyList.Space)
                .AddButton(JoystickList.XboxA);

            Attack.ClearConfig()
                .AddKey(KeyList.C)
                .AddButton(JoystickList.XboxX);

            LateralMotion.AxisDeadZone = 0.5f;
            VerticalMotion.AxisDeadZone = 0.5f;
        }

        private readonly EventWrapper _wrapper = new EventWrapper(null);

        public IActionUpdate OnEvent(InputEvent @event) {
            _wrapper.Event = @event;
            return _actionList.Update(_wrapper);
        }

        public void ClearJustStates() {
            _actionList.ClearJustStates();
        }

        public void Debug(IActionUpdate action) {
            if (_logger.IsEnabled(TraceLevel.Debug)) {
                if (_wrapper.IsMotion()) {
                    _logger.Debug($"Axis {_wrapper.Device}[{_wrapper.Axis}]:{_wrapper.GetStrength()} ({_wrapper.AxisValue}) ({action?.Name})");
                } else if (_wrapper.IsAnyButton()) {
                    _logger.Debug($"Button {_wrapper.Device}[{_wrapper.Button}]:{_wrapper.Pressed} ({_wrapper.Pressure}) ({action?.Name})");
                } else if (_wrapper.IsAnyKey()) {
                    _logger.Debug($"Key \"{_wrapper.KeyString}\" #{_wrapper.Key} Pressed:{_wrapper.Pressed}/Echo:{_wrapper.Echo} ({action?.Name})");
                }
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