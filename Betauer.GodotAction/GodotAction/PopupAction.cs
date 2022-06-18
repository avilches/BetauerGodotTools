using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class PopupAction : Popup {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public PopupAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public PopupAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public PopupAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public PopupAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public PopupAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public PopupAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public PopupAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public PopupAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public PopupAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public PopupAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action? _onAboutToShowAction; 
        public PopupAction OnAboutToShow(Action action) {
            if (_onAboutToShowAction == null) 
                Connect("about_to_show", this, nameof(ExecuteAboutToShow));
            _onAboutToShowAction = action;
            return this;
        }
        public PopupAction RemoveOnAboutToShow() {
            if (_onAboutToShowAction == null) return this; 
            Disconnect("about_to_show", this, nameof(ExecuteAboutToShow));
            _onAboutToShowAction = null;
            return this;
        }
        private void ExecuteAboutToShow() =>
            _onAboutToShowAction?.Invoke();
        

        private Action? _onDrawAction; 
        public PopupAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public PopupAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onFocusEnteredAction; 
        public PopupAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null) 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = action;
            return this;
        }
        public PopupAction RemoveOnFocusEntered() {
            if (_onFocusEnteredAction == null) return this; 
            Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = null;
            return this;
        }
        private void ExecuteFocusEntered() =>
            _onFocusEnteredAction?.Invoke();
        

        private Action? _onFocusExitedAction; 
        public PopupAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null) 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = action;
            return this;
        }
        public PopupAction RemoveOnFocusExited() {
            if (_onFocusExitedAction == null) return this; 
            Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = null;
            return this;
        }
        private void ExecuteFocusExited() =>
            _onFocusExitedAction?.Invoke();
        

        private Action<InputEvent>? _onGuiInputAction; 
        public PopupAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null) 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = action;
            return this;
        }
        public PopupAction RemoveOnGuiInput() {
            if (_onGuiInputAction == null) return this; 
            Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = null;
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) =>
            _onGuiInputAction?.Invoke(@event);
        

        private Action? _onHideAction; 
        public PopupAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public PopupAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public PopupAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public PopupAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onMinimumSizeChangedAction; 
        public PopupAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null) 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = action;
            return this;
        }
        public PopupAction RemoveOnMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null) return this; 
            Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = null;
            return this;
        }
        private void ExecuteMinimumSizeChanged() =>
            _onMinimumSizeChangedAction?.Invoke();
        

        private Action? _onModalClosedAction; 
        public PopupAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null) 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = action;
            return this;
        }
        public PopupAction RemoveOnModalClosed() {
            if (_onModalClosedAction == null) return this; 
            Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = null;
            return this;
        }
        private void ExecuteModalClosed() =>
            _onModalClosedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public PopupAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public PopupAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public PopupAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public PopupAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onPopupHideAction; 
        public PopupAction OnPopupHide(Action action) {
            if (_onPopupHideAction == null) 
                Connect("popup_hide", this, nameof(ExecutePopupHide));
            _onPopupHideAction = action;
            return this;
        }
        public PopupAction RemoveOnPopupHide() {
            if (_onPopupHideAction == null) return this; 
            Disconnect("popup_hide", this, nameof(ExecutePopupHide));
            _onPopupHideAction = null;
            return this;
        }
        private void ExecutePopupHide() =>
            _onPopupHideAction?.Invoke();
        

        private Action? _onReadyAction; 
        public PopupAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public PopupAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public PopupAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public PopupAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onResizedAction; 
        public PopupAction OnResized(Action action) {
            if (_onResizedAction == null) 
                Connect("resized", this, nameof(ExecuteResized));
            _onResizedAction = action;
            return this;
        }
        public PopupAction RemoveOnResized() {
            if (_onResizedAction == null) return this; 
            Disconnect("resized", this, nameof(ExecuteResized));
            _onResizedAction = null;
            return this;
        }
        private void ExecuteResized() =>
            _onResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public PopupAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public PopupAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onSizeFlagsChangedAction; 
        public PopupAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null) 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = action;
            return this;
        }
        public PopupAction RemoveOnSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null) return this; 
            Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = null;
            return this;
        }
        private void ExecuteSizeFlagsChanged() =>
            _onSizeFlagsChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public PopupAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public PopupAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public PopupAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public PopupAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public PopupAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public PopupAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public PopupAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public PopupAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}