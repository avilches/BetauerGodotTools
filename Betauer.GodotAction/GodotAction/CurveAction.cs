using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class CurveAction : Node {
        public CurveAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onChangedAction; 
        public CurveAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                GetParent().Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnChanged(Action action) {
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
        

        private List<Action>? _onRangeChangedAction; 
        public CurveAction OnRangeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) {
                _onRangeChangedAction ??= new List<Action>(); 
                GetParent().Connect("range_changed", this, nameof(_GodotSignalRangeChanged));
            }
            _onRangeChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnRangeChanged(Action action) {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) return this;
            _onRangeChangedAction.Remove(action); 
            if (_onRangeChangedAction.Count == 0) {
                GetParent().Disconnect("range_changed", this, nameof(_GodotSignalRangeChanged));
            }
            return this;
        }
        private void _GodotSignalRangeChanged() {
            if (_onRangeChangedAction == null || _onRangeChangedAction.Count == 0) return;
            for (var i = 0; i < _onRangeChangedAction.Count; i++) _onRangeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public CurveAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public CurveAction RemoveOnScriptChanged(Action action) {
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