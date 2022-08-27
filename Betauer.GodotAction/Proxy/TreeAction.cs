using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class TreeAction : ProxyNode {

        private List<Action<int, int, TreeItem>>? _onButtonPressedAction; 
        public TreeAction OnButtonPressed(Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnButtonPressed(Action<int, int, TreeItem> action) {
            RemoveSignal(_onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action);
            return this;
        }

        private TreeAction _GodotSignalButtonPressed(int column, int id, TreeItem item) {
            ExecuteSignal(_onButtonPressedAction, column, id, item);
            return this;
        }

        private List<Action>? _onCellSelectedAction; 
        public TreeAction OnCellSelected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCellSelectedAction, "cell_selected", nameof(_GodotSignalCellSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnCellSelected(Action action) {
            RemoveSignal(_onCellSelectedAction, "cell_selected", nameof(_GodotSignalCellSelected), action);
            return this;
        }

        private TreeAction _GodotSignalCellSelected() {
            ExecuteSignal(_onCellSelectedAction);
            return this;
        }

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public TreeAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private TreeAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public TreeAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private TreeAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action<int>>? _onColumnTitlePressedAction; 
        public TreeAction OnColumnTitlePressed(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onColumnTitlePressedAction, "column_title_pressed", nameof(_GodotSignalColumnTitlePressed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnColumnTitlePressed(Action<int> action) {
            RemoveSignal(_onColumnTitlePressedAction, "column_title_pressed", nameof(_GodotSignalColumnTitlePressed), action);
            return this;
        }

        private TreeAction _GodotSignalColumnTitlePressed(int column) {
            ExecuteSignal(_onColumnTitlePressedAction, column);
            return this;
        }

        private List<Action<bool>>? _onCustomPopupEditedAction; 
        public TreeAction OnCustomPopupEdited(Action<bool> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCustomPopupEditedAction, "custom_popup_edited", nameof(_GodotSignalCustomPopupEdited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnCustomPopupEdited(Action<bool> action) {
            RemoveSignal(_onCustomPopupEditedAction, "custom_popup_edited", nameof(_GodotSignalCustomPopupEdited), action);
            return this;
        }

        private TreeAction _GodotSignalCustomPopupEdited(bool arrow_clicked) {
            ExecuteSignal(_onCustomPopupEditedAction, arrow_clicked);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public TreeAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private TreeAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action<Vector2>>? _onEmptyRmbAction; 
        public TreeAction OnEmptyRmb(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEmptyRmbAction, "empty_rmb", nameof(_GodotSignalEmptyRmb), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnEmptyRmb(Action<Vector2> action) {
            RemoveSignal(_onEmptyRmbAction, "empty_rmb", nameof(_GodotSignalEmptyRmb), action);
            return this;
        }

        private TreeAction _GodotSignalEmptyRmb(Vector2 position) {
            ExecuteSignal(_onEmptyRmbAction, position);
            return this;
        }

        private List<Action<Vector2>>? _onEmptyTreeRmbSelectedAction; 
        public TreeAction OnEmptyTreeRmbSelected(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEmptyTreeRmbSelectedAction, "empty_tree_rmb_selected", nameof(_GodotSignalEmptyTreeRmbSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnEmptyTreeRmbSelected(Action<Vector2> action) {
            RemoveSignal(_onEmptyTreeRmbSelectedAction, "empty_tree_rmb_selected", nameof(_GodotSignalEmptyTreeRmbSelected), action);
            return this;
        }

        private TreeAction _GodotSignalEmptyTreeRmbSelected(Vector2 position) {
            ExecuteSignal(_onEmptyTreeRmbSelectedAction, position);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public TreeAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private TreeAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public TreeAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private TreeAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public TreeAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private TreeAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public TreeAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private TreeAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemActivatedAction; 
        public TreeAction OnItemActivated(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemActivated(Action action) {
            RemoveSignal(_onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action);
            return this;
        }

        private TreeAction _GodotSignalItemActivated() {
            ExecuteSignal(_onItemActivatedAction);
            return this;
        }

        private List<Action<TreeItem>>? _onItemCollapsedAction; 
        public TreeAction OnItemCollapsed(Action<TreeItem> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemCollapsedAction, "item_collapsed", nameof(_GodotSignalItemCollapsed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemCollapsed(Action<TreeItem> action) {
            RemoveSignal(_onItemCollapsedAction, "item_collapsed", nameof(_GodotSignalItemCollapsed), action);
            return this;
        }

        private TreeAction _GodotSignalItemCollapsed(TreeItem item) {
            ExecuteSignal(_onItemCollapsedAction, item);
            return this;
        }

        private List<Action>? _onItemCustomButtonPressedAction; 
        public TreeAction OnItemCustomButtonPressed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemCustomButtonPressedAction, "item_custom_button_pressed", nameof(_GodotSignalItemCustomButtonPressed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemCustomButtonPressed(Action action) {
            RemoveSignal(_onItemCustomButtonPressedAction, "item_custom_button_pressed", nameof(_GodotSignalItemCustomButtonPressed), action);
            return this;
        }

        private TreeAction _GodotSignalItemCustomButtonPressed() {
            ExecuteSignal(_onItemCustomButtonPressedAction);
            return this;
        }

        private List<Action>? _onItemDoubleClickedAction; 
        public TreeAction OnItemDoubleClicked(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemDoubleClickedAction, "item_double_clicked", nameof(_GodotSignalItemDoubleClicked), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemDoubleClicked(Action action) {
            RemoveSignal(_onItemDoubleClickedAction, "item_double_clicked", nameof(_GodotSignalItemDoubleClicked), action);
            return this;
        }

        private TreeAction _GodotSignalItemDoubleClicked() {
            ExecuteSignal(_onItemDoubleClickedAction);
            return this;
        }

        private List<Action>? _onItemEditedAction; 
        public TreeAction OnItemEdited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemEditedAction, "item_edited", nameof(_GodotSignalItemEdited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemEdited(Action action) {
            RemoveSignal(_onItemEditedAction, "item_edited", nameof(_GodotSignalItemEdited), action);
            return this;
        }

        private TreeAction _GodotSignalItemEdited() {
            ExecuteSignal(_onItemEditedAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public TreeAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private TreeAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onItemRmbEditedAction; 
        public TreeAction OnItemRmbEdited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRmbEditedAction, "item_rmb_edited", nameof(_GodotSignalItemRmbEdited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemRmbEdited(Action action) {
            RemoveSignal(_onItemRmbEditedAction, "item_rmb_edited", nameof(_GodotSignalItemRmbEdited), action);
            return this;
        }

        private TreeAction _GodotSignalItemRmbEdited() {
            ExecuteSignal(_onItemRmbEditedAction);
            return this;
        }

        private List<Action<Vector2>>? _onItemRmbSelectedAction; 
        public TreeAction OnItemRmbSelected(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemRmbSelected(Action<Vector2> action) {
            RemoveSignal(_onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action);
            return this;
        }

        private TreeAction _GodotSignalItemRmbSelected(Vector2 position) {
            ExecuteSignal(_onItemRmbSelectedAction, position);
            return this;
        }

        private List<Action>? _onItemSelectedAction; 
        public TreeAction OnItemSelected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnItemSelected(Action action) {
            RemoveSignal(_onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action);
            return this;
        }

        private TreeAction _GodotSignalItemSelected() {
            ExecuteSignal(_onItemSelectedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public TreeAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private TreeAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public TreeAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private TreeAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public TreeAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private TreeAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public TreeAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private TreeAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action<int, TreeItem, bool>>? _onMultiSelectedAction; 
        public TreeAction OnMultiSelected(Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnMultiSelected(Action<int, TreeItem, bool> action) {
            RemoveSignal(_onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action);
            return this;
        }

        private TreeAction _GodotSignalMultiSelected(int column, TreeItem item, bool selected) {
            ExecuteSignal(_onMultiSelectedAction, column, item, selected);
            return this;
        }

        private List<Action>? _onNothingSelectedAction; 
        public TreeAction OnNothingSelected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnNothingSelected(Action action) {
            RemoveSignal(_onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action);
            return this;
        }

        private TreeAction _GodotSignalNothingSelected() {
            ExecuteSignal(_onNothingSelectedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public TreeAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private TreeAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public TreeAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private TreeAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public TreeAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private TreeAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public TreeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private TreeAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public TreeAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private TreeAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public TreeAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private TreeAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public TreeAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private TreeAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public TreeAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private TreeAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public TreeAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public TreeAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private TreeAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}