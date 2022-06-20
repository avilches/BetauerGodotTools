using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeOneShotAction : AnimationNodeOneShot {


        private List<Action>? _onChangedAction; 
        public AnimationNodeOneShotAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(ExecuteChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public AnimationNodeOneShotAction RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                Disconnect("changed", this, nameof(ExecuteChanged));
            }
            return this;
        }
        private void ExecuteChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeOneShotAction OnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) {
                _onRemovedFromGraphAction ??= new List<Action>(); 
                Connect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            }
            _onRemovedFromGraphAction.Add(action);
            return this;
        }
        public AnimationNodeOneShotAction RemoveOnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return this;
            _onRemovedFromGraphAction.Remove(action); 
            if (_onRemovedFromGraphAction.Count == 0) {
                Disconnect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            }
            return this;
        }
        private void ExecuteRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return;
            for (var i = 0; i < _onRemovedFromGraphAction.Count; i++) _onRemovedFromGraphAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeOneShotAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationNodeOneShotAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeOneShotAction OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) {
                _onTreeChangedAction ??= new List<Action>(); 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            }
            _onTreeChangedAction.Add(action);
            return this;
        }
        public AnimationNodeOneShotAction RemoveOnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return this;
            _onTreeChangedAction.Remove(action); 
            if (_onTreeChangedAction.Count == 0) {
                Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            }
            return this;
        }
        private void ExecuteTreeChanged() {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return;
            for (var i = 0; i < _onTreeChangedAction.Count; i++) _onTreeChangedAction[i].Invoke();
        }
        
    }
}