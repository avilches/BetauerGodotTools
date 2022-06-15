using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class RemoteTransformAction : RemoteTransform {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public RemoteTransformAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public RemoteTransformAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public RemoteTransformAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public RemoteTransformAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public RemoteTransformAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public RemoteTransformAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public RemoteTransformAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public RemoteTransformAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public RemoteTransformAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public RemoteTransformAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action? _onGameplayEnteredAction; 
        public RemoteTransformAction OnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null) 
                Connect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnGameplayEntered() {
            if (_onGameplayEnteredAction == null) return this; 
            Disconnect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = null;
            return this;
        }
        private void ExecuteGameplayEntered() =>
            _onGameplayEnteredAction?.Invoke();
        

        private Action? _onGameplayExitedAction; 
        public RemoteTransformAction OnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null) 
                Connect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnGameplayExited() {
            if (_onGameplayExitedAction == null) return this; 
            Disconnect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = null;
            return this;
        }
        private void ExecuteGameplayExited() =>
            _onGameplayExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public RemoteTransformAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public RemoteTransformAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public RemoteTransformAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public RemoteTransformAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public RemoteTransformAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public RemoteTransformAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public RemoteTransformAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public RemoteTransformAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}