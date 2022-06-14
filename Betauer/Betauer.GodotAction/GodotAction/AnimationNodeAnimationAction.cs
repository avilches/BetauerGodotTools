using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeAnimationAction : AnimationNodeAnimation {


        private Action? _onChangedAction; 
        public AnimationNodeAnimationAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AnimationNodeAnimationAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onRemovedFromGraphAction; 
        public AnimationNodeAnimationAction OnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null) 
                Connect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = action;
            return this;
        }
        public AnimationNodeAnimationAction RemoveOnRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null) return this; 
            Disconnect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = null;
            return this;
        }
        private void ExecuteRemovedFromGraph() =>
            _onRemovedFromGraphAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationNodeAnimationAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationNodeAnimationAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeChangedAction; 
        public AnimationNodeAnimationAction OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null) 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = action;
            return this;
        }
        public AnimationNodeAnimationAction RemoveOnTreeChanged() {
            if (_onTreeChangedAction == null) return this; 
            Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = null;
            return this;
        }
        private void ExecuteTreeChanged() =>
            _onTreeChangedAction?.Invoke();
        
    }
}