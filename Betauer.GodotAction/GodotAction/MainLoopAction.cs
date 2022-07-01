using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MainLoopAction : Node {
        public MainLoopAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public MainLoopAction OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) {
                _onRequestPermissionsResultAction ??= new List<Action<bool, string>>(); 
                GetParent().Connect("on_request_permissions_result", this, nameof(_GodotSignalRequestPermissionsResult));
            }
            _onRequestPermissionsResultAction.Add(action);
            return this;
        }
        public MainLoopAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return this;
            _onRequestPermissionsResultAction.Remove(action); 
            if (_onRequestPermissionsResultAction.Count == 0) {
                GetParent().Disconnect("on_request_permissions_result", this, nameof(_GodotSignalRequestPermissionsResult));
            }
            return this;
        }
        private void _GodotSignalRequestPermissionsResult(bool granted, string permission) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return;
            for (var i = 0; i < _onRequestPermissionsResultAction.Count; i++) _onRequestPermissionsResultAction[i].Invoke(granted, permission);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public MainLoopAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public MainLoopAction RemoveOnScriptChanged(Action action) {
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