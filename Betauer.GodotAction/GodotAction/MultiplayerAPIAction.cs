using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerAPIAction : MultiplayerAPI {


        private Action? _onConnectedToServerAction; 
        public MultiplayerAPIAction OnConnectedToServer(Action action) {
            if (_onConnectedToServerAction == null) 
                Connect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            _onConnectedToServerAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnConnectedToServer() {
            if (_onConnectedToServerAction == null) return this; 
            Disconnect("connected_to_server", this, nameof(ExecuteConnectedToServer));
            _onConnectedToServerAction = null;
            return this;
        }
        private void ExecuteConnectedToServer() =>
            _onConnectedToServerAction?.Invoke();
        

        private Action? _onConnectionFailedAction; 
        public MultiplayerAPIAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null) 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnConnectionFailed() {
            if (_onConnectionFailedAction == null) return this; 
            Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = null;
            return this;
        }
        private void ExecuteConnectionFailed() =>
            _onConnectionFailedAction?.Invoke();
        

        private Action<int>? _onNetworkPeerConnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerConnected(Action<int> action) {
            if (_onNetworkPeerConnectedAction == null) 
                Connect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            _onNetworkPeerConnectedAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerConnected() {
            if (_onNetworkPeerConnectedAction == null) return this; 
            Disconnect("network_peer_connected", this, nameof(ExecuteNetworkPeerConnected));
            _onNetworkPeerConnectedAction = null;
            return this;
        }
        private void ExecuteNetworkPeerConnected(int id) =>
            _onNetworkPeerConnectedAction?.Invoke(id);
        

        private Action<int>? _onNetworkPeerDisconnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerDisconnected(Action<int> action) {
            if (_onNetworkPeerDisconnectedAction == null) 
                Connect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            _onNetworkPeerDisconnectedAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerDisconnected() {
            if (_onNetworkPeerDisconnectedAction == null) return this; 
            Disconnect("network_peer_disconnected", this, nameof(ExecuteNetworkPeerDisconnected));
            _onNetworkPeerDisconnectedAction = null;
            return this;
        }
        private void ExecuteNetworkPeerDisconnected(int id) =>
            _onNetworkPeerDisconnectedAction?.Invoke(id);
        

        private Action<int, byte[]>? _onNetworkPeerPacketAction; 
        public MultiplayerAPIAction OnNetworkPeerPacket(Action<int, byte[]> action) {
            if (_onNetworkPeerPacketAction == null) 
                Connect("network_peer_packet", this, nameof(ExecuteNetworkPeerPacket));
            _onNetworkPeerPacketAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnNetworkPeerPacket() {
            if (_onNetworkPeerPacketAction == null) return this; 
            Disconnect("network_peer_packet", this, nameof(ExecuteNetworkPeerPacket));
            _onNetworkPeerPacketAction = null;
            return this;
        }
        private void ExecuteNetworkPeerPacket(int id, byte[] packet) =>
            _onNetworkPeerPacketAction?.Invoke(id, packet);
        

        private Action? _onScriptChangedAction; 
        public MultiplayerAPIAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onServerDisconnectedAction; 
        public MultiplayerAPIAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null) 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = action;
            return this;
        }
        public MultiplayerAPIAction RemoveOnServerDisconnected() {
            if (_onServerDisconnectedAction == null) return this; 
            Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = null;
            return this;
        }
        private void ExecuteServerDisconnected() =>
            _onServerDisconnectedAction?.Invoke();
        
    }
}