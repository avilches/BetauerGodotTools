using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class NetworkedMultiplayerENetAction : NetworkedMultiplayerENet {


        private List<Action>? _onConnectionFailedAction; 
        public NetworkedMultiplayerENetAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return this;
            _onConnectionFailedAction.Remove(action); 
            if (_onConnectionFailedAction.Count == 0) {
                Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            }
            return this;
        }
        private void ExecuteConnectionFailed() {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFailedAction.Count; i++) _onConnectionFailedAction[i].Invoke();
        }
        

        private List<Action>? _onConnectionSucceededAction; 
        public NetworkedMultiplayerENetAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) {
                _onConnectionSucceededAction ??= new List<Action>(); 
                Connect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            }
            _onConnectionSucceededAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return this;
            _onConnectionSucceededAction.Remove(action); 
            if (_onConnectionSucceededAction.Count == 0) {
                Disconnect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            }
            return this;
        }
        private void ExecuteConnectionSucceeded() {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return;
            for (var i = 0; i < _onConnectionSucceededAction.Count; i++) _onConnectionSucceededAction[i].Invoke();
        }
        

        private List<Action<int>>? _onPeerConnectedAction; 
        public NetworkedMultiplayerENetAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) {
                _onPeerConnectedAction ??= new List<Action<int>>(); 
                Connect("peer_connected", this, nameof(ExecutePeerConnected));
            }
            _onPeerConnectedAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return this;
            _onPeerConnectedAction.Remove(action); 
            if (_onPeerConnectedAction.Count == 0) {
                Disconnect("peer_connected", this, nameof(ExecutePeerConnected));
            }
            return this;
        }
        private void ExecutePeerConnected(int id) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerConnectedAction.Count; i++) _onPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public NetworkedMultiplayerENetAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) {
                _onPeerDisconnectedAction ??= new List<Action<int>>(); 
                Connect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            }
            _onPeerDisconnectedAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return this;
            _onPeerDisconnectedAction.Remove(action); 
            if (_onPeerDisconnectedAction.Count == 0) {
                Disconnect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            }
            return this;
        }
        private void ExecutePeerDisconnected(int id) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerDisconnectedAction.Count; i++) _onPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public NetworkedMultiplayerENetAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public NetworkedMultiplayerENetAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public NetworkedMultiplayerENetAction RemoveOnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return this;
            _onServerDisconnectedAction.Remove(action); 
            if (_onServerDisconnectedAction.Count == 0) {
                Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            }
            return this;
        }
        private void ExecuteServerDisconnected() {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onServerDisconnectedAction.Count; i++) _onServerDisconnectedAction[i].Invoke();
        }
        
    }
}