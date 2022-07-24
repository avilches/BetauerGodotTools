using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class NetworkedMultiplayerENetAction : ProxyNode {

        private List<Action>? _onConnectionFailedAction; 
        public NetworkedMultiplayerENetAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnConnectionFailed(Action action) {
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalConnectionFailed() {
            ExecuteSignal(_onConnectionFailedAction);
            return this;
        }

        private List<Action>? _onConnectionSucceededAction; 
        public NetworkedMultiplayerENetAction OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnConnectionSucceeded(Action action) {
            RemoveSignal(_onConnectionSucceededAction, "connection_succeeded", nameof(_GodotSignalConnectionSucceeded), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalConnectionSucceeded() {
            ExecuteSignal(_onConnectionSucceededAction);
            return this;
        }

        private List<Action<int>>? _onPeerConnectedAction; 
        public NetworkedMultiplayerENetAction OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnPeerConnected(Action<int> action) {
            RemoveSignal(_onPeerConnectedAction, "peer_connected", nameof(_GodotSignalPeerConnected), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalPeerConnected(int id) {
            ExecuteSignal(_onPeerConnectedAction, id);
            return this;
        }

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public NetworkedMultiplayerENetAction OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnPeerDisconnected(Action<int> action) {
            RemoveSignal(_onPeerDisconnectedAction, "peer_disconnected", nameof(_GodotSignalPeerDisconnected), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalPeerDisconnected(int id) {
            ExecuteSignal(_onPeerDisconnectedAction, id);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public NetworkedMultiplayerENetAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onServerDisconnectedAction; 
        public NetworkedMultiplayerENetAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action, oneShot, deferred);
            return this;
        }

        public NetworkedMultiplayerENetAction RemoveOnServerDisconnected(Action action) {
            RemoveSignal(_onServerDisconnectedAction, "server_disconnected", nameof(_GodotSignalServerDisconnected), action);
            return this;
        }

        private NetworkedMultiplayerENetAction _GodotSignalServerDisconnected() {
            ExecuteSignal(_onServerDisconnectedAction);
            return this;
        }
    }
}