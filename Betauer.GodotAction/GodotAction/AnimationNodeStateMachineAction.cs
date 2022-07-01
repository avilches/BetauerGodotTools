using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeStateMachineAction : Node {
        public AnimationNodeStateMachineAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onChangedAction; 
        public AnimationNodeStateMachineAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                GetParent().Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineAction RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                GetParent().Disconnect("changed", this, nameof(_GodotSignalChanged));
            }
            return this;
        }
        private void _GodotSignalChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeStateMachineAction OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) {
                _onRemovedFromGraphAction ??= new List<Action>(); 
                GetParent().Connect("removed_from_graph", this, nameof(_GodotSignalRemovedFromGraph));
            }
            _onRemovedFromGraphAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineAction RemoveOnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return this;
            _onRemovedFromGraphAction.Remove(action); 
            if (_onRemovedFromGraphAction.Count == 0) {
                GetParent().Disconnect("removed_from_graph", this, nameof(_GodotSignalRemovedFromGraph));
            }
            return this;
        }
        private void _GodotSignalRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return;
            for (var i = 0; i < _onRemovedFromGraphAction.Count; i++) _onRemovedFromGraphAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeStateMachineAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeStateMachineAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) {
                _onTreeChangedAction ??= new List<Action>(); 
                GetParent().Connect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            _onTreeChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineAction RemoveOnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return this;
            _onTreeChangedAction.Remove(action); 
            if (_onTreeChangedAction.Count == 0) {
                GetParent().Disconnect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            return this;
        }
        private void _GodotSignalTreeChanged() {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return;
            for (var i = 0; i < _onTreeChangedAction.Count; i++) _onTreeChangedAction[i].Invoke();
        }
        
    }
}