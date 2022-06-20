using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class FileDialogAction : FileDialog {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public FileDialogAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public FileDialogAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public FileDialogAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public FileDialogAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public FileDialogAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public FileDialogAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public FileDialogAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public FileDialogAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public FileDialogAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public FileDialogAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action>? _onAboutToShowAction; 
        public FileDialogAction OnAboutToShow(Action action) {
            if (_onAboutToShowAction == null || _onAboutToShowAction.Count == 0) {
                _onAboutToShowAction ??= new List<Action>(); 
                Connect("about_to_show", this, nameof(ExecuteAboutToShow));
            }
            _onAboutToShowAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnAboutToShow(Action action) {
            if (_onAboutToShowAction == null || _onAboutToShowAction.Count == 0) return this;
            _onAboutToShowAction.Remove(action); 
            if (_onAboutToShowAction.Count == 0) {
                Disconnect("about_to_show", this, nameof(ExecuteAboutToShow));
            }
            return this;
        }
        private void ExecuteAboutToShow() {
            if (_onAboutToShowAction == null || _onAboutToShowAction.Count == 0) return;
            for (var i = 0; i < _onAboutToShowAction.Count; i++) _onAboutToShowAction[i].Invoke();
        }
        

        private List<Action>? _onConfirmedAction; 
        public FileDialogAction OnConfirmed(Action action) {
            if (_onConfirmedAction == null || _onConfirmedAction.Count == 0) {
                _onConfirmedAction ??= new List<Action>(); 
                Connect("confirmed", this, nameof(ExecuteConfirmed));
            }
            _onConfirmedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnConfirmed(Action action) {
            if (_onConfirmedAction == null || _onConfirmedAction.Count == 0) return this;
            _onConfirmedAction.Remove(action); 
            if (_onConfirmedAction.Count == 0) {
                Disconnect("confirmed", this, nameof(ExecuteConfirmed));
            }
            return this;
        }
        private void ExecuteConfirmed() {
            if (_onConfirmedAction == null || _onConfirmedAction.Count == 0) return;
            for (var i = 0; i < _onConfirmedAction.Count; i++) _onConfirmedAction[i].Invoke();
        }
        

        private List<Action<string>>? _onCustomActionAction; 
        public FileDialogAction OnCustomAction(Action<string> action) {
            if (_onCustomActionAction == null || _onCustomActionAction.Count == 0) {
                _onCustomActionAction ??= new List<Action<string>>(); 
                Connect("custom_action", this, nameof(ExecuteCustomAction));
            }
            _onCustomActionAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnCustomAction(Action<string> action) {
            if (_onCustomActionAction == null || _onCustomActionAction.Count == 0) return this;
            _onCustomActionAction.Remove(action); 
            if (_onCustomActionAction.Count == 0) {
                Disconnect("custom_action", this, nameof(ExecuteCustomAction));
            }
            return this;
        }
        private void ExecuteCustomAction(string action) {
            if (_onCustomActionAction == null || _onCustomActionAction.Count == 0) return;
            for (var i = 0; i < _onCustomActionAction.Count; i++) _onCustomActionAction[i].Invoke(action);
        }
        

        private List<Action<string>>? _onDirSelectedAction; 
        public FileDialogAction OnDirSelected(Action<string> action) {
            if (_onDirSelectedAction == null || _onDirSelectedAction.Count == 0) {
                _onDirSelectedAction ??= new List<Action<string>>(); 
                Connect("dir_selected", this, nameof(ExecuteDirSelected));
            }
            _onDirSelectedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnDirSelected(Action<string> action) {
            if (_onDirSelectedAction == null || _onDirSelectedAction.Count == 0) return this;
            _onDirSelectedAction.Remove(action); 
            if (_onDirSelectedAction.Count == 0) {
                Disconnect("dir_selected", this, nameof(ExecuteDirSelected));
            }
            return this;
        }
        private void ExecuteDirSelected(string dir) {
            if (_onDirSelectedAction == null || _onDirSelectedAction.Count == 0) return;
            for (var i = 0; i < _onDirSelectedAction.Count; i++) _onDirSelectedAction[i].Invoke(dir);
        }
        

        private List<Action>? _onDrawAction; 
        public FileDialogAction OnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                Connect("draw", this, nameof(ExecuteDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnDraw(Action action) {
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
        

        private List<Action<string>>? _onFileSelectedAction; 
        public FileDialogAction OnFileSelected(Action<string> action) {
            if (_onFileSelectedAction == null || _onFileSelectedAction.Count == 0) {
                _onFileSelectedAction ??= new List<Action<string>>(); 
                Connect("file_selected", this, nameof(ExecuteFileSelected));
            }
            _onFileSelectedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnFileSelected(Action<string> action) {
            if (_onFileSelectedAction == null || _onFileSelectedAction.Count == 0) return this;
            _onFileSelectedAction.Remove(action); 
            if (_onFileSelectedAction.Count == 0) {
                Disconnect("file_selected", this, nameof(ExecuteFileSelected));
            }
            return this;
        }
        private void ExecuteFileSelected(string path) {
            if (_onFileSelectedAction == null || _onFileSelectedAction.Count == 0) return;
            for (var i = 0; i < _onFileSelectedAction.Count; i++) _onFileSelectedAction[i].Invoke(path);
        }
        

        private List<Action<string[]>>? _onFilesSelectedAction; 
        public FileDialogAction OnFilesSelected(Action<string[]> action) {
            if (_onFilesSelectedAction == null || _onFilesSelectedAction.Count == 0) {
                _onFilesSelectedAction ??= new List<Action<string[]>>(); 
                Connect("files_selected", this, nameof(ExecuteFilesSelected));
            }
            _onFilesSelectedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnFilesSelected(Action<string[]> action) {
            if (_onFilesSelectedAction == null || _onFilesSelectedAction.Count == 0) return this;
            _onFilesSelectedAction.Remove(action); 
            if (_onFilesSelectedAction.Count == 0) {
                Disconnect("files_selected", this, nameof(ExecuteFilesSelected));
            }
            return this;
        }
        private void ExecuteFilesSelected(string[] paths) {
            if (_onFilesSelectedAction == null || _onFilesSelectedAction.Count == 0) return;
            for (var i = 0; i < _onFilesSelectedAction.Count; i++) _onFilesSelectedAction[i].Invoke(paths);
        }
        

        private List<Action>? _onFocusEnteredAction; 
        public FileDialogAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnFocusEntered(Action action) {
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
        public FileDialogAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnFocusExited(Action action) {
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
        public FileDialogAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnGuiInput(Action<InputEvent> action) {
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
        public FileDialogAction OnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                Connect("hide", this, nameof(ExecuteHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnHide(Action action) {
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
        

        private List<Action>? _onItemRectChangedAction; 
        public FileDialogAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnItemRectChanged(Action action) {
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
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public FileDialogAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnMinimumSizeChanged(Action action) {
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
        public FileDialogAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnModalClosed(Action action) {
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
        public FileDialogAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnMouseEntered(Action action) {
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
        public FileDialogAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnMouseExited(Action action) {
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
        

        private List<Action>? _onPopupHideAction; 
        public FileDialogAction OnPopupHide(Action action) {
            if (_onPopupHideAction == null || _onPopupHideAction.Count == 0) {
                _onPopupHideAction ??= new List<Action>(); 
                Connect("popup_hide", this, nameof(ExecutePopupHide));
            }
            _onPopupHideAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnPopupHide(Action action) {
            if (_onPopupHideAction == null || _onPopupHideAction.Count == 0) return this;
            _onPopupHideAction.Remove(action); 
            if (_onPopupHideAction.Count == 0) {
                Disconnect("popup_hide", this, nameof(ExecutePopupHide));
            }
            return this;
        }
        private void ExecutePopupHide() {
            if (_onPopupHideAction == null || _onPopupHideAction.Count == 0) return;
            for (var i = 0; i < _onPopupHideAction.Count; i++) _onPopupHideAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public FileDialogAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnReady(Action action) {
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
        public FileDialogAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnRenamed(Action action) {
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
        public FileDialogAction OnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                Connect("resized", this, nameof(ExecuteResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnResized(Action action) {
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
        public FileDialogAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnScriptChanged(Action action) {
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
        public FileDialogAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnSizeFlagsChanged(Action action) {
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
        public FileDialogAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnTreeEntered(Action action) {
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
        public FileDialogAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnTreeExited(Action action) {
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
        public FileDialogAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnTreeExiting(Action action) {
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
        public FileDialogAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public FileDialogAction RemoveOnVisibilityChanged(Action action) {
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