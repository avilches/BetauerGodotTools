using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptAction : VisualScript {


        private List<Action>? _onChangedAction; 
        public VisualScriptAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                Disconnect("changed", this, nameof(_GodotSignalChanged));
            }
            return this;
        }
        private void _GodotSignalChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action<string, int>>? _onNodePortsChangedAction; 
        public VisualScriptAction OnNodePortsChanged(Action<string, int> action) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) {
                _onNodePortsChangedAction ??= new List<Action<string, int>>(); 
                Connect("node_ports_changed", this, nameof(_GodotSignalNodePortsChanged));
            }
            _onNodePortsChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnNodePortsChanged(Action<string, int> action) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) return this;
            _onNodePortsChangedAction.Remove(action); 
            if (_onNodePortsChangedAction.Count == 0) {
                Disconnect("node_ports_changed", this, nameof(_GodotSignalNodePortsChanged));
            }
            return this;
        }
        private void _GodotSignalNodePortsChanged(string function, int id) {
            if (_onNodePortsChangedAction == null || _onNodePortsChangedAction.Count == 0) return;
            for (var i = 0; i < _onNodePortsChangedAction.Count; i++) _onNodePortsChangedAction[i].Invoke(function, id);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisualScriptAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        
    }
}