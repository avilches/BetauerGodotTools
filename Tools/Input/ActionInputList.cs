using System.Collections.Generic;
using System.Linq;

namespace Betauer.Tools.Input {
    public class ActionInputList : IActionUpdate {
        private readonly List<IActionUpdate> _actions = new List<IActionUpdate>();
        private readonly PlayerActions _playerActions;
        private readonly int _deviceId;

        public ActionInputList(PlayerActions playerActions, int deviceId = -1) {
            _playerActions = playerActions;
            _deviceId = deviceId;
        }

        public DirectionInput AddDirectionalMotion(string name) {
            var actionState = new DirectionInput(name, _playerActions, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public ActionState AddAction(string name) {
            var actionState = new ActionState(name, _playerActions, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public override bool Update(EventWrapper w) {
            return _actions.Any(actionInput => actionInput.Update(w));
        }

        public override void ClearJustState() {
            _actions.ForEach(actionInput => actionInput.ClearJustState());
        }
    }
}