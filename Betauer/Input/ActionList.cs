using Betauer.Collections;

namespace Betauer.Input {
    public class ActionList : IKeyboardOrController {
        public bool IsUsingKeyboard { get; set; }
        private readonly SimpleLinkedList<IActionUpdate> _actions = new SimpleLinkedList<IActionUpdate>();
        private readonly int _deviceId;

        public ActionList(int deviceId) {
            _deviceId = deviceId;
        }

        public DirectionInput AddDirectionalMotion(string name) {
            var actionState = new DirectionInput(name, this, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public ActionState AddAction(string name) {
            var actionState = new ActionState(name, this, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public IActionUpdate Update(EventWrapper w) {
            return _actions.Find(actionInput => actionInput.Enabled && actionInput.Update(w));
        }

        public void ClearJustStates() {
            _actions.ForEach(actionInput => actionInput.ClearJustPressedState());
        }

        public void ClearStates() {
            _actions.ForEach(actionInput => actionInput.ClearPressedState());
        }
    }
}