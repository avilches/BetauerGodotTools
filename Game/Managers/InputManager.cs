using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class InputManager {
        private readonly ActionList _actionList;

        private readonly LateralAction _lateralMotion;
        private readonly VerticalAction _verticalMotion;
        private readonly ActionState _jump;
        private readonly ActionState _attack;
        private readonly ActionState _uiLeft;
        private readonly ActionState _uiRight;
        private readonly ActionState _uiAccept;
        private readonly ActionState _uiSelect;
        private readonly ActionState _uiStart;
        private readonly ActionState _uiCancel;
        private readonly ActionState _pixelPerfect;
        
        private readonly LateralAction UiLateralMotion;
        private readonly VerticalAction UiVerticalMotion;
        [Singleton] private LateralAction LateralMotion => _lateralMotion;
        [Singleton] private VerticalAction VerticalMotion => _verticalMotion;
        [Singleton] private ActionState Jump => _jump;
        [Singleton] private ActionState Attack => _attack;
        [Singleton] private ActionState UiLeft => _uiLeft;
        [Singleton] private ActionState UiRight => _uiRight;
        [Singleton] private ActionState UiAccept => _uiAccept;
        [Singleton] private ActionState UiSelect => _uiSelect;
        [Singleton] private ActionState UiStart => _uiStart;
        [Singleton] private ActionState UiCancel => _uiCancel;
        [Singleton] private ActionState PixelPerfect => _pixelPerfect;


        public readonly List<ActionState> ConfigurableActionList = new List<ActionState>();

        public InputManager() {
            _actionList = new ActionList(-1);

            // Game actions
            _lateralMotion = _actionList.AddLateralAction("left", "right");
            _verticalMotion = _actionList.AddVerticalAction("up", "down");
            _jump = _actionList.AddConfigurableAction("Jump");
            _attack = _actionList.AddConfigurableAction("Attack");

            _pixelPerfect = _actionList.AddAction("PixelPerfect");

            // UI actions
            UiLateralMotion = _actionList.AddLateralAction("ui_left", "ui_right");
            UiVerticalMotion = _actionList.AddVerticalAction("ui_up", "ui_down");
            
            _uiLeft = _actionList.AddAction("ui_left");
            _uiRight = _actionList.AddAction("ui_right");
            _uiAccept = _actionList.AddAction("ui_accept");
            _uiCancel = _actionList.AddAction("ui_cancel");
            _uiSelect = _actionList.AddAction("ui_select");
            _uiStart = _actionList.AddAction("ui_start");
            
            ConfigurableActionList.Add(Jump);
            ConfigurableActionList.Add(Attack);

            Configure();
            
        }

        public void RedefineButton(ActionState actionState, int button) {
            actionState.AddButton((JoystickList)button);
        }

        public void RedefineKey(ActionState actionState, int key) {
            actionState.AddKey((KeyList)key);
        }

        public void Configure() {
            // Game actions
            LateralMotion
                .SetDeadZone(0.5f)
                .ConfigureAxis(JoystickList.Axis0)
                .AddLateralCursorKeys()
                .AddLateralDPadButtons()
                .Build();

            UiLeft
                .AddKey(KeyList.Left)
                .AddButton(JoystickList.DpadLeft)
                .Build();

            UiRight
                .AddKey(KeyList.Right)
                .AddButton(JoystickList.DpadRight)
                .Build();

            VerticalMotion
                .SetDeadZone(0.5f)
                .ConfigureAxis(JoystickList.Axis1)
                .AddVerticalCursorKeys()
                .AddVerticalDPadButtons()
                .Build();

            Jump
                .AddKey(KeyList.Space)
                .AddButton(JoystickList.XboxA)
                .Build();

            Attack
                .AddKey(KeyList.C)
                .AddButton(JoystickList.XboxX)
                .Build();

            PixelPerfect
                .AddKey(KeyList.F9)
                .Build();

            // UI actions
            UiAccept
                .AddKey(KeyList.Space)
                .AddKey(KeyList.Enter)
                .AddButton(JoystickList.XboxA)
                .Build();

            UiCancel
                .AddKey(KeyList.Escape)
                .AddButton(JoystickList.XboxB)
                .Build();

            UiStart
                .AddKey(KeyList.Escape)
                .AddButton(JoystickList.Start)
                .Build();

            UiLateralMotion.Copy(LateralMotion).Build();
            UiVerticalMotion.Copy(VerticalMotion).Build();
        }

        public ActionState? FindActionState(string actionName) {
            return _actionList.FindActionState(actionName);
        }
    }
}