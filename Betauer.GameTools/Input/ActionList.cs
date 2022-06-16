using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class ActionList {
        private readonly List<BaseAction> _actions = new List<BaseAction>();
        private readonly Dictionary<string, ActionState> _map = new Dictionary<string, ActionState>();
        private readonly int _deviceId;

        public ActionList(int deviceId) {
            _deviceId = deviceId;
        }

        public VerticalAction AddVerticalAction(string negativeName, string positiveName) {
            var actionVertical = new VerticalAction(negativeName, positiveName, _deviceId);
            _actions.Add(actionVertical);
            return actionVertical;
        }

        public LateralAction AddLateralAction(string negativeName, string positiveName) {
            var actionState = new LateralAction(negativeName, positiveName, _deviceId);
            _actions.Add(actionState);
            return actionState;
        }

        public ActionState AddConfigurableAction(string name) {
            var actionState = new ActionState(name, _deviceId);
            _actions.Add(actionState);
            _map.Add(name, actionState);
            return actionState;
        }

        public ActionState AddAction(string name) {
            var actionState = new ActionState(name, _deviceId);
            _actions.Add(actionState);
            _map.Add(name, actionState);
            return actionState;
        }

        public ActionState? FindActionState(string name) {
            return _map.TryGetValue(name, out var action) ? action : null;
        }

        public BaseAction? FindAction(InputEvent inputEvent, bool echo = false) {
            return _actions.Find(action => action.IsEventAction(inputEvent, echo));
        }
    }
}