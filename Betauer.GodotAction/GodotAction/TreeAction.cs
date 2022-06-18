using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TreeAction : Tree {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public TreeAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public TreeAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public TreeAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public TreeAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public TreeAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public TreeAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public TreeAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public TreeAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public TreeAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public TreeAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessActions == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

        private Action<int, int, TreeItem>? _onButtonPressedAction; 
        public TreeAction OnButtonPressed(Action<int, int, TreeItem> action) {
            if (_onButtonPressedAction == null) 
                Connect("button_pressed", this, nameof(ExecuteButtonPressed));
            _onButtonPressedAction = action;
            return this;
        }
        public TreeAction RemoveOnButtonPressed() {
            if (_onButtonPressedAction == null) return this; 
            Disconnect("button_pressed", this, nameof(ExecuteButtonPressed));
            _onButtonPressedAction = null;
            return this;
        }
        private void ExecuteButtonPressed(int column, int id, TreeItem item) =>
            _onButtonPressedAction?.Invoke(column, id, item);
        

        private Action? _onCellSelectedAction; 
        public TreeAction OnCellSelected(Action action) {
            if (_onCellSelectedAction == null) 
                Connect("cell_selected", this, nameof(ExecuteCellSelected));
            _onCellSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnCellSelected() {
            if (_onCellSelectedAction == null) return this; 
            Disconnect("cell_selected", this, nameof(ExecuteCellSelected));
            _onCellSelectedAction = null;
            return this;
        }
        private void ExecuteCellSelected() =>
            _onCellSelectedAction?.Invoke();
        

        private Action<int>? _onColumnTitlePressedAction; 
        public TreeAction OnColumnTitlePressed(Action<int> action) {
            if (_onColumnTitlePressedAction == null) 
                Connect("column_title_pressed", this, nameof(ExecuteColumnTitlePressed));
            _onColumnTitlePressedAction = action;
            return this;
        }
        public TreeAction RemoveOnColumnTitlePressed() {
            if (_onColumnTitlePressedAction == null) return this; 
            Disconnect("column_title_pressed", this, nameof(ExecuteColumnTitlePressed));
            _onColumnTitlePressedAction = null;
            return this;
        }
        private void ExecuteColumnTitlePressed(int column) =>
            _onColumnTitlePressedAction?.Invoke(column);
        

        private Action<bool>? _onCustomPopupEditedAction; 
        public TreeAction OnCustomPopupEdited(Action<bool> action) {
            if (_onCustomPopupEditedAction == null) 
                Connect("custom_popup_edited", this, nameof(ExecuteCustomPopupEdited));
            _onCustomPopupEditedAction = action;
            return this;
        }
        public TreeAction RemoveOnCustomPopupEdited() {
            if (_onCustomPopupEditedAction == null) return this; 
            Disconnect("custom_popup_edited", this, nameof(ExecuteCustomPopupEdited));
            _onCustomPopupEditedAction = null;
            return this;
        }
        private void ExecuteCustomPopupEdited(bool arrow_clicked) =>
            _onCustomPopupEditedAction?.Invoke(arrow_clicked);
        

        private Action? _onDrawAction; 
        public TreeAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public TreeAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action<Vector2>? _onEmptyRmbAction; 
        public TreeAction OnEmptyRmb(Action<Vector2> action) {
            if (_onEmptyRmbAction == null) 
                Connect("empty_rmb", this, nameof(ExecuteEmptyRmb));
            _onEmptyRmbAction = action;
            return this;
        }
        public TreeAction RemoveOnEmptyRmb() {
            if (_onEmptyRmbAction == null) return this; 
            Disconnect("empty_rmb", this, nameof(ExecuteEmptyRmb));
            _onEmptyRmbAction = null;
            return this;
        }
        private void ExecuteEmptyRmb(Vector2 position) =>
            _onEmptyRmbAction?.Invoke(position);
        

        private Action<Vector2>? _onEmptyTreeRmbSelectedAction; 
        public TreeAction OnEmptyTreeRmbSelected(Action<Vector2> action) {
            if (_onEmptyTreeRmbSelectedAction == null) 
                Connect("empty_tree_rmb_selected", this, nameof(ExecuteEmptyTreeRmbSelected));
            _onEmptyTreeRmbSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnEmptyTreeRmbSelected() {
            if (_onEmptyTreeRmbSelectedAction == null) return this; 
            Disconnect("empty_tree_rmb_selected", this, nameof(ExecuteEmptyTreeRmbSelected));
            _onEmptyTreeRmbSelectedAction = null;
            return this;
        }
        private void ExecuteEmptyTreeRmbSelected(Vector2 position) =>
            _onEmptyTreeRmbSelectedAction?.Invoke(position);
        

        private Action? _onFocusEnteredAction; 
        public TreeAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null) 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = action;
            return this;
        }
        public TreeAction RemoveOnFocusEntered() {
            if (_onFocusEnteredAction == null) return this; 
            Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = null;
            return this;
        }
        private void ExecuteFocusEntered() =>
            _onFocusEnteredAction?.Invoke();
        

        private Action? _onFocusExitedAction; 
        public TreeAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null) 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = action;
            return this;
        }
        public TreeAction RemoveOnFocusExited() {
            if (_onFocusExitedAction == null) return this; 
            Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = null;
            return this;
        }
        private void ExecuteFocusExited() =>
            _onFocusExitedAction?.Invoke();
        

        private Action<InputEvent>? _onGuiInputAction; 
        public TreeAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null) 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = action;
            return this;
        }
        public TreeAction RemoveOnGuiInput() {
            if (_onGuiInputAction == null) return this; 
            Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = null;
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) =>
            _onGuiInputAction?.Invoke(@event);
        

        private Action? _onHideAction; 
        public TreeAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public TreeAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemActivatedAction; 
        public TreeAction OnItemActivated(Action action) {
            if (_onItemActivatedAction == null) 
                Connect("item_activated", this, nameof(ExecuteItemActivated));
            _onItemActivatedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemActivated() {
            if (_onItemActivatedAction == null) return this; 
            Disconnect("item_activated", this, nameof(ExecuteItemActivated));
            _onItemActivatedAction = null;
            return this;
        }
        private void ExecuteItemActivated() =>
            _onItemActivatedAction?.Invoke();
        

        private Action<TreeItem>? _onItemCollapsedAction; 
        public TreeAction OnItemCollapsed(Action<TreeItem> action) {
            if (_onItemCollapsedAction == null) 
                Connect("item_collapsed", this, nameof(ExecuteItemCollapsed));
            _onItemCollapsedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemCollapsed() {
            if (_onItemCollapsedAction == null) return this; 
            Disconnect("item_collapsed", this, nameof(ExecuteItemCollapsed));
            _onItemCollapsedAction = null;
            return this;
        }
        private void ExecuteItemCollapsed(TreeItem item) =>
            _onItemCollapsedAction?.Invoke(item);
        

        private Action? _onItemCustomButtonPressedAction; 
        public TreeAction OnItemCustomButtonPressed(Action action) {
            if (_onItemCustomButtonPressedAction == null) 
                Connect("item_custom_button_pressed", this, nameof(ExecuteItemCustomButtonPressed));
            _onItemCustomButtonPressedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemCustomButtonPressed() {
            if (_onItemCustomButtonPressedAction == null) return this; 
            Disconnect("item_custom_button_pressed", this, nameof(ExecuteItemCustomButtonPressed));
            _onItemCustomButtonPressedAction = null;
            return this;
        }
        private void ExecuteItemCustomButtonPressed() =>
            _onItemCustomButtonPressedAction?.Invoke();
        

        private Action? _onItemDoubleClickedAction; 
        public TreeAction OnItemDoubleClicked(Action action) {
            if (_onItemDoubleClickedAction == null) 
                Connect("item_double_clicked", this, nameof(ExecuteItemDoubleClicked));
            _onItemDoubleClickedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemDoubleClicked() {
            if (_onItemDoubleClickedAction == null) return this; 
            Disconnect("item_double_clicked", this, nameof(ExecuteItemDoubleClicked));
            _onItemDoubleClickedAction = null;
            return this;
        }
        private void ExecuteItemDoubleClicked() =>
            _onItemDoubleClickedAction?.Invoke();
        

        private Action? _onItemEditedAction; 
        public TreeAction OnItemEdited(Action action) {
            if (_onItemEditedAction == null) 
                Connect("item_edited", this, nameof(ExecuteItemEdited));
            _onItemEditedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemEdited() {
            if (_onItemEditedAction == null) return this; 
            Disconnect("item_edited", this, nameof(ExecuteItemEdited));
            _onItemEditedAction = null;
            return this;
        }
        private void ExecuteItemEdited() =>
            _onItemEditedAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public TreeAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onItemRmbEditedAction; 
        public TreeAction OnItemRmbEdited(Action action) {
            if (_onItemRmbEditedAction == null) 
                Connect("item_rmb_edited", this, nameof(ExecuteItemRmbEdited));
            _onItemRmbEditedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemRmbEdited() {
            if (_onItemRmbEditedAction == null) return this; 
            Disconnect("item_rmb_edited", this, nameof(ExecuteItemRmbEdited));
            _onItemRmbEditedAction = null;
            return this;
        }
        private void ExecuteItemRmbEdited() =>
            _onItemRmbEditedAction?.Invoke();
        

        private Action<Vector2>? _onItemRmbSelectedAction; 
        public TreeAction OnItemRmbSelected(Action<Vector2> action) {
            if (_onItemRmbSelectedAction == null) 
                Connect("item_rmb_selected", this, nameof(ExecuteItemRmbSelected));
            _onItemRmbSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemRmbSelected() {
            if (_onItemRmbSelectedAction == null) return this; 
            Disconnect("item_rmb_selected", this, nameof(ExecuteItemRmbSelected));
            _onItemRmbSelectedAction = null;
            return this;
        }
        private void ExecuteItemRmbSelected(Vector2 position) =>
            _onItemRmbSelectedAction?.Invoke(position);
        

        private Action? _onItemSelectedAction; 
        public TreeAction OnItemSelected(Action action) {
            if (_onItemSelectedAction == null) 
                Connect("item_selected", this, nameof(ExecuteItemSelected));
            _onItemSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnItemSelected() {
            if (_onItemSelectedAction == null) return this; 
            Disconnect("item_selected", this, nameof(ExecuteItemSelected));
            _onItemSelectedAction = null;
            return this;
        }
        private void ExecuteItemSelected() =>
            _onItemSelectedAction?.Invoke();
        

        private Action? _onMinimumSizeChangedAction; 
        public TreeAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null) 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = action;
            return this;
        }
        public TreeAction RemoveOnMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null) return this; 
            Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = null;
            return this;
        }
        private void ExecuteMinimumSizeChanged() =>
            _onMinimumSizeChangedAction?.Invoke();
        

        private Action? _onModalClosedAction; 
        public TreeAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null) 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = action;
            return this;
        }
        public TreeAction RemoveOnModalClosed() {
            if (_onModalClosedAction == null) return this; 
            Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = null;
            return this;
        }
        private void ExecuteModalClosed() =>
            _onModalClosedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public TreeAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public TreeAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public TreeAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public TreeAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action<int, TreeItem, bool>? _onMultiSelectedAction; 
        public TreeAction OnMultiSelected(Action<int, TreeItem, bool> action) {
            if (_onMultiSelectedAction == null) 
                Connect("multi_selected", this, nameof(ExecuteMultiSelected));
            _onMultiSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnMultiSelected() {
            if (_onMultiSelectedAction == null) return this; 
            Disconnect("multi_selected", this, nameof(ExecuteMultiSelected));
            _onMultiSelectedAction = null;
            return this;
        }
        private void ExecuteMultiSelected(int column, TreeItem item, bool selected) =>
            _onMultiSelectedAction?.Invoke(column, item, selected);
        

        private Action? _onNothingSelectedAction; 
        public TreeAction OnNothingSelected(Action action) {
            if (_onNothingSelectedAction == null) 
                Connect("nothing_selected", this, nameof(ExecuteNothingSelected));
            _onNothingSelectedAction = action;
            return this;
        }
        public TreeAction RemoveOnNothingSelected() {
            if (_onNothingSelectedAction == null) return this; 
            Disconnect("nothing_selected", this, nameof(ExecuteNothingSelected));
            _onNothingSelectedAction = null;
            return this;
        }
        private void ExecuteNothingSelected() =>
            _onNothingSelectedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public TreeAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public TreeAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public TreeAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public TreeAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onResizedAction; 
        public TreeAction OnResized(Action action) {
            if (_onResizedAction == null) 
                Connect("resized", this, nameof(ExecuteResized));
            _onResizedAction = action;
            return this;
        }
        public TreeAction RemoveOnResized() {
            if (_onResizedAction == null) return this; 
            Disconnect("resized", this, nameof(ExecuteResized));
            _onResizedAction = null;
            return this;
        }
        private void ExecuteResized() =>
            _onResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public TreeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public TreeAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onSizeFlagsChangedAction; 
        public TreeAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null) 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = action;
            return this;
        }
        public TreeAction RemoveOnSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null) return this; 
            Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = null;
            return this;
        }
        private void ExecuteSizeFlagsChanged() =>
            _onSizeFlagsChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public TreeAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public TreeAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public TreeAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public TreeAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public TreeAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public TreeAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public TreeAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public TreeAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}