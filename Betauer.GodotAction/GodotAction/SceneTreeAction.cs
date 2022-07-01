using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class SceneTreeAction : Node {
        public SceneTreeAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onConnectedToServerAction; 
        public SceneTreeAction OnConnectedToServer(Action action, bool oneShot = false, bool deferred = false) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) {
                _onConnectedToServerAction ??= new List<Action>(); 
                GetParent().Connect("connected_to_server", this, nameof(_GodotSignalConnectedToServer));
            }
            _onConnectedToServerAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return this;
            _onConnectedToServerAction.Remove(action); 
            if (_onConnectedToServerAction.Count == 0) {
                GetParent().Disconnect("connected_to_server", this, nameof(_GodotSignalConnectedToServer));
            }
            return this;
        }
        private void _GodotSignalConnectedToServer() {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return;
            for (var i = 0; i < _onConnectedToServerAction.Count; i++) _onConnectedToServerAction[i].Invoke();
        }
        

        private List<Action>? _onConnectionFailedAction; 
        public SceneTreeAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                GetParent().Connect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return this;
            _onConnectionFailedAction.Remove(action); 
            if (_onConnectionFailedAction.Count == 0) {
                GetParent().Disconnect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            return this;
        }
        private void _GodotSignalConnectionFailed() {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFailedAction.Count; i++) _onConnectionFailedAction[i].Invoke();
        }
        

        private List<Action<string[], int>>? _onFilesDroppedAction; 
        public SceneTreeAction OnFilesDropped(Action<string[], int> action, bool oneShot = false, bool deferred = false) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) {
                _onFilesDroppedAction ??= new List<Action<string[], int>>(); 
                GetParent().Connect("files_dropped", this, nameof(_GodotSignalFilesDropped));
            }
            _onFilesDroppedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnFilesDropped(Action<string[], int> action) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) return this;
            _onFilesDroppedAction.Remove(action); 
            if (_onFilesDroppedAction.Count == 0) {
                GetParent().Disconnect("files_dropped", this, nameof(_GodotSignalFilesDropped));
            }
            return this;
        }
        private void _GodotSignalFilesDropped(string[] files, int screen) {
            if (_onFilesDroppedAction == null || _onFilesDroppedAction.Count == 0) return;
            for (var i = 0; i < _onFilesDroppedAction.Count; i++) _onFilesDroppedAction[i].Invoke(files, screen);
        }
        

        private List<Action<object, object>>? _onGlobalMenuActionAction; 
        public SceneTreeAction OnGlobalMenuAction(Action<object, object> action, bool oneShot = false, bool deferred = false) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) {
                _onGlobalMenuActionAction ??= new List<Action<object, object>>(); 
                GetParent().Connect("global_menu_action", this, nameof(_GodotSignalGlobalMenuAction));
            }
            _onGlobalMenuActionAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnGlobalMenuAction(Action<object, object> action) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) return this;
            _onGlobalMenuActionAction.Remove(action); 
            if (_onGlobalMenuActionAction.Count == 0) {
                GetParent().Disconnect("global_menu_action", this, nameof(_GodotSignalGlobalMenuAction));
            }
            return this;
        }
        private void _GodotSignalGlobalMenuAction(object id, object meta) {
            if (_onGlobalMenuActionAction == null || _onGlobalMenuActionAction.Count == 0) return;
            for (var i = 0; i < _onGlobalMenuActionAction.Count; i++) _onGlobalMenuActionAction[i].Invoke(id, meta);
        }
        

        private List<Action>? _onIdleFrameAction; 
        public SceneTreeAction OnIdleFrame(Action action, bool oneShot = false, bool deferred = false) {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) {
                _onIdleFrameAction ??= new List<Action>(); 
                GetParent().Connect("idle_frame", this, nameof(_GodotSignalIdleFrame));
            }
            _onIdleFrameAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnIdleFrame(Action action) {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) return this;
            _onIdleFrameAction.Remove(action); 
            if (_onIdleFrameAction.Count == 0) {
                GetParent().Disconnect("idle_frame", this, nameof(_GodotSignalIdleFrame));
            }
            return this;
        }
        private void _GodotSignalIdleFrame() {
            if (_onIdleFrameAction == null || _onIdleFrameAction.Count == 0) return;
            for (var i = 0; i < _onIdleFrameAction.Count; i++) _onIdleFrameAction[i].Invoke();
        }
        

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public SceneTreeAction OnNetworkPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) {
                _onNetworkPeerConnectedAction ??= new List<Action<int>>(); 
                GetParent().Connect("network_peer_connected", this, nameof(_GodotSignalNetworkPeerConnected));
            }
            _onNetworkPeerConnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return this;
            _onNetworkPeerConnectedAction.Remove(action); 
            if (_onNetworkPeerConnectedAction.Count == 0) {
                GetParent().Disconnect("network_peer_connected", this, nameof(_GodotSignalNetworkPeerConnected));
            }
            return this;
        }
        private void _GodotSignalNetworkPeerConnected(int id) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerConnectedAction.Count; i++) _onNetworkPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public SceneTreeAction OnNetworkPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) {
                _onNetworkPeerDisconnectedAction ??= new List<Action<int>>(); 
                GetParent().Connect("network_peer_disconnected", this, nameof(_GodotSignalNetworkPeerDisconnected));
            }
            _onNetworkPeerDisconnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return this;
            _onNetworkPeerDisconnectedAction.Remove(action); 
            if (_onNetworkPeerDisconnectedAction.Count == 0) {
                GetParent().Disconnect("network_peer_disconnected", this, nameof(_GodotSignalNetworkPeerDisconnected));
            }
            return this;
        }
        private void _GodotSignalNetworkPeerDisconnected(int id) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerDisconnectedAction.Count; i++) _onNetworkPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action<Node>>? _onNodeAddedAction; 
        public SceneTreeAction OnNodeAdded(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) {
                _onNodeAddedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_added", this, nameof(_GodotSignalNodeAdded));
            }
            _onNodeAddedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeAdded(Action<Node> action) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) return this;
            _onNodeAddedAction.Remove(action); 
            if (_onNodeAddedAction.Count == 0) {
                GetParent().Disconnect("node_added", this, nameof(_GodotSignalNodeAdded));
            }
            return this;
        }
        private void _GodotSignalNodeAdded(Node node) {
            if (_onNodeAddedAction == null || _onNodeAddedAction.Count == 0) return;
            for (var i = 0; i < _onNodeAddedAction.Count; i++) _onNodeAddedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeConfigurationWarningChangedAction; 
        public SceneTreeAction OnNodeConfigurationWarningChanged(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) {
                _onNodeConfigurationWarningChangedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_configuration_warning_changed", this, nameof(_GodotSignalNodeConfigurationWarningChanged));
            }
            _onNodeConfigurationWarningChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeConfigurationWarningChanged(Action<Node> action) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) return this;
            _onNodeConfigurationWarningChangedAction.Remove(action); 
            if (_onNodeConfigurationWarningChangedAction.Count == 0) {
                GetParent().Disconnect("node_configuration_warning_changed", this, nameof(_GodotSignalNodeConfigurationWarningChanged));
            }
            return this;
        }
        private void _GodotSignalNodeConfigurationWarningChanged(Node node) {
            if (_onNodeConfigurationWarningChangedAction == null || _onNodeConfigurationWarningChangedAction.Count == 0) return;
            for (var i = 0; i < _onNodeConfigurationWarningChangedAction.Count; i++) _onNodeConfigurationWarningChangedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeRemovedAction; 
        public SceneTreeAction OnNodeRemoved(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) {
                _onNodeRemovedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_removed", this, nameof(_GodotSignalNodeRemoved));
            }
            _onNodeRemovedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeRemoved(Action<Node> action) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) return this;
            _onNodeRemovedAction.Remove(action); 
            if (_onNodeRemovedAction.Count == 0) {
                GetParent().Disconnect("node_removed", this, nameof(_GodotSignalNodeRemoved));
            }
            return this;
        }
        private void _GodotSignalNodeRemoved(Node node) {
            if (_onNodeRemovedAction == null || _onNodeRemovedAction.Count == 0) return;
            for (var i = 0; i < _onNodeRemovedAction.Count; i++) _onNodeRemovedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeRenamedAction; 
        public SceneTreeAction OnNodeRenamed(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) {
                _onNodeRenamedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_renamed", this, nameof(_GodotSignalNodeRenamed));
            }
            _onNodeRenamedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnNodeRenamed(Action<Node> action) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) return this;
            _onNodeRenamedAction.Remove(action); 
            if (_onNodeRenamedAction.Count == 0) {
                GetParent().Disconnect("node_renamed", this, nameof(_GodotSignalNodeRenamed));
            }
            return this;
        }
        private void _GodotSignalNodeRenamed(Node node) {
            if (_onNodeRenamedAction == null || _onNodeRenamedAction.Count == 0) return;
            for (var i = 0; i < _onNodeRenamedAction.Count; i++) _onNodeRenamedAction[i].Invoke(node);
        }
        

        private List<Action>? _onPhysicsFrameAction; 
        public SceneTreeAction OnPhysicsFrame(Action action, bool oneShot = false, bool deferred = false) {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) {
                _onPhysicsFrameAction ??= new List<Action>(); 
                GetParent().Connect("physics_frame", this, nameof(_GodotSignalPhysicsFrame));
            }
            _onPhysicsFrameAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnPhysicsFrame(Action action) {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) return this;
            _onPhysicsFrameAction.Remove(action); 
            if (_onPhysicsFrameAction.Count == 0) {
                GetParent().Disconnect("physics_frame", this, nameof(_GodotSignalPhysicsFrame));
            }
            return this;
        }
        private void _GodotSignalPhysicsFrame() {
            if (_onPhysicsFrameAction == null || _onPhysicsFrameAction.Count == 0) return;
            for (var i = 0; i < _onPhysicsFrameAction.Count; i++) _onPhysicsFrameAction[i].Invoke();
        }
        

        private List<Action<bool, string>>? _onRequestPermissionsResultAction; 
        public SceneTreeAction OnRequestPermissionsResult(Action<bool, string> action, bool oneShot = false, bool deferred = false) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) {
                _onRequestPermissionsResultAction ??= new List<Action<bool, string>>(); 
                GetParent().Connect("on_request_permissions_result", this, nameof(_GodotSignalRequestPermissionsResult));
            }
            _onRequestPermissionsResultAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return this;
            _onRequestPermissionsResultAction.Remove(action); 
            if (_onRequestPermissionsResultAction.Count == 0) {
                GetParent().Disconnect("on_request_permissions_result", this, nameof(_GodotSignalRequestPermissionsResult));
            }
            return this;
        }
        private void _GodotSignalRequestPermissionsResult(bool granted, string permission) {
            if (_onRequestPermissionsResultAction == null || _onRequestPermissionsResultAction.Count == 0) return;
            for (var i = 0; i < _onRequestPermissionsResultAction.Count; i++) _onRequestPermissionsResultAction[i].Invoke(granted, permission);
        }
        

        private List<Action>? _onScreenResizedAction; 
        public SceneTreeAction OnScreenResized(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) {
                _onScreenResizedAction ??= new List<Action>(); 
                GetParent().Connect("screen_resized", this, nameof(_GodotSignalScreenResized));
            }
            _onScreenResizedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnScreenResized(Action action) {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) return this;
            _onScreenResizedAction.Remove(action); 
            if (_onScreenResizedAction.Count == 0) {
                GetParent().Disconnect("screen_resized", this, nameof(_GodotSignalScreenResized));
            }
            return this;
        }
        private void _GodotSignalScreenResized() {
            if (_onScreenResizedAction == null || _onScreenResizedAction.Count == 0) return;
            for (var i = 0; i < _onScreenResizedAction.Count; i++) _onScreenResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public SceneTreeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public SceneTreeAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                GetParent().Connect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return this;
            _onServerDisconnectedAction.Remove(action); 
            if (_onServerDisconnectedAction.Count == 0) {
                GetParent().Disconnect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            return this;
        }
        private void _GodotSignalServerDisconnected() {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onServerDisconnectedAction.Count; i++) _onServerDisconnectedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeChangedAction; 
        public SceneTreeAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) {
                _onTreeChangedAction ??= new List<Action>(); 
                GetParent().Connect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            _onTreeChangedAction.Add(action);
            return this;
        }
        public SceneTreeAction RemoveOnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return this;
            _onTreeChangedAction.Remove(action); 
            if (_onTreeChangedAction.Count == 0) {
                GetParent().Disconnect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            return this;
        }
        private void _GodotSignalTreeChanged() {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return;
            for (var i = 0; i < _onTreeChangedAction.Count; i++) _onTreeChangedAction[i].Invoke();
        }
        
    }
}