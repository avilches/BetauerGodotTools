using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class MethodTweenerAction : ProxyNode {

        private List<Action>? _onFinishedAction; 
        public MethodTweenerAction OnFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFinishedAction, "finished", nameof(_GodotSignalFinished), action, oneShot, deferred);
            return this;
        }

        public MethodTweenerAction RemoveOnFinished(Action action) {
            RemoveSignal(_onFinishedAction, "finished", nameof(_GodotSignalFinished), action);
            return this;
        }

        private MethodTweenerAction _GodotSignalFinished() {
            ExecuteSignal(_onFinishedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public MethodTweenerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public MethodTweenerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private MethodTweenerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}