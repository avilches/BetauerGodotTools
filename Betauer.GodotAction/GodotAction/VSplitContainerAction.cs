using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VSplitContainerAction : ProxyNode {

        private List<Action<int>>? _onDraggedAction; 
        public VSplitContainerAction OnDragged(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDraggedAction, "dragged", nameof(_GodotSignalDragged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnDragged(Action<int> action) {
            RemoveSignal(_onDraggedAction, "dragged", nameof(_GodotSignalDragged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalDragged(int offset) {
            ExecuteSignal(_onDraggedAction, offset);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public VSplitContainerAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public VSplitContainerAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public VSplitContainerAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public VSplitContainerAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public VSplitContainerAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public VSplitContainerAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public VSplitContainerAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public VSplitContainerAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public VSplitContainerAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public VSplitContainerAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public VSplitContainerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public VSplitContainerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public VSplitContainerAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VSplitContainerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public VSplitContainerAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onSortChildrenAction; 
        public VSplitContainerAction OnSortChildren(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnSortChildren(Action action) {
            RemoveSignal(_onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalSortChildren() {
            ExecuteSignal(_onSortChildrenAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public VSplitContainerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public VSplitContainerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public VSplitContainerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public VSplitContainerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public VSplitContainerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private VSplitContainerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}