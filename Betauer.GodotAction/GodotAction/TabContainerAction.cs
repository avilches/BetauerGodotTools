using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TabContainerAction : Node {
        public TabContainerAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public TabContainerAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public TabContainerAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public TabContainerAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public TabContainerAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public TabContainerAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public TabContainerAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public TabContainerAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public TabContainerAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public TabContainerAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public TabContainerAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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

        private List<Action>? _onDrawAction; 
        public TabContainerAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                GetParent().Connect("draw", this, nameof(_GodotSignalDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return this;
            _onDrawAction.Remove(action); 
            if (_onDrawAction.Count == 0) {
                GetParent().Disconnect("draw", this, nameof(_GodotSignalDraw));
            }
            return this;
        }
        private void _GodotSignalDraw() {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return;
            for (var i = 0; i < _onDrawAction.Count; i++) _onDrawAction[i].Invoke();
        }
        

        private List<Action>? _onFocusEnteredAction; 
        public TabContainerAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                GetParent().Connect("focus_entered", this, nameof(_GodotSignalFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return this;
            _onFocusEnteredAction.Remove(action); 
            if (_onFocusEnteredAction.Count == 0) {
                GetParent().Disconnect("focus_entered", this, nameof(_GodotSignalFocusEntered));
            }
            return this;
        }
        private void _GodotSignalFocusEntered() {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return;
            for (var i = 0; i < _onFocusEnteredAction.Count; i++) _onFocusEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onFocusExitedAction; 
        public TabContainerAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                GetParent().Connect("focus_exited", this, nameof(_GodotSignalFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return this;
            _onFocusExitedAction.Remove(action); 
            if (_onFocusExitedAction.Count == 0) {
                GetParent().Disconnect("focus_exited", this, nameof(_GodotSignalFocusExited));
            }
            return this;
        }
        private void _GodotSignalFocusExited() {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return;
            for (var i = 0; i < _onFocusExitedAction.Count; i++) _onFocusExitedAction[i].Invoke();
        }
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public TabContainerAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                GetParent().Connect("gui_input", this, nameof(_GodotSignalGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return this;
            _onGuiInputAction.Remove(action); 
            if (_onGuiInputAction.Count == 0) {
                GetParent().Disconnect("gui_input", this, nameof(_GodotSignalGuiInput));
            }
            return this;
        }
        private void _GodotSignalGuiInput(InputEvent @event) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return;
            for (var i = 0; i < _onGuiInputAction.Count; i++) _onGuiInputAction[i].Invoke(@event);
        }
        

        private List<Action>? _onHideAction; 
        public TabContainerAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                GetParent().Connect("hide", this, nameof(_GodotSignalHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) return this;
            _onHideAction.Remove(action); 
            if (_onHideAction.Count == 0) {
                GetParent().Disconnect("hide", this, nameof(_GodotSignalHide));
            }
            return this;
        }
        private void _GodotSignalHide() {
            if (_onHideAction == null || _onHideAction.Count == 0) return;
            for (var i = 0; i < _onHideAction.Count; i++) _onHideAction[i].Invoke();
        }
        

        private List<Action>? _onItemRectChangedAction; 
        public TabContainerAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                GetParent().Connect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return this;
            _onItemRectChangedAction.Remove(action); 
            if (_onItemRectChangedAction.Count == 0) {
                GetParent().Disconnect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            return this;
        }
        private void _GodotSignalItemRectChanged() {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return;
            for (var i = 0; i < _onItemRectChangedAction.Count; i++) _onItemRectChangedAction[i].Invoke();
        }
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public TabContainerAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                GetParent().Connect("minimum_size_changed", this, nameof(_GodotSignalMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return this;
            _onMinimumSizeChangedAction.Remove(action); 
            if (_onMinimumSizeChangedAction.Count == 0) {
                GetParent().Disconnect("minimum_size_changed", this, nameof(_GodotSignalMinimumSizeChanged));
            }
            return this;
        }
        private void _GodotSignalMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return;
            for (var i = 0; i < _onMinimumSizeChangedAction.Count; i++) _onMinimumSizeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onModalClosedAction; 
        public TabContainerAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                GetParent().Connect("modal_closed", this, nameof(_GodotSignalModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return this;
            _onModalClosedAction.Remove(action); 
            if (_onModalClosedAction.Count == 0) {
                GetParent().Disconnect("modal_closed", this, nameof(_GodotSignalModalClosed));
            }
            return this;
        }
        private void _GodotSignalModalClosed() {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return;
            for (var i = 0; i < _onModalClosedAction.Count; i++) _onModalClosedAction[i].Invoke();
        }
        

        private List<Action>? _onMouseEnteredAction; 
        public TabContainerAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                GetParent().Connect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return this;
            _onMouseEnteredAction.Remove(action); 
            if (_onMouseEnteredAction.Count == 0) {
                GetParent().Disconnect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            return this;
        }
        private void _GodotSignalMouseEntered() {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return;
            for (var i = 0; i < _onMouseEnteredAction.Count; i++) _onMouseEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onMouseExitedAction; 
        public TabContainerAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                GetParent().Connect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return this;
            _onMouseExitedAction.Remove(action); 
            if (_onMouseExitedAction.Count == 0) {
                GetParent().Disconnect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            return this;
        }
        private void _GodotSignalMouseExited() {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return;
            for (var i = 0; i < _onMouseExitedAction.Count; i++) _onMouseExitedAction[i].Invoke();
        }
        

        private List<Action>? _onPrePopupPressedAction; 
        public TabContainerAction OnPrePopupPressed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onPrePopupPressedAction == null || _onPrePopupPressedAction.Count == 0) {
                _onPrePopupPressedAction ??= new List<Action>(); 
                GetParent().Connect("pre_popup_pressed", this, nameof(_GodotSignalPrePopupPressed));
            }
            _onPrePopupPressedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnPrePopupPressed(Action action) {
            if (_onPrePopupPressedAction == null || _onPrePopupPressedAction.Count == 0) return this;
            _onPrePopupPressedAction.Remove(action); 
            if (_onPrePopupPressedAction.Count == 0) {
                GetParent().Disconnect("pre_popup_pressed", this, nameof(_GodotSignalPrePopupPressed));
            }
            return this;
        }
        private void _GodotSignalPrePopupPressed() {
            if (_onPrePopupPressedAction == null || _onPrePopupPressedAction.Count == 0) return;
            for (var i = 0; i < _onPrePopupPressedAction.Count; i++) _onPrePopupPressedAction[i].Invoke();
        }
        

        private List<Action>? _onReadyAction; 
        public TabContainerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                GetParent().Connect("ready", this, nameof(_GodotSignalReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                GetParent().Disconnect("ready", this, nameof(_GodotSignalReady));
            }
            return this;
        }
        private void _GodotSignalReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public TabContainerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                GetParent().Connect("renamed", this, nameof(_GodotSignalRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                GetParent().Disconnect("renamed", this, nameof(_GodotSignalRenamed));
            }
            return this;
        }
        private void _GodotSignalRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onResizedAction; 
        public TabContainerAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                GetParent().Connect("resized", this, nameof(_GodotSignalResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return this;
            _onResizedAction.Remove(action); 
            if (_onResizedAction.Count == 0) {
                GetParent().Disconnect("resized", this, nameof(_GodotSignalResized));
            }
            return this;
        }
        private void _GodotSignalResized() {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return;
            for (var i = 0; i < _onResizedAction.Count; i++) _onResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public TabContainerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public TabContainerAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                GetParent().Connect("size_flags_changed", this, nameof(_GodotSignalSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return this;
            _onSizeFlagsChangedAction.Remove(action); 
            if (_onSizeFlagsChangedAction.Count == 0) {
                GetParent().Disconnect("size_flags_changed", this, nameof(_GodotSignalSizeFlagsChanged));
            }
            return this;
        }
        private void _GodotSignalSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return;
            for (var i = 0; i < _onSizeFlagsChangedAction.Count; i++) _onSizeFlagsChangedAction[i].Invoke();
        }
        

        private List<Action>? _onSortChildrenAction; 
        public TabContainerAction OnSortChildren(Action action, bool oneShot = false, bool deferred = false) {
            if (_onSortChildrenAction == null || _onSortChildrenAction.Count == 0) {
                _onSortChildrenAction ??= new List<Action>(); 
                GetParent().Connect("sort_children", this, nameof(_GodotSignalSortChildren));
            }
            _onSortChildrenAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnSortChildren(Action action) {
            if (_onSortChildrenAction == null || _onSortChildrenAction.Count == 0) return this;
            _onSortChildrenAction.Remove(action); 
            if (_onSortChildrenAction.Count == 0) {
                GetParent().Disconnect("sort_children", this, nameof(_GodotSignalSortChildren));
            }
            return this;
        }
        private void _GodotSignalSortChildren() {
            if (_onSortChildrenAction == null || _onSortChildrenAction.Count == 0) return;
            for (var i = 0; i < _onSortChildrenAction.Count; i++) _onSortChildrenAction[i].Invoke();
        }
        

        private List<Action<int>>? _onTabChangedAction; 
        public TabContainerAction OnTabChanged(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onTabChangedAction == null || _onTabChangedAction.Count == 0) {
                _onTabChangedAction ??= new List<Action<int>>(); 
                GetParent().Connect("tab_changed", this, nameof(_GodotSignalTabChanged));
            }
            _onTabChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnTabChanged(Action<int> action) {
            if (_onTabChangedAction == null || _onTabChangedAction.Count == 0) return this;
            _onTabChangedAction.Remove(action); 
            if (_onTabChangedAction.Count == 0) {
                GetParent().Disconnect("tab_changed", this, nameof(_GodotSignalTabChanged));
            }
            return this;
        }
        private void _GodotSignalTabChanged(int tab) {
            if (_onTabChangedAction == null || _onTabChangedAction.Count == 0) return;
            for (var i = 0; i < _onTabChangedAction.Count; i++) _onTabChangedAction[i].Invoke(tab);
        }
        

        private List<Action<int>>? _onTabSelectedAction; 
        public TabContainerAction OnTabSelected(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onTabSelectedAction == null || _onTabSelectedAction.Count == 0) {
                _onTabSelectedAction ??= new List<Action<int>>(); 
                GetParent().Connect("tab_selected", this, nameof(_GodotSignalTabSelected));
            }
            _onTabSelectedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnTabSelected(Action<int> action) {
            if (_onTabSelectedAction == null || _onTabSelectedAction.Count == 0) return this;
            _onTabSelectedAction.Remove(action); 
            if (_onTabSelectedAction.Count == 0) {
                GetParent().Disconnect("tab_selected", this, nameof(_GodotSignalTabSelected));
            }
            return this;
        }
        private void _GodotSignalTabSelected(int tab) {
            if (_onTabSelectedAction == null || _onTabSelectedAction.Count == 0) return;
            for (var i = 0; i < _onTabSelectedAction.Count; i++) _onTabSelectedAction[i].Invoke(tab);
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public TabContainerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                GetParent().Connect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                GetParent().Disconnect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            return this;
        }
        private void _GodotSignalTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public TabContainerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                GetParent().Connect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                GetParent().Disconnect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            return this;
        }
        private void _GodotSignalTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public TabContainerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                GetParent().Connect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                GetParent().Disconnect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            return this;
        }
        private void _GodotSignalTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public TabContainerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                GetParent().Connect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public TabContainerAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                GetParent().Disconnect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            return this;
        }
        private void _GodotSignalVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}