using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TextEditAction : ProxyNode {

        private List<Action<int>>? _onBreakpointToggledAction; 
        public TextEditAction OnBreakpointToggled(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBreakpointToggledAction, "breakpoint_toggled", nameof(_GodotSignalBreakpointToggled), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnBreakpointToggled(Action<int> action) {
            RemoveSignal(_onBreakpointToggledAction, "breakpoint_toggled", nameof(_GodotSignalBreakpointToggled), action);
            return this;
        }

        private TextEditAction _GodotSignalBreakpointToggled(int row) {
            ExecuteSignal(_onBreakpointToggledAction, row);
            return this;
        }

        private List<Action>? _onCursorChangedAction; 
        public TextEditAction OnCursorChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCursorChangedAction, "cursor_changed", nameof(_GodotSignalCursorChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnCursorChanged(Action action) {
            RemoveSignal(_onCursorChangedAction, "cursor_changed", nameof(_GodotSignalCursorChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalCursorChanged() {
            ExecuteSignal(_onCursorChangedAction);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public TextEditAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private TextEditAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public TextEditAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private TextEditAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public TextEditAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private TextEditAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public TextEditAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private TextEditAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public TextEditAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private TextEditAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action<string, int>>? _onInfoClickedAction; 
        public TextEditAction OnInfoClicked(Action<string, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onInfoClickedAction, "info_clicked", nameof(_GodotSignalInfoClicked), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnInfoClicked(Action<string, int> action) {
            RemoveSignal(_onInfoClickedAction, "info_clicked", nameof(_GodotSignalInfoClicked), action);
            return this;
        }

        private TextEditAction _GodotSignalInfoClicked(string info, int row) {
            ExecuteSignal(_onInfoClickedAction, info, row);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public TextEditAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public TextEditAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public TextEditAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private TextEditAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public TextEditAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private TextEditAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public TextEditAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private TextEditAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public TextEditAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private TextEditAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public TextEditAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private TextEditAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onRequestCompletionAction; 
        public TextEditAction OnRequestCompletion(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRequestCompletionAction, "request_completion", nameof(_GodotSignalRequestCompletion), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnRequestCompletion(Action action) {
            RemoveSignal(_onRequestCompletionAction, "request_completion", nameof(_GodotSignalRequestCompletion), action);
            return this;
        }

        private TextEditAction _GodotSignalRequestCompletion() {
            ExecuteSignal(_onRequestCompletionAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public TextEditAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private TextEditAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public TextEditAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public TextEditAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action<int, int, string>>? _onSymbolLookupAction; 
        public TextEditAction OnSymbolLookup(Action<int, int, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSymbolLookupAction, "symbol_lookup", nameof(_GodotSignalSymbolLookup), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnSymbolLookup(Action<int, int, string> action) {
            RemoveSignal(_onSymbolLookupAction, "symbol_lookup", nameof(_GodotSignalSymbolLookup), action);
            return this;
        }

        private TextEditAction _GodotSignalSymbolLookup(int column, int row, string symbol) {
            ExecuteSignal(_onSymbolLookupAction, column, row, symbol);
            return this;
        }

        private List<Action>? _onTextChangedAction; 
        public TextEditAction OnTextChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTextChangedAction, "text_changed", nameof(_GodotSignalTextChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnTextChanged(Action action) {
            RemoveSignal(_onTextChangedAction, "text_changed", nameof(_GodotSignalTextChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalTextChanged() {
            ExecuteSignal(_onTextChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public TextEditAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private TextEditAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public TextEditAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private TextEditAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public TextEditAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private TextEditAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public TextEditAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public TextEditAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private TextEditAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}