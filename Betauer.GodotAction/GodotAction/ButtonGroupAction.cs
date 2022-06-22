using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class ButtonGroupAction : ButtonGroup {


        private List<Action>? _onChangedAction; 
        public ButtonGroupAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public ButtonGroupAction RemoveOnChanged(Action action) {
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
        

        private List<Action<Object>>? _onPressedAction; 
        public ButtonGroupAction OnPressed(Action<Object> action) {
            if (_onPressedAction == null || _onPressedAction.Count == 0) {
                _onPressedAction ??= new List<Action<Object>>(); 
                Connect("pressed", this, nameof(_GodotSignalPressed));
            }
            _onPressedAction.Add(action);
            return this;
        }
        public ButtonGroupAction RemoveOnPressed(Action<Object> action) {
            if (_onPressedAction == null || _onPressedAction.Count == 0) return this;
            _onPressedAction.Remove(action); 
            if (_onPressedAction.Count == 0) {
                Disconnect("pressed", this, nameof(_GodotSignalPressed));
            }
            return this;
        }
        private void _GodotSignalPressed(Object button) {
            if (_onPressedAction == null || _onPressedAction.Count == 0) return;
            for (var i = 0; i < _onPressedAction.Count; i++) _onPressedAction[i].Invoke(button);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public ButtonGroupAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public ButtonGroupAction RemoveOnScriptChanged(Action action) {
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