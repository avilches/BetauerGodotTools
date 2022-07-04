using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeInputAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualShaderNodeInputAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeInputAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualShaderNodeInputAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onEditorRefreshRequestAction; 
        public VisualShaderNodeInputAction OnEditorRefreshRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeInputAction RemoveOnEditorRefreshRequest(Action action) {
            RemoveSignal(_onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action);
            return this;
        }

        private VisualShaderNodeInputAction _GodotSignalEditorRefreshRequest() {
            ExecuteSignal(_onEditorRefreshRequestAction);
            return this;
        }

        private List<Action>? _onInputTypeChangedAction; 
        public VisualShaderNodeInputAction OnInputTypeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onInputTypeChangedAction, "input_type_changed", nameof(_GodotSignalInputTypeChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeInputAction RemoveOnInputTypeChanged(Action action) {
            RemoveSignal(_onInputTypeChangedAction, "input_type_changed", nameof(_GodotSignalInputTypeChanged), action);
            return this;
        }

        private VisualShaderNodeInputAction _GodotSignalInputTypeChanged() {
            ExecuteSignal(_onInputTypeChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualShaderNodeInputAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeInputAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualShaderNodeInputAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}