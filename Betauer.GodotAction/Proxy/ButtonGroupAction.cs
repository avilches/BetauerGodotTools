using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class ButtonGroupAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public ButtonGroupAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public ButtonGroupAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private ButtonGroupAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action<Object>>? _onPressedAction; 
        public ButtonGroupAction OnPressed(Action<Object> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPressedAction, "pressed", nameof(_GodotSignalPressed), action, oneShot, deferred);
            return this;
        }

        public ButtonGroupAction RemoveOnPressed(Action<Object> action) {
            RemoveSignal(_onPressedAction, "pressed", nameof(_GodotSignalPressed), action);
            return this;
        }

        private ButtonGroupAction _GodotSignalPressed(Object button) {
            ExecuteSignal(_onPressedAction, button);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ButtonGroupAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ButtonGroupAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ButtonGroupAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}