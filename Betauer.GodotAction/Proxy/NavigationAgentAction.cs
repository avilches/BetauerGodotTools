using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class NavigationAgentAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public NavigationAgentAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public NavigationAgentAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onNavigationFinishedAction; 
        public NavigationAgentAction OnNavigationFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNavigationFinishedAction, "navigation_finished", nameof(_GodotSignalNavigationFinished), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnNavigationFinished(Action action) {
            RemoveSignal(_onNavigationFinishedAction, "navigation_finished", nameof(_GodotSignalNavigationFinished), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalNavigationFinished() {
            ExecuteSignal(_onNavigationFinishedAction);
            return this;
        }

        private List<Action>? _onPathChangedAction; 
        public NavigationAgentAction OnPathChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPathChangedAction, "path_changed", nameof(_GodotSignalPathChanged), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnPathChanged(Action action) {
            RemoveSignal(_onPathChangedAction, "path_changed", nameof(_GodotSignalPathChanged), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalPathChanged() {
            ExecuteSignal(_onPathChangedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public NavigationAgentAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public NavigationAgentAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public NavigationAgentAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTargetReachedAction; 
        public NavigationAgentAction OnTargetReached(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTargetReachedAction, "target_reached", nameof(_GodotSignalTargetReached), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnTargetReached(Action action) {
            RemoveSignal(_onTargetReachedAction, "target_reached", nameof(_GodotSignalTargetReached), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalTargetReached() {
            ExecuteSignal(_onTargetReachedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public NavigationAgentAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public NavigationAgentAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public NavigationAgentAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action<Vector3>>? _onVelocityComputedAction; 
        public NavigationAgentAction OnVelocityComputed(Action<Vector3> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVelocityComputedAction, "velocity_computed", nameof(_GodotSignalVelocityComputed), action, oneShot, deferred);
            return this;
        }

        public NavigationAgentAction RemoveOnVelocityComputed(Action<Vector3> action) {
            RemoveSignal(_onVelocityComputedAction, "velocity_computed", nameof(_GodotSignalVelocityComputed), action);
            return this;
        }

        private NavigationAgentAction _GodotSignalVelocityComputed(Vector3 safe_velocity) {
            ExecuteSignal(_onVelocityComputedAction, safe_velocity);
            return this;
        }
    }
}