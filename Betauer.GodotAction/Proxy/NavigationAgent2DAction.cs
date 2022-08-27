using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class NavigationAgent2DAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public NavigationAgent2DAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public NavigationAgent2DAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onNavigationFinishedAction; 
        public NavigationAgent2DAction OnNavigationFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNavigationFinishedAction, "navigation_finished", nameof(_GodotSignalNavigationFinished), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnNavigationFinished(Action action) {
            RemoveSignal(_onNavigationFinishedAction, "navigation_finished", nameof(_GodotSignalNavigationFinished), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalNavigationFinished() {
            ExecuteSignal(_onNavigationFinishedAction);
            return this;
        }

        private List<Action>? _onPathChangedAction; 
        public NavigationAgent2DAction OnPathChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPathChangedAction, "path_changed", nameof(_GodotSignalPathChanged), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnPathChanged(Action action) {
            RemoveSignal(_onPathChangedAction, "path_changed", nameof(_GodotSignalPathChanged), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalPathChanged() {
            ExecuteSignal(_onPathChangedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public NavigationAgent2DAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public NavigationAgent2DAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public NavigationAgent2DAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTargetReachedAction; 
        public NavigationAgent2DAction OnTargetReached(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTargetReachedAction, "target_reached", nameof(_GodotSignalTargetReached), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnTargetReached(Action action) {
            RemoveSignal(_onTargetReachedAction, "target_reached", nameof(_GodotSignalTargetReached), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalTargetReached() {
            ExecuteSignal(_onTargetReachedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public NavigationAgent2DAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public NavigationAgent2DAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public NavigationAgent2DAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action<Vector2>>? _onVelocityComputedAction; 
        public NavigationAgent2DAction OnVelocityComputed(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVelocityComputedAction, "velocity_computed", nameof(_GodotSignalVelocityComputed), action, oneShot, deferred);
            return this;
        }

        public NavigationAgent2DAction RemoveOnVelocityComputed(Action<Vector2> action) {
            RemoveSignal(_onVelocityComputedAction, "velocity_computed", nameof(_GodotSignalVelocityComputed), action);
            return this;
        }

        private NavigationAgent2DAction _GodotSignalVelocityComputed(Vector2 safe_velocity) {
            ExecuteSignal(_onVelocityComputedAction, safe_velocity);
            return this;
        }
    }
}