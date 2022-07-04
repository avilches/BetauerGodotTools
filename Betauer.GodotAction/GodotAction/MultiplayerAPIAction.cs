using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerAPIAction : ProxyNode {

        private List<Action>? _onConnectedToServerAction; 
        public MultiplayerAPIAction OnConnectedToServer(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnConnectedToServer(Action action) {
            RemoveSignal(_onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalConnectedToServer() {
            ExecuteSignal(_onConnectedToServerAction);
            return this;
        }

        private List<Action>? _onConnectionFailedAction; 
        public MultiplayerAPIAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnNetworkPeerConnected(Action<int> action) {
            RemoveSignal(_onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalNetworkPeerConnected(int id) {
            ExecuteSignal(_onNetworkPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public MultiplayerAPIAction OnNetworkPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnNetworkPeerDisconnected(Action<int> action) {
            RemoveSignal(_onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalNetworkPeerDisconnected(int id) {
            ExecuteSignal(_onNetworkPeerDisconnectedAction, id);
            return this;
        }

        private List<Action<int, byte[]>>? _onNetworkPeerPacketAction; 
        public MultiplayerAPIAction OnNetworkPeerPacket(Action<int, byte[]> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNetworkPeerPacketAction, "network_peer_packet", nameof(_GodotSignalNetworkPeerPacket), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnNetworkPeerPacket(Action<int, byte[]> action) {
            RemoveSignal(_onNetworkPeerPacketAction, "network_peer_packet", nameof(_GodotSignalNetworkPeerPacket), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalNetworkPeerPacket(int id, byte[] packet) {
            ExecuteSignal(_onNetworkPeerPacketAction, id, packet);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public MultiplayerAPIAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public MultiplayerAPIAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public MultiplayerAPIAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private MultiplayerAPIAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }
    }
}