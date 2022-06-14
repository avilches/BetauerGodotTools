using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeStateMachineTransitionAction : AnimationNodeStateMachineTransition {


        private Action? _onAdvanceConditionChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnAdvanceConditionChanged(Action action) {
            if (_onAdvanceConditionChangedAction == null) 
                Connect("advance_condition_changed", this, nameof(ExecuteAdvanceConditionChanged));
            _onAdvanceConditionChangedAction = action;
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnAdvanceConditionChanged() {
            if (_onAdvanceConditionChangedAction == null) return this; 
            Disconnect("advance_condition_changed", this, nameof(ExecuteAdvanceConditionChanged));
            _onAdvanceConditionChangedAction = null;
            return this;
        }
        private void ExecuteAdvanceConditionChanged() =>
            _onAdvanceConditionChangedAction?.Invoke();
        

        private Action? _onChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}