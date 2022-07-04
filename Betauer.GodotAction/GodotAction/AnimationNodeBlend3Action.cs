using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeBlend3Action : ProxyNode {

        private List<Action>? _onChangedAction; 
        public AnimationNodeBlend3Action OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlend3Action RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationNodeBlend3Action _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeBlend3Action OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlend3Action RemoveOnRemovedFromGraph(Action action) {
            RemoveSignal(_onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action);
            return this;
        }

        private AnimationNodeBlend3Action _GodotSignalRemovedFromGraph() {
            ExecuteSignal(_onRemovedFromGraphAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeBlend3Action OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlend3Action RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationNodeBlend3Action _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeBlend3Action OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlend3Action RemoveOnTreeChanged(Action action) {
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);
            return this;
        }

        private AnimationNodeBlend3Action _GodotSignalTreeChanged() {
            ExecuteSignal(_onTreeChangedAction);
            return this;
        }
    }
}