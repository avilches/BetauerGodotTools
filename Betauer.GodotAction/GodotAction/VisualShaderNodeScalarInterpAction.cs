using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeScalarInterpAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualShaderNodeScalarInterpAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeScalarInterpAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualShaderNodeScalarInterpAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onEditorRefreshRequestAction; 
        public VisualShaderNodeScalarInterpAction OnEditorRefreshRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeScalarInterpAction RemoveOnEditorRefreshRequest(Action action) {
            RemoveSignal(_onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action);
            return this;
        }

        private VisualShaderNodeScalarInterpAction _GodotSignalEditorRefreshRequest() {
            ExecuteSignal(_onEditorRefreshRequestAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualShaderNodeScalarInterpAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeScalarInterpAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualShaderNodeScalarInterpAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}