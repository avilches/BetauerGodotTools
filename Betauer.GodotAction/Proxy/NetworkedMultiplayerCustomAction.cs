using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class NetworkedMultiplayerCustomAction : ProxyNode {

        private List<Action>? _onConnectionFailedAction; 
        public NetworkedMultiplayerCustomAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action>? _onConnectionSucceededAction; 
        public NetworkedMultiplayerCustomAction OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnConnectionSucceeded(Action action) {
            RemoveSignal(_onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalConnectionSucceeded() {
            ExecuteSignal(_onConnectionSucceededAction);
            return this;
        }

        private List<Action<byte[], int, int>>? _onPacketGeneratedAction; 
        public NetworkedMultiplayerCustomAction OnPacketGenerated(Action<byte[], int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPacketGeneratedAction, "packet_generated", nameof(_GodotSignalPacketGenerated), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnPacketGenerated(Action<byte[], int, int> action) {
            RemoveSignal(_onPacketGeneratedAction, "packet_generated", nameof(_GodotSignalPacketGenerated), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalPacketGenerated(byte[] buffer, int peer_id, int transfer_mode) {
            ExecuteSignal(_onPacketGeneratedAction, buffer, peer_id, transfer_mode);
            return this;
        }

        private List<Action<int>>? _onPeerConnectedAction; 
        public NetworkedMultiplayerCustomAction OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnPeerConnected(Action<int> action) {
            RemoveSignal(_onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalPeerConnected(int id) {
            ExecuteSignal(_onPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public NetworkedMultiplayerCustomAction OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnPeerDisconnected(Action<int> action) {
            RemoveSignal(_onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalPeerDisconnected(int id) {
            ExecuteSignal(_onPeerDisconnectedAction, id);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public NetworkedMultiplayerCustomAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public NetworkedMultiplayerCustomAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerCustomAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private NetworkedMultiplayerCustomAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }
    }
}