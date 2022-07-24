using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisualScriptSwitchAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualScriptSwitchAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptSwitchAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualScriptSwitchAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onPortsChangedAction; 
        public VisualScriptSwitchAction OnPortsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptSwitchAction RemoveOnPortsChanged(Action action) {
            RemoveSignal(_onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action);
            return this;
        }

        private VisualScriptSwitchAction _GodotSignalPortsChanged() {
            ExecuteSignal(_onPortsChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptSwitchAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptSwitchAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualScriptSwitchAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}