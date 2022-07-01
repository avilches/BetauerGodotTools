using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeScalarClampAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public void OnChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);

        public void RemoveOnChanged(Action action) =>
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);

        private void _GodotSignalChanged() =>
            ExecuteSignal(_onChangedAction);
        

        private List<Action>? _onEditorRefreshRequestAction; 
        public void OnEditorRefreshRequest(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action, oneShot, deferred);

        public void RemoveOnEditorRefreshRequest(Action action) =>
            RemoveSignal(_onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action);

        private void _GodotSignalEditorRefreshRequest() =>
            ExecuteSignal(_onEditorRefreshRequestAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        
    }
}