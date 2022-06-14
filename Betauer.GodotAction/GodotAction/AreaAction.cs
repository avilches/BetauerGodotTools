using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AreaAction : Area {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public AreaAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public AreaAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public AreaAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public AreaAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public AreaAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public AreaAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public AreaAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public AreaAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public AreaAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public AreaAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action<Area>? _onAreaEnteredAction; 
        public AreaAction OnAreaEntered(Action<Area> action) {
            if (_onAreaEnteredAction == null) 
                Connect("area_entered", this, nameof(ExecuteAreaEntered));
            _onAreaEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnAreaEntered() {
            if (_onAreaEnteredAction == null) return this; 
            Disconnect("area_entered", this, nameof(ExecuteAreaEntered));
            _onAreaEnteredAction = null;
            return this;
        }
        private void ExecuteAreaEntered(Area area) =>
            _onAreaEnteredAction?.Invoke(area);
        

        private Action<Area>? _onAreaExitedAction; 
        public AreaAction OnAreaExited(Action<Area> action) {
            if (_onAreaExitedAction == null) 
                Connect("area_exited", this, nameof(ExecuteAreaExited));
            _onAreaExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnAreaExited() {
            if (_onAreaExitedAction == null) return this; 
            Disconnect("area_exited", this, nameof(ExecuteAreaExited));
            _onAreaExitedAction = null;
            return this;
        }
        private void ExecuteAreaExited(Area area) =>
            _onAreaExitedAction?.Invoke(area);
        

        private Action<Area, RID, int, int>? _onAreaShapeEnteredAction; 
        public AreaAction OnAreaShapeEntered(Action<Area, RID, int, int> action) {
            if (_onAreaShapeEnteredAction == null) 
                Connect("area_shape_entered", this, nameof(ExecuteAreaShapeEntered));
            _onAreaShapeEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnAreaShapeEntered() {
            if (_onAreaShapeEnteredAction == null) return this; 
            Disconnect("area_shape_entered", this, nameof(ExecuteAreaShapeEntered));
            _onAreaShapeEnteredAction = null;
            return this;
        }
        private void ExecuteAreaShapeEntered(Area area, RID area_rid, int area_shape_index, int local_shape_index) =>
            _onAreaShapeEnteredAction?.Invoke(area, area_rid, area_shape_index, local_shape_index);
        

        private Action<Area, RID, int, int>? _onAreaShapeExitedAction; 
        public AreaAction OnAreaShapeExited(Action<Area, RID, int, int> action) {
            if (_onAreaShapeExitedAction == null) 
                Connect("area_shape_exited", this, nameof(ExecuteAreaShapeExited));
            _onAreaShapeExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnAreaShapeExited() {
            if (_onAreaShapeExitedAction == null) return this; 
            Disconnect("area_shape_exited", this, nameof(ExecuteAreaShapeExited));
            _onAreaShapeExitedAction = null;
            return this;
        }
        private void ExecuteAreaShapeExited(Area area, RID area_rid, int area_shape_index, int local_shape_index) =>
            _onAreaShapeExitedAction?.Invoke(area, area_rid, area_shape_index, local_shape_index);
        

        private Action<Node>? _onBodyEnteredAction; 
        public AreaAction OnBodyEntered(Action<Node> action) {
            if (_onBodyEnteredAction == null) 
                Connect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnBodyEntered() {
            if (_onBodyEnteredAction == null) return this; 
            Disconnect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = null;
            return this;
        }
        private void ExecuteBodyEntered(Node body) =>
            _onBodyEnteredAction?.Invoke(body);
        

        private Action<Node>? _onBodyExitedAction; 
        public AreaAction OnBodyExited(Action<Node> action) {
            if (_onBodyExitedAction == null) 
                Connect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnBodyExited() {
            if (_onBodyExitedAction == null) return this; 
            Disconnect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = null;
            return this;
        }
        private void ExecuteBodyExited(Node body) =>
            _onBodyExitedAction?.Invoke(body);
        

        private Action<Node, RID, int, int>? _onBodyShapeEnteredAction; 
        public AreaAction OnBodyShapeEntered(Action<Node, RID, int, int> action) {
            if (_onBodyShapeEnteredAction == null) 
                Connect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnBodyShapeEntered() {
            if (_onBodyShapeEnteredAction == null) return this; 
            Disconnect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = null;
            return this;
        }
        private void ExecuteBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeEnteredAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action<Node, RID, int, int>? _onBodyShapeExitedAction; 
        public AreaAction OnBodyShapeExited(Action<Node, RID, int, int> action) {
            if (_onBodyShapeExitedAction == null) 
                Connect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnBodyShapeExited() {
            if (_onBodyShapeExitedAction == null) return this; 
            Disconnect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = null;
            return this;
        }
        private void ExecuteBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeExitedAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action? _onGameplayEnteredAction; 
        public AreaAction OnGameplayEntered(Action action) {
            if (_onGameplayEnteredAction == null) 
                Connect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnGameplayEntered() {
            if (_onGameplayEnteredAction == null) return this; 
            Disconnect("gameplay_entered", this, nameof(ExecuteGameplayEntered));
            _onGameplayEnteredAction = null;
            return this;
        }
        private void ExecuteGameplayEntered() =>
            _onGameplayEnteredAction?.Invoke();
        

        private Action? _onGameplayExitedAction; 
        public AreaAction OnGameplayExited(Action action) {
            if (_onGameplayExitedAction == null) 
                Connect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnGameplayExited() {
            if (_onGameplayExitedAction == null) return this; 
            Disconnect("gameplay_exited", this, nameof(ExecuteGameplayExited));
            _onGameplayExitedAction = null;
            return this;
        }
        private void ExecuteGameplayExited() =>
            _onGameplayExitedAction?.Invoke();
        

        private Action<InputEvent, Node, Vector3, Vector3, int>? _onInputEventAction; 
        public AreaAction OnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action) {
            if (_onInputEventAction == null) 
                Connect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = action;
            return this;
        }
        public AreaAction RemoveOnInputEvent() {
            if (_onInputEventAction == null) return this; 
            Disconnect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = null;
            return this;
        }
        private void ExecuteInputEvent(InputEvent @event, Node camera, Vector3 normal, Vector3 position, int shape_idx) =>
            _onInputEventAction?.Invoke(@event, camera, normal, position, shape_idx);
        

        private Action? _onMouseEnteredAction; 
        public AreaAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public AreaAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public AreaAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public AreaAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public AreaAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public AreaAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AreaAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AreaAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public AreaAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public AreaAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public AreaAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public AreaAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public AreaAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public AreaAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public AreaAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public AreaAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}