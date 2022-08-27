using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class ARVRControllerAction : ProxyNode {

        private List<Action<int>>? _onButtonPressedAction; 
        public ARVRControllerAction OnButtonPressed(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnButtonPressed(Action<int> action) {
            RemoveSignal(_onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalButtonPressed(int button) {
            ExecuteSignal(_onButtonPressedAction, button);
            return this;
        }

        private List<Action<int>>? _onButtonReleaseAction; 
        public ARVRControllerAction OnButtonRelease(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onButtonReleaseAction, "button_release", nameof(_GodotSignalButtonRelease), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnButtonRelease(Action<int> action) {
            RemoveSignal(_onButtonReleaseAction, "button_release", nameof(_GodotSignalButtonRelease), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalButtonRelease(int button) {
            ExecuteSignal(_onButtonReleaseAction, button);
            return this;
        }

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public ARVRControllerAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public ARVRControllerAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public ARVRControllerAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public ARVRControllerAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action<Mesh>>? _onMeshUpdatedAction; 
        public ARVRControllerAction OnMeshUpdated(Action<Mesh> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMeshUpdatedAction, "mesh_updated", nameof(_GodotSignalMeshUpdated), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnMeshUpdated(Action<Mesh> action) {
            RemoveSignal(_onMeshUpdatedAction, "mesh_updated", nameof(_GodotSignalMeshUpdated), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalMeshUpdated(Mesh mesh) {
            ExecuteSignal(_onMeshUpdatedAction, mesh);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ARVRControllerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ARVRControllerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ARVRControllerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ARVRControllerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ARVRControllerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ARVRControllerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public ARVRControllerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public ARVRControllerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private ARVRControllerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}