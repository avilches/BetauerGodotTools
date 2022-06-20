using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class CurveAction : Curve {


        private List<Action>? _onChangedAction; 
        public CurveAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(ExecuteChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnChanged(Action action) {
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
        

        private List<Action>? _onRangeChangedAction; 
        public CurveAction OnRangeChanged(Action action) {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) {
                _onRangeChangedAction ??= new List<Action>(); 
                Connect("range_changed", this, nameof(ExecuteRangeChanged));
            }
            _onRangeChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnRangeChanged(Action action) {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) return this;
            _onRangeChangedAction.Remove(action); 
            if (_onRangeChangedAction.Count == 0) {
                Disconnect("range_changed", this, nameof(ExecuteRangeChanged));
            }
            return this;
        }
        private void ExecuteRangeChanged() {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) return;
            for (var i = 0; i < _onRangeChangedAction.Count; i++) _onRangeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public CurveAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnScriptChanged(Action action) {
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