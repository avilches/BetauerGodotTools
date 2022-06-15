using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class SceneTreeAction : SceneTree {


        private Action? _onConnectedToServerAction; 
        public SceneTreeAction OnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null) 
                Connect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            _onConnectedToServerAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnConnectedToServer() {
            if (_onConnectedToServerAction == null) return this; 
            Disconnect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            _onConnectedToServerAction = null;
            return this;
        }
        private void ExecuteConnectedToServer() =>
            _onConnectedToServerAction?.Invoke();
        

        private Action? _onConnectionFailedAction; 
        public SceneTreeAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null) 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnConnectionFailed() {
            if (_onConnectionFailedAction == null) return this; 
            Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = null;
            return this;
        }
        private void ExecuteConnectionFailed() =>
            _onConnectionFailedAction?.Invoke();
        

        private Action<string[], int>? _onFilesDroppedAction; 
        public SceneTreeAction OnFilesDropped(Action<string[], int> action) {
            if (_onFilesDroppedAction == null) 
                Connect("files_dropped", this, nameof(ExecuteFilesDropped));
            _onFilesDroppedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnFilesDropped() {
            if (_onFilesDroppedAction == null) return this; 
            Disconnect("files_dropped", this, nameof(ExecuteFilesDropped));
            _onFilesDroppedAction = null;
            return this;
        }
        private void ExecuteFilesDropped(string[] files, int screen) =>
            _onFilesDroppedAction?.Invoke(files, screen);
        

        private Action<object, object>? _onGlobalMenuActionAction; 
        public SceneTreeAction OnGlobalMenuAction(Action<object, object> action) {
            if (_onGlobalMenuActionAction == null) 
                Connect("global_menu_action", this, nameof(ExecuteGlobalMenuAction));
            _onGlobalMenuActionAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnGlobalMenuAction() {
            if (_onGlobalMenuActionAction == null) return this; 
            Disconnect("global_menu_action", this, nameof(ExecuteGlobalMenuAction));
            _onGlobalMenuActionAction = null;
            return this;
        }
        private void ExecuteGlobalMenuAction(object id, object meta) =>
            _onGlobalMenuActionAction?.Invoke(id, meta);
        

        private Action? _onIdleFrameAction; 
        public SceneTreeAction OnIdleFrame(Action action) {
            if (_onIdleFrameAction == null) 
                Connect("idle_frame", this, nameof(ExecuteIdleFrame));
            _onIdleFrameAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnIdleFrame() {
            if (_onIdleFrameAction == null) return this; 
            Disconnect("idle_frame", this, nameof(ExecuteIdleFrame));
            _onIdleFrameAction = null;
            return this;
        }
        private void ExecuteIdleFrame() =>
            _onIdleFrameAction?.Invoke();
        

        private Action<int>? _onNetworkPeerConnectedAction; 
        public SceneTreeAction OnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null) 
                Connect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            _onNetworkPeerConnectedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerConnected() {
            if (_onNetworkPeerConnectedAction == null) return this; 
            Disconnect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            _onNetworkPeerConnectedAction = null;
            return this;
        }
        private void ExecuteNetworkPeerConnected(int id) =>
            _onNetworkPeerConnectedAction?.Invoke(id);
        

        private Action<int>? _onNetworkPeerDisconnectedAction; 
        public SceneTreeAction OnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null) 
                Connect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            _onNetworkPeerDisconnectedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerDisconnected() {
            if (_onNetworkPeerDisconnectedAction == null) return this; 
            Disconnect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            _onNetworkPeerDisconnectedAction = null;
            return this;
        }
        private void ExecuteNetworkPeerDisconnected(int id) =>
            _onNetworkPeerDisconnectedAction?.Invoke(id);
        

        private Action<Node>? _onNodeAddedAction; 
        public SceneTreeAction OnNodeAdded(Action<Node> action) {
            if (_onNodeAddedAction == null) 
                Connect("node_added", this, nameof(ExecuteNodeAdded));
            _onNodeAddedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNodeAdded() {
            if (_onNodeAddedAction == null) return this; 
            Disconnect("node_added", this, nameof(ExecuteNodeAdded));
            _onNodeAddedAction = null;
            return this;
        }
        private void ExecuteNodeAdded(Node node) =>
            _onNodeAddedAction?.Invoke(node);
        

        private Action<Node>? _onNodeConfigurationWarningChangedAction; 
        public SceneTreeAction OnNodeConfigurationWarningChanged(Action<Node> action) {
            if (_onNodeConfigurationWarningChangedAction == null) 
                Connect("node_configuration_warning_changed", this, nameof(ExecuteNodeConfigurationWarningChanged));
            _onNodeConfigurationWarningChangedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNodeConfigurationWarningChanged() {
            if (_onNodeConfigurationWarningChangedAction == null) return this; 
            Disconnect("node_configuration_warning_changed", this, nameof(ExecuteNodeConfigurationWarningChanged));
            _onNodeConfigurationWarningChangedAction = null;
            return this;
        }
        private void ExecuteNodeConfigurationWarningChanged(Node node) =>
            _onNodeConfigurationWarningChangedAction?.Invoke(node);
        

        private Action<Node>? _onNodeRemovedAction; 
        public SceneTreeAction OnNodeRemoved(Action<Node> action) {
            if (_onNodeRemovedAction == null) 
                Connect("node_removed", this, nameof(ExecuteNodeRemoved));
            _onNodeRemovedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNodeRemoved() {
            if (_onNodeRemovedAction == null) return this; 
            Disconnect("node_removed", this, nameof(ExecuteNodeRemoved));
            _onNodeRemovedAction = null;
            return this;
        }
        private void ExecuteNodeRemoved(Node node) =>
            _onNodeRemovedAction?.Invoke(node);
        

        private Action<Node>? _onNodeRenamedAction; 
        public SceneTreeAction OnNodeRenamed(Action<Node> action) {
            if (_onNodeRenamedAction == null) 
                Connect("node_renamed", this, nameof(ExecuteNodeRenamed));
            _onNodeRenamedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnNodeRenamed() {
            if (_onNodeRenamedAction == null) return this; 
            Disconnect("node_renamed", this, nameof(ExecuteNodeRenamed));
            _onNodeRenamedAction = null;
            return this;
        }
        private void ExecuteNodeRenamed(Node node) =>
            _onNodeRenamedAction?.Invoke(node);
        

        private Action? _onPhysicsFrameAction; 
        public SceneTreeAction OnPhysicsFrame(Action action) {
            if (_onPhysicsFrameAction == null) 
                Connect("physics_frame", this, nameof(ExecutePhysicsFrame));
            _onPhysicsFrameAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnPhysicsFrame() {
            if (_onPhysicsFrameAction == null) return this; 
            Disconnect("physics_frame", this, nameof(ExecutePhysicsFrame));
            _onPhysicsFrameAction = null;
            return this;
        }
        private void ExecutePhysicsFrame() =>
            _onPhysicsFrameAction?.Invoke();
        

        private Action<bool, string>? _onRequestPermissionsResultAction; 
        public SceneTreeAction OnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null) 
                Connect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            _onRequestPermissionsResultAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnRequestPermissionsResult() {
            if (_onRequestPermissionsResultAction == null) return this; 
            Disconnect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            _onRequestPermissionsResultAction = null;
            return this;
        }
        private void ExecuteRequestPermissionsResult(bool granted, string permission) =>
            _onRequestPermissionsResultAction?.Invoke(granted, permission);
        

        private Action? _onScreenResizedAction; 
        public SceneTreeAction OnScreenResized(Action action) {
            if (_onScreenResizedAction == null) 
                Connect("screen_resized", this, nameof(ExecuteScreenResized));
            _onScreenResizedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnScreenResized() {
            if (_onScreenResizedAction == null) return this; 
            Disconnect("screen_resized", this, nameof(ExecuteScreenResized));
            _onScreenResizedAction = null;
            return this;
        }
        private void ExecuteScreenResized() =>
            _onScreenResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public SceneTreeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onServerDisconnectedAction; 
        public SceneTreeAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null) 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnServerDisconnected() {
            if (_onServerDisconnectedAction == null) return this; 
            Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = null;
            return this;
        }
        private void ExecuteServerDisconnected() =>
            _onServerDisconnectedAction?.Invoke();
        

        private Action? _onTreeChangedAction; 
        public SceneTreeAction OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null) 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = action;
            return this;
        }
        public SceneTreeAction RemoveOnTreeChanged() {
            if (_onTreeChangedAction == null) return this; 
            Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = null;
            return this;
        }
        private void ExecuteTreeChanged() =>
            _onTreeChangedAction?.Invoke();
        
    }
}