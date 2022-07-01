using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationPlayerAction : ProxyNode {

        private List<Action<string, string>>? _onAnimationChangedAction; 
        public void OnAnimationChanged(Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAnimationChangedAction, "animation_changed", nameof(_GodotSignalAnimationChanged), action, oneShot, deferred);

        public void RemoveOnAnimationChanged(Action<string, string> action) =>
            RemoveSignal(_onAnimationChangedAction, "animation_changed", nameof(_GodotSignalAnimationChanged), action);

        private void _GodotSignalAnimationChanged(string new_name, string old_name) =>
            ExecuteSignal(_onAnimationChangedAction, new_name, old_name);
        

        private List<Action<string>>? _onAnimationFinishedAction; 
        public void OnAnimationFinished(Action<string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action, oneShot, deferred);

        public void RemoveOnAnimationFinished(Action<string> action) =>
            RemoveSignal(_onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action);

        private void _GodotSignalAnimationFinished(string anim_name) =>
            ExecuteSignal(_onAnimationFinishedAction, anim_name);
        

        private List<Action<string>>? _onAnimationStartedAction; 
        public void OnAnimationStarted(Action<string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAnimationStartedAction, "animation_started", nameof(_GodotSignalAnimationStarted), action, oneShot, deferred);

        public void RemoveOnAnimationStarted(Action<string> action) =>
            RemoveSignal(_onAnimationStartedAction, "animation_started", nameof(_GodotSignalAnimationStarted), action);

        private void _GodotSignalAnimationStarted(string anim_name) =>
            ExecuteSignal(_onAnimationStartedAction, anim_name);
        

        private List<Action>? _onCachesClearedAction; 
        public void OnCachesCleared(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onCachesClearedAction, "caches_cleared", nameof(_GodotSignalCachesCleared), action, oneShot, deferred);

        public void RemoveOnCachesCleared(Action action) =>
            RemoveSignal(_onCachesClearedAction, "caches_cleared", nameof(_GodotSignalCachesCleared), action);

        private void _GodotSignalCachesCleared() =>
            ExecuteSignal(_onCachesClearedAction);
        

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
        
    }
}