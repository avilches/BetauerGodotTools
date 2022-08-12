using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class MainLoopAction : ProxyNode {

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public MainLoopAction OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action, oneShot, deferred);
            return this;
        }

        public MainLoopAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            RemoveSignal(_onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action);
            return this;
        }

        private MainLoopAction _GodotSignalRequestPermissionsResult(bool granted, string permission) {
            ExecuteSignal(_onRequestPermissionsResultAction, granted, permission);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public MainLoopAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public MainLoopAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private MainLoopAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}