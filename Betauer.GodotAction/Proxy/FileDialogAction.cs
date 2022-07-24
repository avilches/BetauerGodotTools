using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class FileDialogAction : ProxyNode {

        private List<Action>? _onAboutToShowAction; 
        public FileDialogAction OnAboutToShow(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAboutToShowAction, "about_to_show", nameof(_GodotSignalAboutToShow), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnAboutToShow(Action action) {
            RemoveSignal(_onAboutToShowAction, "about_to_show", nameof(_GodotSignalAboutToShow), action);
            return this;
        }

        private FileDialogAction _GodotSignalAboutToShow() {
            ExecuteSignal(_onAboutToShowAction);
            return this;
        }

        private List<Action>? _onConfirmedAction; 
        public FileDialogAction OnConfirmed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConfirmedAction, "confirmed", nameof(_GodotSignalConfirmed), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnConfirmed(Action action) {
            RemoveSignal(_onConfirmedAction, "confirmed", nameof(_GodotSignalConfirmed), action);
            return this;
        }

        private FileDialogAction _GodotSignalConfirmed() {
            ExecuteSignal(_onConfirmedAction);
            return this;
        }

        private List<Action<string>>? _onCustomActionAction; 
        public FileDialogAction OnCustomAction(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCustomActionAction, "custom_action", nameof(_GodotSignalCustomAction), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnCustomAction(Action<string> action) {
            RemoveSignal(_onCustomActionAction, "custom_action", nameof(_GodotSignalCustomAction), action);
            return this;
        }

        private FileDialogAction _GodotSignalCustomAction(string action) {
            ExecuteSignal(_onCustomActionAction, action);
            return this;
        }

        private List<Action<string>>? _onDirSelectedAction; 
        public FileDialogAction OnDirSelected(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDirSelectedAction, "dir_selected", nameof(_GodotSignalDirSelected), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnDirSelected(Action<string> action) {
            RemoveSignal(_onDirSelectedAction, "dir_selected", nameof(_GodotSignalDirSelected), action);
            return this;
        }

        private FileDialogAction _GodotSignalDirSelected(string dir) {
            ExecuteSignal(_onDirSelectedAction, dir);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public FileDialogAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private FileDialogAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action<string>>? _onFileSelectedAction; 
        public FileDialogAction OnFileSelected(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFileSelectedAction, "file_selected", nameof(_GodotSignalFileSelected), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnFileSelected(Action<string> action) {
            RemoveSignal(_onFileSelectedAction, "file_selected", nameof(_GodotSignalFileSelected), action);
            return this;
        }

        private FileDialogAction _GodotSignalFileSelected(string path) {
            ExecuteSignal(_onFileSelectedAction, path);
            return this;
        }

        private List<Action<string[]>>? _onFilesSelectedAction; 
        public FileDialogAction OnFilesSelected(Action<string[]> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFilesSelectedAction, "files_selected", nameof(_GodotSignalFilesSelected), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnFilesSelected(Action<string[]> action) {
            RemoveSignal(_onFilesSelectedAction, "files_selected", nameof(_GodotSignalFilesSelected), action);
            return this;
        }

        private FileDialogAction _GodotSignalFilesSelected(string[] paths) {
            ExecuteSignal(_onFilesSelectedAction, paths);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public FileDialogAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private FileDialogAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public FileDialogAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private FileDialogAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public FileDialogAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private FileDialogAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public FileDialogAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private FileDialogAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public FileDialogAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private FileDialogAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public FileDialogAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private FileDialogAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public FileDialogAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private FileDialogAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public FileDialogAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private FileDialogAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public FileDialogAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private FileDialogAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onPopupHideAction; 
        public FileDialogAction OnPopupHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPopupHideAction, "popup_hide", nameof(_GodotSignalPopupHide), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnPopupHide(Action action) {
            RemoveSignal(_onPopupHideAction, "popup_hide", nameof(_GodotSignalPopupHide), action);
            return this;
        }

        private FileDialogAction _GodotSignalPopupHide() {
            ExecuteSignal(_onPopupHideAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public FileDialogAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private FileDialogAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public FileDialogAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private FileDialogAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public FileDialogAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private FileDialogAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public FileDialogAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private FileDialogAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public FileDialogAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private FileDialogAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public FileDialogAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private FileDialogAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public FileDialogAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private FileDialogAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public FileDialogAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private FileDialogAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public FileDialogAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public FileDialogAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private FileDialogAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}