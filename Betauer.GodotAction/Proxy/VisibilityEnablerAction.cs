using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VisibilityEnablerAction : ProxyNode {

        private List<Action<Camera>>? _onCameraEnteredAction; 
        public VisibilityEnablerAction OnCameraEntered(Action<Camera> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCameraEnteredAction, "camera_entered", nameof(_GodotSignalCameraEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnCameraEntered(Action<Camera> action) {
            RemoveSignal(_onCameraEnteredAction, "camera_entered", nameof(_GodotSignalCameraEntered), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalCameraEntered(Camera camera) {
            ExecuteSignal(_onCameraEnteredAction, camera);
            return this;
        }

        private List<Action<Camera>>? _onCameraExitedAction; 
        public VisibilityEnablerAction OnCameraExited(Action<Camera> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCameraExitedAction, "camera_exited", nameof(_GodotSignalCameraExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnCameraExited(Action<Camera> action) {
            RemoveSignal(_onCameraExitedAction, "camera_exited", nameof(_GodotSignalCameraExited), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalCameraExited(Camera camera) {
            ExecuteSignal(_onCameraExitedAction, camera);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public VisibilityEnablerAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public VisibilityEnablerAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public VisibilityEnablerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public VisibilityEnablerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScreenEnteredAction; 
        public VisibilityEnablerAction OnScreenEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnScreenEntered(Action action) {
            RemoveSignal(_onScreenEnteredAction, "screen_entered", nameof(_GodotSignalScreenEntered), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalScreenEntered() {
            ExecuteSignal(_onScreenEnteredAction);
            return this;
        }

        private List<Action>? _onScreenExitedAction; 
        public VisibilityEnablerAction OnScreenExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnScreenExited(Action action) {
            RemoveSignal(_onScreenExitedAction, "screen_exited", nameof(_GodotSignalScreenExited), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalScreenExited() {
            ExecuteSignal(_onScreenExitedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisibilityEnablerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public VisibilityEnablerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public VisibilityEnablerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public VisibilityEnablerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public VisibilityEnablerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public VisibilityEnablerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private VisibilityEnablerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}