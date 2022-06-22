using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TweenAction : Tween {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public TweenAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public TweenAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public TweenAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public TweenAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public TweenAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public TweenAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public TweenAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessActions == null || _onProcessActions.Count == 0) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null || _onPhysicsProcessActions.Count == 0) {
                SetPhysicsProcess(false);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null || _onInputActions?.Count == 0) {
                SetProcessInput(false);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null || _onUnhandledInputActions.Count == 0) {
                SetProcessUnhandledInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null || _onUnhandledKeyInputActions.Count == 0) {
                SetProcessUnhandledKeyInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

        private List<Action>? _onReadyAction; 
        public TweenAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(_GodotSignalReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                Disconnect("ready", this, nameof(_GodotSignalReady));
            }
            return this;
        }
        private void _GodotSignalReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public TweenAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(_GodotSignalRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                Disconnect("renamed", this, nameof(_GodotSignalRenamed));
            }
            return this;
        }
        private void _GodotSignalRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public TweenAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public TweenAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                Disconnect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            return this;
        }
        private void _GodotSignalTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public TweenAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                Disconnect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            return this;
        }
        private void _GodotSignalTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public TweenAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                Disconnect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            return this;
        }
        private void _GodotSignalTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onTweenAllCompletedAction; 
        public TweenAction OnTweenAllCompleted(Action action) {
            if (_onTweenAllCompletedAction == null || _onTweenAllCompletedAction.Count == 0) {
                _onTweenAllCompletedAction ??= new List<Action>(); 
                Connect("tween_all_completed", this, nameof(_GodotSignalTweenAllCompleted));
            }
            _onTweenAllCompletedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTweenAllCompleted(Action action) {
            if (_onTweenAllCompletedAction == null || _onTweenAllCompletedAction.Count == 0) return this;
            _onTweenAllCompletedAction.Remove(action); 
            if (_onTweenAllCompletedAction.Count == 0) {
                Disconnect("tween_all_completed", this, nameof(_GodotSignalTweenAllCompleted));
            }
            return this;
        }
        private void _GodotSignalTweenAllCompleted() {
            if (_onTweenAllCompletedAction == null || _onTweenAllCompletedAction.Count == 0) return;
            for (var i = 0; i < _onTweenAllCompletedAction.Count; i++) _onTweenAllCompletedAction[i].Invoke();
        }
        

        private List<Action<Object, NodePath>>? _onTweenCompletedAction; 
        public TweenAction OnTweenCompleted(Action<Object, NodePath> action) {
            if (_onTweenCompletedAction == null || _onTweenCompletedAction.Count == 0) {
                _onTweenCompletedAction ??= new List<Action<Object, NodePath>>(); 
                Connect("tween_completed", this, nameof(_GodotSignalTweenCompleted));
            }
            _onTweenCompletedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTweenCompleted(Action<Object, NodePath> action) {
            if (_onTweenCompletedAction == null || _onTweenCompletedAction.Count == 0) return this;
            _onTweenCompletedAction.Remove(action); 
            if (_onTweenCompletedAction.Count == 0) {
                Disconnect("tween_completed", this, nameof(_GodotSignalTweenCompleted));
            }
            return this;
        }
        private void _GodotSignalTweenCompleted(Object @object, NodePath key) {
            if (_onTweenCompletedAction == null || _onTweenCompletedAction.Count == 0) return;
            for (var i = 0; i < _onTweenCompletedAction.Count; i++) _onTweenCompletedAction[i].Invoke(@object, key);
        }
        

        private List<Action<Object, NodePath>>? _onTweenStartedAction; 
        public TweenAction OnTweenStarted(Action<Object, NodePath> action) {
            if (_onTweenStartedAction == null || _onTweenStartedAction.Count == 0) {
                _onTweenStartedAction ??= new List<Action<Object, NodePath>>(); 
                Connect("tween_started", this, nameof(_GodotSignalTweenStarted));
            }
            _onTweenStartedAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTweenStarted(Action<Object, NodePath> action) {
            if (_onTweenStartedAction == null || _onTweenStartedAction.Count == 0) return this;
            _onTweenStartedAction.Remove(action); 
            if (_onTweenStartedAction.Count == 0) {
                Disconnect("tween_started", this, nameof(_GodotSignalTweenStarted));
            }
            return this;
        }
        private void _GodotSignalTweenStarted(Object @object, NodePath key) {
            if (_onTweenStartedAction == null || _onTweenStartedAction.Count == 0) return;
            for (var i = 0; i < _onTweenStartedAction.Count; i++) _onTweenStartedAction[i].Invoke(@object, key);
        }
        

        private List<Action<Object, float, NodePath, Object>>? _onTweenStepAction; 
        public TweenAction OnTweenStep(Action<Object, float, NodePath, Object> action) {
            if (_onTweenStepAction == null || _onTweenStepAction.Count == 0) {
                _onTweenStepAction ??= new List<Action<Object, float, NodePath, Object>>(); 
                Connect("tween_step", this, nameof(_GodotSignalTweenStep));
            }
            _onTweenStepAction.Add(action);
            return this;
        }
        public TweenAction RemoveOnTweenStep(Action<Object, float, NodePath, Object> action) {
            if (_onTweenStepAction == null || _onTweenStepAction.Count == 0) return this;
            _onTweenStepAction.Remove(action); 
            if (_onTweenStepAction.Count == 0) {
                Disconnect("tween_step", this, nameof(_GodotSignalTweenStep));
            }
            return this;
        }
        private void _GodotSignalTweenStep(Object @object, float elapsed, NodePath key, Object value) {
            if (_onTweenStepAction == null || _onTweenStepAction.Count == 0) return;
            for (var i = 0; i < _onTweenStepAction.Count; i++) _onTweenStepAction[i].Invoke(@object, elapsed, key, value);
        }
        
    }
}