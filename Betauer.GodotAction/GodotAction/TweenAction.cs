using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class TweenAction : ProxyNode {

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
        

        private List<Action>? _onTweenAllCompletedAction; 
        public void OnTweenAllCompleted(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTweenAllCompletedAction, "tween_all_completed", nameof(_GodotSignalTweenAllCompleted), action, oneShot, deferred);

        public void RemoveOnTweenAllCompleted(Action action) =>
            RemoveSignal(_onTweenAllCompletedAction, "tween_all_completed", nameof(_GodotSignalTweenAllCompleted), action);

        private void _GodotSignalTweenAllCompleted() =>
            ExecuteSignal(_onTweenAllCompletedAction);
        

        private List<Action<Object, NodePath>>? _onTweenCompletedAction; 
        public void OnTweenCompleted(Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTweenCompletedAction, "tween_completed", nameof(_GodotSignalTweenCompleted), action, oneShot, deferred);

        public void RemoveOnTweenCompleted(Action<Object, NodePath> action) =>
            RemoveSignal(_onTweenCompletedAction, "tween_completed", nameof(_GodotSignalTweenCompleted), action);

        private void _GodotSignalTweenCompleted(Object @object, NodePath key) =>
            ExecuteSignal(_onTweenCompletedAction, @object, key);
        

        private List<Action<Object, NodePath>>? _onTweenStartedAction; 
        public void OnTweenStarted(Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTweenStartedAction, "tween_started", nameof(_GodotSignalTweenStarted), action, oneShot, deferred);

        public void RemoveOnTweenStarted(Action<Object, NodePath> action) =>
            RemoveSignal(_onTweenStartedAction, "tween_started", nameof(_GodotSignalTweenStarted), action);

        private void _GodotSignalTweenStarted(Object @object, NodePath key) =>
            ExecuteSignal(_onTweenStartedAction, @object, key);
        

        private List<Action<Object, float, NodePath, Object>>? _onTweenStepAction; 
        public void OnTweenStep(Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTweenStepAction, "tween_step", nameof(_GodotSignalTweenStep), action, oneShot, deferred);

        public void RemoveOnTweenStep(Action<Object, float, NodePath, Object> action) =>
            RemoveSignal(_onTweenStepAction, "tween_step", nameof(_GodotSignalTweenStep), action);

        private void _GodotSignalTweenStep(Object @object, float elapsed, NodePath key, Object value) =>
            ExecuteSignal(_onTweenStepAction, @object, elapsed, key, value);
        
    }
}