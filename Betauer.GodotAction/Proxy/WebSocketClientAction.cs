using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class WebSocketClientAction : ProxyNode {

        private List<Action<bool>>? _onConnectionClosedAction; 
        public WebSocketClientAction OnConnectionClosed(Action<bool> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionClosedAction, "connection_closed", nameof(_GodotSignalConnectionClosed), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnConnectionClosed(Action<bool> action) {
            RemoveSignal(_onConnectionClosedAction, "connection_closed", nameof(_GodotSignalConnectionClosed), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalConnectionClosed(bool was_clean_close) {
            ExecuteSignal(_onConnectionClosedAction, was_clean_close);
            return this;
        }

        private List<Action>? _onConnectionErrorAction; 
        public WebSocketClientAction OnConnectionError(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionErrorAction, "connection_error", nameof(_GodotSignalConnectionError), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnConnectionError(Action action) {
            RemoveSignal(_onConnectionErrorAction, "connection_error", nameof(_GodotSignalConnectionError), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalConnectionError() {
            ExecuteSignal(_onConnectionErrorAction);
            return this;
        }

        private List<Action<string>>? _onConnectionEstablishedAction; 
        public WebSocketClientAction OnConnectionEstablished(Action<string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionEstablishedAction, "connection_established", nameof(_GodotSignalConnectionEstablished), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnConnectionEstablished(Action<string> action) {
            RemoveSignal(_onConnectionEstablishedAction, "connection_established", nameof(_GodotSignalConnectionEstablished), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalConnectionEstablished(string protocol) {
            ExecuteSignal(_onConnectionEstablishedAction, protocol);
            return this;
        }

        private List<Action>? _onConnectionFailedAction; 
        public WebSocketClientAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action>? _onConnectionSucceededAction; 
        public WebSocketClientAction OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnConnectionSucceeded(Action action) {
            RemoveSignal(_onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalConnectionSucceeded() {
            ExecuteSignal(_onConnectionSucceededAction);
            return this;
        }

        private List<Action>? _onDataReceivedAction; 
        public WebSocketClientAction OnDataReceived(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnDataReceived(Action action) {
            RemoveSignal(_onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalDataReceived() {
            ExecuteSignal(_onDataReceivedAction);
            return this;
        }

        private List<Action<int>>? _onPeerConnectedAction; 
        public WebSocketClientAction OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnPeerConnected(Action<int> action) {
            RemoveSignal(_onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalPeerConnected(int id) {
            ExecuteSignal(_onPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public WebSocketClientAction OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnPeerDisconnected(Action<int> action) {
            RemoveSignal(_onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalPeerDisconnected(int id) {
            ExecuteSignal(_onPeerDisconnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerPacketAction; 
        public WebSocketClientAction OnPeerPacket(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnPeerPacket(Action<int> action) {
            RemoveSignal(_onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalPeerPacket(int peer_source) {
            ExecuteSignal(_onPeerPacketAction, peer_source);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public WebSocketClientAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action<int, string>>? _onServerCloseRequestAction; 
        public WebSocketClientAction OnServerCloseRequest(Action<int, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerCloseRequestAction, "server_close_request", nameof(_GodotSignalServerCloseRequest), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnServerCloseRequest(Action<int, string> action) {
            RemoveSignal(_onServerCloseRequestAction, "server_close_request", nameof(_GodotSignalServerCloseRequest), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalServerCloseRequest(int code, string reason) {
            ExecuteSignal(_onServerCloseRequestAction, code, reason);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public WebSocketClientAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketClientAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private WebSocketClientAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }
    }
}