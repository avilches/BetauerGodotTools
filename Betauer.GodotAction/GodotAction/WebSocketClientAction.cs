using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketClientAction : WebSocketClient {


        private Action<bool>? _onConnectionClosedAction; 
        public WebSocketClientAction OnConnectionClosed(Action<bool> action) {
            if (_onConnectionClosedAction == null) 
                Connect("connection_closed", this, nameof(ExecuteConnectionClosed));
            _onConnectionClosedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionClosed() {
            if (_onConnectionClosedAction == null) return this; 
            Disconnect("connection_closed", this, nameof(ExecuteConnectionClosed));
            _onConnectionClosedAction = null;
            return this;
        }
        private void ExecuteConnectionClosed(bool was_clean_close) =>
            _onConnectionClosedAction?.Invoke(was_clean_close);
        

        private Action? _onConnectionErrorAction; 
        public WebSocketClientAction OnConnectionError(Action action) {
            if (_onConnectionErrorAction == null) 
                Connect("connection_error", this, nameof(ExecuteConnectionError));
            _onConnectionErrorAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionError() {
            if (_onConnectionErrorAction == null) return this; 
            Disconnect("connection_error", this, nameof(ExecuteConnectionError));
            _onConnectionErrorAction = null;
            return this;
        }
        private void ExecuteConnectionError() =>
            _onConnectionErrorAction?.Invoke();
        

        private Action<string>? _onConnectionEstablishedAction; 
        public WebSocketClientAction OnConnectionEstablished(Action<string> action) {
            if (_onConnectionEstablishedAction == null) 
                Connect("connection_established", this, nameof(ExecuteConnectionEstablished));
            _onConnectionEstablishedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionEstablished() {
            if (_onConnectionEstablishedAction == null) return this; 
            Disconnect("connection_established", this, nameof(ExecuteConnectionEstablished));
            _onConnectionEstablishedAction = null;
            return this;
        }
        private void ExecuteConnectionEstablished(string protocol) =>
            _onConnectionEstablishedAction?.Invoke(protocol);
        

        private Action? _onConnectionFailedAction; 
        public WebSocketClientAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null) 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionFailed() {
            if (_onConnectionFailedAction == null) return this; 
            Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = null;
            return this;
        }
        private void ExecuteConnectionFailed() =>
            _onConnectionFailedAction?.Invoke();
        

        private Action? _onConnectionSucceededAction; 
        public WebSocketClientAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null) 
                Connect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionSucceeded() {
            if (_onConnectionSucceededAction == null) return this; 
            Disconnect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = null;
            return this;
        }
        private void ExecuteConnectionSucceeded() =>
            _onConnectionSucceededAction?.Invoke();
        

        private Action? _onDataReceivedAction; 
        public WebSocketClientAction OnDataReceived(Action action) {
            if (_onDataReceivedAction == null) 
                Connect("data_received", this, nameof(ExecuteDataReceived));
            _onDataReceivedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnDataReceived() {
            if (_onDataReceivedAction == null) return this; 
            Disconnect("data_received", this, nameof(ExecuteDataReceived));
            _onDataReceivedAction = null;
            return this;
        }
        private void ExecuteDataReceived() =>
            _onDataReceivedAction?.Invoke();
        

        private Action<int>? _onPeerConnectedAction; 
        public WebSocketClientAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null) 
                Connect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnPeerConnected() {
            if (_onPeerConnectedAction == null) return this; 
            Disconnect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = null;
            return this;
        }
        private void ExecutePeerConnected(int id) =>
            _onPeerConnectedAction?.Invoke(id);
        

        private Action<int>? _onPeerDisconnectedAction; 
        public WebSocketClientAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null) 
                Connect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnPeerDisconnected() {
            if (_onPeerDisconnectedAction == null) return this; 
            Disconnect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = null;
            return this;
        }
        private void ExecutePeerDisconnected(int id) =>
            _onPeerDisconnectedAction?.Invoke(id);
        

        private Action<int>? _onPeerPacketAction; 
        public WebSocketClientAction OnPeerPacket(Action<int> action) {
            if (_onPeerPacketAction == null) 
                Connect("peer_packet", this, nameof(ExecutePeerPacket));
            _onPeerPacketAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnPeerPacket() {
            if (_onPeerPacketAction == null) return this; 
            Disconnect("peer_packet", this, nameof(ExecutePeerPacket));
            _onPeerPacketAction = null;
            return this;
        }
        private void ExecutePeerPacket(int peer_source) =>
            _onPeerPacketAction?.Invoke(peer_source);
        

        private Action? _onScriptChangedAction; 
        public WebSocketClientAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action<int, string>? _onServerCloseRequestAction; 
        public WebSocketClientAction OnServerCloseRequest(Action<int, string> action) {
            if (_onServerCloseRequestAction == null) 
                Connect("server_close_request", this, nameof(ExecuteServerCloseRequest));
            _onServerCloseRequestAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnServerCloseRequest() {
            if (_onServerCloseRequestAction == null) return this; 
            Disconnect("server_close_request", this, nameof(ExecuteServerCloseRequest));
            _onServerCloseRequestAction = null;
            return this;
        }
        private void ExecuteServerCloseRequest(int code, string reason) =>
            _onServerCloseRequestAction?.Invoke(code, reason);
        

        private Action? _onServerDisconnectedAction; 
        public WebSocketClientAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null) 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = action;
            return this;
        }
        public WebSocketClientAction RemoveOnServerDisconnected() {
            if (_onServerDisconnectedAction == null) return this; 
            Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = null;
            return this;
        }
        private void ExecuteServerDisconnected() =>
            _onServerDisconnectedAction?.Invoke();
        
    }
}