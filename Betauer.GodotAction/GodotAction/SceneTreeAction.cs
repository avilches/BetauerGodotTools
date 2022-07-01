using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class SceneTreeAction : ProxyNode {

        private List<Action>? _onConnectedToServerAction; 
        public void OnConnectedToServer(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action, oneShot, deferred);

        public void RemoveOnConnectedToServer(Action action) =>
            RemoveSignal(_onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action);

        private void _GodotSignalConnectedToServer() =>
            ExecuteSignal(_onConnectedToServerAction);
        

        private List<Action>? _onConnectionFailedAction; 
        public void OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);

        public void RemoveOnConnectionFailed(Action action) =>
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);

        private void _GodotSignalConnectionFailed() =>
            ExecuteSignal(_onConnectionFailedAction);
        

        private List<Action<string[], int>>? _onFilesDroppedAction; 
        public void OnFilesDropped(Action<string[], int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFilesDroppedAction, "files_dropped", nameof(_GodotSignalFilesDropped), action, oneShot, deferred);

        public void RemoveOnFilesDropped(Action<string[], int> action) =>
            RemoveSignal(_onFilesDroppedAction, "files_dropped", nameof(_GodotSignalFilesDropped), action);

        private void _GodotSignalFilesDropped(string[] files, int screen) =>
            ExecuteSignal(_onFilesDroppedAction, files, screen);
        

        private List<Action<object, object>>? _onGlobalMenuActionAction; 
        public void OnGlobalMenuAction(Action<object, object> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGlobalMenuActionAction, "global_menu_action", nameof(_GodotSignalGlobalMenuAction), action, oneShot, deferred);

        public void RemoveOnGlobalMenuAction(Action<object, object> action) =>
            RemoveSignal(_onGlobalMenuActionAction, "global_menu_action", nameof(_GodotSignalGlobalMenuAction), action);

        private void _GodotSignalGlobalMenuAction(object id, object meta) =>
            ExecuteSignal(_onGlobalMenuActionAction, id, meta);
        

        private List<Action>? _onIdleFrameAction; 
        public void OnIdleFrame(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onIdleFrameAction, "idle_frame", nameof(_GodotSignalIdleFrame), action, oneShot, deferred);

        public void RemoveOnIdleFrame(Action action) =>
            RemoveSignal(_onIdleFrameAction, "idle_frame", nameof(_GodotSignalIdleFrame), action);

        private void _GodotSignalIdleFrame() =>
            ExecuteSignal(_onIdleFrameAction);
        

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public void OnNetworkPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action, oneShot, deferred);

        public void RemoveOnNetworkPeerConnected(Action<int> action) =>
            RemoveSignal(_onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action);

        private void _GodotSignalNetworkPeerConnected(int id) =>
            ExecuteSignal(_onNetworkPeerConnectedAction, id);
        

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public void OnNetworkPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action, oneShot, deferred);

        public void RemoveOnNetworkPeerDisconnected(Action<int> action) =>
            RemoveSignal(_onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action);

        private void _GodotSignalNetworkPeerDisconnected(int id) =>
            ExecuteSignal(_onNetworkPeerDisconnectedAction, id);
        

        private List<Action<Node>>? _onNodeAddedAction; 
        public void OnNodeAdded(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeAddedAction, "node_added", nameof(_GodotSignalNodeAdded), action, oneShot, deferred);

        public void RemoveOnNodeAdded(Action<Node> action) =>
            RemoveSignal(_onNodeAddedAction, "node_added", nameof(_GodotSignalNodeAdded), action);

        private void _GodotSignalNodeAdded(Node node) =>
            ExecuteSignal(_onNodeAddedAction, node);
        

        private List<Action<Node>>? _onNodeConfigurationWarningChangedAction; 
        public void OnNodeConfigurationWarningChanged(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeConfigurationWarningChangedAction, "node_configuration_warning_changed", nameof(_GodotSignalNodeConfigurationWarningChanged), action, oneShot, deferred);

        public void RemoveOnNodeConfigurationWarningChanged(Action<Node> action) =>
            RemoveSignal(_onNodeConfigurationWarningChangedAction, "node_configuration_warning_changed", nameof(_GodotSignalNodeConfigurationWarningChanged), action);

        private void _GodotSignalNodeConfigurationWarningChanged(Node node) =>
            ExecuteSignal(_onNodeConfigurationWarningChangedAction, node);
        

        private List<Action<Node>>? _onNodeRemovedAction; 
        public void OnNodeRemoved(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeRemovedAction, "node_removed", nameof(_GodotSignalNodeRemoved), action, oneShot, deferred);

        public void RemoveOnNodeRemoved(Action<Node> action) =>
            RemoveSignal(_onNodeRemovedAction, "node_removed", nameof(_GodotSignalNodeRemoved), action);

        private void _GodotSignalNodeRemoved(Node node) =>
            ExecuteSignal(_onNodeRemovedAction, node);
        

        private List<Action<Node>>? _onNodeRenamedAction; 
        public void OnNodeRenamed(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeRenamedAction, "node_renamed", nameof(_GodotSignalNodeRenamed), action, oneShot, deferred);

        public void RemoveOnNodeRenamed(Action<Node> action) =>
            RemoveSignal(_onNodeRenamedAction, "node_renamed", nameof(_GodotSignalNodeRenamed), action);

        private void _GodotSignalNodeRenamed(Node node) =>
            ExecuteSignal(_onNodeRenamedAction, node);
        

        private List<Action>? _onPhysicsFrameAction; 
        public void OnPhysicsFrame(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPhysicsFrameAction, "physics_frame", nameof(_GodotSignalPhysicsFrame), action, oneShot, deferred);

        public void RemoveOnPhysicsFrame(Action action) =>
            RemoveSignal(_onPhysicsFrameAction, "physics_frame", nameof(_GodotSignalPhysicsFrame), action);

        private void _GodotSignalPhysicsFrame() =>
            ExecuteSignal(_onPhysicsFrameAction);
        

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public void OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action, oneShot, deferred);

        public void RemoveOnRequestPermissionsResult(Action<bool, string> action) =>
            RemoveSignal(_onRequestPermissionsResultAction, "on_request_permissions_result", nameof(_GodotSignalRequestPermissionsResult), action);

        private void _GodotSignalRequestPermissionsResult(bool granted, string permission) =>
            ExecuteSignal(_onRequestPermissionsResultAction, granted, permission);
        

        private List<Action>? _onScreenResizedAction; 
        public void OnScreenResized(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScreenResizedAction, "screen_resized", nameof(_GodotSignalScreenResized), action, oneShot, deferred);

        public void RemoveOnScreenResized(Action action) =>
            RemoveSignal(_onScreenResizedAction, "screen_resized", nameof(_GodotSignalScreenResized), action);

        private void _GodotSignalScreenResized() =>
            ExecuteSignal(_onScreenResizedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onServerDisconnectedAction; 
        public void OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);

        public void RemoveOnServerDisconnected(Action action) =>
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);

        private void _GodotSignalServerDisconnected() =>
            ExecuteSignal(_onServerDisconnectedAction);
        

        private List<Action>? _onTreeChangedAction; 
        public void OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);

        public void RemoveOnTreeChanged(Action action) =>
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);

        private void _GodotSignalTreeChanged() =>
            ExecuteSignal(_onTreeChangedAction);
        
    }
}