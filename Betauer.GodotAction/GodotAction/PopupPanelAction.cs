using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class PopupPanelAction : PopupPanel {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public PopupPanelAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public PopupPanelAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public PopupPanelAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public PopupPanelAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public PopupPanelAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public PopupPanelAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public PopupPanelAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public PopupPanelAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public PopupPanelAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public PopupPanelAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessAction == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessAction.Count; i++) _onProcessAction[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcess == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcess.Count; i++) _onPhysicsProcess[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInput == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInput.Count; i++) _onInput[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInput == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInput.Count; i++) _onUnhandledInput[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInput == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInput.Count; i++) _onUnhandledKeyInput[i].Invoke(@event);
        }

        private Action? _onAboutToShowAction; 
        public PopupPanelAction OnAboutToShow(Action action) {
            if (_onAboutToShowAction == null) 
                Connect("about_to_show", this, nameof(ExecuteAboutToShow));
            _onAboutToShowAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnAboutToShow() {
            if (_onAboutToShowAction == null) return this; 
            Disconnect("about_to_show", this, nameof(ExecuteAboutToShow));
            _onAboutToShowAction = null;
            return this;
        }
        private void ExecuteAboutToShow() =>
            _onAboutToShowAction?.Invoke();
        

        private Action? _onDrawAction; 
        public PopupPanelAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onFocusEnteredAction; 
        public PopupPanelAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null) 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnFocusEntered() {
            if (_onFocusEnteredAction == null) return this; 
            Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = null;
            return this;
        }
        private void ExecuteFocusEntered() =>
            _onFocusEnteredAction?.Invoke();
        

        private Action? _onFocusExitedAction; 
        public PopupPanelAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null) 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnFocusExited() {
            if (_onFocusExitedAction == null) return this; 
            Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = null;
            return this;
        }
        private void ExecuteFocusExited() =>
            _onFocusExitedAction?.Invoke();
        

        private Action<InputEvent>? _onGuiInputAction; 
        public PopupPanelAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null) 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnGuiInput() {
            if (_onGuiInputAction == null) return this; 
            Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = null;
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) =>
            _onGuiInputAction?.Invoke(@event);
        

        private Action? _onHideAction; 
        public PopupPanelAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public PopupPanelAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onMinimumSizeChangedAction; 
        public PopupPanelAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null) 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null) return this; 
            Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = null;
            return this;
        }
        private void ExecuteMinimumSizeChanged() =>
            _onMinimumSizeChangedAction?.Invoke();
        

        private Action? _onModalClosedAction; 
        public PopupPanelAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null) 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnModalClosed() {
            if (_onModalClosedAction == null) return this; 
            Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = null;
            return this;
        }
        private void ExecuteModalClosed() =>
            _onModalClosedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public PopupPanelAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public PopupPanelAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onPopupHideAction; 
        public PopupPanelAction OnPopupHide(Action action) {
            if (_onPopupHideAction == null) 
                Connect("popup_hide", this, nameof(ExecutePopupHide));
            _onPopupHideAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnPopupHide() {
            if (_onPopupHideAction == null) return this; 
            Disconnect("popup_hide", this, nameof(ExecutePopupHide));
            _onPopupHideAction = null;
            return this;
        }
        private void ExecutePopupHide() =>
            _onPopupHideAction?.Invoke();
        

        private Action? _onReadyAction; 
        public PopupPanelAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public PopupPanelAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onResizedAction; 
        public PopupPanelAction OnResized(Action action) {
            if (_onResizedAction == null) 
                Connect("resized", this, nameof(ExecuteResized));
            _onResizedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnResized() {
            if (_onResizedAction == null) return this; 
            Disconnect("resized", this, nameof(ExecuteResized));
            _onResizedAction = null;
            return this;
        }
        private void ExecuteResized() =>
            _onResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public PopupPanelAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onSizeFlagsChangedAction; 
        public PopupPanelAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null) 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null) return this; 
            Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = null;
            return this;
        }
        private void ExecuteSizeFlagsChanged() =>
            _onSizeFlagsChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public PopupPanelAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public PopupPanelAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public PopupPanelAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public PopupPanelAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public PopupPanelAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}