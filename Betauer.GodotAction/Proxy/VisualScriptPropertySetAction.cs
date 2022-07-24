using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisualScriptPropertySetAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualScriptPropertySetAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptPropertySetAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualScriptPropertySetAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onPortsChangedAction; 
        public VisualScriptPropertySetAction OnPortsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptPropertySetAction RemoveOnPortsChanged(Action action) {
            RemoveSignal(_onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action);
            return this;
        }

        private VisualScriptPropertySetAction _GodotSignalPortsChanged() {
            ExecuteSignal(_onPortsChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptPropertySetAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptPropertySetAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualScriptPropertySetAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}