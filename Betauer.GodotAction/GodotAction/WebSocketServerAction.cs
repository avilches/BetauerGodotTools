using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebSocketServerAction : ProxyNode {

        private List<Action<int, int, string>>? _onClientCloseRequestAction; 
        public WebSocketServerAction OnClientCloseRequest(Action<int, int, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onClientCloseRequestAction, "client_close_request", nameof(_GodotSignalClientCloseRequest), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnClientCloseRequest(Action<int, int, string> action) {
            RemoveSignal(_onClientCloseRequestAction, "client_close_request", nameof(_GodotSignalClientCloseRequest), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalClientCloseRequest(int code, int id, string reason) {
            ExecuteSignal(_onClientCloseRequestAction, code, id, reason);
            return this;
        }

        private List<Action<int, string>>? _onClientConnectedAction; 
        public WebSocketServerAction OnClientConnected(Action<int, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onClientConnectedAction, "client_connected", nameof(_GodotSignalClientConnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnClientConnected(Action<int, string> action) {
            RemoveSignal(_onClientConnectedAction, "client_connected", nameof(_GodotSignalClientConnected), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalClientConnected(int id, string protocol) {
            ExecuteSignal(_onClientConnectedAction, id, protocol);
            return this;
        }

        private List<Action<int, bool>>? _onClientDisconnectedAction; 
        public WebSocketServerAction OnClientDisconnected(Action<int, bool> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onClientDisconnectedAction, "client_disconnected", nameof(_GodotSignalClientDisconnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnClientDisconnected(Action<int, bool> action) {
            RemoveSignal(_onClientDisconnectedAction, "client_disconnected", nameof(_GodotSignalClientDisconnected), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalClientDisconnected(int id, bool was_clean_close) {
            ExecuteSignal(_onClientDisconnectedAction, id, was_clean_close);
            return this;
        }

        private List<Action>? _onConnectionFailedAction; 
        public WebSocketServerAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action>? _onConnectionSucceededAction; 
        public WebSocketServerAction OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnConnectionSucceeded(Action action) {
            RemoveSignal(_onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalConnectionSucceeded() {
            ExecuteSignal(_onConnectionSucceededAction);
            return this;
        }

        private List<Action<int>>? _onDataReceivedAction; 
        public WebSocketServerAction OnDataReceived(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnDataReceived(Action<int> action) {
            RemoveSignal(_onDataReceivedAction, "data_received", nameof(_GodotSignalDataReceived), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalDataReceived(int id) {
            ExecuteSignal(_onDataReceivedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerConnectedAction; 
        public WebSocketServerAction OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnPeerConnected(Action<int> action) {
            RemoveSignal(_onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalPeerConnected(int id) {
            ExecuteSignal(_onPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public WebSocketServerAction OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnPeerDisconnected(Action<int> action) {
            RemoveSignal(_onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalPeerDisconnected(int id) {
            ExecuteSignal(_onPeerDisconnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerPacketAction; 
        public WebSocketServerAction OnPeerPacket(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnPeerPacket(Action<int> action) {
            RemoveSignal(_onPeerPacketAction, "peer_packet", nameof(_GodotSignalPeerPacket), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalPeerPacket(int peer_source) {
            ExecuteSignal(_onPeerPacketAction, peer_source);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public WebSocketServerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public WebSocketServerAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public WebSocketServerAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private WebSocketServerAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }
    }
}