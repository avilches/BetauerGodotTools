using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketServerAction : WebSocketServer {


        private List<Action<int, int, string>>? _onClientCloseRequestAction; 
        public WebSocketServerAction OnClientCloseRequest(Action<int, int, string> action) {
            if (_onClientCloseRequestAction == null || _onClientCloseRequestAction.Count == 0) {
                _onClientCloseRequestAction ??= new List<Action<int, int, string>>(); 
                Connect("client_close_request", this, nameof(_GodotSignalClientCloseRequest));
            }
            _onClientCloseRequestAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnClientCloseRequest(Action<int, int, string> action) {
            if (_onClientCloseRequestAction == null || _onClientCloseRequestAction.Count == 0) return this;
            _onClientCloseRequestAction.Remove(action); 
            if (_onClientCloseRequestAction.Count == 0) {
                Disconnect("client_close_request", this, nameof(_GodotSignalClientCloseRequest));
            }
            return this;
        }
        private void _GodotSignalClientCloseRequest(int code, int id, string reason) {
            if (_onClientCloseRequestAction == null || _onClientCloseRequestAction.Count == 0) return;
            for (var i = 0; i < _onClientCloseRequestAction.Count; i++) _onClientCloseRequestAction[i].Invoke(code, id, reason);
        }
        

        private List<Action<int, string>>? _onClientConnectedAction; 
        public WebSocketServerAction OnClientConnected(Action<int, string> action) {
            if (_onClientConnectedAction == null || _onClientConnectedAction.Count == 0) {
                _onClientConnectedAction ??= new List<Action<int, string>>(); 
                Connect("client_connected", this, nameof(_GodotSignalClientConnected));
            }
            _onClientConnectedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnClientConnected(Action<int, string> action) {
            if (_onClientConnectedAction == null || _onClientConnectedAction.Count == 0) return this;
            _onClientConnectedAction.Remove(action); 
            if (_onClientConnectedAction.Count == 0) {
                Disconnect("client_connected", this, nameof(_GodotSignalClientConnected));
            }
            return this;
        }
        private void _GodotSignalClientConnected(int id, string protocol) {
            if (_onClientConnectedAction == null || _onClientConnectedAction.Count == 0) return;
            for (var i = 0; i < _onClientConnectedAction.Count; i++) _onClientConnectedAction[i].Invoke(id, protocol);
        }
        

        private List<Action<int, bool>>? _onClientDisconnectedAction; 
        public WebSocketServerAction OnClientDisconnected(Action<int, bool> action) {
            if (_onClientDisconnectedAction == null || _onClientDisconnectedAction.Count == 0) {
                _onClientDisconnectedAction ??= new List<Action<int, bool>>(); 
                Connect("client_disconnected", this, nameof(_GodotSignalClientDisconnected));
            }
            _onClientDisconnectedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnClientDisconnected(Action<int, bool> action) {
            if (_onClientDisconnectedAction == null || _onClientDisconnectedAction.Count == 0) return this;
            _onClientDisconnectedAction.Remove(action); 
            if (_onClientDisconnectedAction.Count == 0) {
                Disconnect("client_disconnected", this, nameof(_GodotSignalClientDisconnected));
            }
            return this;
        }
        private void _GodotSignalClientDisconnected(int id, bool was_clean_close) {
            if (_onClientDisconnectedAction == null || _onClientDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onClientDisconnectedAction.Count; i++) _onClientDisconnectedAction[i].Invoke(id, was_clean_close);
        }
        

        private List<Action>? _onConnectionFailedAction; 
        public WebSocketServerAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                Connect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnConnectionFailed(Action action) {
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
        public WebSocketServerAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) {
                _onConnectionSucceededAction ??= new List<Action>(); 
                Connect("connection_succeeded", this, nameof(_GodotSignalConnectionSucceeded));
            }
            _onConnectionSucceededAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnConnectionSucceeded(Action action) {
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
        

        private List<Action<int>>? _onDataReceivedAction; 
        public WebSocketServerAction OnDataReceived(Action<int> action) {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) {
                _onDataReceivedAction ??= new List<Action<int>>(); 
                Connect("data_received", this, nameof(_GodotSignalDataReceived));
            }
            _onDataReceivedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnDataReceived(Action<int> action) {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) return this;
            _onDataReceivedAction.Remove(action); 
            if (_onDataReceivedAction.Count == 0) {
                Disconnect("data_received", this, nameof(_GodotSignalDataReceived));
            }
            return this;
        }
        private void _GodotSignalDataReceived(int id) {
            if (_onDataReceivedAction == null || _onDataReceivedAction.Count == 0) return;
            for (var i = 0; i < _onDataReceivedAction.Count; i++) _onDataReceivedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onPeerConnectedAction; 
        public WebSocketServerAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) {
                _onPeerConnectedAction ??= new List<Action<int>>(); 
                Connect("peer_connected", this, nameof(_GodotSignalPeerConnected));
            }
            _onPeerConnectedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnPeerConnected(Action<int> action) {
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
        public WebSocketServerAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) {
                _onPeerDisconnectedAction ??= new List<Action<int>>(); 
                Connect("peer_disconnected", this, nameof(_GodotSignalPeerDisconnected));
            }
            _onPeerDisconnectedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnPeerDisconnected(Action<int> action) {
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
        public WebSocketServerAction OnPeerPacket(Action<int> action) {
            if (_onPeerPacketAction == null || _onPeerPacketAction.Count == 0) {
                _onPeerPacketAction ??= new List<Action<int>>(); 
                Connect("peer_packet", this, nameof(_GodotSignalPeerPacket));
            }
            _onPeerPacketAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnPeerPacket(Action<int> action) {
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
        public WebSocketServerAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnScriptChanged(Action action) {
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
        public WebSocketServerAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                Connect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public WebSocketServerAction RemoveOnServerDisconnected(Action action) {
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