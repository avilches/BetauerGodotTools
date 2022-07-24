using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class ItemListAction : ProxyNode {

        private List<Action>? _onDrawAction; 
        public ItemListAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private ItemListAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public ItemListAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private ItemListAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public ItemListAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private ItemListAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public ItemListAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private ItemListAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public ItemListAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private ItemListAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action<int>>? _onItemActivatedAction; 
        public ItemListAction OnItemActivated(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnItemActivated(Action<int> action) {
            RemoveSignal(_onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action);
            return this;
        }

        private ItemListAction _GodotSignalItemActivated(int index) {
            ExecuteSignal(_onItemActivatedAction, index);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public ItemListAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private ItemListAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action<Vector2, int>>? _onItemRmbSelectedAction; 
        public ItemListAction OnItemRmbSelected(Action<Vector2, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnItemRmbSelected(Action<Vector2, int> action) {
            RemoveSignal(_onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action);
            return this;
        }

        private ItemListAction _GodotSignalItemRmbSelected(Vector2 at_position, int index) {
            ExecuteSignal(_onItemRmbSelectedAction, at_position, index);
            return this;
        }

        private List<Action<int>>? _onItemSelectedAction; 
        public ItemListAction OnItemSelected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnItemSelected(Action<int> action) {
            RemoveSignal(_onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action);
            return this;
        }

        private ItemListAction _GodotSignalItemSelected(int index) {
            ExecuteSignal(_onItemSelectedAction, index);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public ItemListAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private ItemListAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public ItemListAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private ItemListAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public ItemListAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private ItemListAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public ItemListAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private ItemListAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action<int, bool>>? _onMultiSelectedAction; 
        public ItemListAction OnMultiSelected(Action<int, bool> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnMultiSelected(Action<int, bool> action) {
            RemoveSignal(_onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action);
            return this;
        }

        private ItemListAction _GodotSignalMultiSelected(int index, bool selected) {
            ExecuteSignal(_onMultiSelectedAction, index, selected);
            return this;
        }

        private List<Action>? _onNothingSelectedAction; 
        public ItemListAction OnNothingSelected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnNothingSelected(Action action) {
            RemoveSignal(_onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action);
            return this;
        }

        private ItemListAction _GodotSignalNothingSelected() {
            ExecuteSignal(_onNothingSelectedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ItemListAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ItemListAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ItemListAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ItemListAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public ItemListAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private ItemListAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action<Vector2>>? _onRmbClickedAction; 
        public ItemListAction OnRmbClicked(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRmbClickedAction, "rmb_clicked", nameof(_GodotSignalRmbClicked), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnRmbClicked(Action<Vector2> action) {
            RemoveSignal(_onRmbClickedAction, "rmb_clicked", nameof(_GodotSignalRmbClicked), action);
            return this;
        }

        private ItemListAction _GodotSignalRmbClicked(Vector2 at_position) {
            ExecuteSignal(_onRmbClickedAction, at_position);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ItemListAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ItemListAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public ItemListAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private ItemListAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ItemListAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ItemListAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ItemListAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ItemListAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ItemListAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ItemListAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public ItemListAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public ItemListAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private ItemListAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}