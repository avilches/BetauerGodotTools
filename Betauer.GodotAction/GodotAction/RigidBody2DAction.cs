using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class RigidBody2DAction : ProxyNode {

        private List<Action<Node>>? _onBodyEnteredAction; 
        public RigidBody2DAction OnBodyEntered(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnBodyEntered(Action<Node> action) {
            RemoveSignal(_onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalBodyEntered(Node body) {
            ExecuteSignal(_onBodyEnteredAction, body);
            return this;
        }

        private List<Action<Node>>? _onBodyExitedAction; 
        public RigidBody2DAction OnBodyExited(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnBodyExited(Action<Node> action) {
            RemoveSignal(_onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalBodyExited(Node body) {
            ExecuteSignal(_onBodyExitedAction, body);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeEnteredAction; 
        public RigidBody2DAction OnBodyShapeEntered(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnBodyShapeEntered(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeEnteredAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeExitedAction; 
        public RigidBody2DAction OnBodyShapeExited(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnBodyShapeExited(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeExitedAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public RigidBody2DAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onHideAction; 
        public RigidBody2DAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action<InputEvent, int, Node>>? _onInputEventAction; 
        public RigidBody2DAction OnInputEvent(Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnInputEvent(Action<InputEvent, int, Node> action) {
            RemoveSignal(_onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalInputEvent(InputEvent @event, int shape_idx, Node viewport) {
            ExecuteSignal(_onInputEventAction, @event, shape_idx, viewport);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public RigidBody2DAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public RigidBody2DAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public RigidBody2DAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public RigidBody2DAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public RigidBody2DAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public RigidBody2DAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSleepingStateChangedAction; 
        public RigidBody2DAction OnSleepingStateChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSleepingStateChangedAction, "sleeping_state_changed", nameof(_GodotSignalSleepingStateChanged), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnSleepingStateChanged(Action action) {
            RemoveSignal(_onSleepingStateChangedAction, "sleeping_state_changed", nameof(_GodotSignalSleepingStateChanged), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalSleepingStateChanged() {
            ExecuteSignal(_onSleepingStateChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public RigidBody2DAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public RigidBody2DAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public RigidBody2DAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public RigidBody2DAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public RigidBody2DAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private RigidBody2DAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}