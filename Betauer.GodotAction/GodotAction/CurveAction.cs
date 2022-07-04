using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class CurveAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public CurveAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public CurveAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private CurveAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onRangeChangedAction; 
        public CurveAction OnRangeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRangeChangedAction, "range_changed", nameof(_GodotSignalRangeChanged), action, oneShot, deferred);
            return this;
        }

        public CurveAction RemoveOnRangeChanged(Action action) {
            RemoveSignal(_onRangeChangedAction, "range_changed", nameof(_GodotSignalRangeChanged), action);
            return this;
        }

        private CurveAction _GodotSignalRangeChanged() {
            ExecuteSignal(_onRangeChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public CurveAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public CurveAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private CurveAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}