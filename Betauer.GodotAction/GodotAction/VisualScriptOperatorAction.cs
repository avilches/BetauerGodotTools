using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptOperatorAction : VisualScriptOperator {


        private Action? _onChangedAction; 
        public VisualScriptOperatorAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public VisualScriptOperatorAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onPortsChangedAction; 
        public VisualScriptOperatorAction OnPortsChanged(Action action) {
            if (_onPortsChangedAction == null) 
                Connect("ports_changed", this, nameof(ExecutePortsChanged));
            _onPortsChangedAction = action;
            return this;
        }
        public VisualScriptOperatorAction RemoveOnPortsChanged() {
            if (_onPortsChangedAction == null) return this; 
            Disconnect("ports_changed", this, nameof(ExecutePortsChanged));
            _onPortsChangedAction = null;
            return this;
        }
        private void ExecutePortsChanged() =>
            _onPortsChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public VisualScriptOperatorAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisualScriptOperatorAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}