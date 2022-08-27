using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class PropertyTweenerAction : ProxyNode {

        private List<Action>? _onFinishedAction; 
        public PropertyTweenerAction OnFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFinishedAction, "finished", nameof(_GodotSignalFinished), action, oneShot, deferred);
            return this;
        }

        public PropertyTweenerAction RemoveOnFinished(Action action) {
            RemoveSignal(_onFinishedAction, "finished", nameof(_GodotSignalFinished), action);
            return this;
        }

        private PropertyTweenerAction _GodotSignalFinished() {
            ExecuteSignal(_onFinishedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public PropertyTweenerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public PropertyTweenerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private PropertyTweenerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}