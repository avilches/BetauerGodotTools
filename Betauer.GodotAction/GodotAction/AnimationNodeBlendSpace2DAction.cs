using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeBlendSpace2DAction : AnimationNodeBlendSpace2D {


        private Action? _onChangedAction; 
        public AnimationNodeBlendSpace2DAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AnimationNodeBlendSpace2DAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onRemovedFromGraphAction; 
        public AnimationNodeBlendSpace2DAction OnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null) 
                Connect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = action;
            return this;
        }
        public AnimationNodeBlendSpace2DAction RemoveOnRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null) return this; 
            Disconnect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = null;
            return this;
        }
        private void ExecuteRemovedFromGraph() =>
            _onRemovedFromGraphAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationNodeBlendSpace2DAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationNodeBlendSpace2DAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeChangedAction; 
        public AnimationNodeBlendSpace2DAction OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null) 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = action;
            return this;
        }
        public AnimationNodeBlendSpace2DAction RemoveOnTreeChanged() {
            if (_onTreeChangedAction == null) return this; 
            Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = null;
            return this;
        }
        private void ExecuteTreeChanged() =>
            _onTreeChangedAction?.Invoke();
        

        private Action? _onTrianglesUpdatedAction; 
        public AnimationNodeBlendSpace2DAction OnTrianglesUpdated(Action action) {
            if (_onTrianglesUpdatedAction == null) 
                Connect("triangles_updated", this, nameof(ExecuteTrianglesUpdated));
            _onTrianglesUpdatedAction = action;
            return this;
        }
        public AnimationNodeBlendSpace2DAction RemoveOnTrianglesUpdated() {
            if (_onTrianglesUpdatedAction == null) return this; 
            Disconnect("triangles_updated", this, nameof(ExecuteTrianglesUpdated));
            _onTrianglesUpdatedAction = null;
            return this;
        }
        private void ExecuteTrianglesUpdated() =>
            _onTrianglesUpdatedAction?.Invoke();
        
    }
}