using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptVariableGetAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualScriptVariableGetAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptVariableGetAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualScriptVariableGetAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onPortsChangedAction; 
        public VisualScriptVariableGetAction OnPortsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptVariableGetAction RemoveOnPortsChanged(Action action) {
            RemoveSignal(_onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action);
            return this;
        }

        private VisualScriptVariableGetAction _GodotSignalPortsChanged() {
            ExecuteSignal(_onPortsChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptVariableGetAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptVariableGetAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualScriptVariableGetAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}