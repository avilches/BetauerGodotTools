using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class RichTextLabelAction : RichTextLabel {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public RichTextLabelAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public RichTextLabelAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public RichTextLabelAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public RichTextLabelAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public RichTextLabelAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public RichTextLabelAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public RichTextLabelAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public RichTextLabelAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public RichTextLabelAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public RichTextLabelAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private Action? _onDrawAction; 
        public RichTextLabelAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onFocusEnteredAction; 
        public RichTextLabelAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null) 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnFocusEntered() {
            if (_onFocusEnteredAction == null) return this; 
            Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = null;
            return this;
        }
        private void ExecuteFocusEntered() =>
            _onFocusEnteredAction?.Invoke();
        

        private Action? _onFocusExitedAction; 
        public RichTextLabelAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null) 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnFocusExited() {
            if (_onFocusExitedAction == null) return this; 
            Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = null;
            return this;
        }
        private void ExecuteFocusExited() =>
            _onFocusExitedAction?.Invoke();
        

        private Action<InputEvent>? _onGuiInputAction; 
        public RichTextLabelAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null) 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnGuiInput() {
            if (_onGuiInputAction == null) return this; 
            Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = null;
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) =>
            _onGuiInputAction?.Invoke(@event);
        

        private Action? _onHideAction; 
        public RichTextLabelAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public RichTextLabelAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action<object>? _onMetaClickedAction; 
        public RichTextLabelAction OnMetaClicked(Action<object> action) {
            if (_onMetaClickedAction == null) 
                Connect("meta_clicked", this, nameof(ExecuteMetaClicked));
            _onMetaClickedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMetaClicked() {
            if (_onMetaClickedAction == null) return this; 
            Disconnect("meta_clicked", this, nameof(ExecuteMetaClicked));
            _onMetaClickedAction = null;
            return this;
        }
        private void ExecuteMetaClicked(object meta) =>
            _onMetaClickedAction?.Invoke(meta);
        

        private Action<object>? _onMetaHoverEndedAction; 
        public RichTextLabelAction OnMetaHoverEnded(Action<object> action) {
            if (_onMetaHoverEndedAction == null) 
                Connect("meta_hover_ended", this, nameof(ExecuteMetaHoverEnded));
            _onMetaHoverEndedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMetaHoverEnded() {
            if (_onMetaHoverEndedAction == null) return this; 
            Disconnect("meta_hover_ended", this, nameof(ExecuteMetaHoverEnded));
            _onMetaHoverEndedAction = null;
            return this;
        }
        private void ExecuteMetaHoverEnded(object meta) =>
            _onMetaHoverEndedAction?.Invoke(meta);
        

        private Action<object>? _onMetaHoverStartedAction; 
        public RichTextLabelAction OnMetaHoverStarted(Action<object> action) {
            if (_onMetaHoverStartedAction == null) 
                Connect("meta_hover_started", this, nameof(ExecuteMetaHoverStarted));
            _onMetaHoverStartedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMetaHoverStarted() {
            if (_onMetaHoverStartedAction == null) return this; 
            Disconnect("meta_hover_started", this, nameof(ExecuteMetaHoverStarted));
            _onMetaHoverStartedAction = null;
            return this;
        }
        private void ExecuteMetaHoverStarted(object meta) =>
            _onMetaHoverStartedAction?.Invoke(meta);
        

        private Action? _onMinimumSizeChangedAction; 
        public RichTextLabelAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null) 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null) return this; 
            Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = null;
            return this;
        }
        private void ExecuteMinimumSizeChanged() =>
            _onMinimumSizeChangedAction?.Invoke();
        

        private Action? _onModalClosedAction; 
        public RichTextLabelAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null) 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnModalClosed() {
            if (_onModalClosedAction == null) return this; 
            Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = null;
            return this;
        }
        private void ExecuteModalClosed() =>
            _onModalClosedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public RichTextLabelAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public RichTextLabelAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public RichTextLabelAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public RichTextLabelAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onResizedAction; 
        public RichTextLabelAction OnResized(Action action) {
            if (_onResizedAction == null) 
                Connect("resized", this, nameof(ExecuteResized));
            _onResizedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnResized() {
            if (_onResizedAction == null) return this; 
            Disconnect("resized", this, nameof(ExecuteResized));
            _onResizedAction = null;
            return this;
        }
        private void ExecuteResized() =>
            _onResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public RichTextLabelAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onSizeFlagsChangedAction; 
        public RichTextLabelAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null) 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null) return this; 
            Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = null;
            return this;
        }
        private void ExecuteSizeFlagsChanged() =>
            _onSizeFlagsChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public RichTextLabelAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public RichTextLabelAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public RichTextLabelAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public RichTextLabelAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public RichTextLabelAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}