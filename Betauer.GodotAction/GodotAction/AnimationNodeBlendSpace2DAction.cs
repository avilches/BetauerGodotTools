using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeBlendSpace2DAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public AnimationNodeBlendSpace2DAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlendSpace2DAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationNodeBlendSpace2DAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeBlendSpace2DAction OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlendSpace2DAction RemoveOnRemovedFromGraph(Action action) {
            RemoveSignal(_onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action);
            return this;
        }

        private AnimationNodeBlendSpace2DAction _GodotSignalRemovedFromGraph() {
            ExecuteSignal(_onRemovedFromGraphAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeBlendSpace2DAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlendSpace2DAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationNodeBlendSpace2DAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeBlendSpace2DAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlendSpace2DAction RemoveOnTreeChanged(Action action) {
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);
            return this;
        }

        private AnimationNodeBlendSpace2DAction _GodotSignalTreeChanged() {
            ExecuteSignal(_onTreeChangedAction);
            return this;
        }

        private List<Action>? _onTrianglesUpdatedAction; 
        public AnimationNodeBlendSpace2DAction OnTrianglesUpdated(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTrianglesUpdatedAction, "triangles_updated", nameof(_GodotSignalTrianglesUpdated), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeBlendSpace2DAction RemoveOnTrianglesUpdated(Action action) {
            RemoveSignal(_onTrianglesUpdatedAction, "triangles_updated", nameof(_GodotSignalTrianglesUpdated), action);
            return this;
        }

        private AnimationNodeBlendSpace2DAction _GodotSignalTrianglesUpdated() {
            ExecuteSignal(_onTrianglesUpdatedAction);
            return this;
        }
    }
}