using Betauer.Collections;
using Godot;

namespace Betauer.Input {
    public class ActionList : IKeyboardOrController {
        public bool IsUsingKeyboard { get; set; }
        private readonly SimpleLinkedList<BaseAction> _actions = new SimpleLinkedList<BaseAction>();
        private readonly int _deviceId;

        public ActionList(int deviceId) {
            _deviceId = deviceId;
        }

        public VerticalAction AddVerticalAction(string negativeName, string positiveName) {
            var actionVertical = new VerticalAction(negativeName, positiveName, this, _deviceId);
            _actions.Add(actionVertical);
            return actionVertical;
        }

        public LateralAction AddLateralAction(string negativeName, string positiveName) {
            var actionState = new LateralAction(negativeName, positiveName, this, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public ActionState AddAction(string name) {
            var actionState = new ActionState(name, this, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public BaseAction FindAction(InputEvent inputEvent, bool echo = false) {
            return _actions.Find(action => action.IsEventPressed(inputEvent, echo));
        }
    }
}