using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class ListenerAction : ProxyNode {

        private List<Action>? _onGameplayEnteredAction; 
        public ListenerAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private ListenerAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public ListenerAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private ListenerAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ListenerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ListenerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ListenerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ListenerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ListenerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ListenerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ListenerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ListenerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ListenerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ListenerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ListenerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ListenerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public ListenerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public ListenerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private ListenerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}