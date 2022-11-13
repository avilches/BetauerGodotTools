using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class InputActionsContainer {
        public readonly List<InputAction> ActionList = new List<InputAction>();
        public readonly Dictionary<string, InputAction> ActionMap = new Dictionary<string, InputAction>();
        
        public InputAction? FindAction(string name) {
            return ActionMap.TryGetValue(name, out var action) ? action : null;
        }

        public InputAction? FindAction(InputEvent inputEvent, bool echo = false) {
            return ActionList.Find(action => action.IsEvent(inputEvent, echo));
        }

        public InputAction? FindAction(JoyButton button) {
            return ActionList.Find(action => action.HasButton(button));
        }

        public InputAction? FindAction(Key key) {
            return ActionList.Find(action => action.HasKey(key));
        }

        public void Add(InputAction inputAction) {
            ActionList.Add(inputAction);
            ActionMap.Add(inputAction.Name, inputAction);
            inputAction.OnAddToInputContainer(this);
        }

        public void Setup() {
            ActionList.ForEach(action => action.Setup());
        }

        public void RemoveSetup() {
            ActionList.ForEach(action => action.RemoveSetup());
        }
    }
}