using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class InterpolatedCameraAction : ProxyNode {

        private List<Action>? _onGameplayEnteredAction; 
        public InterpolatedCameraAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public InterpolatedCameraAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public InterpolatedCameraAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public InterpolatedCameraAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public InterpolatedCameraAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public InterpolatedCameraAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public InterpolatedCameraAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public InterpolatedCameraAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public InterpolatedCameraAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public InterpolatedCameraAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private InterpolatedCameraAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}