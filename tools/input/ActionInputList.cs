using System.Collections.Generic;
using System.Linq;

namespace Veronenger.tools.input {
    public class ActionInputList : IActionUpdate {
        private List<IActionUpdate> actions = new List<IActionUpdate>();
        private readonly PlayerController _playerController;

        public ActionInputList(PlayerController playerController) {
            _playerController = playerController;
        }

        public DirectionInput AddDirectionalMotion(string name) {
            var actionState = new DirectionInput(name, _playerController);
            actions.Add(actionState);
            return actionState;
        }

        public ActionState AddAction(string name) {
            var actionState = new ActionState(name, _playerController);
            actions.Add(actionState);
            return actionState;
        }

        public override bool Update(EventWrapper w) {
            return actions.Any(actionInput => actionInput.Update(w));
        }

        public override void ClearJustState() {
            actions.ForEach(actionInput => actionInput.ClearJustState());
        }
    }
}