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

        private List<Action<int, int, TreeItem>>? _onButtonPressedAction; 
        public TreeAction OnButtonPressed(Action<int, int, TreeItem> action) {
            if (_onButtonPressedAction == null || _onButtonPressedAction.Count == 0) {
                _onButtonPressedAction ??= new List<Action<int, int, TreeItem>>(); 
                Connect("button_pressed", this, nameof(ExecuteButtonPressed));
            }
            _onButtonPressedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnButtonPressed(Action<int, int, TreeItem> action) {
            if (_onButtonPressedAction == null || _onButtonPressedAction.Count == 0) return this;
            _onButtonPressedAction.Remove(action); 
            if (_onButtonPressedAction.Count == 0) {
                Disconnect("button_pressed", this, nameof(ExecuteButtonPressed));
            }
            return this;
        }
        private void ExecuteButtonPressed(int column, int id, TreeItem item) {
            if (_onButtonPressedAction == null || _onButtonPressedAction.Count == 0) return;
            for (var i = 0; i < _onButtonPressedAction.Count; i++) _onButtonPressedAction[i].Invoke(column, id, item);
        }
        

        private List<Action>? _onCellSelectedAction; 
        public TreeAction OnCellSelected(Action action) {
            if (_onCellSelectedAction == null || _onCellSelectedAction.Count == 0) {
                _onCellSelectedAction ??= new List<Action>(); 
                Connect("cell_selected", this, nameof(ExecuteCellSelected));
            }
            _onCellSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnCellSelected(Action action) {
            if (_onCellSelectedAction == null || _onCellSelectedAction.Count == 0) return this;
            _onCellSelectedAction.Remove(action); 
            if (_onCellSelectedAction.Count == 0) {
                Disconnect("cell_selected", this, nameof(ExecuteCellSelected));
            }
            return this;
        }
        private void ExecuteCellSelected() {
            if (_onCellSelectedAction == null || _onCellSelectedAction.Count == 0) return;
            for (var i = 0; i < _onCellSelectedAction.Count; i++) _onCellSelectedAction[i].Invoke();
        }
        

        private List<Action<int>>? _onColumnTitlePressedAction; 
        public TreeAction OnColumnTitlePressed(Action<int> action) {
            if (_onColumnTitlePressedAction == null || _onColumnTitlePressedAction.Count == 0) {
                _onColumnTitlePressedAction ??= new List<Action<int>>(); 
                Connect("column_title_pressed", this, nameof(ExecuteColumnTitlePressed));
            }
            _onColumnTitlePressedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnColumnTitlePressed(Action<int> action) {
            if (_onColumnTitlePressedAction == null || _onColumnTitlePressedAction.Count == 0) return this;
            _onColumnTitlePressedAction.Remove(action); 
            if (_onColumnTitlePressedAction.Count == 0) {
                Disconnect("column_title_pressed", this, nameof(ExecuteColumnTitlePressed));
            }
            return this;
        }
        private void ExecuteColumnTitlePressed(int column) {
            if (_onColumnTitlePressedAction == null || _onColumnTitlePressedAction.Count == 0) return;
            for (var i = 0; i < _onColumnTitlePressedAction.Count; i++) _onColumnTitlePressedAction[i].Invoke(column);
        }
        

        private List<Action<bool>>? _onCustomPopupEditedAction; 
        public TreeAction OnCustomPopupEdited(Action<bool> action) {
            if (_onCustomPopupEditedAction == null || _onCustomPopupEditedAction.Count == 0) {
                _onCustomPopupEditedAction ??= new List<Action<bool>>(); 
                Connect("custom_popup_edited", this, nameof(ExecuteCustomPopupEdited));
            }
            _onCustomPopupEditedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnCustomPopupEdited(Action<bool> action) {
            if (_onCustomPopupEditedAction == null || _onCustomPopupEditedAction.Count == 0) return this;
            _onCustomPopupEditedAction.Remove(action); 
            if (_onCustomPopupEditedAction.Count == 0) {
                Disconnect("custom_popup_edited", this, nameof(ExecuteCustomPopupEdited));
            }
            return this;
        }
        private void ExecuteCustomPopupEdited(bool arrow_clicked) {
            if (_onCustomPopupEditedAction == null || _onCustomPopupEditedAction.Count == 0) return;
            for (var i = 0; i < _onCustomPopupEditedAction.Count; i++) _onCustomPopupEditedAction[i].Invoke(arrow_clicked);
        }
        

        private List<Action>? _onDrawAction; 
        public TreeAction OnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                Connect("draw", this, nameof(ExecuteDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return this;
            _onDrawAction.Remove(action); 
            if (_onDrawAction.Count == 0) {
                Disconnect("draw", this, nameof(ExecuteDraw));
            }
            return this;
        }
        private void ExecuteDraw() {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return;
            for (var i = 0; i < _onDrawAction.Count; i++) _onDrawAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onEmptyRmbAction; 
        public TreeAction OnEmptyRmb(Action<Vector2> action) {
            if (_onEmptyRmbAction == null || _onEmptyRmbAction.Count == 0) {
                _onEmptyRmbAction ??= new List<Action<Vector2>>(); 
                Connect("empty_rmb", this, nameof(ExecuteEmptyRmb));
            }
            _onEmptyRmbAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnEmptyRmb(Action<Vector2> action) {
            if (_onEmptyRmbAction == null || _onEmptyRmbAction.Count == 0) return this;
            _onEmptyRmbAction.Remove(action); 
            if (_onEmptyRmbAction.Count == 0) {
                Disconnect("empty_rmb", this, nameof(ExecuteEmptyRmb));
            }
            return this;
        }
        private void ExecuteEmptyRmb(Vector2 position) {
            if (_onEmptyRmbAction == null || _onEmptyRmbAction.Count == 0) return;
            for (var i = 0; i < _onEmptyRmbAction.Count; i++) _onEmptyRmbAction[i].Invoke(position);
        }
        

        private List<Action<Vector2>>? _onEmptyTreeRmbSelectedAction; 
        public TreeAction OnEmptyTreeRmbSelected(Action<Vector2> action) {
            if (_onEmptyTreeRmbSelectedAction == null || _onEmptyTreeRmbSelectedAction.Count == 0) {
                _onEmptyTreeRmbSelectedAction ??= new List<Action<Vector2>>(); 
                Connect("empty_tree_rmb_selected", this, nameof(ExecuteEmptyTreeRmbSelected));
            }
            _onEmptyTreeRmbSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnEmptyTreeRmbSelected(Action<Vector2> action) {
            if (_onEmptyTreeRmbSelectedAction == null || _onEmptyTreeRmbSelectedAction.Count == 0) return this;
            _onEmptyTreeRmbSelectedAction.Remove(action); 
            if (_onEmptyTreeRmbSelectedAction.Count == 0) {
                Disconnect("empty_tree_rmb_selected", this, nameof(ExecuteEmptyTreeRmbSelected));
            }
            return this;
        }
        private void ExecuteEmptyTreeRmbSelected(Vector2 position) {
            if (_onEmptyTreeRmbSelectedAction == null || _onEmptyTreeRmbSelectedAction.Count == 0) return;
            for (var i = 0; i < _onEmptyTreeRmbSelectedAction.Count; i++) _onEmptyTreeRmbSelectedAction[i].Invoke(position);
        }
        

        private List<Action>? _onFocusEnteredAction; 
        public TreeAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return this;
            _onFocusEnteredAction.Remove(action); 
            if (_onFocusEnteredAction.Count == 0) {
                Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            return this;
        }
        private void ExecuteFocusEntered() {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return;
            for (var i = 0; i < _onFocusEnteredAction.Count; i++) _onFocusEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onFocusExitedAction; 
        public TreeAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return this;
            _onFocusExitedAction.Remove(action); 
            if (_onFocusExitedAction.Count == 0) {
                Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            return this;
        }
        private void ExecuteFocusExited() {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return;
            for (var i = 0; i < _onFocusExitedAction.Count; i++) _onFocusExitedAction[i].Invoke();
        }
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public TreeAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return this;
            _onGuiInputAction.Remove(action); 
            if (_onGuiInputAction.Count == 0) {
                Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            }
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return;
            for (var i = 0; i < _onGuiInputAction.Count; i++) _onGuiInputAction[i].Invoke(@event);
        }
        

        private List<Action>? _onHideAction; 
        public TreeAction OnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                Connect("hide", this, nameof(ExecuteHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) return this;
            _onHideAction.Remove(action); 
            if (_onHideAction.Count == 0) {
                Disconnect("hide", this, nameof(ExecuteHide));
            }
            return this;
        }
        private void ExecuteHide() {
            if (_onHideAction == null || _onHideAction.Count == 0) return;
            for (var i = 0; i < _onHideAction.Count; i++) _onHideAction[i].Invoke();
        }
        

        private List<Action>? _onItemActivatedAction; 
        public TreeAction OnItemActivated(Action action) {
            if (_onItemActivatedAction == null || _onItemActivatedAction.Count == 0) {
                _onItemActivatedAction ??= new List<Action>(); 
                Connect("item_activated", this, nameof(ExecuteItemActivated));
            }
            _onItemActivatedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemActivated(Action action) {
            if (_onItemActivatedAction == null || _onItemActivatedAction.Count == 0) return this;
            _onItemActivatedAction.Remove(action); 
            if (_onItemActivatedAction.Count == 0) {
                Disconnect("item_activated", this, nameof(ExecuteItemActivated));
            }
            return this;
        }
        private void ExecuteItemActivated() {
            if (_onItemActivatedAction == null || _onItemActivatedAction.Count == 0) return;
            for (var i = 0; i < _onItemActivatedAction.Count; i++) _onItemActivatedAction[i].Invoke();
        }
        

        private List<Action<TreeItem>>? _onItemCollapsedAction; 
        public TreeAction OnItemCollapsed(Action<TreeItem> action) {
            if (_onItemCollapsedAction == null || _onItemCollapsedAction.Count == 0) {
                _onItemCollapsedAction ??= new List<Action<TreeItem>>(); 
                Connect("item_collapsed", this, nameof(ExecuteItemCollapsed));
            }
            _onItemCollapsedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemCollapsed(Action<TreeItem> action) {
            if (_onItemCollapsedAction == null || _onItemCollapsedAction.Count == 0) return this;
            _onItemCollapsedAction.Remove(action); 
            if (_onItemCollapsedAction.Count == 0) {
                Disconnect("item_collapsed", this, nameof(ExecuteItemCollapsed));
            }
            return this;
        }
        private void ExecuteItemCollapsed(TreeItem item) {
            if (_onItemCollapsedAction == null || _onItemCollapsedAction.Count == 0) return;
            for (var i = 0; i < _onItemCollapsedAction.Count; i++) _onItemCollapsedAction[i].Invoke(item);
        }
        

        private List<Action>? _onItemCustomButtonPressedAction; 
        public TreeAction OnItemCustomButtonPressed(Action action) {
            if (_onItemCustomButtonPressedAction == null || _onItemCustomButtonPressedAction.Count == 0) {
                _onItemCustomButtonPressedAction ??= new List<Action>(); 
                Connect("item_custom_button_pressed", this, nameof(ExecuteItemCustomButtonPressed));
            }
            _onItemCustomButtonPressedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemCustomButtonPressed(Action action) {
            if (_onItemCustomButtonPressedAction == null || _onItemCustomButtonPressedAction.Count == 0) return this;
            _onItemCustomButtonPressedAction.Remove(action); 
            if (_onItemCustomButtonPressedAction.Count == 0) {
                Disconnect("item_custom_button_pressed", this, nameof(ExecuteItemCustomButtonPressed));
            }
            return this;
        }
        private void ExecuteItemCustomButtonPressed() {
            if (_onItemCustomButtonPressedAction == null || _onItemCustomButtonPressedAction.Count == 0) return;
            for (var i = 0; i < _onItemCustomButtonPressedAction.Count; i++) _onItemCustomButtonPressedAction[i].Invoke();
        }
        

        private List<Action>? _onItemDoubleClickedAction; 
        public TreeAction OnItemDoubleClicked(Action action) {
            if (_onItemDoubleClickedAction == null || _onItemDoubleClickedAction.Count == 0) {
                _onItemDoubleClickedAction ??= new List<Action>(); 
                Connect("item_double_clicked", this, nameof(ExecuteItemDoubleClicked));
            }
            _onItemDoubleClickedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemDoubleClicked(Action action) {
            if (_onItemDoubleClickedAction == null || _onItemDoubleClickedAction.Count == 0) return this;
            _onItemDoubleClickedAction.Remove(action); 
            if (_onItemDoubleClickedAction.Count == 0) {
                Disconnect("item_double_clicked", this, nameof(ExecuteItemDoubleClicked));
            }
            return this;
        }
        private void ExecuteItemDoubleClicked() {
            if (_onItemDoubleClickedAction == null || _onItemDoubleClickedAction.Count == 0) return;
            for (var i = 0; i < _onItemDoubleClickedAction.Count; i++) _onItemDoubleClickedAction[i].Invoke();
        }
        

        private List<Action>? _onItemEditedAction; 
        public TreeAction OnItemEdited(Action action) {
            if (_onItemEditedAction == null || _onItemEditedAction.Count == 0) {
                _onItemEditedAction ??= new List<Action>(); 
                Connect("item_edited", this, nameof(ExecuteItemEdited));
            }
            _onItemEditedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemEdited(Action action) {
            if (_onItemEditedAction == null || _onItemEditedAction.Count == 0) return this;
            _onItemEditedAction.Remove(action); 
            if (_onItemEditedAction.Count == 0) {
                Disconnect("item_edited", this, nameof(ExecuteItemEdited));
            }
            return this;
        }
        private void ExecuteItemEdited() {
            if (_onItemEditedAction == null || _onItemEditedAction.Count == 0) return;
            for (var i = 0; i < _onItemEditedAction.Count; i++) _onItemEditedAction[i].Invoke();
        }
        

        private List<Action>? _onItemRectChangedAction; 
        public TreeAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return this;
            _onItemRectChangedAction.Remove(action); 
            if (_onItemRectChangedAction.Count == 0) {
                Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            return this;
        }
        private void ExecuteItemRectChanged() {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return;
            for (var i = 0; i < _onItemRectChangedAction.Count; i++) _onItemRectChangedAction[i].Invoke();
        }
        

        private List<Action>? _onItemRmbEditedAction; 
        public TreeAction OnItemRmbEdited(Action action) {
            if (_onItemRmbEditedAction == null || _onItemRmbEditedAction.Count == 0) {
                _onItemRmbEditedAction ??= new List<Action>(); 
                Connect("item_rmb_edited", this, nameof(ExecuteItemRmbEdited));
            }
            _onItemRmbEditedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemRmbEdited(Action action) {
            if (_onItemRmbEditedAction == null || _onItemRmbEditedAction.Count == 0) return this;
            _onItemRmbEditedAction.Remove(action); 
            if (_onItemRmbEditedAction.Count == 0) {
                Disconnect("item_rmb_edited", this, nameof(ExecuteItemRmbEdited));
            }
            return this;
        }
        private void ExecuteItemRmbEdited() {
            if (_onItemRmbEditedAction == null || _onItemRmbEditedAction.Count == 0) return;
            for (var i = 0; i < _onItemRmbEditedAction.Count; i++) _onItemRmbEditedAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onItemRmbSelectedAction; 
        public TreeAction OnItemRmbSelected(Action<Vector2> action) {
            if (_onItemRmbSelectedAction == null || _onItemRmbSelectedAction.Count == 0) {
                _onItemRmbSelectedAction ??= new List<Action<Vector2>>(); 
                Connect("item_rmb_selected", this, nameof(ExecuteItemRmbSelected));
            }
            _onItemRmbSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemRmbSelected(Action<Vector2> action) {
            if (_onItemRmbSelectedAction == null || _onItemRmbSelectedAction.Count == 0) return this;
            _onItemRmbSelectedAction.Remove(action); 
            if (_onItemRmbSelectedAction.Count == 0) {
                Disconnect("item_rmb_selected", this, nameof(ExecuteItemRmbSelected));
            }
            return this;
        }
        private void ExecuteItemRmbSelected(Vector2 position) {
            if (_onItemRmbSelectedAction == null || _onItemRmbSelectedAction.Count == 0) return;
            for (var i = 0; i < _onItemRmbSelectedAction.Count; i++) _onItemRmbSelectedAction[i].Invoke(position);
        }
        

        private List<Action>? _onItemSelectedAction; 
        public TreeAction OnItemSelected(Action action) {
            if (_onItemSelectedAction == null || _onItemSelectedAction.Count == 0) {
                _onItemSelectedAction ??= new List<Action>(); 
                Connect("item_selected", this, nameof(ExecuteItemSelected));
            }
            _onItemSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnItemSelected(Action action) {
            if (_onItemSelectedAction == null || _onItemSelectedAction.Count == 0) return this;
            _onItemSelectedAction.Remove(action); 
            if (_onItemSelectedAction.Count == 0) {
                Disconnect("item_selected", this, nameof(ExecuteItemSelected));
            }
            return this;
        }
        private void ExecuteItemSelected() {
            if (_onItemSelectedAction == null || _onItemSelectedAction.Count == 0) return;
            for (var i = 0; i < _onItemSelectedAction.Count; i++) _onItemSelectedAction[i].Invoke();
        }
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public TreeAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return this;
            _onMinimumSizeChangedAction.Remove(action); 
            if (_onMinimumSizeChangedAction.Count == 0) {
                Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            return this;
        }
        private void ExecuteMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return;
            for (var i = 0; i < _onMinimumSizeChangedAction.Count; i++) _onMinimumSizeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onModalClosedAction; 
        public TreeAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return this;
            _onModalClosedAction.Remove(action); 
            if (_onModalClosedAction.Count == 0) {
                Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            return this;
        }
        private void ExecuteModalClosed() {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return;
            for (var i = 0; i < _onModalClosedAction.Count; i++) _onModalClosedAction[i].Invoke();
        }
        

        private List<Action>? _onMouseEnteredAction; 
        public TreeAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return this;
            _onMouseEnteredAction.Remove(action); 
            if (_onMouseEnteredAction.Count == 0) {
                Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            return this;
        }
        private void ExecuteMouseEntered() {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return;
            for (var i = 0; i < _onMouseEnteredAction.Count; i++) _onMouseEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onMouseExitedAction; 
        public TreeAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return this;
            _onMouseExitedAction.Remove(action); 
            if (_onMouseExitedAction.Count == 0) {
                Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            return this;
        }
        private void ExecuteMouseExited() {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return;
            for (var i = 0; i < _onMouseExitedAction.Count; i++) _onMouseExitedAction[i].Invoke();
        }
        

        private List<Action<int, TreeItem, bool>>? _onMultiSelectedAction; 
        public TreeAction OnMultiSelected(Action<int, TreeItem, bool> action) {
            if (_onMultiSelectedAction == null || _onMultiSelectedAction.Count == 0) {
                _onMultiSelectedAction ??= new List<Action<int, TreeItem, bool>>(); 
                Connect("multi_selected", this, nameof(ExecuteMultiSelected));
            }
            _onMultiSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnMultiSelected(Action<int, TreeItem, bool> action) {
            if (_onMultiSelectedAction == null || _onMultiSelectedAction.Count == 0) return this;
            _onMultiSelectedAction.Remove(action); 
            if (_onMultiSelectedAction.Count == 0) {
                Disconnect("multi_selected", this, nameof(ExecuteMultiSelected));
            }
            return this;
        }
        private void ExecuteMultiSelected(int column, TreeItem item, bool selected) {
            if (_onMultiSelectedAction == null || _onMultiSelectedAction.Count == 0) return;
            for (var i = 0; i < _onMultiSelectedAction.Count; i++) _onMultiSelectedAction[i].Invoke(column, item, selected);
        }
        

        private List<Action>? _onNothingSelectedAction; 
        public TreeAction OnNothingSelected(Action action) {
            if (_onNothingSelectedAction == null || _onNothingSelectedAction.Count == 0) {
                _onNothingSelectedAction ??= new List<Action>(); 
                Connect("nothing_selected", this, nameof(ExecuteNothingSelected));
            }
            _onNothingSelectedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnNothingSelected(Action action) {
            if (_onNothingSelectedAction == null || _onNothingSelectedAction.Count == 0) return this;
            _onNothingSelectedAction.Remove(action); 
            if (_onNothingSelectedAction.Count == 0) {
                Disconnect("nothing_selected", this, nameof(ExecuteNothingSelected));
            }
            return this;
        }
        private void ExecuteNothingSelected() {
            if (_onNothingSelectedAction == null || _onNothingSelectedAction.Count == 0) return;
            for (var i = 0; i < _onNothingSelectedAction.Count; i++) _onNothingSelectedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public TreeAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                Disconnect("ready", this, nameof(ExecuteReady));
            }
            return this;
        }
        private void ExecuteReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public TreeAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                Disconnect("renamed", this, nameof(ExecuteRenamed));
            }
            return this;
        }
        private void ExecuteRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onResizedAction; 
        public TreeAction OnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                Connect("resized", this, nameof(ExecuteResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return this;
            _onResizedAction.Remove(action); 
            if (_onResizedAction.Count == 0) {
                Disconnect("resized", this, nameof(ExecuteResized));
            }
            return this;
        }
        private void ExecuteResized() {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return;
            for (var i = 0; i < _onResizedAction.Count; i++) _onResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public TreeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public TreeAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return this;
            _onSizeFlagsChangedAction.Remove(action); 
            if (_onSizeFlagsChangedAction.Count == 0) {
                Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            return this;
        }
        private void ExecuteSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return;
            for (var i = 0; i < _onSizeFlagsChangedAction.Count; i++) _onSizeFlagsChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public TreeAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            return this;
        }
        private void ExecuteTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public TreeAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            return this;
        }
        private void ExecuteTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public TreeAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            return this;
        }
        private void ExecuteTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public TreeAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public TreeAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            return this;
        }
        private void ExecuteVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}