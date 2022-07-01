using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptAction : Node {
        public VisualScriptAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onChangedAction; 
        public VisualScriptAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                GetParent().Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnChanged(Action action) {
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
        

        private List<Action<string, int>>? _onNodePortsChangedAction; 
        public VisualScriptAction OnNodePortsChanged(Action<string, int> action, bool oneShot = false, bool deferred = false) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) {
                _onNodePortsChangedAction ??= new List<Action<string, int>>(); 
                GetParent().Connect("node_ports_changed", this, nameof(_GodotSignalNodePortsChanged));
            }
            _onNodePortsChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnNodePortsChanged(Action<string, int> action) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) return this;
            _onNodePortsChangedAction.Remove(action); 
            if (_onNodePortsChangedAction.Count == 0) {
                GetParent().Disconnect("node_ports_changed", this, nameof(_GodotSignalNodePortsChanged));
            }
            return this;
        }
        private void _GodotSignalNodePortsChanged(string function, int id) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) return;
            for (var i = 0; i < _onNodePortsChangedAction.Count; i++) _onNodePortsChangedAction[i].Invoke(function, id);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnScriptChanged(Action action) {
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
        
    }
}