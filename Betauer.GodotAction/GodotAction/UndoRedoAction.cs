using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class UndoRedoAction : UndoRedo {


        private List<Action>? _onScriptChangedAction; 
        public UndoRedoAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public UndoRedoAction RemoveOnScriptChanged(Action action) {
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
        

        private List<Action>? _onVersionChangedAction; 
        public UndoRedoAction OnVersionChanged(Action action) {
            if (_onVersionChangedAction == null || _onVersionChangedAction.Count == 0) {
                _onVersionChangedAction ??= new List<Action>(); 
                Connect("version_changed", this, nameof(_GodotSignalVersionChanged));
            }
            _onVersionChangedAction.Add(action);
            return this;
        }
        public UndoRedoAction RemoveOnVersionChanged(Action action) {
            if (_onVersionChangedAction == null || _onVersionChangedAction.Count == 0) return this;
            _onVersionChangedAction.Remove(action); 
            if (_onVersionChangedAction.Count == 0) {
                Disconnect("version_changed", this, nameof(_GodotSignalVersionChanged));
            }
            return this;
        }
        private void _GodotSignalVersionChanged() {
            if (_onVersionChangedAction == null || _onVersionChangedAction.Count == 0) return;
            for (var i = 0; i < _onVersionChangedAction.Count; i++) _onVersionChangedAction[i].Invoke();
        }
        
    }
}