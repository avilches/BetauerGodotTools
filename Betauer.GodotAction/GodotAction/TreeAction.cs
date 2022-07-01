using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TreeAction : ProxyNode {

        private List<Action<int, int, TreeItem>>? _onButtonPressedAction; 
        public void OnButtonPressed(Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action, oneShot, deferred);

        public void RemoveOnButtonPressed(Action<int, int, TreeItem> action) =>
            RemoveSignal(_onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action);

        private void _GodotSignalButtonPressed(int column, int id, TreeItem item) =>
            ExecuteSignal(_onButtonPressedAction, column, id, item);
        

        private List<Action>? _onCellSelectedAction; 
        public void OnCellSelected(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onCellSelectedAction, "cell_selected", nameof(_GodotSignalCellSelected), action, oneShot, deferred);

        public void RemoveOnCellSelected(Action action) =>
            RemoveSignal(_onCellSelectedAction, "cell_selected", nameof(_GodotSignalCellSelected), action);

        private void _GodotSignalCellSelected() =>
            ExecuteSignal(_onCellSelectedAction);
        

        private List<Action<int>>? _onColumnTitlePressedAction; 
        public void OnColumnTitlePressed(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onColumnTitlePressedAction, "column_title_pressed", nameof(_GodotSignalColumnTitlePressed), action, oneShot, deferred);

        public void RemoveOnColumnTitlePressed(Action<int> action) =>
            RemoveSignal(_onColumnTitlePressedAction, "column_title_pressed", nameof(_GodotSignalColumnTitlePressed), action);

        private void _GodotSignalColumnTitlePressed(int column) =>
            ExecuteSignal(_onColumnTitlePressedAction, column);
        

        private List<Action<bool>>? _onCustomPopupEditedAction; 
        public void OnCustomPopupEdited(Action<bool> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onCustomPopupEditedAction, "custom_popup_edited", nameof(_GodotSignalCustomPopupEdited), action, oneShot, deferred);

        public void RemoveOnCustomPopupEdited(Action<bool> action) =>
            RemoveSignal(_onCustomPopupEditedAction, "custom_popup_edited", nameof(_GodotSignalCustomPopupEdited), action);

        private void _GodotSignalCustomPopupEdited(bool arrow_clicked) =>
            ExecuteSignal(_onCustomPopupEditedAction, arrow_clicked);
        

        private List<Action>? _onDrawAction; 
        public void OnDraw(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);

        public void RemoveOnDraw(Action action) =>
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);

        private void _GodotSignalDraw() =>
            ExecuteSignal(_onDrawAction);
        

        private List<Action<Vector2>>? _onEmptyRmbAction; 
        public void OnEmptyRmb(Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onEmptyRmbAction, "empty_rmb", nameof(_GodotSignalEmptyRmb), action, oneShot, deferred);

        public void RemoveOnEmptyRmb(Action<Vector2> action) =>
            RemoveSignal(_onEmptyRmbAction, "empty_rmb", nameof(_GodotSignalEmptyRmb), action);

        private void _GodotSignalEmptyRmb(Vector2 position) =>
            ExecuteSignal(_onEmptyRmbAction, position);
        

        private List<Action<Vector2>>? _onEmptyTreeRmbSelectedAction; 
        public void OnEmptyTreeRmbSelected(Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onEmptyTreeRmbSelectedAction, "empty_tree_rmb_selected", nameof(_GodotSignalEmptyTreeRmbSelected), action, oneShot, deferred);

        public void RemoveOnEmptyTreeRmbSelected(Action<Vector2> action) =>
            RemoveSignal(_onEmptyTreeRmbSelectedAction, "empty_tree_rmb_selected", nameof(_GodotSignalEmptyTreeRmbSelected), action);

        private void _GodotSignalEmptyTreeRmbSelected(Vector2 position) =>
            ExecuteSignal(_onEmptyTreeRmbSelectedAction, position);
        

        private List<Action>? _onFocusEnteredAction; 
        public void OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);

        public void RemoveOnFocusEntered(Action action) =>
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);

        private void _GodotSignalFocusEntered() =>
            ExecuteSignal(_onFocusEnteredAction);
        

        private List<Action>? _onFocusExitedAction; 
        public void OnFocusExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);

        public void RemoveOnFocusExited(Action action) =>
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);

        private void _GodotSignalFocusExited() =>
            ExecuteSignal(_onFocusExitedAction);
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public void OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);

        public void RemoveOnGuiInput(Action<InputEvent> action) =>
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);

        private void _GodotSignalGuiInput(InputEvent @event) =>
            ExecuteSignal(_onGuiInputAction, @event);
        

        private List<Action>? _onHideAction; 
        public void OnHide(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);

        public void RemoveOnHide(Action action) =>
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);

        private void _GodotSignalHide() =>
            ExecuteSignal(_onHideAction);
        

        private List<Action>? _onItemActivatedAction; 
        public void OnItemActivated(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action, oneShot, deferred);

        public void RemoveOnItemActivated(Action action) =>
            RemoveSignal(_onItemActivatedAction, "item_activated", nameof(_GodotSignalItemActivated), action);

        private void _GodotSignalItemActivated() =>
            ExecuteSignal(_onItemActivatedAction);
        

        private List<Action<TreeItem>>? _onItemCollapsedAction; 
        public void OnItemCollapsed(Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemCollapsedAction, "item_collapsed", nameof(_GodotSignalItemCollapsed), action, oneShot, deferred);

        public void RemoveOnItemCollapsed(Action<TreeItem> action) =>
            RemoveSignal(_onItemCollapsedAction, "item_collapsed", nameof(_GodotSignalItemCollapsed), action);

        private void _GodotSignalItemCollapsed(TreeItem item) =>
            ExecuteSignal(_onItemCollapsedAction, item);
        

        private List<Action>? _onItemCustomButtonPressedAction; 
        public void OnItemCustomButtonPressed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemCustomButtonPressedAction, "item_custom_button_pressed", nameof(_GodotSignalItemCustomButtonPressed), action, oneShot, deferred);

        public void RemoveOnItemCustomButtonPressed(Action action) =>
            RemoveSignal(_onItemCustomButtonPressedAction, "item_custom_button_pressed", nameof(_GodotSignalItemCustomButtonPressed), action);

        private void _GodotSignalItemCustomButtonPressed() =>
            ExecuteSignal(_onItemCustomButtonPressedAction);
        

        private List<Action>? _onItemDoubleClickedAction; 
        public void OnItemDoubleClicked(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemDoubleClickedAction, "item_double_clicked", nameof(_GodotSignalItemDoubleClicked), action, oneShot, deferred);

        public void RemoveOnItemDoubleClicked(Action action) =>
            RemoveSignal(_onItemDoubleClickedAction, "item_double_clicked", nameof(_GodotSignalItemDoubleClicked), action);

        private void _GodotSignalItemDoubleClicked() =>
            ExecuteSignal(_onItemDoubleClickedAction);
        

        private List<Action>? _onItemEditedAction; 
        public void OnItemEdited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemEditedAction, "item_edited", nameof(_GodotSignalItemEdited), action, oneShot, deferred);

        public void RemoveOnItemEdited(Action action) =>
            RemoveSignal(_onItemEditedAction, "item_edited", nameof(_GodotSignalItemEdited), action);

        private void _GodotSignalItemEdited() =>
            ExecuteSignal(_onItemEditedAction);
        

        private List<Action>? _onItemRectChangedAction; 
        public void OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);

        public void RemoveOnItemRectChanged(Action action) =>
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);

        private void _GodotSignalItemRectChanged() =>
            ExecuteSignal(_onItemRectChangedAction);
        

        private List<Action>? _onItemRmbEditedAction; 
        public void OnItemRmbEdited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRmbEditedAction, "item_rmb_edited", nameof(_GodotSignalItemRmbEdited), action, oneShot, deferred);

        public void RemoveOnItemRmbEdited(Action action) =>
            RemoveSignal(_onItemRmbEditedAction, "item_rmb_edited", nameof(_GodotSignalItemRmbEdited), action);

        private void _GodotSignalItemRmbEdited() =>
            ExecuteSignal(_onItemRmbEditedAction);
        

        private List<Action<Vector2>>? _onItemRmbSelectedAction; 
        public void OnItemRmbSelected(Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action, oneShot, deferred);

        public void RemoveOnItemRmbSelected(Action<Vector2> action) =>
            RemoveSignal(_onItemRmbSelectedAction, "item_rmb_selected", nameof(_GodotSignalItemRmbSelected), action);

        private void _GodotSignalItemRmbSelected(Vector2 position) =>
            ExecuteSignal(_onItemRmbSelectedAction, position);
        

        private List<Action>? _onItemSelectedAction; 
        public void OnItemSelected(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action, oneShot, deferred);

        public void RemoveOnItemSelected(Action action) =>
            RemoveSignal(_onItemSelectedAction, "item_selected", nameof(_GodotSignalItemSelected), action);

        private void _GodotSignalItemSelected() =>
            ExecuteSignal(_onItemSelectedAction);
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public void OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);

        public void RemoveOnMinimumSizeChanged(Action action) =>
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);

        private void _GodotSignalMinimumSizeChanged() =>
            ExecuteSignal(_onMinimumSizeChangedAction);
        

        private List<Action>? _onModalClosedAction; 
        public void OnModalClosed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);

        public void RemoveOnModalClosed(Action action) =>
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);

        private void _GodotSignalModalClosed() =>
            ExecuteSignal(_onModalClosedAction);
        

        private List<Action>? _onMouseEnteredAction; 
        public void OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);

        public void RemoveOnMouseEntered(Action action) =>
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);

        private void _GodotSignalMouseEntered() =>
            ExecuteSignal(_onMouseEnteredAction);
        

        private List<Action>? _onMouseExitedAction; 
        public void OnMouseExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);

        public void RemoveOnMouseExited(Action action) =>
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);

        private void _GodotSignalMouseExited() =>
            ExecuteSignal(_onMouseExitedAction);
        

        private List<Action<int, TreeItem, bool>>? _onMultiSelectedAction; 
        public void OnMultiSelected(Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action, oneShot, deferred);

        public void RemoveOnMultiSelected(Action<int, TreeItem, bool> action) =>
            RemoveSignal(_onMultiSelectedAction, "multi_selected", nameof(_GodotSignalMultiSelected), action);

        private void _GodotSignalMultiSelected(int column, TreeItem item, bool selected) =>
            ExecuteSignal(_onMultiSelectedAction, column, item, selected);
        

        private List<Action>? _onNothingSelectedAction; 
        public void OnNothingSelected(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action, oneShot, deferred);

        public void RemoveOnNothingSelected(Action action) =>
            RemoveSignal(_onNothingSelectedAction, "nothing_selected", nameof(_GodotSignalNothingSelected), action);

        private void _GodotSignalNothingSelected() =>
            ExecuteSignal(_onNothingSelectedAction);
        

        private List<Action>? _onReadyAction; 
        public void OnReady(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);

        public void RemoveOnReady(Action action) =>
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);

        private void _GodotSignalReady() =>
            ExecuteSignal(_onReadyAction);
        

        private List<Action>? _onRenamedAction; 
        public void OnRenamed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);

        public void RemoveOnRenamed(Action action) =>
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);

        private void _GodotSignalRenamed() =>
            ExecuteSignal(_onRenamedAction);
        

        private List<Action>? _onResizedAction; 
        public void OnResized(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);

        public void RemoveOnResized(Action action) =>
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);

        private void _GodotSignalResized() =>
            ExecuteSignal(_onResizedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public void OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);

        public void RemoveOnSizeFlagsChanged(Action action) =>
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);

        private void _GodotSignalSizeFlagsChanged() =>
            ExecuteSignal(_onSizeFlagsChangedAction);
        

        private List<Action>? _onTreeEnteredAction; 
        public void OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);

        public void RemoveOnTreeEntered(Action action) =>
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);

        private void _GodotSignalTreeEntered() =>
            ExecuteSignal(_onTreeEnteredAction);
        

        private List<Action>? _onTreeExitedAction; 
        public void OnTreeExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);

        public void RemoveOnTreeExited(Action action) =>
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);

        private void _GodotSignalTreeExited() =>
            ExecuteSignal(_onTreeExitedAction);
        

        private List<Action>? _onTreeExitingAction; 
        public void OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);

        public void RemoveOnTreeExiting(Action action) =>
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);

        private void _GodotSignalTreeExiting() =>
            ExecuteSignal(_onTreeExitingAction);
        

        private List<Action>? _onVisibilityChangedAction; 
        public void OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);

        public void RemoveOnVisibilityChanged(Action action) =>
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);

        private void _GodotSignalVisibilityChanged() =>
            ExecuteSignal(_onVisibilityChangedAction);
        
    }
}