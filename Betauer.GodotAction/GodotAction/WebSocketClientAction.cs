using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketClientAction : ProxyNode {

        private List<Action<bool>>? _onConnectionClosedAction; 
        public void OnConnectionClosed(Action<bool> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionClosedAction, "connection_closed", nameof(_GodotSignalConnectionClosed), action, oneShot, deferred);

        public void RemoveOnConnectionClosed(Action<bool> action) =>
            RemoveSignal(_onConnectionClosedAction, "connection_closed", nameof(_GodotSignalConnectionClosed), action);

        private void _GodotSignalConnectionClosed(bool was_clean_close) =>
            ExecuteSignal(_onConnectionClosedAction, was_clean_close);
        

        private List<Action>? _onConnectionErrorAction; 
        public void OnConnectionError(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionErrorAction, "connection_error", nameof(_GodotSignalConnectionError), action, oneShot, deferred);

        public void RemoveOnConnectionError(Action action) =>
            RemoveSignal(_onConnectionErrorAction, "connection_error", nameof(_GodotSignalConnectionError), action);

        private void _GodotSignalConnectionError() =>
            ExecuteSignal(_onConnectionErrorAction);
        

        private List<Action<string>>? _onConnectionEstablishedAction; 
        public void OnConnectionEstablished(Action<string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionEstablishedAction, "connection_established", nameof(_GodotSignalConnectionEstablished), action, oneShot, deferred);

        public void RemoveOnConnectionEstablished(Action<string> action) =>
            RemoveSignal(_onConnectionEstablishedAction, "connection_established", nameof(_GodotSignalConnectionEstablished), action);

        private void _GodotSignalConnectionEstablished(string protocol) =>
            ExecuteSignal(_onConnectionEstablishedAction, protocol);
        

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
        

        private List<Action>? _onDataReceivedAction; 
        public void OnDataReceived(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action, oneShot, deferred);

        public void RemoveOnDataReceived(Action action) =>
            RemoveSignal(_onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action);

        private void _GodotSignalDataReceived() =>
            ExecuteSignal(_onDataReceivedAction);
        

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
        

        private List<Action<int, string>>? _onServerCloseRequestAction; 
        public void OnServerCloseRequest(Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onServerCloseRequestAction, "server_close_request", nameof(_GodotSignalServerCloseRequest), action, oneShot, deferred);

        public void RemoveOnServerCloseRequest(Action<int, string> action) =>
            RemoveSignal(_onServerCloseRequestAction, "server_close_request", nameof(_GodotSignalServerCloseRequest), action);

        private void _GodotSignalServerCloseRequest(int code, string reason) =>
            ExecuteSignal(_onServerCloseRequestAction, code, reason);
        

        private List<Action>? _onServerDisconnectedAction; 
        public void OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);

        public void RemoveOnServerDisconnected(Action action) =>
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);

        private void _GodotSignalServerDisconnected() =>
            ExecuteSignal(_onServerDisconnectedAction);
        
    }
}