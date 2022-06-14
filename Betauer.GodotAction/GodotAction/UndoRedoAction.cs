using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class UndoRedoAction : UndoRedo {


        private Action? _onScriptChangedAction; 
        public UndoRedoAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public UndoRedoAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onVersionChangedAction; 
        public UndoRedoAction OnVersionChanged(Action action) {
            if (_onVersionChangedAction == null) 
                Connect("version_changed", this, nameof(ExecuteVersionChanged));
            _onVersionChangedAction = action;
            return this;
        }
        public UndoRedoAction RemoveOnVersionChanged() {
            if (_onVersionChangedAction == null) return this; 
            Disconnect("version_changed", this, nameof(ExecuteVersionChanged));
            _onVersionChangedAction = null;
            return this;
        }
        private void ExecuteVersionChanged() =>
            _onVersionChangedAction?.Invoke();
        
    }
}