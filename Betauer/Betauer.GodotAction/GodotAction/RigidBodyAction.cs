using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class RigidBodyAction : RigidBody {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public RigidBodyAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public RigidBodyAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public RigidBodyAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public RigidBodyAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public RigidBodyAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public RigidBodyAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public RigidBodyAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public RigidBodyAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public RigidBodyAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public RigidBodyAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action<Node>? _onBodyEnteredAction; 
        public RigidBodyAction OnBodyEntered(Action<Node> action) {
            if (_onBodyEnteredAction == null) 
                Connect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnBodyEntered() {
            if (_onBodyEnteredAction == null) return this; 
            Disconnect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = null;
            return this;
        }
        private void ExecuteBodyEntered(Node body) =>
            _onBodyEnteredAction?.Invoke(body);
        

        private Action<Node>? _onBodyExitedAction; 
        public RigidBodyAction OnBodyExited(Action<Node> action) {
            if (_onBodyExitedAction == null) 
                Connect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnBodyExited() {
            if (_onBodyExitedAction == null) return this; 
            Disconnect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = null;
            return this;
        }
        private void ExecuteBodyExited(Node body) =>
            _onBodyExitedAction?.Invoke(body);
        

        private Action<Node, RID, int, int>? _onBodyShapeEnteredAction; 
        public RigidBodyAction OnBodyShapeEntered(Action<Node, RID, int, int> action) {
            if (_onBodyShapeEnteredAction == null) 
                Connect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnBodyShapeEntered() {
            if (_onBodyShapeEnteredAction == null) return this; 
            Disconnect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = null;
            return this;
        }
        private void ExecuteBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeEnteredAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action<Node, RID, int, int>? _onBodyShapeExitedAction; 
        public RigidBodyAction OnBodyShapeExited(Action<Node, RID, int, int> action) {
            if (_onBodyShapeExitedAction == null) 
                Connect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnBodyShapeExited() {
            if (_onBodyShapeExitedAction == null) return this; 
            Disconnect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = null;
            return this;
        }
        private void ExecuteBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeExitedAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action? _onGameplayEnteredAction; 
        public RigidBodyAction OnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null) 
                Connect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnGameplayEntered() {
            if (_onGameplayEnteredAction == null) return this; 
            Disconnect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = null;
            return this;
        }
        private void ExecuteGameplayEntered() =>
            _onGameplayEnteredAction?.Invoke();
        

        private Action? _onGameplayExitedAction; 
        public RigidBodyAction OnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null) 
                Connect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnGameplayExited() {
            if (_onGameplayExitedAction == null) return this; 
            Disconnect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = null;
            return this;
        }
        private void ExecuteGameplayExited() =>
            _onGameplayExitedAction?.Invoke();
        

        private Action<InputEvent, Node, Vector3, Vector3, int>? _onInputEventAction; 
        public RigidBodyAction OnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action) {
            if (_onInputEventAction == null) 
                Connect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnInputEvent() {
            if (_onInputEventAction == null) return this; 
            Disconnect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = null;
            return this;
        }
        private void ExecuteInputEvent(InputEvent @event, Node camera, Vector3 normal, Vector3 position, int shape_idx) =>
            _onInputEventAction?.Invoke(@event, camera, normal, position, shape_idx);
        

        private Action? _onMouseEnteredAction; 
        public RigidBodyAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public RigidBodyAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public RigidBodyAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public RigidBodyAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public RigidBodyAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onSleepingStateChangedAction; 
        public RigidBodyAction OnSleepingStateChanged(Action action) {
            if (_onSleepingStateChangedAction == null) 
                Connect("sleeping_state_changed", this, nameof(ExecuteSleepingStateChanged));
            _onSleepingStateChangedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnSleepingStateChanged() {
            if (_onSleepingStateChangedAction == null) return this; 
            Disconnect("sleeping_state_changed", this, nameof(ExecuteSleepingStateChanged));
            _onSleepingStateChangedAction = null;
            return this;
        }
        private void ExecuteSleepingStateChanged() =>
            _onSleepingStateChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public RigidBodyAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public RigidBodyAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public RigidBodyAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public RigidBodyAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public RigidBodyAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}