using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerPeerGDNativeAction : ProxyNode {

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