using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class IntervalTweenerAction : ProxyNode {

        private List<Action>? _onFinishedAction; 
        public IntervalTweenerAction OnFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFinishedAction, "finished", nameof(_GodotSignalFinished), action, oneShot, deferred);
            return this;
        }

        public IntervalTweenerAction RemoveOnFinished(Action action) {
            RemoveSignal(_onFinishedAction, "finished", nameof(_GodotSignalFinished), action);
            return this;
        }

        private IntervalTweenerAction _GodotSignalFinished() {
            ExecuteSignal(_onFinishedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public IntervalTweenerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public IntervalTweenerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private IntervalTweenerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}