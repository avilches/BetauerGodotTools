using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class SceneTreeAction : ProxyNode {

        private List<Action>? _onConnectedToServerAction; 
        public SceneTreeAction OnConnectedToServer(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnConnectedToServer(Action action) {
            RemoveSignal(_onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action);
            return this;
        }

        private SceneTreeAction _GodotSignalConnectedToServer() {
            ExecuteSignal(_onConnectedToServerAction);
            return this;
        }

        private List<Action>? _onConnectionFailedAction; 
        public SceneTreeAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private SceneTreeAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action<string[], int>>? _onFilesDroppedAction; 
        public SceneTreeAction OnFilesDropped(Action<string[], int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFilesDroppedAction, "files_dropped", nameof(_GodotSignalFilesDropped), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnFilesDropped(Action<string[], int> action) {
            RemoveSignal(_onFilesDroppedAction, "files_dropped", nameof(_GodotSignalFilesDropped), action);
            return this;
        }

        private SceneTreeAction _GodotSignalFilesDropped(string[] files, int screen) {
            ExecuteSignal(_onFilesDroppedAction, files, screen);
            return this;
        }

        private List<Action<object, object>>? _onGlobalMenuActionAction; 
        public SceneTreeAction OnGlobalMenuAction(Action<object, object> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGlobalMenuActionAction, "global_menu_action", nameof(_GodotSignalGlobalMenuAction), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnGlobalMenuAction(Action<object, object> action) {
            RemoveSignal(_onGlobalMenuActionAction, "global_menu_action", nameof(_GodotSignalGlobalMenuAction), action);
            return this;
        }

        private SceneTreeAction _GodotSignalGlobalMenuAction(object id, object meta) {
            ExecuteSignal(_onGlobalMenuActionAction, id, meta);
            return this;
        }

        private List<Action>? _onIdleFrameAction; 
        public SceneTreeAction OnIdleFrame(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onIdleFrameAction, "idle_frame", nameof(_GodotSignalIdleFrame), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnIdleFrame(Action action) {
            RemoveSignal(_onIdleFrameAction, "idle_frame", nameof(_GodotSignalIdleFrame), action);
            return this;
        }

        private SceneTreeAction _GodotSignalIdleFrame() {
            ExecuteSignal(_onIdleFrameAction);
            return this;
        }

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public SceneTreeAction OnNetworkPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNetworkPeerConnected(Action<int> action) {
            RemoveSignal(_onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNetworkPeerConnected(int id) {
            ExecuteSignal(_onNetworkPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public SceneTreeAction OnNetworkPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNetworkPeerDisconnected(Action<int> action) {
            RemoveSignal(_onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNetworkPeerDisconnected(int id) {
            ExecuteSignal(_onNetworkPeerDisconnectedAction, id);
            return this;
        }

        private List<Action<Node>>? _onNodeAddedAction; 
        public SceneTreeAction OnNodeAdded(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeAddedAction, "node_added", nameof(_GodotSignalNodeAdded), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNodeAdded(Action<Node> action) {
            RemoveSignal(_onNodeAddedAction, "node_added", nameof(_GodotSignalNodeAdded), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNodeAdded(Node node) {
            ExecuteSignal(_onNodeAddedAction, node);
            return this;
        }

        private List<Action<Node>>? _onNodeConfigurationWarningChangedAction; 
        public SceneTreeAction OnNodeConfigurationWarningChanged(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeConfigurationWarningChangedAction, "node_configuration_warning_changed", nameof(_GodotSignalNodeConfigurationWarningChanged), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNodeConfigurationWarningChanged(Action<Node> action) {
            RemoveSignal(_onNodeConfigurationWarningChangedAction, "node_configuration_warning_changed", nameof(_GodotSignalNodeConfigurationWarningChanged), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNodeConfigurationWarningChanged(Node node) {
            ExecuteSignal(_onNodeConfigurationWarningChangedAction, node);
            return this;
        }

        private List<Action<Node>>? _onNodeRemovedAction; 
        public SceneTreeAction OnNodeRemoved(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeRemovedAction, "node_removed", nameof(_GodotSignalNodeRemoved), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNodeRemoved(Action<Node> action) {
            RemoveSignal(_onNodeRemovedAction, "node_removed", nameof(_GodotSignalNodeRemoved), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNodeRemoved(Node node) {
            ExecuteSignal(_onNodeRemovedAction, node);
            return this;
        }

        private List<Action<Node>>? _onNodeRenamedAction; 
        public SceneTreeAction OnNodeRenamed(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeRenamedAction, "node_renamed", nameof(_GodotSignalNodeRenamed), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnNodeRenamed(Action<Node> action) {
            RemoveSignal(_onNodeRenamedAction, "node_renamed", nameof(_GodotSignalNodeRenamed), action);
            return this;
        }

        private SceneTreeAction _GodotSignalNodeRenamed(Node node) {
            ExecuteSignal(_onNodeRenamedAction, node);
            return this;
        }

        private List<Action>? _onPhysicsFrameAction; 
        public SceneTreeAction OnPhysicsFrame(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPhysicsFrameAction, "physics_frame", nameof(_GodotSignalPhysicsFrame), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnPhysicsFrame(Action action) {
            RemoveSignal(_onPhysicsFrameAction, "physics_frame", nameof(_GodotSignalPhysicsFrame), action);
            return this;
        }

        private SceneTreeAction _GodotSignalPhysicsFrame() {
            ExecuteSignal(_onPhysicsFrameAction);
            return this;
        }

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public SceneTreeAction OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            RemoveSignal(_onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action);
            return this;
        }

        private SceneTreeAction _GodotSignalRequestPermissionsResult(bool granted, string permission) {
            ExecuteSignal(_onRequestPermissionsResultAction, granted, permission);
            return this;
        }

        private List<Action>? _onScreenResizedAction; 
        public SceneTreeAction OnScreenResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScreenResizedAction, "screen_resized", nameof(_GodotSignalScreenResized), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnScreenResized(Action action) {
            RemoveSignal(_onScreenResizedAction, "screen_resized", nameof(_GodotSignalScreenResized), action);
            return this;
        }

        private SceneTreeAction _GodotSignalScreenResized() {
            ExecuteSignal(_onScreenResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public SceneTreeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private SceneTreeAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public SceneTreeAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private SceneTreeAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }

        private List<Action>? _onTreeChangedAction; 
        public SceneTreeAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);
            return this;
        }

        public SceneTreeAction RemoveOnTreeChanged(Action action) {
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);
            return this;
        }

        private SceneTreeAction _GodotSignalTreeChanged() {
            ExecuteSignal(_onTreeChangedAction);
            return this;
        }
    }
}