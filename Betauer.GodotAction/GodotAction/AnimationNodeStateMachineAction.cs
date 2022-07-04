using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeStateMachineAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public AnimationNodeStateMachineAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationNodeStateMachineAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeStateMachineAction OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineAction RemoveOnRemovedFromGraph(Action action) {
            RemoveSignal(_onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action);
            return this;
        }

        private AnimationNodeStateMachineAction _GodotSignalRemovedFromGraph() {
            ExecuteSignal(_onRemovedFromGraphAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeStateMachineAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationNodeStateMachineAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeStateMachineAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineAction RemoveOnTreeChanged(Action action) {
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);
            return this;
        }

        private AnimationNodeStateMachineAction _GodotSignalTreeChanged() {
            ExecuteSignal(_onTreeChangedAction);
            return this;
        }
    }
}