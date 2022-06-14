using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisibilityNotifierAction : VisibilityNotifier {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public VisibilityNotifierAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public VisibilityNotifierAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public VisibilityNotifierAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public VisibilityNotifierAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public VisibilityNotifierAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public VisibilityNotifierAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action<Camera>? _onCameraEnteredAction; 
        public VisibilityNotifierAction OnCameraEntered(Action<Camera> action) {
            if (_onCameraEnteredAction == null) 
                Connect("camera_entered", this, nameof(ExecuteCameraEntered));
            _onCameraEnteredAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnCameraEntered() {
            if (_onCameraEnteredAction == null) return this; 
            Disconnect("camera_entered", this, nameof(ExecuteCameraEntered));
            _onCameraEnteredAction = null;
            return this;
        }
        private void ExecuteCameraEntered(Camera camera) =>
            _onCameraEnteredAction?.Invoke(camera);
        

        private Action<Camera>? _onCameraExitedAction; 
        public VisibilityNotifierAction OnCameraExited(Action<Camera> action) {
            if (_onCameraExitedAction == null) 
                Connect("camera_exited", this, nameof(ExecuteCameraExited));
            _onCameraExitedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnCameraExited() {
            if (_onCameraExitedAction == null) return this; 
            Disconnect("camera_exited", this, nameof(ExecuteCameraExited));
            _onCameraExitedAction = null;
            return this;
        }
        private void ExecuteCameraExited(Camera camera) =>
            _onCameraExitedAction?.Invoke(camera);
        

        private Action? _onGameplayEnteredAction; 
        public VisibilityNotifierAction OnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null) 
                Connect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnGameplayEntered() {
            if (_onGameplayEnteredAction == null) return this; 
            Disconnect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = null;
            return this;
        }
        private void ExecuteGameplayEntered() =>
            _onGameplayEnteredAction?.Invoke();
        

        private Action? _onGameplayExitedAction; 
        public VisibilityNotifierAction OnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null) 
                Connect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnGameplayExited() {
            if (_onGameplayExitedAction == null) return this; 
            Disconnect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = null;
            return this;
        }
        private void ExecuteGameplayExited() =>
            _onGameplayExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public VisibilityNotifierAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public VisibilityNotifierAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScreenEnteredAction; 
        public VisibilityNotifierAction OnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null) 
                Connect("screen_entered", this, nameof(ExecuteScreenEntered));
            _onScreenEnteredAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnScreenEntered() {
            if (_onScreenEnteredAction == null) return this; 
            Disconnect("screen_entered", this, nameof(ExecuteScreenEntered));
            _onScreenEnteredAction = null;
            return this;
        }
        private void ExecuteScreenEntered() =>
            _onScreenEnteredAction?.Invoke();
        

        private Action? _onScreenExitedAction; 
        public VisibilityNotifierAction OnScreenExited(Action action) {
            if (_onScreenExitedAction == null) 
                Connect("screen_exited", this, nameof(ExecuteScreenExited));
            _onScreenExitedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnScreenExited() {
            if (_onScreenExitedAction == null) return this; 
            Disconnect("screen_exited", this, nameof(ExecuteScreenExited));
            _onScreenExitedAction = null;
            return this;
        }
        private void ExecuteScreenExited() =>
            _onScreenExitedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public VisibilityNotifierAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public VisibilityNotifierAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public VisibilityNotifierAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public VisibilityNotifierAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public VisibilityNotifierAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public VisibilityNotifierAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}