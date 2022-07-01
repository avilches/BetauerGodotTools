using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimatedSpriteAction : ProxyNode {

        private List<Action>? _onAnimationFinishedAction; 
        public void OnAnimationFinished(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action, oneShot, deferred);

        public void RemoveOnAnimationFinished(Action action) =>
            RemoveSignal(_onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action);

        private void _GodotSignalAnimationFinished() =>
            ExecuteSignal(_onAnimationFinishedAction);
        

        private List<Action>? _onDrawAction; 
        public void OnDraw(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);

        public void RemoveOnDraw(Action action) =>
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);

        private void _GodotSignalDraw() =>
            ExecuteSignal(_onDrawAction);
        

        private List<Action>? _onFrameChangedAction; 
        public void OnFrameChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFrameChangedAction, "frame_changed", nameof(_GodotSignalFrameChanged), action, oneShot, deferred);

        public void RemoveOnFrameChanged(Action action) =>
            RemoveSignal(_onFrameChangedAction, "frame_changed", nameof(_GodotSignalFrameChanged), action);

        private void _GodotSignalFrameChanged() =>
            ExecuteSignal(_onFrameChangedAction);
        

        private List<Action>? _onHideAction; 
        public void OnHide(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);

        public void RemoveOnHide(Action action) =>
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);

        private void _GodotSignalHide() =>
            ExecuteSignal(_onHideAction);
        

        private List<Action>? _onItemRectChangedAction; 
        public void OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);

        public void RemoveOnItemRectChanged(Action action) =>
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);

        private void _GodotSignalItemRectChanged() =>
            ExecuteSignal(_onItemRectChangedAction);
        

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
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

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