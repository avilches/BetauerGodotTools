using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationPlayerAction : ProxyNode {

        private List<Action<string, string>>? _onAnimationChangedAction; 
        public AnimationPlayerAction OnAnimationChanged(Action<string, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAnimationChangedAction, "animation_changed", nameof(_GodotSignalAnimationChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnAnimationChanged(Action<string, string> action) {
            RemoveSignal(_onAnimationChangedAction, "animation_changed", nameof(_GodotSignalAnimationChanged), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalAnimationChanged(string new_name, string old_name) {
            ExecuteSignal(_onAnimationChangedAction, new_name, old_name);
            return this;
        }

        private List<Action<string>>? _onAnimationFinishedAction; 
        public AnimationPlayerAction OnAnimationFinished(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnAnimationFinished(Action<string> action) {
            RemoveSignal(_onAnimationFinishedAction, "animation_finished", nameof(_GodotSignalAnimationFinished), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalAnimationFinished(string anim_name) {
            ExecuteSignal(_onAnimationFinishedAction, anim_name);
            return this;
        }

        private List<Action<string>>? _onAnimationStartedAction; 
        public AnimationPlayerAction OnAnimationStarted(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onAnimationStartedAction, "animation_started", nameof(_GodotSignalAnimationStarted), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnAnimationStarted(Action<string> action) {
            RemoveSignal(_onAnimationStartedAction, "animation_started", nameof(_GodotSignalAnimationStarted), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalAnimationStarted(string anim_name) {
            ExecuteSignal(_onAnimationStartedAction, anim_name);
            return this;
        }

        private List<Action>? _onCachesClearedAction; 
        public AnimationPlayerAction OnCachesCleared(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCachesClearedAction, "caches_cleared", nameof(_GodotSignalCachesCleared), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnCachesCleared(Action action) {
            RemoveSignal(_onCachesClearedAction, "caches_cleared", nameof(_GodotSignalCachesCleared), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalCachesCleared() {
            ExecuteSignal(_onCachesClearedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public AnimationPlayerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public AnimationPlayerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationPlayerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public AnimationPlayerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public AnimationPlayerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public AnimationPlayerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public AnimationPlayerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private AnimationPlayerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }
    }
}