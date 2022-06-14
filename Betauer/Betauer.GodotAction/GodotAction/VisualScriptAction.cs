using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptAction : VisualScript {


        private Action? _onChangedAction; 
        public VisualScriptAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public VisualScriptAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action<string, int>? _onNodePortsChangedAction; 
        public VisualScriptAction OnNodePortsChanged(Action<string, int> action) {
            if (_onNodePortsChangedAction == null) 
                Connect("node_ports_changed", this, nameof(ExecuteNodePortsChanged));
            _onNodePortsChangedAction = action;
            return this;
        }
        public VisualScriptAction RemoveOnNodePortsChanged() {
            if (_onNodePortsChangedAction == null) return this; 
            Disconnect("node_ports_changed", this, nameof(ExecuteNodePortsChanged));
            _onNodePortsChangedAction = null;
            return this;
        }
        private void ExecuteNodePortsChanged(string function, int id) =>
            _onNodePortsChangedAction?.Invoke(function, id);
        

        private Action? _onScriptChangedAction; 
        public VisualScriptAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisualScriptAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}