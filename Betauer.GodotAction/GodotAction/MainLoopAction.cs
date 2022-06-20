using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MainLoopAction : MainLoop {


        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public MainLoopAction OnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) {
                _onRequestPermissionsResultAction ??= new List<Action<bool, string>>(); 
                Connect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            }
            _onRequestPermissionsResultAction.Add(action);
            return this;
        }
        public MainLoopAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return this;
            _onRequestPermissionsResultAction.Remove(action); 
            if (_onRequestPermissionsResultAction.Count == 0) {
                Disconnect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            }
            return this;
        }
        private void ExecuteRequestPermissionsResult(bool granted, string permission) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return;
            for (var i = 0; i < _onRequestPermissionsResultAction.Count; i++) _onRequestPermissionsResultAction[i].Invoke(granted, permission);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public MainLoopAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public MainLoopAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        
    }
}