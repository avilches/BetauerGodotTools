using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisibilityEnabler2DAction : VisibilityEnabler2D {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public VisibilityEnabler2DAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public VisibilityEnabler2DAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public VisibilityEnabler2DAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public VisibilityEnabler2DAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public VisibilityEnabler2DAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public VisibilityEnabler2DAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
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
        public VisibilityEnabler2DAction OnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                Connect("draw", this, nameof(ExecuteDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnDraw(Action action) {
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
        

        private List<Action>? _onHideAction; 
        public VisibilityEnabler2DAction OnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                Connect("hide", this, nameof(ExecuteHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnHide(Action action) {
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
        public VisibilityEnabler2DAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnItemRectChanged(Action action) {
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
        

        private List<Action>? _onReadyAction; 
        public VisibilityEnabler2DAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnReady(Action action) {
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
        public VisibilityEnabler2DAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnRenamed(Action action) {
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
        

        private List<Action>? _onScreenEnteredAction; 
        public VisibilityEnabler2DAction OnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) {
                _onScreenEnteredAction ??= new List<Action>(); 
                Connect("screen_entered", this, nameof(ExecuteScreenEntered));
            }
            _onScreenEnteredAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) return this;
            _onScreenEnteredAction.Remove(action); 
            if (_onScreenEnteredAction.Count == 0) {
                Disconnect("screen_entered", this, nameof(ExecuteScreenEntered));
            }
            return this;
        }
        private void ExecuteScreenEntered() {
            if (_onScreenEnteredAction == null || _onScreenEnteredAction.Count == 0) return;
            for (var i = 0; i < _onScreenEnteredAction.Count; i++) _onScreenEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onScreenExitedAction; 
        public VisibilityEnabler2DAction OnScreenExited(Action action) {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) {
                _onScreenExitedAction ??= new List<Action>(); 
                Connect("screen_exited", this, nameof(ExecuteScreenExited));
            }
            _onScreenExitedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnScreenExited(Action action) {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) return this;
            _onScreenExitedAction.Remove(action); 
            if (_onScreenExitedAction.Count == 0) {
                Disconnect("screen_exited", this, nameof(ExecuteScreenExited));
            }
            return this;
        }
        private void ExecuteScreenExited() {
            if (_onScreenExitedAction == null || _onScreenExitedAction.Count == 0) return;
            for (var i = 0; i < _onScreenExitedAction.Count; i++) _onScreenExitedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public VisibilityEnabler2DAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnScriptChanged(Action action) {
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
        

        private List<Action>? _onTreeEnteredAction; 
        public VisibilityEnabler2DAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnTreeEntered(Action action) {
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
        public VisibilityEnabler2DAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnTreeExited(Action action) {
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
        public VisibilityEnabler2DAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnTreeExiting(Action action) {
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
        

        private List<Action<Viewport>>? _onViewportEnteredAction; 
        public VisibilityEnabler2DAction OnViewportEntered(Action<Viewport> action) {
            if (_onViewportEnteredAction == null || _onViewportEnteredAction.Count == 0) {
                _onViewportEnteredAction ??= new List<Action<Viewport>>(); 
                Connect("viewport_entered", this, nameof(ExecuteViewportEntered));
            }
            _onViewportEnteredAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnViewportEntered(Action<Viewport> action) {
            if (_onViewportEnteredAction == null || _onViewportEnteredAction.Count == 0) return this;
            _onViewportEnteredAction.Remove(action); 
            if (_onViewportEnteredAction.Count == 0) {
                Disconnect("viewport_entered", this, nameof(ExecuteViewportEntered));
            }
            return this;
        }
        private void ExecuteViewportEntered(Viewport viewport) {
            if (_onViewportEnteredAction == null || _onViewportEnteredAction.Count == 0) return;
            for (var i = 0; i < _onViewportEnteredAction.Count; i++) _onViewportEnteredAction[i].Invoke(viewport);
        }
        

        private List<Action<Viewport>>? _onViewportExitedAction; 
        public VisibilityEnabler2DAction OnViewportExited(Action<Viewport> action) {
            if (_onViewportExitedAction == null || _onViewportExitedAction.Count == 0) {
                _onViewportExitedAction ??= new List<Action<Viewport>>(); 
                Connect("viewport_exited", this, nameof(ExecuteViewportExited));
            }
            _onViewportExitedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnViewportExited(Action<Viewport> action) {
            if (_onViewportExitedAction == null || _onViewportExitedAction.Count == 0) return this;
            _onViewportExitedAction.Remove(action); 
            if (_onViewportExitedAction.Count == 0) {
                Disconnect("viewport_exited", this, nameof(ExecuteViewportExited));
            }
            return this;
        }
        private void ExecuteViewportExited(Viewport viewport) {
            if (_onViewportExitedAction == null || _onViewportExitedAction.Count == 0) return;
            for (var i = 0; i < _onViewportExitedAction.Count; i++) _onViewportExitedAction[i].Invoke(viewport);
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public VisibilityEnabler2DAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public VisibilityEnabler2DAction RemoveOnVisibilityChanged(Action action) {
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