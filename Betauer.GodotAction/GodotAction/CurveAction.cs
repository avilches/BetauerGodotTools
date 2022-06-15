using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class CurveAction : Curve {


        private Action? _onChangedAction; 
        public CurveAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public CurveAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onRangeChangedAction; 
        public CurveAction OnRangeChanged(Action action) {
            if (_onRangeChangedAction == null) 
                Connect("range_changed", this, nameof(ExecuteRangeChanged));
            _onRangeChangedAction = action;
            return this;
        }
        public CurveAction RemoveOnRangeChanged() {
            if (_onRangeChangedAction == null) return this; 
            Disconnect("range_changed", this, nameof(ExecuteRangeChanged));
            _onRangeChangedAction = null;
            return this;
        }
        private void ExecuteRangeChanged() =>
            _onRangeChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public CurveAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public CurveAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}