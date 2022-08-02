using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace DemoAnimation.Game.Managers {
    [Service]
    public class InputManager {
        private readonly ActionList _actionList;

        private readonly ActionState _uiLeft;
        private readonly ActionState _uiRight;
        private readonly ActionState _uiAccept;
        private readonly ActionState _uiSelect;
        private readonly ActionState _uiStart;
        private readonly ActionState _uiCancel;
        
        private readonly LateralAction UiLateralMotion;
        private readonly VerticalAction UiVerticalMotion;
        [Service] private ActionState UiLeft => _uiLeft;
        [Service] private ActionState UiRight => _uiRight;
        [Service] private ActionState UiAccept => _uiAccept;
        [Service] private ActionState UiSelect => _uiSelect;
        [Service] private ActionState UiStart => _uiStart;
        [Service] private ActionState UiCancel => _uiCancel;


        public readonly List<ActionState> ConfigurableActionList = new List<ActionState>();

        public InputManager() {
            _actionList = new ActionList(-1);

            // UI actions
            UiLateralMotion = _actionList.AddLateralAction("ui_left", "ui_right");
            UiVerticalMotion = _actionList.AddVerticalAction("ui_up", "ui_down");
            
            _uiLeft = _actionList.AddAction("ui_left");
            _uiRight = _actionList.AddAction("ui_right");
            _uiAccept = _actionList.AddAction("ui_accept");
            _uiCancel = _actionList.AddAction("ui_cancel");
            _uiSelect = _actionList.AddAction("ui_select");
            _uiStart = _actionList.AddAction("ui_start");
            
            Configure();
            
        }

        public void Configure() {
            UiLeft
                .AddKey(KeyList.Left)
                .AddButton(JoystickList.DpadLeft)
                .Build();

            UiRight
                .AddKey(KeyList.Right)
                .AddButton(JoystickList.DpadRight)
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
        }

    }
}