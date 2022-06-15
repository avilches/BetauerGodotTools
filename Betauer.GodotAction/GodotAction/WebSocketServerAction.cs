using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketServerAction : WebSocketServer {


        private Action<int, int, string>? _onClientCloseRequestAction; 
        public WebSocketServerAction OnClientCloseRequest(Action<int, int, string> action) {
            if (_onClientCloseRequestAction == null) 
                Connect("client_close_request", this, nameof(ExecuteClientCloseRequest));
            _onClientCloseRequestAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnClientCloseRequest() {
            if (_onClientCloseRequestAction == null) return this; 
            Disconnect("client_close_request", this, nameof(ExecuteClientCloseRequest));
            _onClientCloseRequestAction = null;
            return this;
        }
        private void ExecuteClientCloseRequest(int code, int id, string reason) =>
            _onClientCloseRequestAction?.Invoke(code, id, reason);
        

        private Action<int, string>? _onClientConnectedAction; 
        public WebSocketServerAction OnClientConnected(Action<int, string> action) {
            if (_onClientConnectedAction == null) 
                Connect("client_connected", this, nameof(ExecuteClientConnected));
            _onClientConnectedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnClientConnected() {
            if (_onClientConnectedAction == null) return this; 
            Disconnect("client_connected", this, nameof(ExecuteClientConnected));
            _onClientConnectedAction = null;
            return this;
        }
        private void ExecuteClientConnected(int id, string protocol) =>
            _onClientConnectedAction?.Invoke(id, protocol);
        

        private Action<int, bool>? _onClientDisconnectedAction; 
        public WebSocketServerAction OnClientDisconnected(Action<int, bool> action) {
            if (_onClientDisconnectedAction == null) 
                Connect("client_disconnected", this, nameof(ExecuteClientDisconnected));
            _onClientDisconnectedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnClientDisconnected() {
            if (_onClientDisconnectedAction == null) return this; 
            Disconnect("client_disconnected", this, nameof(ExecuteClientDisconnected));
            _onClientDisconnectedAction = null;
            return this;
        }
        private void ExecuteClientDisconnected(int id, bool was_clean_close) =>
            _onClientDisconnectedAction?.Invoke(id, was_clean_close);
        

        private Action? _onConnectionFailedAction; 
        public WebSocketServerAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null) 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnConnectionFailed() {
            if (_onConnectionFailedAction == null) return this; 
            Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = null;
            return this;
        }
        private void ExecuteConnectionFailed() =>
            _onConnectionFailedAction?.Invoke();
        

        private Action? _onConnectionSucceededAction; 
        public WebSocketServerAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null) 
                Connect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnConnectionSucceeded() {
            if (_onConnectionSucceededAction == null) return this; 
            Disconnect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = null;
            return this;
        }
        private void ExecuteConnectionSucceeded() =>
            _onConnectionSucceededAction?.Invoke();
        

        private Action<int>? _onDataReceivedAction; 
        public WebSocketServerAction OnDataReceived(Action<int> action) {
            if (_onDataReceivedAction == null) 
                Connect("data_received", this, nameof(ExecuteDataReceived));
            _onDataReceivedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnDataReceived() {
            if (_onDataReceivedAction == null) return this; 
            Disconnect("data_received", this, nameof(ExecuteDataReceived));
            _onDataReceivedAction = null;
            return this;
        }
        private void ExecuteDataReceived(int id) =>
            _onDataReceivedAction?.Invoke(id);
        

        private Action<int>? _onPeerConnectedAction; 
        public WebSocketServerAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null) 
                Connect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnPeerConnected() {
            if (_onPeerConnectedAction == null) return this; 
            Disconnect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = null;
            return this;
        }
        private void ExecutePeerConnected(int id) =>
            _onPeerConnectedAction?.Invoke(id);
        

        private Action<int>? _onPeerDisconnectedAction; 
        public WebSocketServerAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null) 
                Connect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnPeerDisconnected() {
            if (_onPeerDisconnectedAction == null) return this; 
            Disconnect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = null;
            return this;
        }
        private void ExecutePeerDisconnected(int id) =>
            _onPeerDisconnectedAction?.Invoke(id);
        

        private Action<int>? _onPeerPacketAction; 
        public WebSocketServerAction OnPeerPacket(Action<int> action) {
            if (_onPeerPacketAction == null) 
                Connect("peer_packet", this, nameof(ExecutePeerPacket));
            _onPeerPacketAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnPeerPacket() {
            if (_onPeerPacketAction == null) return this; 
            Disconnect("peer_packet", this, nameof(ExecutePeerPacket));
            _onPeerPacketAction = null;
            return this;
        }
        private void ExecutePeerPacket(int peer_source) =>
            _onPeerPacketAction?.Invoke(peer_source);
        

        private Action? _onScriptChangedAction; 
        public WebSocketServerAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onServerDisconnectedAction; 
        public WebSocketServerAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null) 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = action;
            return this;
        }
        public WebSocketServerAction RemoveOnServerDisconnected() {
            if (_onServerDisconnectedAction == null) return this; 
            Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = null;
            return this;
        }
        private void ExecuteServerDisconnected() =>
            _onServerDisconnectedAction?.Invoke();
        
    }
}