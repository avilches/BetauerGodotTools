using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class ButtonGroupAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public void OnChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);

        public void RemoveOnChanged(Action action) =>
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);

        private void _GodotSignalChanged() =>
            ExecuteSignal(_onChangedAction);
        

        private List<Action<Object>>? _onPressedAction; 
        public void OnPressed(Action<Object> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPressedAction, "pressed", nameof(_GodotSignalPressed), action, oneShot, deferred);

        public void RemoveOnPressed(Action<Object> action) =>
            RemoveSignal(_onPressedAction, "pressed", nameof(_GodotSignalPressed), action);

        private void _GodotSignalPressed(Object button) =>
            ExecuteSignal(_onPressedAction, button);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        
    }
}