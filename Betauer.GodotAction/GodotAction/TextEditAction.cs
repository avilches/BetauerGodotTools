using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TextEditAction : ProxyNode {

        private List<Action<int>>? _onBreakpointToggledAction; 
        public void OnBreakpointToggled(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBreakpointToggledAction, "breakpoint_toggled", nameof(_GodotSignalBreakpointToggled), action, oneShot, deferred);

        public void RemoveOnBreakpointToggled(Action<int> action) =>
            RemoveSignal(_onBreakpointToggledAction, "breakpoint_toggled", nameof(_GodotSignalBreakpointToggled), action);

        private void _GodotSignalBreakpointToggled(int row) =>
            ExecuteSignal(_onBreakpointToggledAction, row);
        

        private List<Action>? _onCursorChangedAction; 
        public void OnCursorChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onCursorChangedAction, "cursor_changed", nameof(_GodotSignalCursorChanged), action, oneShot, deferred);

        public void RemoveOnCursorChanged(Action action) =>
            RemoveSignal(_onCursorChangedAction, "cursor_changed", nameof(_GodotSignalCursorChanged), action);

        private void _GodotSignalCursorChanged() =>
            ExecuteSignal(_onCursorChangedAction);
        

        private List<Action>? _onDrawAction; 
        public void OnDraw(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);

        public void RemoveOnDraw(Action action) =>
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);

        private void _GodotSignalDraw() =>
            ExecuteSignal(_onDrawAction);
        

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
        

        private List<Action<string, int>>? _onInfoClickedAction; 
        public void OnInfoClicked(Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onInfoClickedAction, "info_clicked", nameof(_GodotSignalInfoClicked), action, oneShot, deferred);

        public void RemoveOnInfoClicked(Action<string, int> action) =>
            RemoveSignal(_onInfoClickedAction, "info_clicked", nameof(_GodotSignalInfoClicked), action);

        private void _GodotSignalInfoClicked(string info, int row) =>
            ExecuteSignal(_onInfoClickedAction, info, row);
        

        private List<Action>? _onItemRectChangedAction; 
        public void OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);

        public void RemoveOnItemRectChanged(Action action) =>
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);

        private void _GodotSignalItemRectChanged() =>
            ExecuteSignal(_onItemRectChangedAction);
        

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
        

        private List<Action>? _onRequestCompletionAction; 
        public void OnRequestCompletion(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRequestCompletionAction, "request_completion", nameof(_GodotSignalRequestCompletion), action, oneShot, deferred);

        public void RemoveOnRequestCompletion(Action action) =>
            RemoveSignal(_onRequestCompletionAction, "request_completion", nameof(_GodotSignalRequestCompletion), action);

        private void _GodotSignalRequestCompletion() =>
            ExecuteSignal(_onRequestCompletionAction);
        

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
        

        private List<Action<int, int, string>>? _onSymbolLookupAction; 
        public void OnSymbolLookup(Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onSymbolLookupAction, "symbol_lookup", nameof(_GodotSignalSymbolLookup), action, oneShot, deferred);

        public void RemoveOnSymbolLookup(Action<int, int, string> action) =>
            RemoveSignal(_onSymbolLookupAction, "symbol_lookup", nameof(_GodotSignalSymbolLookup), action);

        private void _GodotSignalSymbolLookup(int column, int row, string symbol) =>
            ExecuteSignal(_onSymbolLookupAction, column, row, symbol);
        

        private List<Action>? _onTextChangedAction; 
        public void OnTextChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTextChangedAction, "text_changed", nameof(_GodotSignalTextChanged), action, oneShot, deferred);

        public void RemoveOnTextChanged(Action action) =>
            RemoveSignal(_onTextChangedAction, "text_changed", nameof(_GodotSignalTextChanged), action);

        private void _GodotSignalTextChanged() =>
            ExecuteSignal(_onTextChangedAction);
        

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