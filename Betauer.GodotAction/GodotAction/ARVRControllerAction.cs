using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class ARVRControllerAction : ProxyNode {

        private List<Action<int>>? _onButtonPressedAction; 
        public void OnButtonPressed(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action, oneShot, deferred);

        public void RemoveOnButtonPressed(Action<int> action) =>
            RemoveSignal(_onButtonPressedAction, "button_pressed", nameof(_GodotSignalButtonPressed), action);

        private void _GodotSignalButtonPressed(int button) =>
            ExecuteSignal(_onButtonPressedAction, button);
        

        private List<Action<int>>? _onButtonReleaseAction; 
        public void OnButtonRelease(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onButtonReleaseAction, "button_release", nameof(_GodotSignalButtonRelease), action, oneShot, deferred);

        public void RemoveOnButtonRelease(Action<int> action) =>
            RemoveSignal(_onButtonReleaseAction, "button_release", nameof(_GodotSignalButtonRelease), action);

        private void _GodotSignalButtonRelease(int button) =>
            ExecuteSignal(_onButtonReleaseAction, button);
        

        private List<Action>? _onGameplayEnteredAction; 
        public void OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);

        public void RemoveOnGameplayEntered(Action action) =>
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);

        private void _GodotSignalGameplayEntered() =>
            ExecuteSignal(_onGameplayEnteredAction);
        

        private List<Action>? _onGameplayExitedAction; 
        public void OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);

        public void RemoveOnGameplayExited(Action action) =>
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);

        private void _GodotSignalGameplayExited() =>
            ExecuteSignal(_onGameplayExitedAction);
        

        private List<Action<Mesh>>? _onMeshUpdatedAction; 
        public void OnMeshUpdated(Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMeshUpdatedAction, "mesh_updated", nameof(_GodotSignalMeshUpdated), action, oneShot, deferred);

        public void RemoveOnMeshUpdated(Action<Mesh> action) =>
            RemoveSignal(_onMeshUpdatedAction, "mesh_updated", nameof(_GodotSignalMeshUpdated), action);

        private void _GodotSignalMeshUpdated(Mesh mesh) =>
            ExecuteSignal(_onMeshUpdatedAction, mesh);
        

        private List<Action>? _onReadyAction; 
        public void OnReady(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);

        public void RemoveOnReady(Action action) =>
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);

        private void _GodotSignalReady() =>
            ExecuteSignal(_onReadyAction);
        

        private List<Action>? _onRenamedAction; 
        public void OnRenamed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);

        public void RemoveOnRenamed(Action action) =>
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);

        private void _GodotSignalRenamed() =>
            ExecuteSignal(_onRenamedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onTreeEnteredAction; 
        public void OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);

        public void RemoveOnTreeEntered(Action action) =>
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);

        private void _GodotSignalTreeEntered() =>
            ExecuteSignal(_onTreeEnteredAction);
        

        private List<Action>? _onTreeExitedAction; 
        public void OnTreeExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);

        public void RemoveOnTreeExited(Action action) =>
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);

        private void _GodotSignalTreeExited() =>
            ExecuteSignal(_onTreeExitedAction);
        

        private List<Action>? _onTreeExitingAction; 
        public void OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);

        public void RemoveOnTreeExiting(Action action) =>
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);

        private void _GodotSignalTreeExiting() =>
            ExecuteSignal(_onTreeExitingAction);
        

        private List<Action>? _onVisibilityChangedAction; 
        public void OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);

        public void RemoveOnVisibilityChanged(Action action) =>
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);

        private void _GodotSignalVisibilityChanged() =>
            ExecuteSignal(_onVisibilityChangedAction);
        
    }
}