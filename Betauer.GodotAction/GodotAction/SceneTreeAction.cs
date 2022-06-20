using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class SceneTreeAction : SceneTree {


        private List<Action>? _onConnectedToServerAction; 
        public SceneTreeAction OnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) {
                _onConnectedToServerAction ??= new List<Action>(); 
                Connect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            }
            _onConnectedToServerAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return this;
            _onConnectedToServerAction.Remove(action); 
            if (_onConnectedToServerAction.Count == 0) {
                Disconnect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            }
            return this;
        }
        private void ExecuteConnectedToServer() {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return;
            for (var i = 0; i < _onConnectedToServerAction.Count; i++) _onConnectedToServerAction[i].Invoke();
        }
        

        private List<Action>? _onConnectionFailedAction; 
        public SceneTreeAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return this;
            _onConnectionFailedAction.Remove(action); 
            if (_onConnectionFailedAction.Count == 0) {
                Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            }
            return this;
        }
        private void ExecuteConnectionFailed() {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFailedAction.Count; i++) _onConnectionFailedAction[i].Invoke();
        }
        

        private List<Action<string[], int>>? _onFilesDroppedAction; 
        public SceneTreeAction OnFilesDropped(Action<string[], int> action) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) {
                _onFilesDroppedAction ??= new List<Action<string[], int>>(); 
                Connect("files_dropped", this, nameof(ExecuteFilesDropped));
            }
            _onFilesDroppedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnFilesDropped(Action<string[], int> action) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) return this;
            _onFilesDroppedAction.Remove(action); 
            if (_onFilesDroppedAction.Count == 0) {
                Disconnect("files_dropped", this, nameof(ExecuteFilesDropped));
            }
            return this;
        }
        private void ExecuteFilesDropped(string[] files, int screen) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) return;
            for (var i = 0; i < _onFilesDroppedAction.Count; i++) _onFilesDroppedAction[i].Invoke(files, screen);
        }
        

        private List<Action<object, object>>? _onGlobalMenuActionAction; 
        public SceneTreeAction OnGlobalMenuAction(Action<object, object> action) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) {
                _onGlobalMenuActionAction ??= new List<Action<object, object>>(); 
                Connect("global_menu_action", this, nameof(ExecuteGlobalMenuAction));
            }
            _onGlobalMenuActionAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnGlobalMenuAction(Action<object, object> action) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) return this;
            _onGlobalMenuActionAction.Remove(action); 
            if (_onGlobalMenuActionAction.Count == 0) {
                Disconnect("global_menu_action", this, nameof(ExecuteGlobalMenuAction));
            }
            return this;
        }
        private void ExecuteGlobalMenuAction(object id, object meta) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) return;
            for (var i = 0; i < _onGlobalMenuActionAction.Count; i++) _onGlobalMenuActionAction[i].Invoke(id, meta);
        }
        

        private List<Action>? _onIdleFrameAction; 
        public SceneTreeAction OnIdleFrame(Action action) {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) {
                _onIdleFrameAction ??= new List<Action>(); 
                Connect("idle_frame", this, nameof(ExecuteIdleFrame));
            }
            _onIdleFrameAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnIdleFrame(Action action) {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) return this;
            _onIdleFrameAction.Remove(action); 
            if (_onIdleFrameAction.Count == 0) {
                Disconnect("idle_frame", this, nameof(ExecuteIdleFrame));
            }
            return this;
        }
        private void ExecuteIdleFrame() {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) return;
            for (var i = 0; i < _onIdleFrameAction.Count; i++) _onIdleFrameAction[i].Invoke();
        }
        

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public SceneTreeAction OnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) {
                _onNetworkPeerConnectedAction ??= new List<Action<int>>(); 
                Connect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            }
            _onNetworkPeerConnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return this;
            _onNetworkPeerConnectedAction.Remove(action); 
            if (_onNetworkPeerConnectedAction.Count == 0) {
                Disconnect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            }
            return this;
        }
        private void ExecuteNetworkPeerConnected(int id) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerConnectedAction.Count; i++) _onNetworkPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public SceneTreeAction OnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) {
                _onNetworkPeerDisconnectedAction ??= new List<Action<int>>(); 
                Connect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            }
            _onNetworkPeerDisconnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return this;
            _onNetworkPeerDisconnectedAction.Remove(action); 
            if (_onNetworkPeerDisconnectedAction.Count == 0) {
                Disconnect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            }
            return this;
        }
        private void ExecuteNetworkPeerDisconnected(int id) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerDisconnectedAction.Count; i++) _onNetworkPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action<Node>>? _onNodeAddedAction; 
        public SceneTreeAction OnNodeAdded(Action<Node> action) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) {
                _onNodeAddedAction ??= new List<Action<Node>>(); 
                Connect("node_added", this, nameof(ExecuteNodeAdded));
            }
            _onNodeAddedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeAdded(Action<Node> action) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) return this;
            _onNodeAddedAction.Remove(action); 
            if (_onNodeAddedAction.Count == 0) {
                Disconnect("node_added", this, nameof(ExecuteNodeAdded));
            }
            return this;
        }
        private void ExecuteNodeAdded(Node node) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) return;
            for (var i = 0; i < _onNodeAddedAction.Count; i++) _onNodeAddedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeConfigurationWarningChangedAction; 
        public SceneTreeAction OnNodeConfigurationWarningChanged(Action<Node> action) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) {
                _onNodeConfigurationWarningChangedAction ??= new List<Action<Node>>(); 
                Connect("node_configuration_warning_changed", this, nameof(ExecuteNodeConfigurationWarningChanged));
            }
            _onNodeConfigurationWarningChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeConfigurationWarningChanged(Action<Node> action) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) return this;
            _onNodeConfigurationWarningChangedAction.Remove(action); 
            if (_onNodeConfigurationWarningChangedAction.Count == 0) {
                Disconnect("node_configuration_warning_changed", this, nameof(ExecuteNodeConfigurationWarningChanged));
            }
            return this;
        }
        private void ExecuteNodeConfigurationWarningChanged(Node node) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) return;
            for (var i = 0; i < _onNodeConfigurationWarningChangedAction.Count; i++) _onNodeConfigurationWarningChangedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeRemovedAction; 
        public SceneTreeAction OnNodeRemoved(Action<Node> action) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) {
                _onNodeRemovedAction ??= new List<Action<Node>>(); 
                Connect("node_removed", this, nameof(ExecuteNodeRemoved));
            }
            _onNodeRemovedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeRemoved(Action<Node> action) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) return this;
            _onNodeRemovedAction.Remove(action); 
            if (_onNodeRemovedAction.Count == 0) {
                Disconnect("node_removed", this, nameof(ExecuteNodeRemoved));
            }
            return this;
        }
        private void ExecuteNodeRemoved(Node node) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) return;
            for (var i = 0; i < _onNodeRemovedAction.Count; i++) _onNodeRemovedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeRenamedAction; 
        public SceneTreeAction OnNodeRenamed(Action<Node> action) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) {
                _onNodeRenamedAction ??= new List<Action<Node>>(); 
                Connect("node_renamed", this, nameof(ExecuteNodeRenamed));
            }
            _onNodeRenamedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeRenamed(Action<Node> action) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) return this;
            _onNodeRenamedAction.Remove(action); 
            if (_onNodeRenamedAction.Count == 0) {
                Disconnect("node_renamed", this, nameof(ExecuteNodeRenamed));
            }
            return this;
        }
        private void ExecuteNodeRenamed(Node node) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) return;
            for (var i = 0; i < _onNodeRenamedAction.Count; i++) _onNodeRenamedAction[i].Invoke(node);
        }
        

        private List<Action>? _onPhysicsFrameAction; 
        public SceneTreeAction OnPhysicsFrame(Action action) {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) {
                _onPhysicsFrameAction ??= new List<Action>(); 
                Connect("physics_frame", this, nameof(ExecutePhysicsFrame));
            }
            _onPhysicsFrameAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnPhysicsFrame(Action action) {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) return this;
            _onPhysicsFrameAction.Remove(action); 
            if (_onPhysicsFrameAction.Count == 0) {
                Disconnect("physics_frame", this, nameof(ExecutePhysicsFrame));
            }
            return this;
        }
        private void ExecutePhysicsFrame() {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) return;
            for (var i = 0; i < _onPhysicsFrameAction.Count; i++) _onPhysicsFrameAction[i].Invoke();
        }
        

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public SceneTreeAction OnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) {
                _onRequestPermissionsResultAction ??= new List<Action<bool, string>>(); 
                Connect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            }
            _onRequestPermissionsResultAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return this;
            _onRequestPermissionsResultAction.Remove(action); 
            if (_onRequestPermissionsResultAction.Count == 0) {
                Disconnect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            }
            return this;
        }
        private void ExecuteRequestPermissionsResult(bool granted, string permission) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return;
            for (var i = 0; i < _onRequestPermissionsResultAction.Count; i++) _onRequestPermissionsResultAction[i].Invoke(granted, permission);
        }
        

        private List<Action>? _onScreenResizedAction; 
        public SceneTreeAction OnScreenResized(Action action) {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) {
                _onScreenResizedAction ??= new List<Action>(); 
                Connect("screen_resized", this, nameof(ExecuteScreenResized));
            }
            _onScreenResizedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnScreenResized(Action action) {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) return this;
            _onScreenResizedAction.Remove(action); 
            if (_onScreenResizedAction.Count == 0) {
                Disconnect("screen_resized", this, nameof(ExecuteScreenResized));
            }
            return this;
        }
        private void ExecuteScreenResized() {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) return;
            for (var i = 0; i < _onScreenResizedAction.Count; i++) _onScreenResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public SceneTreeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public SceneTreeAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return this;
            _onServerDisconnectedAction.Remove(action); 
            if (_onServerDisconnectedAction.Count == 0) {
                Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            }
            return this;
        }
        private void ExecuteServerDisconnected() {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onServerDisconnectedAction.Count; i++) _onServerDisconnectedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeChangedAction; 
        public SceneTreeAction OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) {
                _onTreeChangedAction ??= new List<Action>(); 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            }
            _onTreeChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return this;
            _onTreeChangedAction.Remove(action); 
            if (_onTreeChangedAction.Count == 0) {
                Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            }
            return this;
        }
        private void ExecuteTreeChanged() {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return;
            for (var i = 0; i < _onTreeChangedAction.Count; i++) _onTreeChangedAction[i].Invoke();
        }
        
    }
}