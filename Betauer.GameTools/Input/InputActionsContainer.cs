using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class InputActionsContainer {
        public readonly List<IInputAction> ConfigurableActionList = new List<IInputAction>();
        public readonly List<IInputAction> ActionList = new List<IInputAction>();
        public readonly Dictionary<string, IInputAction> ActionMap = new Dictionary<string, IInputAction>();

        public IInputAction? FindAction(string name) {
            return ActionMap.TryGetValue(name, out var action) ? action : null;
        }

        public IInputAction? FindAction(InputEvent inputEvent, bool echo = false) {
            return ActionList.Find(action => action.IsEventAction(inputEvent, echo));
        }

        public T? FindAction<T>(InputEvent inputEvent, bool echo = false) where T : class, IInputAction {
            return ActionList.Find(action => action.IsEventAction(inputEvent, echo)) as T;
        }

        public T? FindAction<T>(string name) where T : class, IInputAction {
            return ActionMap.TryGetValue(name, out var action) ? action as T: null;
        }

        internal void Add(IInputAction actionState) {
            ActionList.Add(actionState);
            ActionMap.Add(actionState.Name, actionState);
            if (actionState.IsConfigurable()) {
                ConfigurableActionList.Add(actionState);
            }
        }
    }
}