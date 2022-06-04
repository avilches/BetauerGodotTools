using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly ActionState UiLeft;
        public readonly ActionState UiRight;
        public readonly ActionState UiAccept;
        public readonly ActionState UiSelect;
        public readonly ActionState UiStart;
        public readonly ActionState UiCancel;
        
        public readonly ActionState PixelPerfect;

        private readonly ActionList _actionList;
        public readonly List<ActionState> ConfigurableActionList;

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(InputManager));

        // TODO: subscribe to signal with the mapping preferences on load or on change
        public InputManager() {
            _actionList = new ActionList(-1);

            // Game actions
            LateralMotion = _actionList.AddLateralAction("left", "right");
            VerticalMotion = _actionList.AddVerticalAction("up", "down");
            Jump = _actionList.AddAction("Jump");
            Attack = _actionList.AddAction("Attack");

            PixelPerfect = _actionList.AddAction("PixelPerfect");

            // UI actions
            UiLateralMotion = _actionList.AddLateralAction("ui_left", "ui_right");
            UiVerticalMotion = _actionList.AddVerticalAction("ui_up", "ui_down");
            
            UiLeft = _actionList.AddAction("ui_left");
            UiRight = _actionList.AddAction("ui_right");
            UiAccept = _actionList.AddAction("ui_accept");
            UiCancel = _actionList.AddAction("ui_cancel");
            UiSelect = _actionList.AddAction("ui_select");
            UiStart = _actionList.AddAction("ui_start");
            
            ConfigurableActionList = new []{ Jump, Attack }.ToList();
            Configure();
        }

        public void RedefineButton(ActionState actionState, int button) {
            actionState.ClearConfig().AddButton((JoystickList)button);
        }

        public void RedefineKey(ActionState actionState, int key) {
            actionState.ClearConfig().AddKey((KeyList)key);
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

            UiLeft.ClearConfig()
                .AddKey(KeyList.Left)
                .AddButton(JoystickList.DpadLeft)
                .Build();

            UiRight.ClearConfig()
                .AddKey(KeyList.Right)
                .AddButton(JoystickList.DpadRight)
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


        public void Debug(InputEvent e, bool actionsOnly = true) {
            var action = FindAction(e);
            string actionName = null;
            switch (action) {
                case ActionState state:
                    actionName = state.Name;
                    break;
                case DirectionalAction directional:
                    // TODO: move this code to DirectionalAction, and create a IsPressed(e) and IsReleased(e)
                    if (directional.Strength == 0f) {
                        actionName = e.IsActionReleased(directional.NegativeName)
                            ? directional.NegativeName
                            : directional.PositiveName;
                    } else {
                        actionName = directional.Strength < 0 ? directional.NegativeName : directional.PositiveName;
                    }
                    break;
            }
            if (actionName == null && actionsOnly) return;
            var wrapper = new EventWrapper(e);
            if (wrapper.IsMotion()) {
                Logger.Debug(
                    $"Axis {wrapper.Device}[{wrapper.Axis}]:{wrapper.GetStrength()} ({wrapper.AxisValue}) {actionName}");
            } else if (wrapper.IsAnyButton()) {
                Logger.Debug(
                    $"Button {wrapper.Device}[{wrapper.Button}]:{wrapper.Pressed} ({wrapper.Pressure}) {actionName}");
            } else if (wrapper.IsAnyKey()) {
                Logger.Debug(
                    $"Key \"{wrapper.KeyString}\" #{wrapper.Key} Pressed:{wrapper.Pressed}/Echo:{wrapper.Echo} {actionName}");
            }
        }

        public ActionState FindActionState(string name) {
            return _actionList.FindActionState(name);
        }

        public BaseAction FindAction(InputEvent e, bool echo = false) {
            return _actionList.FindAction(e, echo);
        }

    }
}