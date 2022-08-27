using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class ViewportContainerAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public ViewportContainerAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public ViewportContainerAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public ViewportContainerAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public ViewportContainerAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public ViewportContainerAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public ViewportContainerAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public ViewportContainerAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public ViewportContainerAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public ViewportContainerAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public ViewportContainerAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public ViewportContainerAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public ViewportContainerAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ViewportContainerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ViewportContainerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public ViewportContainerAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ViewportContainerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public ViewportContainerAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onSortChildrenAction; 
        public ViewportContainerAction OnSortChildren(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnSortChildren(Action action) {
            RemoveSignal(_onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalSortChildren() {
            ExecuteSignal(_onSortChildrenAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ViewportContainerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ViewportContainerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ViewportContainerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public ViewportContainerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportContainerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private ViewportContainerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}