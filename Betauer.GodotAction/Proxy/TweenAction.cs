using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class TweenAction : ProxyNode {

        private List<Action<Node>>? _onChildEnteredTreeAction; 
        public TweenAction OnChildEnteredTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnChildEnteredTree(Action<Node> action) {
            RemoveSignal(_onChildEnteredTreeAction, "child_entered_tree", nameof(_GodotSignalChildEnteredTree), action);
            return this;
        }

        private TweenAction _GodotSignalChildEnteredTree(Node node) {
            ExecuteSignal(_onChildEnteredTreeAction, node);
            return this;
        }

        private List<Action<Node>>? _onChildExitingTreeAction; 
        public TweenAction OnChildExitingTree(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnChildExitingTree(Action<Node> action) {
            RemoveSignal(_onChildExitingTreeAction, "child_exiting_tree", nameof(_GodotSignalChildExitingTree), action);
            return this;
        }

        private TweenAction _GodotSignalChildExitingTree(Node node) {
            ExecuteSignal(_onChildExitingTreeAction, node);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public TweenAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private TweenAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public TweenAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private TweenAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public TweenAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private TweenAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public TweenAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private TweenAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public TweenAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private TweenAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public TweenAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private TweenAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onTweenAllCompletedAction; 
        public TweenAction OnTweenAllCompleted(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTweenAllCompletedAction, "tween_all_completed", nameof(_GodotSignalTweenAllCompleted), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTweenAllCompleted(Action action) {
            RemoveSignal(_onTweenAllCompletedAction, "tween_all_completed", nameof(_GodotSignalTweenAllCompleted), action);
            return this;
        }

        private TweenAction _GodotSignalTweenAllCompleted() {
            ExecuteSignal(_onTweenAllCompletedAction);
            return this;
        }

        private List<Action<Object, NodePath>>? _onTweenCompletedAction; 
        public TweenAction OnTweenCompleted(Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTweenCompletedAction, "tween_completed", nameof(_GodotSignalTweenCompleted), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTweenCompleted(Action<Object, NodePath> action) {
            RemoveSignal(_onTweenCompletedAction, "tween_completed", nameof(_GodotSignalTweenCompleted), action);
            return this;
        }

        private TweenAction _GodotSignalTweenCompleted(Object @object, NodePath key) {
            ExecuteSignal(_onTweenCompletedAction, @object, key);
            return this;
        }

        private List<Action<Object, NodePath>>? _onTweenStartedAction; 
        public TweenAction OnTweenStarted(Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTweenStartedAction, "tween_started", nameof(_GodotSignalTweenStarted), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTweenStarted(Action<Object, NodePath> action) {
            RemoveSignal(_onTweenStartedAction, "tween_started", nameof(_GodotSignalTweenStarted), action);
            return this;
        }

        private TweenAction _GodotSignalTweenStarted(Object @object, NodePath key) {
            ExecuteSignal(_onTweenStartedAction, @object, key);
            return this;
        }

        private List<Action<Object, float, NodePath, Object>>? _onTweenStepAction; 
        public TweenAction OnTweenStep(Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTweenStepAction, "tween_step", nameof(_GodotSignalTweenStep), action, oneShot, deferred);
            return this;
        }

        public TweenAction RemoveOnTweenStep(Action<Object, float, NodePath, Object> action) {
            RemoveSignal(_onTweenStepAction, "tween_step", nameof(_GodotSignalTweenStep), action);
            return this;
        }

        private TweenAction _GodotSignalTweenStep(Object @object, float elapsed, NodePath key, Object value) {
            ExecuteSignal(_onTweenStepAction, @object, elapsed, key, value);
            return this;
        }
    }
}