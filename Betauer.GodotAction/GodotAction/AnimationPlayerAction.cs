using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationPlayerAction : AnimationPlayer {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public AnimationPlayerAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public AnimationPlayerAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public AnimationPlayerAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public AnimationPlayerAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public AnimationPlayerAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public AnimationPlayerAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public AnimationPlayerAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action<string, string>? _onAnimationChangedAction; 
        public AnimationPlayerAction OnAnimationChanged(Action<string, string> action) {
            if (_onAnimationChangedAction == null) 
                Connect("animation_changed", this, nameof(ExecuteAnimationChanged));
            _onAnimationChangedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationChanged() {
            if (_onAnimationChangedAction == null) return this; 
            Disconnect("animation_changed", this, nameof(ExecuteAnimationChanged));
            _onAnimationChangedAction = null;
            return this;
        }
        private void ExecuteAnimationChanged(string new_name, string old_name) =>
            _onAnimationChangedAction?.Invoke(new_name, old_name);
        

        private Action<string>? _onAnimationFinishedAction; 
        public AnimationPlayerAction OnAnimationFinished(Action<string> action) {
            if (_onAnimationFinishedAction == null) 
                Connect("animation_finished", this, nameof(ExecuteAnimationFinished));
            _onAnimationFinishedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationFinished() {
            if (_onAnimationFinishedAction == null) return this; 
            Disconnect("animation_finished", this, nameof(ExecuteAnimationFinished));
            _onAnimationFinishedAction = null;
            return this;
        }
        private void ExecuteAnimationFinished(string anim_name) =>
            _onAnimationFinishedAction?.Invoke(anim_name);
        

        private Action<string>? _onAnimationStartedAction; 
        public AnimationPlayerAction OnAnimationStarted(Action<string> action) {
            if (_onAnimationStartedAction == null) 
                Connect("animation_started", this, nameof(ExecuteAnimationStarted));
            _onAnimationStartedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnAnimationStarted() {
            if (_onAnimationStartedAction == null) return this; 
            Disconnect("animation_started", this, nameof(ExecuteAnimationStarted));
            _onAnimationStartedAction = null;
            return this;
        }
        private void ExecuteAnimationStarted(string anim_name) =>
            _onAnimationStartedAction?.Invoke(anim_name);
        

        private Action? _onCachesClearedAction; 
        public AnimationPlayerAction OnCachesCleared(Action action) {
            if (_onCachesClearedAction == null) 
                Connect("caches_cleared", this, nameof(ExecuteCachesCleared));
            _onCachesClearedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnCachesCleared() {
            if (_onCachesClearedAction == null) return this; 
            Disconnect("caches_cleared", this, nameof(ExecuteCachesCleared));
            _onCachesClearedAction = null;
            return this;
        }
        private void ExecuteCachesCleared() =>
            _onCachesClearedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public AnimationPlayerAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public AnimationPlayerAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationPlayerAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public AnimationPlayerAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public AnimationPlayerAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public AnimationPlayerAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public AnimationPlayerAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        
    }
}