using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketClientAction : WebSocketClient {


        private List<Action<bool>>? _onConnectionClosedAction; 
        public WebSocketClientAction OnConnectionClosed(Action<bool> action) {
            if (_onConnectionClosedAction == null || _onConnectionClosedAction.Count == 0) {
                _onConnectionClosedAction ??= new List<Action<bool>>(); 
                Connect("connection_closed", this, nameof(_GodotSignalConnectionClosed));
            }
            _onConnectionClosedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionClosed(Action<bool> action) {
            if (_onConnectionClosedAction == null || _onConnectionClosedAction.Count == 0) return this;
            _onConnectionClosedAction.Remove(action); 
            if (_onConnectionClosedAction.Count == 0) {
                Disconnect("connection_closed", this, nameof(_GodotSignalConnectionClosed));
            }
            return this;
        }
        private void _GodotSignalConnectionClosed(bool was_clean_close) {
            if (_onConnectionClosedAction == null || _onConnectionClosedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionClosedAction.Count; i++) _onConnectionClosedAction[i].Invoke(was_clean_close);
        }
        

        private List<Action>? _onConnectionErrorAction; 
        public WebSocketClientAction OnConnectionError(Action action) {
            if (_onConnectionErrorAction == null || _onConnectionErrorAction.Count == 0) {
                _onConnectionErrorAction ??= new List<Action>(); 
                Connect("connection_error", this, nameof(_GodotSignalConnectionError));
            }
            _onConnectionErrorAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionError(Action action) {
            if (_onConnectionErrorAction == null || _onConnectionErrorAction.Count == 0) return this;
            _onConnectionErrorAction.Remove(action); 
            if (_onConnectionErrorAction.Count == 0) {
                Disconnect("connection_error", this, nameof(_GodotSignalConnectionError));
            }
            return this;
        }
        private void _GodotSignalConnectionError() {
            if (_onConnectionErrorAction == null || _onConnectionErrorAction.Count == 0) return;
            for (var i = 0; i < _onConnectionErrorAction.Count; i++) _onConnectionErrorAction[i].Invoke();
        }
        

        private List<Action<string>>? _onConnectionEstablishedAction; 
        public WebSocketClientAction OnConnectionEstablished(Action<string> action) {
            if (_onConnectionEstablishedAction == null || _onConnectionEstablishedAction.Count == 0) {
                _onConnectionEstablishedAction ??= new List<Action<string>>(); 
                Connect("connection_established", this, nameof(_GodotSignalConnectionEstablished));
            }
            _onConnectionEstablishedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionEstablished(Action<string> action) {
            if (_onConnectionEstablishedAction == null || _onConnectionEstablishedAction.Count == 0) return this;
            _onConnectionEstablishedAction.Remove(action); 
            if (_onConnectionEstablishedAction.Count == 0) {
                Disconnect("connection_established", this, nameof(_GodotSignalConnectionEstablished));
            }
            return this;
        }
        private void _GodotSignalConnectionEstablished(string protocol) {
            if (_onConnectionEstablishedAction == null || _onConnectionEstablishedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionEstablishedAction.Count; i++) _onConnectionEstablishedAction[i].Invoke(protocol);
        }
        

        private List<Action>? _onConnectionFailedAction; 
        public WebSocketClientAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                Connect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionFailed(Action action) {
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
        

        private List<Action>? _onConnectionSucceededAction; 
        public WebSocketClientAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) {
                _onConnectionSucceededAction ??= new List<Action>(); 
                Connect("connection_succeeded", this, nameof(_GodotSignalConnectionSucceeded));
            }
            _onConnectionSucceededAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return this;
            _onConnectionSucceededAction.Remove(action); 
            if (_onConnectionSucceededAction.Count == 0) {
                Disconnect("connection_succeeded", this, nameof(_GodotSignalConnectionSucceeded));
            }
            return this;
        }
        private void _GodotSignalConnectionSucceeded() {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return;
            for (var i = 0; i < _onConnectionSucceededAction.Count; i++) _onConnectionSucceededAction[i].Invoke();
        }
        

        private List<Action>? _onDataReceivedAction; 
        public WebSocketClientAction OnDataReceived(Action action) {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) {
                _onDataReceivedAction ??= new List<Action>(); 
                Connect("data_received", this, nameof(_GodotSignalDataReceived));
            }
            _onDataReceivedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnDataReceived(Action action) {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) return this;
            _onDataReceivedAction.Remove(action); 
            if (_onDataReceivedAction.Count == 0) {
                Disconnect("data_received", this, nameof(_GodotSignalDataReceived));
            }
            return this;
        }
        private void _GodotSignalDataReceived() {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) return;
            for (var i = 0; i < _onDataReceivedAction.Count; i++) _onDataReceivedAction[i].Invoke();
        }
        

        private List<Action<int>>? _onPeerConnectedAction; 
        public WebSocketClientAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) {
                _onPeerConnectedAction ??= new List<Action<int>>(); 
                Connect("peer_connected", this, nameof(_GodotSignalPeerConnected));
            }
            _onPeerConnectedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return this;
            _onPeerConnectedAction.Remove(action); 
            if (_onPeerConnectedAction.Count == 0) {
                Disconnect("peer_connected", this, nameof(_GodotSignalPeerConnected));
            }
            return this;
        }
        private void _GodotSignalPeerConnected(int id) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerConnectedAction.Count; i++) _onPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public WebSocketClientAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) {
                _onPeerDisconnectedAction ??= new List<Action<int>>(); 
                Connect("peer_disconnected", this, nameof(_GodotSignalPeerDisconnected));
            }
            _onPeerDisconnectedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return this;
            _onPeerDisconnectedAction.Remove(action); 
            if (_onPeerDisconnectedAction.Count == 0) {
                Disconnect("peer_disconnected", this, nameof(_GodotSignalPeerDisconnected));
            }
            return this;
        }
        private void _GodotSignalPeerDisconnected(int id) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerDisconnectedAction.Count; i++) _onPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onPeerPacketAction; 
        public WebSocketClientAction OnPeerPacket(Action<int> action) {
            if (_onPeerPacketAction == null || _onPeerPacketAction.Count == 0) {
                _onPeerPacketAction ??= new List<Action<int>>(); 
                Connect("peer_packet", this, nameof(_GodotSignalPeerPacket));
            }
            _onPeerPacketAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnPeerPacket(Action<int> action) {
            if (_onPeerPacketAction == null || _onPeerPacketAction.Count == 0) return this;
            _onPeerPacketAction.Remove(action); 
            if (_onPeerPacketAction.Count == 0) {
                Disconnect("peer_packet", this, nameof(_GodotSignalPeerPacket));
            }
            return this;
        }
        private void _GodotSignalPeerPacket(int peer_source) {
            if (_onPeerPacketAction == null || _onPeerPacketAction.Count == 0) return;
            for (var i = 0; i < _onPeerPacketAction.Count; i++) _onPeerPacketAction[i].Invoke(peer_source);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public WebSocketClientAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnScriptChanged(Action action) {
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
        

        private List<Action<int, string>>? _onServerCloseRequestAction; 
        public WebSocketClientAction OnServerCloseRequest(Action<int, string> action) {
            if (_onServerCloseRequestAction == null || _onServerCloseRequestAction.Count == 0) {
                _onServerCloseRequestAction ??= new List<Action<int, string>>(); 
                Connect("server_close_request", this, nameof(_GodotSignalServerCloseRequest));
            }
            _onServerCloseRequestAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnServerCloseRequest(Action<int, string> action) {
            if (_onServerCloseRequestAction == null || _onServerCloseRequestAction.Count == 0) return this;
            _onServerCloseRequestAction.Remove(action); 
            if (_onServerCloseRequestAction.Count == 0) {
                Disconnect("server_close_request", this, nameof(_GodotSignalServerCloseRequest));
            }
            return this;
        }
        private void _GodotSignalServerCloseRequest(int code, string reason) {
            if (_onServerCloseRequestAction == null || _onServerCloseRequestAction.Count == 0) return;
            for (var i = 0; i < _onServerCloseRequestAction.Count; i++) _onServerCloseRequestAction[i].Invoke(code, reason);
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public WebSocketClientAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                Connect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public WebSocketClientAction RemoveOnServerDisconnected(Action action) {
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