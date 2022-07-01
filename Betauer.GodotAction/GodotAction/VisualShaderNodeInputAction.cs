using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeInputAction : Node {
        public VisualShaderNodeInputAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onChangedAction; 
        public VisualShaderNodeInputAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                GetParent().Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public VisualShaderNodeInputAction RemoveOnChanged(Action action) {
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
        

        private List<Action>? _onEditorRefreshRequestAction; 
        public VisualShaderNodeInputAction OnEditorRefreshRequest(Action action, bool oneShot = false, bool deferred = false) {
            if (_onEditorRefreshRequestAction == null || _onEditorRefreshRequestAction.Count == 0) {
                _onEditorRefreshRequestAction ??= new List<Action>(); 
                GetParent().Connect("editor_refresh_request", this, nameof(_GodotSignalEditorRefreshRequest));
            }
            _onEditorRefreshRequestAction.Add(action);
            return this;
        }
        public VisualShaderNodeInputAction RemoveOnEditorRefreshRequest(Action action) {
            if (_onEditorRefreshRequestAction == null || _onEditorRefreshRequestAction.Count == 0) return this;
            _onEditorRefreshRequestAction.Remove(action); 
            if (_onEditorRefreshRequestAction.Count == 0) {
                GetParent().Disconnect("editor_refresh_request", this, nameof(_GodotSignalEditorRefreshRequest));
            }
            return this;
        }
        private void _GodotSignalEditorRefreshRequest() {
            if (_onEditorRefreshRequestAction == null || _onEditorRefreshRequestAction.Count == 0) return;
            for (var i = 0; i < _onEditorRefreshRequestAction.Count; i++) _onEditorRefreshRequestAction[i].Invoke();
        }
        

        private List<Action>? _onInputTypeChangedAction; 
        public VisualShaderNodeInputAction OnInputTypeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onInputTypeChangedAction == null || _onInputTypeChangedAction.Count == 0) {
                _onInputTypeChangedAction ??= new List<Action>(); 
                GetParent().Connect("input_type_changed", this, nameof(_GodotSignalInputTypeChanged));
            }
            _onInputTypeChangedAction.Add(action);
            return this;
        }
        public VisualShaderNodeInputAction RemoveOnInputTypeChanged(Action action) {
            if (_onInputTypeChangedAction == null || _onInputTypeChangedAction.Count == 0) return this;
            _onInputTypeChangedAction.Remove(action); 
            if (_onInputTypeChangedAction.Count == 0) {
                GetParent().Disconnect("input_type_changed", this, nameof(_GodotSignalInputTypeChanged));
            }
            return this;
        }
        private void _GodotSignalInputTypeChanged() {
            if (_onInputTypeChangedAction == null || _onInputTypeChangedAction.Count == 0) return;
            for (var i = 0; i < _onInputTypeChangedAction.Count; i++) _onInputTypeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisualShaderNodeInputAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisualShaderNodeInputAction RemoveOnScriptChanged(Action action) {
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