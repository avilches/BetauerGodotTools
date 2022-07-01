using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerAPIAction : ProxyNode {

        private List<Action>? _onConnectedToServerAction; 
        public void OnConnectedToServer(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action, oneShot, deferred);

        public void RemoveOnConnectedToServer(Action action) =>
            RemoveSignal(_onConnectedToServerAction, "connected_to_server", nameof(_GodotSignalConnectedToServer), action);

        private void _GodotSignalConnectedToServer() =>
            ExecuteSignal(_onConnectedToServerAction);
        

        private List<Action>? _onConnectionFailedAction; 
        public void OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action, oneShot, deferred);

        public void RemoveOnConnectionFailed(Action action) =>
            RemoveSignal(_onConnectionFailedAction, "connection_failed", nameof(_GodotSignalConnectionFailed), action);

        private void _GodotSignalConnectionFailed() =>
            ExecuteSignal(_onConnectionFailedAction);
        

        private List<Action<int>>? _onNetworkPeerConnectedAction; 
        public void OnNetworkPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action, oneShot, deferred);

        public void RemoveOnNetworkPeerConnected(Action<int> action) =>
            RemoveSignal(_onNetworkPeerConnectedAction, "network_peer_connected", nameof(_GodotSignalNetworkPeerConnected), action);

        private void _GodotSignalNetworkPeerConnected(int id) =>
            ExecuteSignal(_onNetworkPeerConnectedAction, id);
        

        private List<Action<int>>? _onNetworkPeerDisconnectedAction; 
        public void OnNetworkPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action, oneShot, deferred);

        public void RemoveOnNetworkPeerDisconnected(Action<int> action) =>
            RemoveSignal(_onNetworkPeerDisconnectedAction, "network_peer_disconnected", nameof(_GodotSignalNetworkPeerDisconnected), action);

        private void _GodotSignalNetworkPeerDisconnected(int id) =>
            ExecuteSignal(_onNetworkPeerDisconnectedAction, id);
        

        private List<Action<int, byte[]>>? _onNetworkPeerPacketAction; 
        public void OnNetworkPeerPacket(Action<int, byte[]> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNetworkPeerPacketAction, "network_peer_packet", nameof(_GodotSignalNetworkPeerPacket), action, oneShot, deferred);

        public void RemoveOnNetworkPeerPacket(Action<int, byte[]> action) =>
            RemoveSignal(_onNetworkPeerPacketAction, "network_peer_packet", nameof(_GodotSignalNetworkPeerPacket), action);

        private void _GodotSignalNetworkPeerPacket(int id, byte[] packet) =>
            ExecuteSignal(_onNetworkPeerPacketAction, id, packet);
        

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