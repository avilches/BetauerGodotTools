using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerAPIAction : MultiplayerAPI {


        private List<Action>? _onConnectedToServerAction; 
        public MultiplayerAPIAction OnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) {
                _onConnectedToServerAction ??= new List<Action>(); 
                Connect("connected_to_server", this, nameof(_GodotSignalConnectedToServer));
            }
            _onConnectedToServerAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return this;
            _onConnectedToServerAction.Remove(action); 
            if (_onConnectedToServerAction.Count == 0) {
                Disconnect("connected_to_server", this, nameof(_GodotSignalConnectedToServer));
            }
            return this;
        }
        private void _GodotSignalConnectedToServer() {
            if (_onConnectedToServerAction == null || _onConnectedToServerAction.Count == 0) return;
            for (var i = 0; i < _onConnectedToServerAction.Count; i++) _onConnectedToServerAction[i].Invoke();
        }
        

        private List<Action>? _onConnectionFailedAction; 
        public MultiplayerAPIAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                Connect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return this;
            _onConnectionFailedAction.Remove(action); 
            if (_onConnectionFailedAction.Count == 0) {
                Disconnect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            return this;
        }
        private void _GodotSignalConnectionFailed() {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFailedAction.Count; i++) _onConnectionFailedAction[i].Invoke();
        }
        

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) {
                _onNetworkPeerConnectedAction ??= new List<Action<int>>(); 
                Connect("network_peer_connected", this, nameof(_GodotSignalNetworkPeerConnected));
            }
            _onNetworkPeerConnectedAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return this;
            _onNetworkPeerConnectedAction.Remove(action); 
            if (_onNetworkPeerConnectedAction.Count == 0) {
                Disconnect("network_peer_connected", this, nameof(_GodotSignalNetworkPeerConnected));
            }
            return this;
        }
        private void _GodotSignalNetworkPeerConnected(int id) {
            if (_onNetworkPeerConnectedAction == null || _onNetworkPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerConnectedAction.Count; i++) _onNetworkPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) {
                _onNetworkPeerDisconnectedAction ??= new List<Action<int>>(); 
                Connect("network_peer_disconnected", this, nameof(_GodotSignalNetworkPeerDisconnected));
            }
            _onNetworkPeerDisconnectedAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return this;
            _onNetworkPeerDisconnectedAction.Remove(action); 
            if (_onNetworkPeerDisconnectedAction.Count == 0) {
                Disconnect("network_peer_disconnected", this, nameof(_GodotSignalNetworkPeerDisconnected));
            }
            return this;
        }
        private void _GodotSignalNetworkPeerDisconnected(int id) {
            if (_onNetworkPeerDisconnectedAction == null || _onNetworkPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerDisconnectedAction.Count; i++) _onNetworkPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action<int, byte[]>>? _onNetworkPeerPacketAction; 
        public MultiplayerAPIAction OnNetworkPeerPacket(Action<int, byte[]> action) {
            if (_onNetworkPeerPacketAction == null || _onNetworkPeerPacketAction.Count == 0) {
                _onNetworkPeerPacketAction ??= new List<Action<int, byte[]>>(); 
                Connect("network_peer_packet", this, nameof(_GodotSignalNetworkPeerPacket));
            }
            _onNetworkPeerPacketAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerPacket(Action<int, byte[]> action) {
            if (_onNetworkPeerPacketAction == null || _onNetworkPeerPacketAction.Count == 0) return this;
            _onNetworkPeerPacketAction.Remove(action); 
            if (_onNetworkPeerPacketAction.Count == 0) {
                Disconnect("network_peer_packet", this, nameof(_GodotSignalNetworkPeerPacket));
            }
            return this;
        }
        private void _GodotSignalNetworkPeerPacket(int id, byte[] packet) {
            if (_onNetworkPeerPacketAction == null || _onNetworkPeerPacketAction.Count == 0) return;
            for (var i = 0; i < _onNetworkPeerPacketAction.Count; i++) _onNetworkPeerPacketAction[i].Invoke(id, packet);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public MultiplayerAPIAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public MultiplayerAPIAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                Connect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public MultiplayerAPIAction RemoveOnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return this;
            _onServerDisconnectedAction.Remove(action); 
            if (_onServerDisconnectedAction.Count == 0) {
                Disconnect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            return this;
        }
        private void _GodotSignalServerDisconnected() {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onServerDisconnectedAction.Count; i++) _onServerDisconnectedAction[i].Invoke();
        }
        
    }
}