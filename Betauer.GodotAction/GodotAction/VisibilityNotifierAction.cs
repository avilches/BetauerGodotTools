using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisibilityNotifierAction : VisibilityNotifier {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public VisibilityNotifierAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public VisibilityNotifierAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public VisibilityNotifierAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public VisibilityNotifierAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public VisibilityNotifierAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public VisibilityNotifierAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public VisibilityNotifierAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action<Camera>>? _onCameraEnteredAction; 
        public VisibilityNotifierAction OnCameraEntered(Action<Camera> action) {
            if (_onCameraEnteredAction == null || _onCameraEnteredAction.Count == 0) {
                _onCameraEnteredAction ??= new List<Action<Camera>>(); 
                Connect("camera_entered", this, nameof(ExecuteCameraEntered));
            }
            _onCameraEnteredAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnCameraEntered(Action<Camera> action) {
            if (_onCameraEnteredAction == null || _onCameraEnteredAction.Count == 0) return this;
            _onCameraEnteredAction.Remove(action); 
            if (_onCameraEnteredAction.Count == 0) {
                Disconnect("camera_entered", this, nameof(ExecuteCameraEntered));
            }
            return this;
        }
        private void ExecuteCameraEntered(Camera camera) {
            if (_onCameraEnteredAction == null || _onCameraEnteredAction.Count == 0) return;
            for (var i = 0; i < _onCameraEnteredAction.Count; i++) _onCameraEnteredAction[i].Invoke(camera);
        }
        

        private List<Action<Camera>>? _onCameraExitedAction; 
        public VisibilityNotifierAction OnCameraExited(Action<Camera> action) {
            if (_onCameraExitedAction == null || _onCameraExitedAction.Count == 0) {
                _onCameraExitedAction ??= new List<Action<Camera>>(); 
                Connect("camera_exited", this, nameof(ExecuteCameraExited));
            }
            _onCameraExitedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnCameraExited(Action<Camera> action) {
            if (_onCameraExitedAction == null || _onCameraExitedAction.Count == 0) return this;
            _onCameraExitedAction.Remove(action); 
            if (_onCameraExitedAction.Count == 0) {
                Disconnect("camera_exited", this, nameof(ExecuteCameraExited));
            }
            return this;
        }
        private void ExecuteCameraExited(Camera camera) {
            if (_onCameraExitedAction == null || _onCameraExitedAction.Count == 0) return;
            for (var i = 0; i < _onCameraExitedAction.Count; i++) _onCameraExitedAction[i].Invoke(camera);
        }
        

        private List<Action>? _onGameplayEnteredAction; 
        public VisibilityNotifierAction OnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null || _onGameplayEnteredAction.Count == 0) {
                _onGameplayEnteredAction ??= new List<Action>(); 
                Connect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            }
            _onGameplayEnteredAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null || _onGameplayEnteredAction.Count == 0) return this;
            _onGameplayEnteredAction.Remove(action); 
            if (_onGameplayEnteredAction.Count == 0) {
                Disconnect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            }
            return this;
        }
        private void ExecuteGameplayEntered() {
            if (_onGameplayEnteredAction == null || _onGameplayEnteredAction.Count == 0) return;
            for (var i = 0; i < _onGameplayEnteredAction.Count; i++) _onGameplayEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onGameplayExitedAction; 
        public VisibilityNotifierAction OnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null || _onGameplayExitedAction.Count == 0) {
                _onGameplayExitedAction ??= new List<Action>(); 
                Connect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            }
            _onGameplayExitedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null || _onGameplayExitedAction.Count == 0) return this;
            _onGameplayExitedAction.Remove(action); 
            if (_onGameplayExitedAction.Count == 0) {
                Disconnect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            }
            return this;
        }
        private void ExecuteGameplayExited() {
            if (_onGameplayExitedAction == null || _onGameplayExitedAction.Count == 0) return;
            for (var i = 0; i < _onGameplayExitedAction.Count; i++) _onGameplayExitedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public VisibilityNotifierAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnReady(Action action) {
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
        public VisibilityNotifierAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnRenamed(Action action) {
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
        

        private List<Action>? _onScreenEnteredAction; 
        public VisibilityNotifierAction OnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) {
                _onScreenEnteredAction ??= new List<Action>(); 
                Connect("screen_entered", this, nameof(ExecuteScreenEntered));
            }
            _onScreenEnteredAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) return this;
            _onScreenEnteredAction.Remove(action); 
            if (_onScreenEnteredAction.Count == 0) {
                Disconnect("screen_entered", this, nameof(ExecuteScreenEntered));
            }
            return this;
        }
        private void ExecuteScreenEntered() {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) return;
            for (var i = 0; i < _onScreenEnteredAction.Count; i++) _onScreenEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onScreenExitedAction; 
        public VisibilityNotifierAction OnScreenExited(Action action) {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) {
                _onScreenExitedAction ??= new List<Action>(); 
                Connect("screen_exited", this, nameof(ExecuteScreenExited));
            }
            _onScreenExitedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnScreenExited(Action action) {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) return this;
            _onScreenExitedAction.Remove(action); 
            if (_onScreenExitedAction.Count == 0) {
                Disconnect("screen_exited", this, nameof(ExecuteScreenExited));
            }
            return this;
        }
        private void ExecuteScreenExited() {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) return;
            for (var i = 0; i < _onScreenExitedAction.Count; i++) _onScreenExitedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisibilityNotifierAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnScriptChanged(Action action) {
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
        public VisibilityNotifierAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeEntered(Action action) {
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
        public VisibilityNotifierAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeExited(Action action) {
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
        public VisibilityNotifierAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnTreeExiting(Action action) {
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
        

        private List<Action>? _onVisibilityChangedAction; 
        public VisibilityNotifierAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public VisibilityNotifierAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            return this;
        }
        private void ExecuteVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}