using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationPlayerAction : AnimationPlayer {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public AnimationPlayerAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public AnimationPlayerAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public AnimationPlayerAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public AnimationPlayerAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public AnimationPlayerAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public AnimationPlayerAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action<string, string>>? _onAnimationChangedAction; 
        public AnimationPlayerAction OnAnimationChanged(Action<string, string> action) {
            if (_onAnimationChangedAction == null || _onAnimationChangedAction.Count == 0) {
                _onAnimationChangedAction ??= new List<Action<string, string>>(); 
                Connect("animation_changed", this, nameof(ExecuteAnimationChanged));
            }
            _onAnimationChangedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationChanged(Action<string, string> action) {
            if (_onAnimationChangedAction == null || _onAnimationChangedAction.Count == 0) return this;
            _onAnimationChangedAction.Remove(action); 
            if (_onAnimationChangedAction.Count == 0) {
                Disconnect("animation_changed", this, nameof(ExecuteAnimationChanged));
            }
            return this;
        }
        private void ExecuteAnimationChanged(string new_name, string old_name) {
            if (_onAnimationChangedAction == null || _onAnimationChangedAction.Count == 0) return;
            for (var i = 0; i < _onAnimationChangedAction.Count; i++) _onAnimationChangedAction[i].Invoke(new_name, old_name);
        }
        

        private List<Action<string>>? _onAnimationFinishedAction; 
        public AnimationPlayerAction OnAnimationFinished(Action<string> action) {
            if (_onAnimationFinishedAction == null || _onAnimationFinishedAction.Count == 0) {
                _onAnimationFinishedAction ??= new List<Action<string>>(); 
                Connect("animation_finished", this, nameof(ExecuteAnimationFinished));
            }
            _onAnimationFinishedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationFinished(Action<string> action) {
            if (_onAnimationFinishedAction == null || _onAnimationFinishedAction.Count == 0) return this;
            _onAnimationFinishedAction.Remove(action); 
            if (_onAnimationFinishedAction.Count == 0) {
                Disconnect("animation_finished", this, nameof(ExecuteAnimationFinished));
            }
            return this;
        }
        private void ExecuteAnimationFinished(string anim_name) {
            if (_onAnimationFinishedAction == null || _onAnimationFinishedAction.Count == 0) return;
            for (var i = 0; i < _onAnimationFinishedAction.Count; i++) _onAnimationFinishedAction[i].Invoke(anim_name);
        }
        

        private List<Action<string>>? _onAnimationStartedAction; 
        public AnimationPlayerAction OnAnimationStarted(Action<string> action) {
            if (_onAnimationStartedAction == null || _onAnimationStartedAction.Count == 0) {
                _onAnimationStartedAction ??= new List<Action<string>>(); 
                Connect("animation_started", this, nameof(ExecuteAnimationStarted));
            }
            _onAnimationStartedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationStarted(Action<string> action) {
            if (_onAnimationStartedAction == null || _onAnimationStartedAction.Count == 0) return this;
            _onAnimationStartedAction.Remove(action); 
            if (_onAnimationStartedAction.Count == 0) {
                Disconnect("animation_started", this, nameof(ExecuteAnimationStarted));
            }
            return this;
        }
        private void ExecuteAnimationStarted(string anim_name) {
            if (_onAnimationStartedAction == null || _onAnimationStartedAction.Count == 0) return;
            for (var i = 0; i < _onAnimationStartedAction.Count; i++) _onAnimationStartedAction[i].Invoke(anim_name);
        }
        

        private List<Action>? _onCachesClearedAction; 
        public AnimationPlayerAction OnCachesCleared(Action action) {
            if (_onCachesClearedAction == null || _onCachesClearedAction.Count == 0) {
                _onCachesClearedAction ??= new List<Action>(); 
                Connect("caches_cleared", this, nameof(ExecuteCachesCleared));
            }
            _onCachesClearedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnCachesCleared(Action action) {
            if (_onCachesClearedAction == null || _onCachesClearedAction.Count == 0) return this;
            _onCachesClearedAction.Remove(action); 
            if (_onCachesClearedAction.Count == 0) {
                Disconnect("caches_cleared", this, nameof(ExecuteCachesCleared));
            }
            return this;
        }
        private void ExecuteCachesCleared() {
            if (_onCachesClearedAction == null || _onCachesClearedAction.Count == 0) return;
            for (var i = 0; i < _onCachesClearedAction.Count; i++) _onCachesClearedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public AnimationPlayerAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                Disconnect("ready", this, nameof(ExecuteReady));
            }
            return this;
        }
        private void ExecuteReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public AnimationPlayerAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                Disconnect("renamed", this, nameof(ExecuteRenamed));
            }
            return this;
        }
        private void ExecuteRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationPlayerAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public AnimationPlayerAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            return this;
        }
        private void ExecuteTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public AnimationPlayerAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            return this;
        }
        private void ExecuteTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public AnimationPlayerAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            return this;
        }
        private void ExecuteTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        
    }
}