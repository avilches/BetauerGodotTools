using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeTimeSeekAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public void OnChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);

        public void RemoveOnChanged(Action action) =>
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);

        private void _GodotSignalChanged() =>
            ExecuteSignal(_onChangedAction);
        

        private List<Action>? _onRemovedFromGraphAction; 
        public void OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action, oneShot, deferred);

        public void RemoveOnRemovedFromGraph(Action action) =>
            RemoveSignal(_onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action);

        private void _GodotSignalRemovedFromGraph() =>
            ExecuteSignal(_onRemovedFromGraphAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onTreeChangedAction; 
        public void OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);

        public void RemoveOnTreeChanged(Action action) =>
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);

        private void _GodotSignalTreeChanged() =>
            ExecuteSignal(_onTreeChangedAction);
        
    }
}