using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisibilityNotifierAction : ProxyNode {

        private List<Action<Camera>>? _onCameraEnteredAction; 
        public VisibilityNotifierAction OnCameraEntered(Action<Camera> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCameraEnteredAction, "camera_entered", nameof(_GodotSignalCameraEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnCameraEntered(Action<Camera> action) {
            RemoveSignal(_onCameraEnteredAction, "camera_entered", nameof(_GodotSignalCameraEntered), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalCameraEntered(Camera camera) {
            ExecuteSignal(_onCameraEnteredAction, camera);
            return this;
        }

        private List<Action<Camera>>? _onCameraExitedAction; 
        public VisibilityNotifierAction OnCameraExited(Action<Camera> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCameraExitedAction, "camera_exited", nameof(_GodotSignalCameraExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnCameraExited(Action<Camera> action) {
            RemoveSignal(_onCameraExitedAction, "camera_exited", nameof(_GodotSignalCameraExited), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalCameraExited(Camera camera) {
            ExecuteSignal(_onCameraExitedAction, camera);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public VisibilityNotifierAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public VisibilityNotifierAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public VisibilityNotifierAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public VisibilityNotifierAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScreenEnteredAction; 
        public VisibilityNotifierAction OnScreenEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnScreenEntered(Action action) {
            RemoveSignal(_onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalScreenEntered() {
            ExecuteSignal(_onScreenEnteredAction);
            return this;
        }

        private List<Action>? _onScreenExitedAction; 
        public VisibilityNotifierAction OnScreenExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnScreenExited(Action action) {
            RemoveSignal(_onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalScreenExited() {
            ExecuteSignal(_onScreenExitedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisibilityNotifierAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public VisibilityNotifierAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public VisibilityNotifierAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public VisibilityNotifierAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public VisibilityNotifierAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityNotifierAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private VisibilityNotifierAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}