using System.Collections.Generic;
using Betauer.Collections;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class InputManager {
        private readonly ActionList _actionList;

        public readonly LateralAction LateralMotion;
        public readonly LateralAction UiLateralMotion;
        
        public readonly VerticalAction UiVerticalMotion;
        public readonly VerticalAction VerticalMotion;
        
        public readonly ConfigurableAction Jump;
        public readonly ConfigurableAction Attack;
        public readonly ActionState UiLeft;
        public readonly ActionState UiRight;
        public readonly ActionState UiAccept;
        public readonly ActionState UiSelect;
        public readonly ActionState UiStart;
        public readonly ActionState UiCancel;
        
        public readonly ActionState PixelPerfect;

        public readonly List<ConfigurableAction> ConfigurableActionList = new List<ConfigurableAction>();

        public InputManager() {
            _actionList = new ActionList(-1);

            // Game actions
            LateralMotion = _actionList.AddLateralAction("left", "right");
            VerticalMotion = _actionList.AddVerticalAction("up", "down");
            Jump = _actionList.AddConfigurableAction("Jump");
            Attack = _actionList.AddConfigurableAction("Attack");

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