using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketServerAction : ProxyNode {

        private List<Action<int, int, string>>? _onClientCloseRequestAction; 
        public void OnClientCloseRequest(Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onClientCloseRequestAction, "client_close_request", nameof(_GodotSignalClientCloseRequest), action, oneShot, deferred);

        public void RemoveOnClientCloseRequest(Action<int, int, string> action) =>
            RemoveSignal(_onClientCloseRequestAction, "client_close_request", nameof(_GodotSignalClientCloseRequest), action);

        private void _GodotSignalClientCloseRequest(int code, int id, string reason) =>
            ExecuteSignal(_onClientCloseRequestAction, code, id, reason);
        

        private List<Action<int, string>>? _onClientConnectedAction; 
        public void OnClientConnected(Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onClientConnectedAction, "client_connected", nameof(_GodotSignalClientConnected), action, oneShot, deferred);

        public void RemoveOnClientConnected(Action<int, string> action) =>
            RemoveSignal(_onClientConnectedAction, "client_connected", nameof(_GodotSignalClientConnected), action);

        private void _GodotSignalClientConnected(int id, string protocol) =>
            ExecuteSignal(_onClientConnectedAction, id, protocol);
        

        private List<Action<int, bool>>? _onClientDisconnectedAction; 
        public void OnClientDisconnected(Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onClientDisconnectedAction, "client_disconnected", nameof(_GodotSignalClientDisconnected), action, oneShot, deferred);

        public void RemoveOnClientDisconnected(Action<int, bool> action) =>
            RemoveSignal(_onClientDisconnectedAction, "client_disconnected", nameof(_GodotSignalClientDisconnected), action);

        private void _GodotSignalClientDisconnected(int id, bool was_clean_close) =>
            ExecuteSignal(_onClientDisconnectedAction, id, was_clean_close);
        

        private List<Action>? _onConnectionFailedAction; 
        public void OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);

        public void RemoveOnConnectionFailed(Action action) =>
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);

        private void _GodotSignalConnectionFailed() =>
            ExecuteSignal(_onConnectionFailedAction);
        

        private List<Action>? _onConnectionSucceededAction; 
        public void OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action, oneShot, deferred);

        public void RemoveOnConnectionSucceeded(Action action) =>
            RemoveSignal(_onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action);

        private void _GodotSignalConnectionSucceeded() =>
            ExecuteSignal(_onConnectionSucceededAction);
        

        private List<Action<int>>? _onDataReceivedAction; 
        public void OnDataReceived(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action, oneShot, deferred);

        public void RemoveOnDataReceived(Action<int> action) =>
            RemoveSignal(_onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action);

        private void _GodotSignalDataReceived(int id) =>
            ExecuteSignal(_onDataReceivedAction, id);
        

        private List<Action<int>>? _onPeerConnectedAction; 
        public void OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action, oneShot, deferred);

        public void RemoveOnPeerConnected(Action<int> action) =>
            RemoveSignal(_onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action);

        private void _GodotSignalPeerConnected(int id) =>
            ExecuteSignal(_onPeerConnectedAction, id);
        

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public void OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action, oneShot, deferred);

        public void RemoveOnPeerDisconnected(Action<int> action) =>
            RemoveSignal(_onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action);

        private void _GodotSignalPeerDisconnected(int id) =>
            ExecuteSignal(_onPeerDisconnectedAction, id);
        

        private List<Action<int>>? _onPeerPacketAction; 
        public void OnPeerPacket(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action, oneShot, deferred);

        public void RemoveOnPeerPacket(Action<int> action) =>
            RemoveSignal(_onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action);

        private void _GodotSignalPeerPacket(int peer_source) =>
            ExecuteSignal(_onPeerPacketAction, peer_source);
        

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
        
    }
}