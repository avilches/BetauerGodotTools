using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisualScriptWhileAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualScriptWhileAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptWhileAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualScriptWhileAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onPortsChangedAction; 
        public VisualScriptWhileAction OnPortsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptWhileAction RemoveOnPortsChanged(Action action) {
            RemoveSignal(_onPortsChangedAction, "ports_changed", nameof(_GodotSignalPortsChanged), action);
            return this;
        }

        private VisualScriptWhileAction _GodotSignalPortsChanged() {
            ExecuteSignal(_onPortsChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptWhileAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptWhileAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualScriptWhileAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}