using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class Area2DAction : Node {
        public Area2DAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public Area2DAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public Area2DAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public Area2DAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public Area2DAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public Area2DAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public Area2DAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action<Area2D>>? _onAreaEnteredAction; 
        public Area2DAction OnAreaEntered(Action<Area2D> action, bool oneShot = false, bool deferred = false) {
            if (_onAreaEnteredAction == null || _onAreaEnteredAction.Count == 0) {
                _onAreaEnteredAction ??= new List<Action<Area2D>>(); 
                GetParent().Connect("area_entered", this, nameof(_GodotSignalAreaEntered));
            }
            _onAreaEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnAreaEntered(Action<Area2D> action) {
            if (_onAreaEnteredAction == null || _onAreaEnteredAction.Count == 0) return this;
            _onAreaEnteredAction.Remove(action); 
            if (_onAreaEnteredAction.Count == 0) {
                GetParent().Disconnect("area_entered", this, nameof(_GodotSignalAreaEntered));
            }
            return this;
        }
        private void _GodotSignalAreaEntered(Area2D area) {
            if (_onAreaEnteredAction == null || _onAreaEnteredAction.Count == 0) return;
            for (var i = 0; i < _onAreaEnteredAction.Count; i++) _onAreaEnteredAction[i].Invoke(area);
        }
        

        private List<Action<Area2D>>? _onAreaExitedAction; 
        public Area2DAction OnAreaExited(Action<Area2D> action, bool oneShot = false, bool deferred = false) {
            if (_onAreaExitedAction == null || _onAreaExitedAction.Count == 0) {
                _onAreaExitedAction ??= new List<Action<Area2D>>(); 
                GetParent().Connect("area_exited", this, nameof(_GodotSignalAreaExited));
            }
            _onAreaExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnAreaExited(Action<Area2D> action) {
            if (_onAreaExitedAction == null || _onAreaExitedAction.Count == 0) return this;
            _onAreaExitedAction.Remove(action); 
            if (_onAreaExitedAction.Count == 0) {
                GetParent().Disconnect("area_exited", this, nameof(_GodotSignalAreaExited));
            }
            return this;
        }
        private void _GodotSignalAreaExited(Area2D area) {
            if (_onAreaExitedAction == null || _onAreaExitedAction.Count == 0) return;
            for (var i = 0; i < _onAreaExitedAction.Count; i++) _onAreaExitedAction[i].Invoke(area);
        }
        

        private List<Action<Area2D, RID, int, int>>? _onAreaShapeEnteredAction; 
        public Area2DAction OnAreaShapeEntered(Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            if (_onAreaShapeEnteredAction == null || _onAreaShapeEnteredAction.Count == 0) {
                _onAreaShapeEnteredAction ??= new List<Action<Area2D, RID, int, int>>(); 
                GetParent().Connect("area_shape_entered", this, nameof(_GodotSignalAreaShapeEntered));
            }
            _onAreaShapeEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnAreaShapeEntered(Action<Area2D, RID, int, int> action) {
            if (_onAreaShapeEnteredAction == null || _onAreaShapeEnteredAction.Count == 0) return this;
            _onAreaShapeEnteredAction.Remove(action); 
            if (_onAreaShapeEnteredAction.Count == 0) {
                GetParent().Disconnect("area_shape_entered", this, nameof(_GodotSignalAreaShapeEntered));
            }
            return this;
        }
        private void _GodotSignalAreaShapeEntered(Area2D area, RID area_rid, int area_shape_index, int local_shape_index) {
            if (_onAreaShapeEnteredAction == null || _onAreaShapeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onAreaShapeEnteredAction.Count; i++) _onAreaShapeEnteredAction[i].Invoke(area, area_rid, area_shape_index, local_shape_index);
        }
        

        private List<Action<Area2D, RID, int, int>>? _onAreaShapeExitedAction; 
        public Area2DAction OnAreaShapeExited(Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            if (_onAreaShapeExitedAction == null || _onAreaShapeExitedAction.Count == 0) {
                _onAreaShapeExitedAction ??= new List<Action<Area2D, RID, int, int>>(); 
                GetParent().Connect("area_shape_exited", this, nameof(_GodotSignalAreaShapeExited));
            }
            _onAreaShapeExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnAreaShapeExited(Action<Area2D, RID, int, int> action) {
            if (_onAreaShapeExitedAction == null || _onAreaShapeExitedAction.Count == 0) return this;
            _onAreaShapeExitedAction.Remove(action); 
            if (_onAreaShapeExitedAction.Count == 0) {
                GetParent().Disconnect("area_shape_exited", this, nameof(_GodotSignalAreaShapeExited));
            }
            return this;
        }
        private void _GodotSignalAreaShapeExited(Area2D area, RID area_rid, int area_shape_index, int local_shape_index) {
            if (_onAreaShapeExitedAction == null || _onAreaShapeExitedAction.Count == 0) return;
            for (var i = 0; i < _onAreaShapeExitedAction.Count; i++) _onAreaShapeExitedAction[i].Invoke(area, area_rid, area_shape_index, local_shape_index);
        }
        

        private List<Action<Node>>? _onBodyEnteredAction; 
        public Area2DAction OnBodyEntered(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onBodyEnteredAction == null || _onBodyEnteredAction.Count == 0) {
                _onBodyEnteredAction ??= new List<Action<Node>>(); 
                GetParent().Connect("body_entered", this, nameof(_GodotSignalBodyEntered));
            }
            _onBodyEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnBodyEntered(Action<Node> action) {
            if (_onBodyEnteredAction == null || _onBodyEnteredAction.Count == 0) return this;
            _onBodyEnteredAction.Remove(action); 
            if (_onBodyEnteredAction.Count == 0) {
                GetParent().Disconnect("body_entered", this, nameof(_GodotSignalBodyEntered));
            }
            return this;
        }
        private void _GodotSignalBodyEntered(Node body) {
            if (_onBodyEnteredAction == null || _onBodyEnteredAction.Count == 0) return;
            for (var i = 0; i < _onBodyEnteredAction.Count; i++) _onBodyEnteredAction[i].Invoke(body);
        }
        

        private List<Action<Node>>? _onBodyExitedAction; 
        public Area2DAction OnBodyExited(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onBodyExitedAction == null || _onBodyExitedAction.Count == 0) {
                _onBodyExitedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("body_exited", this, nameof(_GodotSignalBodyExited));
            }
            _onBodyExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnBodyExited(Action<Node> action) {
            if (_onBodyExitedAction == null || _onBodyExitedAction.Count == 0) return this;
            _onBodyExitedAction.Remove(action); 
            if (_onBodyExitedAction.Count == 0) {
                GetParent().Disconnect("body_exited", this, nameof(_GodotSignalBodyExited));
            }
            return this;
        }
        private void _GodotSignalBodyExited(Node body) {
            if (_onBodyExitedAction == null || _onBodyExitedAction.Count == 0) return;
            for (var i = 0; i < _onBodyExitedAction.Count; i++) _onBodyExitedAction[i].Invoke(body);
        }
        

        private List<Action<Node, RID, int, int>>? _onBodyShapeEnteredAction; 
        public Area2DAction OnBodyShapeEntered(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            if (_onBodyShapeEnteredAction == null || _onBodyShapeEnteredAction.Count == 0) {
                _onBodyShapeEnteredAction ??= new List<Action<Node, RID, int, int>>(); 
                GetParent().Connect("body_shape_entered", this, nameof(_GodotSignalBodyShapeEntered));
            }
            _onBodyShapeEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnBodyShapeEntered(Action<Node, RID, int, int> action) {
            if (_onBodyShapeEnteredAction == null || _onBodyShapeEnteredAction.Count == 0) return this;
            _onBodyShapeEnteredAction.Remove(action); 
            if (_onBodyShapeEnteredAction.Count == 0) {
                GetParent().Disconnect("body_shape_entered", this, nameof(_GodotSignalBodyShapeEntered));
            }
            return this;
        }
        private void _GodotSignalBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            if (_onBodyShapeEnteredAction == null || _onBodyShapeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onBodyShapeEnteredAction.Count; i++) _onBodyShapeEnteredAction[i].Invoke(body, body_rid, body_shape_index, local_shape_index);
        }
        

        private List<Action<Node, RID, int, int>>? _onBodyShapeExitedAction; 
        public Area2DAction OnBodyShapeExited(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            if (_onBodyShapeExitedAction == null || _onBodyShapeExitedAction.Count == 0) {
                _onBodyShapeExitedAction ??= new List<Action<Node, RID, int, int>>(); 
                GetParent().Connect("body_shape_exited", this, nameof(_GodotSignalBodyShapeExited));
            }
            _onBodyShapeExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnBodyShapeExited(Action<Node, RID, int, int> action) {
            if (_onBodyShapeExitedAction == null || _onBodyShapeExitedAction.Count == 0) return this;
            _onBodyShapeExitedAction.Remove(action); 
            if (_onBodyShapeExitedAction.Count == 0) {
                GetParent().Disconnect("body_shape_exited", this, nameof(_GodotSignalBodyShapeExited));
            }
            return this;
        }
        private void _GodotSignalBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            if (_onBodyShapeExitedAction == null || _onBodyShapeExitedAction.Count == 0) return;
            for (var i = 0; i < _onBodyShapeExitedAction.Count; i++) _onBodyShapeExitedAction[i].Invoke(body, body_rid, body_shape_index, local_shape_index);
        }
        

        private List<Action>? _onDrawAction; 
        public Area2DAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                GetParent().Connect("draw", this, nameof(_GodotSignalDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return this;
            _onDrawAction.Remove(action); 
            if (_onDrawAction.Count == 0) {
                GetParent().Disconnect("draw", this, nameof(_GodotSignalDraw));
            }
            return this;
        }
        private void _GodotSignalDraw() {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return;
            for (var i = 0; i < _onDrawAction.Count; i++) _onDrawAction[i].Invoke();
        }
        

        private List<Action>? _onHideAction; 
        public Area2DAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                GetParent().Connect("hide", this, nameof(_GodotSignalHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) return this;
            _onHideAction.Remove(action); 
            if (_onHideAction.Count == 0) {
                GetParent().Disconnect("hide", this, nameof(_GodotSignalHide));
            }
            return this;
        }
        private void _GodotSignalHide() {
            if (_onHideAction == null || _onHideAction.Count == 0) return;
            for (var i = 0; i < _onHideAction.Count; i++) _onHideAction[i].Invoke();
        }
        

        private List<Action<InputEvent, int, Node>>? _onInputEventAction; 
        public Area2DAction OnInputEvent(Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            if (_onInputEventAction == null || _onInputEventAction.Count == 0) {
                _onInputEventAction ??= new List<Action<InputEvent, int, Node>>(); 
                GetParent().Connect("input_event", this, nameof(_GodotSignalInputEvent));
            }
            _onInputEventAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnInputEvent(Action<InputEvent, int, Node> action) {
            if (_onInputEventAction == null || _onInputEventAction.Count == 0) return this;
            _onInputEventAction.Remove(action); 
            if (_onInputEventAction.Count == 0) {
                GetParent().Disconnect("input_event", this, nameof(_GodotSignalInputEvent));
            }
            return this;
        }
        private void _GodotSignalInputEvent(InputEvent @event, int shape_idx, Node viewport) {
            if (_onInputEventAction == null || _onInputEventAction.Count == 0) return;
            for (var i = 0; i < _onInputEventAction.Count; i++) _onInputEventAction[i].Invoke(@event, shape_idx, viewport);
        }
        

        private List<Action>? _onItemRectChangedAction; 
        public Area2DAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                GetParent().Connect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return this;
            _onItemRectChangedAction.Remove(action); 
            if (_onItemRectChangedAction.Count == 0) {
                GetParent().Disconnect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            return this;
        }
        private void _GodotSignalItemRectChanged() {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return;
            for (var i = 0; i < _onItemRectChangedAction.Count; i++) _onItemRectChangedAction[i].Invoke();
        }
        

        private List<Action>? _onMouseEnteredAction; 
        public Area2DAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                GetParent().Connect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return this;
            _onMouseEnteredAction.Remove(action); 
            if (_onMouseEnteredAction.Count == 0) {
                GetParent().Disconnect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            return this;
        }
        private void _GodotSignalMouseEntered() {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return;
            for (var i = 0; i < _onMouseEnteredAction.Count; i++) _onMouseEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onMouseExitedAction; 
        public Area2DAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                GetParent().Connect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return this;
            _onMouseExitedAction.Remove(action); 
            if (_onMouseExitedAction.Count == 0) {
                GetParent().Disconnect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            return this;
        }
        private void _GodotSignalMouseExited() {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return;
            for (var i = 0; i < _onMouseExitedAction.Count; i++) _onMouseExitedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public Area2DAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                GetParent().Connect("ready", this, nameof(_GodotSignalReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                GetParent().Disconnect("ready", this, nameof(_GodotSignalReady));
            }
            return this;
        }
        private void _GodotSignalReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public Area2DAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                GetParent().Connect("renamed", this, nameof(_GodotSignalRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                GetParent().Disconnect("renamed", this, nameof(_GodotSignalRenamed));
            }
            return this;
        }
        private void _GodotSignalRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public Area2DAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public Area2DAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                GetParent().Connect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                GetParent().Disconnect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            return this;
        }
        private void _GodotSignalTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public Area2DAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                GetParent().Connect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                GetParent().Disconnect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            return this;
        }
        private void _GodotSignalTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public Area2DAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                GetParent().Connect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                GetParent().Disconnect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            return this;
        }
        private void _GodotSignalTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public Area2DAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                GetParent().Connect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public Area2DAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                GetParent().Disconnect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            return this;
        }
        private void _GodotSignalVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}