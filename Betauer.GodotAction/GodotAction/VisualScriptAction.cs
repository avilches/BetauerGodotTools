using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public void OnChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);

        public void RemoveOnChanged(Action action) =>
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);

        private void _GodotSignalChanged() =>
            ExecuteSignal(_onChangedAction);
        

        private List<Action<string, int>>? _onNodePortsChangedAction; 
        public void OnNodePortsChanged(Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodePortsChangedAction, "node_ports_changed", nameof(_GodotSignalNodePortsChanged), action, oneShot, deferred);

        public void RemoveOnNodePortsChanged(Action<string, int> action) =>
            RemoveSignal(_onNodePortsChangedAction, "node_ports_changed", nameof(_GodotSignalNodePortsChanged), action);

        private void _GodotSignalNodePortsChanged(string function, int id) =>
            ExecuteSignal(_onNodePortsChangedAction, function, id);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        
    }
}