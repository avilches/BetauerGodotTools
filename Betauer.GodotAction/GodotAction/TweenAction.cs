using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TweenAction : Tween {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public TweenAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public TweenAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public TweenAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public TweenAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public TweenAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public TweenAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessAction == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessAction.Count; i++) _onProcessAction[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcess == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcess.Count; i++) _onPhysicsProcess[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInput == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInput.Count; i++) _onInput[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInput == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInput.Count; i++) _onUnhandledInput[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInput == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInput.Count; i++) _onUnhandledKeyInput[i].Invoke(@event);
        }

        private Action? _onReadyAction; 
        public TweenAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public TweenAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public TweenAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public TweenAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public TweenAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public TweenAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public TweenAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public TweenAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public TweenAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public TweenAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public TweenAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public TweenAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onTweenAllCompletedAction; 
        public TweenAction OnTweenAllCompleted(Action action) {
            if (_onTweenAllCompletedAction == null) 
                Connect("tween_all_completed", this, nameof(ExecuteTweenAllCompleted));
            _onTweenAllCompletedAction = action;
            return this;
        }
        public TweenAction RemoveOnTweenAllCompleted() {
            if (_onTweenAllCompletedAction == null) return this; 
            Disconnect("tween_all_completed", this, nameof(ExecuteTweenAllCompleted));
            _onTweenAllCompletedAction = null;
            return this;
        }
        private void ExecuteTweenAllCompleted() =>
            _onTweenAllCompletedAction?.Invoke();
        

        private Action<Object, NodePath>? _onTweenCompletedAction; 
        public TweenAction OnTweenCompleted(Action<Object, NodePath> action) {
            if (_onTweenCompletedAction == null) 
                Connect("tween_completed", this, nameof(ExecuteTweenCompleted));
            _onTweenCompletedAction = action;
            return this;
        }
        public TweenAction RemoveOnTweenCompleted() {
            if (_onTweenCompletedAction == null) return this; 
            Disconnect("tween_completed", this, nameof(ExecuteTweenCompleted));
            _onTweenCompletedAction = null;
            return this;
        }
        private void ExecuteTweenCompleted(Object @object, NodePath key) =>
            _onTweenCompletedAction?.Invoke(@object, key);
        

        private Action<Object, NodePath>? _onTweenStartedAction; 
        public TweenAction OnTweenStarted(Action<Object, NodePath> action) {
            if (_onTweenStartedAction == null) 
                Connect("tween_started", this, nameof(ExecuteTweenStarted));
            _onTweenStartedAction = action;
            return this;
        }
        public TweenAction RemoveOnTweenStarted() {
            if (_onTweenStartedAction == null) return this; 
            Disconnect("tween_started", this, nameof(ExecuteTweenStarted));
            _onTweenStartedAction = null;
            return this;
        }
        private void ExecuteTweenStarted(Object @object, NodePath key) =>
            _onTweenStartedAction?.Invoke(@object, key);
        

        private Action<Object, float, NodePath, Object>? _onTweenStepAction; 
        public TweenAction OnTweenStep(Action<Object, float, NodePath, Object> action) {
            if (_onTweenStepAction == null) 
                Connect("tween_step", this, nameof(ExecuteTweenStep));
            _onTweenStepAction = action;
            return this;
        }
        public TweenAction RemoveOnTweenStep() {
            if (_onTweenStepAction == null) return this; 
            Disconnect("tween_step", this, nameof(ExecuteTweenStep));
            _onTweenStepAction = null;
            return this;
        }
        private void ExecuteTweenStep(Object @object, float elapsed, NodePath key, Object value) =>
            _onTweenStepAction?.Invoke(@object, elapsed, key, value);
        
    }
}