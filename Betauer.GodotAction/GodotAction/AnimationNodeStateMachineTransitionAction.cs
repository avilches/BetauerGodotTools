using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeStateMachineTransitionAction : ProxyNode {

        private List<Action>? _onAdvanceConditionChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnAdvanceConditionChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAdvanceConditionChangedAction, "advance_condition_changed", nameof(_GodotSignalAdvanceConditionChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineTransitionAction RemoveOnAdvanceConditionChanged(Action action) {
            RemoveSignal(_onAdvanceConditionChangedAction, "advance_condition_changed", nameof(_GodotSignalAdvanceConditionChanged), action);
            return this;
        }

        private AnimationNodeStateMachineTransitionAction _GodotSignalAdvanceConditionChanged() {
            ExecuteSignal(_onAdvanceConditionChangedAction);
            return this;
        }

        private List<Action>? _onChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineTransitionAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationNodeStateMachineTransitionAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeStateMachineTransitionAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationNodeStateMachineTransitionAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}