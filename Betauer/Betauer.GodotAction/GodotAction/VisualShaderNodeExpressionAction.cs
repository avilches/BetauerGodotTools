using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeExpressionAction : VisualShaderNodeExpression {


        private Action? _onChangedAction; 
        public VisualShaderNodeExpressionAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public VisualShaderNodeExpressionAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onEditorRefreshRequestAction; 
        public VisualShaderNodeExpressionAction OnEditorRefreshRequest(Action action) {
            if (_onEditorRefreshRequestAction == null) 
                Connect("editor_refresh_request", this, nameof(ExecuteEditorRefreshRequest));
            _onEditorRefreshRequestAction = action;
            return this;
        }
        public VisualShaderNodeExpressionAction RemoveOnEditorRefreshRequest() {
            if (_onEditorRefreshRequestAction == null) return this; 
            Disconnect("editor_refresh_request", this, nameof(ExecuteEditorRefreshRequest));
            _onEditorRefreshRequestAction = null;
            return this;
        }
        private void ExecuteEditorRefreshRequest() =>
            _onEditorRefreshRequestAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public VisualShaderNodeExpressionAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisualShaderNodeExpressionAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}