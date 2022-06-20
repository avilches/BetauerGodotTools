using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptMathConstantAction : VisualScriptMathConstant {


        private List<Action>? _onChangedAction; 
        public VisualScriptMathConstantAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(ExecuteChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public VisualScriptMathConstantAction RemoveOnChanged(Action action) {
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
        

        private List<Action>? _onPortsChangedAction; 
        public VisualScriptMathConstantAction OnPortsChanged(Action action) {
            if (_onPortsChangedAction == null || _onPortsChangedAction.Count == 0) {
                _onPortsChangedAction ??= new List<Action>(); 
                Connect("ports_changed", this, nameof(ExecutePortsChanged));
            }
            _onPortsChangedAction.Add(action);
            return this;
        }
        public VisualScriptMathConstantAction RemoveOnPortsChanged(Action action) {
            if (_onPortsChangedAction == null || _onPortsChangedAction.Count == 0) return this;
            _onPortsChangedAction.Remove(action); 
            if (_onPortsChangedAction.Count == 0) {
                Disconnect("ports_changed", this, nameof(ExecutePortsChanged));
            }
            return this;
        }
        private void ExecutePortsChanged() {
            if (_onPortsChangedAction == null || _onPortsChangedAction.Count == 0) return;
            for (var i = 0; i < _onPortsChangedAction.Count; i++) _onPortsChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisualScriptMathConstantAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisualScriptMathConstantAction RemoveOnScriptChanged(Action action) {
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
        
    }
}