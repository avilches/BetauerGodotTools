using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class AreaAction : ProxyNode {

        private List<Action<Area>>? _onAreaEnteredAction; 
        public AreaAction OnAreaEntered(Action<Area> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAreaEnteredAction, "area_entered", nameof(_GodotSignalAreaEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnAreaEntered(Action<Area> action) {
            RemoveSignal(_onAreaEnteredAction, "area_entered", nameof(_GodotSignalAreaEntered), action);
            return this;
        }

        private AreaAction _GodotSignalAreaEntered(Area area) {
            ExecuteSignal(_onAreaEnteredAction, area);
            return this;
        }

        private List<Action<Area>>? _onAreaExitedAction; 
        public AreaAction OnAreaExited(Action<Area> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAreaExitedAction, "area_exited", nameof(_GodotSignalAreaExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnAreaExited(Action<Area> action) {
            RemoveSignal(_onAreaExitedAction, "area_exited", nameof(_GodotSignalAreaExited), action);
            return this;
        }

        private AreaAction _GodotSignalAreaExited(Area area) {
            ExecuteSignal(_onAreaExitedAction, area);
            return this;
        }

        private List<Action<Area, RID, int, int>>? _onAreaShapeEnteredAction; 
        public AreaAction OnAreaShapeEntered(Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAreaShapeEnteredAction, "area_shape_entered", nameof(_GodotSignalAreaShapeEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnAreaShapeEntered(Action<Area, RID, int, int> action) {
            RemoveSignal(_onAreaShapeEnteredAction, "area_shape_entered", nameof(_GodotSignalAreaShapeEntered), action);
            return this;
        }

        private AreaAction _GodotSignalAreaShapeEntered(Area area, RID area_rid, int area_shape_index, int local_shape_index) {
            ExecuteSignal(_onAreaShapeEnteredAction, area, area_rid, area_shape_index, local_shape_index);
            return this;
        }

        private List<Action<Area, RID, int, int>>? _onAreaShapeExitedAction; 
        public AreaAction OnAreaShapeExited(Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAreaShapeExitedAction, "area_shape_exited", nameof(_GodotSignalAreaShapeExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnAreaShapeExited(Action<Area, RID, int, int> action) {
            RemoveSignal(_onAreaShapeExitedAction, "area_shape_exited", nameof(_GodotSignalAreaShapeExited), action);
            return this;
        }

        private AreaAction _GodotSignalAreaShapeExited(Area area, RID area_rid, int area_shape_index, int local_shape_index) {
            ExecuteSignal(_onAreaShapeExitedAction, area, area_rid, area_shape_index, local_shape_index);
            return this;
        }

        private List<Action<Node>>? _onBodyEnteredAction; 
        public AreaAction OnBodyEntered(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnBodyEntered(Action<Node> action) {
            RemoveSignal(_onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action);
            return this;
        }

        private AreaAction _GodotSignalBodyEntered(Node body) {
            ExecuteSignal(_onBodyEnteredAction, body);
            return this;
        }

        private List<Action<Node>>? _onBodyExitedAction; 
        public AreaAction OnBodyExited(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnBodyExited(Action<Node> action) {
            RemoveSignal(_onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action);
            return this;
        }

        private AreaAction _GodotSignalBodyExited(Node body) {
            ExecuteSignal(_onBodyExitedAction, body);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeEnteredAction; 
        public AreaAction OnBodyShapeEntered(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnBodyShapeEntered(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action);
            return this;
        }

        private AreaAction _GodotSignalBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeEnteredAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeExitedAction; 
        public AreaAction OnBodyShapeExited(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnBodyShapeExited(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action);
            return this;
        }

        private AreaAction _GodotSignalBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeExitedAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public AreaAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private AreaAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public AreaAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private AreaAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action<InputEvent, Node, Vector3, Vector3, int>>? _onInputEventAction; 
        public AreaAction OnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action) {
            RemoveSignal(_onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action);
            return this;
        }

        private AreaAction _GodotSignalInputEvent(InputEvent @event, Node camera, Vector3 normal, Vector3 position, int shape_idx) {
            ExecuteSignal(_onInputEventAction, @event, camera, normal, position, shape_idx);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public AreaAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private AreaAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public AreaAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private AreaAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public AreaAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private AreaAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public AreaAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private AreaAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AreaAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AreaAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public AreaAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private AreaAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public AreaAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private AreaAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public AreaAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private AreaAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public AreaAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public AreaAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private AreaAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}