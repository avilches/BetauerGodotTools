using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class CheckBoxAction : CheckBox {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public CheckBoxAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public CheckBoxAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public CheckBoxAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public CheckBoxAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public CheckBoxAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public CheckBoxAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public CheckBoxAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public CheckBoxAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public CheckBoxAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public CheckBoxAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action>? _onButtonDownAction; 
        public CheckBoxAction OnButtonDown(Action action) {
            if (_onButtonDownAction == null || _onButtonDownAction.Count == 0) {
                _onButtonDownAction ??= new List<Action>(); 
                Connect("button_down", this, nameof(ExecuteButtonDown));
            }
            _onButtonDownAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnButtonDown(Action action) {
            if (_onButtonDownAction == null || _onButtonDownAction.Count == 0) return this;
            _onButtonDownAction.Remove(action); 
            if (_onButtonDownAction.Count == 0) {
                Disconnect("button_down", this, nameof(ExecuteButtonDown));
            }
            return this;
        }
        private void ExecuteButtonDown() {
            if (_onButtonDownAction == null || _onButtonDownAction.Count == 0) return;
            for (var i = 0; i < _onButtonDownAction.Count; i++) _onButtonDownAction[i].Invoke();
        }
        

        private List<Action>? _onButtonUpAction; 
        public CheckBoxAction OnButtonUp(Action action) {
            if (_onButtonUpAction == null || _onButtonUpAction.Count == 0) {
                _onButtonUpAction ??= new List<Action>(); 
                Connect("button_up", this, nameof(ExecuteButtonUp));
            }
            _onButtonUpAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnButtonUp(Action action) {
            if (_onButtonUpAction == null || _onButtonUpAction.Count == 0) return this;
            _onButtonUpAction.Remove(action); 
            if (_onButtonUpAction.Count == 0) {
                Disconnect("button_up", this, nameof(ExecuteButtonUp));
            }
            return this;
        }
        private void ExecuteButtonUp() {
            if (_onButtonUpAction == null || _onButtonUpAction.Count == 0) return;
            for (var i = 0; i < _onButtonUpAction.Count; i++) _onButtonUpAction[i].Invoke();
        }
        

        private List<Action>? _onDrawAction; 
        public CheckBoxAction OnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                Connect("draw", this, nameof(ExecuteDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnDraw(Action action) {
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
        

        private List<Action>? _onFocusEnteredAction; 
        public CheckBoxAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnFocusEntered(Action action) {
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
        public CheckBoxAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnFocusExited(Action action) {
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
        public CheckBoxAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnGuiInput(Action<InputEvent> action) {
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
        public CheckBoxAction OnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                Connect("hide", this, nameof(ExecuteHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnHide(Action action) {
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
        public CheckBoxAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnItemRectChanged(Action action) {
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
        public CheckBoxAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnMinimumSizeChanged(Action action) {
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
        public CheckBoxAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnModalClosed(Action action) {
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
        public CheckBoxAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnMouseEntered(Action action) {
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
        public CheckBoxAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnMouseExited(Action action) {
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
        

        private List<Action>? _onPressedAction; 
        public CheckBoxAction OnPressed(Action action) {
            if (_onPressedAction == null || _onPressedAction.Count == 0) {
                _onPressedAction ??= new List<Action>(); 
                Connect("pressed", this, nameof(ExecutePressed));
            }
            _onPressedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnPressed(Action action) {
            if (_onPressedAction == null || _onPressedAction.Count == 0) return this;
            _onPressedAction.Remove(action); 
            if (_onPressedAction.Count == 0) {
                Disconnect("pressed", this, nameof(ExecutePressed));
            }
            return this;
        }
        private void ExecutePressed() {
            if (_onPressedAction == null || _onPressedAction.Count == 0) return;
            for (var i = 0; i < _onPressedAction.Count; i++) _onPressedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public CheckBoxAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnReady(Action action) {
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
        public CheckBoxAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnRenamed(Action action) {
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
        public CheckBoxAction OnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                Connect("resized", this, nameof(ExecuteResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnResized(Action action) {
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
        public CheckBoxAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnScriptChanged(Action action) {
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
        public CheckBoxAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnSizeFlagsChanged(Action action) {
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
        

        private List<Action<bool>>? _onToggledAction; 
        public CheckBoxAction OnToggled(Action<bool> action) {
            if (_onToggledAction == null || _onToggledAction.Count == 0) {
                _onToggledAction ??= new List<Action<bool>>(); 
                Connect("toggled", this, nameof(ExecuteToggled));
            }
            _onToggledAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnToggled(Action<bool> action) {
            if (_onToggledAction == null || _onToggledAction.Count == 0) return this;
            _onToggledAction.Remove(action); 
            if (_onToggledAction.Count == 0) {
                Disconnect("toggled", this, nameof(ExecuteToggled));
            }
            return this;
        }
        private void ExecuteToggled(bool button_pressed) {
            if (_onToggledAction == null || _onToggledAction.Count == 0) return;
            for (var i = 0; i < _onToggledAction.Count; i++) _onToggledAction[i].Invoke(button_pressed);
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public CheckBoxAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnTreeEntered(Action action) {
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
        public CheckBoxAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnTreeExited(Action action) {
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
        public CheckBoxAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnTreeExiting(Action action) {
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
        public CheckBoxAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public CheckBoxAction RemoveOnVisibilityChanged(Action action) {
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