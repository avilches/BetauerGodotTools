using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MainLoopAction : ProxyNode {

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public void OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action, oneShot, deferred);

        public void RemoveOnRequestPermissionsResult(Action<bool, string> action) =>
            RemoveSignal(_onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action);

        private void _GodotSignalRequestPermissionsResult(bool granted, string permission) =>
            ExecuteSignal(_onRequestPermissionsResultAction, granted, permission);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        
    }
}