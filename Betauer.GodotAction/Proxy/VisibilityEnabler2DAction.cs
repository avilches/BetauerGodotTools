using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisibilityEnabler2DAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public VisibilityEnabler2DAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public VisibilityEnabler2DAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public VisibilityEnabler2DAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onHideAction; 
        public VisibilityEnabler2DAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public VisibilityEnabler2DAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public VisibilityEnabler2DAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public VisibilityEnabler2DAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScreenEnteredAction; 
        public VisibilityEnabler2DAction OnScreenEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnScreenEntered(Action action) {
            RemoveSignal(_onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalScreenEntered() {
            ExecuteSignal(_onScreenEnteredAction);
            return this;
        }

        private List<Action>? _onScreenExitedAction; 
        public VisibilityEnabler2DAction OnScreenExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnScreenExited(Action action) {
            RemoveSignal(_onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalScreenExited() {
            ExecuteSignal(_onScreenExitedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisibilityEnabler2DAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public VisibilityEnabler2DAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public VisibilityEnabler2DAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public VisibilityEnabler2DAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action<Viewport>>? _onViewportEnteredAction; 
        public VisibilityEnabler2DAction OnViewportEntered(Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onViewportEnteredAction, "viewport_entered", nameof(_GodotSignalViewportEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnViewportEntered(Action<Viewport> action) {
            RemoveSignal(_onViewportEnteredAction, "viewport_entered", nameof(_GodotSignalViewportEntered), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalViewportEntered(Viewport viewport) {
            ExecuteSignal(_onViewportEnteredAction, viewport);
            return this;
        }

        private List<Action<Viewport>>? _onViewportExitedAction; 
        public VisibilityEnabler2DAction OnViewportExited(Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onViewportExitedAction, "viewport_exited", nameof(_GodotSignalViewportExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnViewportExited(Action<Viewport> action) {
            RemoveSignal(_onViewportExitedAction, "viewport_exited", nameof(_GodotSignalViewportExited), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalViewportExited(Viewport viewport) {
            ExecuteSignal(_onViewportExitedAction, viewport);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public VisibilityEnabler2DAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private VisibilityEnabler2DAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}