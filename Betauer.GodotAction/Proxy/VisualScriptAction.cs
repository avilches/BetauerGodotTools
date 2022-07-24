using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisualScriptAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualScriptAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualScriptAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action<string, int>>? _onNodePortsChangedAction; 
        public VisualScriptAction OnNodePortsChanged(Action<string, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodePortsChangedAction, "node_ports_changed", nameof(_GodotSignalNodePortsChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptAction RemoveOnNodePortsChanged(Action<string, int> action) {
            RemoveSignal(_onNodePortsChangedAction, "node_ports_changed", nameof(_GodotSignalNodePortsChanged), action);
            return this;
        }

        private VisualScriptAction _GodotSignalNodePortsChanged(string function, int id) {
            ExecuteSignal(_onNodePortsChangedAction, function, id);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualScriptAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualScriptAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}