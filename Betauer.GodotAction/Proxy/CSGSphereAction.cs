using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class CSGSphereAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public CSGSphereAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private CSGSphereAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public CSGSphereAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private CSGSphereAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public CSGSphereAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private CSGSphereAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public CSGSphereAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private CSGSphereAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public CSGSphereAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private CSGSphereAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public CSGSphereAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private CSGSphereAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public CSGSphereAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private CSGSphereAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public CSGSphereAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private CSGSphereAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public CSGSphereAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private CSGSphereAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public CSGSphereAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private CSGSphereAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public CSGSphereAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public CSGSphereAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private CSGSphereAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}