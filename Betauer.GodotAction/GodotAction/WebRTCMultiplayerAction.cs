using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebRTCMultiplayerAction : Node {
        public WebRTCMultiplayerAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action>? _onConnectionFailedAction; 
        public WebRTCMultiplayerAction OnConnectionFailed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) {
                _onConnectionFailedAction ??= new List<Action>(); 
                GetParent().Connect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            _onConnectionFailedAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return this;
            _onConnectionFailedAction.Remove(action); 
            if (_onConnectionFailedAction.Count == 0) {
                GetParent().Disconnect("connection_failed", this, nameof(_GodotSignalConnectionFailed));
            }
            return this;
        }
        private void _GodotSignalConnectionFailed() {
            if (_onConnectionFailedAction == null || _onConnectionFailedAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFailedAction.Count; i++) _onConnectionFailedAction[i].Invoke();
        }
        

        private List<Action>? _onConnectionSucceededAction; 
        public WebRTCMultiplayerAction OnConnectionSucceeded(Action action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) {
                _onConnectionSucceededAction ??= new List<Action>(); 
                GetParent().Connect("connection_succeeded", this, nameof(_GodotSignalConnectionSucceeded));
            }
            _onConnectionSucceededAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return this;
            _onConnectionSucceededAction.Remove(action); 
            if (_onConnectionSucceededAction.Count == 0) {
                GetParent().Disconnect("connection_succeeded", this, nameof(_GodotSignalConnectionSucceeded));
            }
            return this;
        }
        private void _GodotSignalConnectionSucceeded() {
            if (_onConnectionSucceededAction == null || _onConnectionSucceededAction.Count == 0) return;
            for (var i = 0; i < _onConnectionSucceededAction.Count; i++) _onConnectionSucceededAction[i].Invoke();
        }
        

        private List<Action<int>>? _onPeerConnectedAction; 
        public WebRTCMultiplayerAction OnPeerConnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) {
                _onPeerConnectedAction ??= new List<Action<int>>(); 
                GetParent().Connect("peer_connected", this, nameof(_GodotSignalPeerConnected));
            }
            _onPeerConnectedAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return this;
            _onPeerConnectedAction.Remove(action); 
            if (_onPeerConnectedAction.Count == 0) {
                GetParent().Disconnect("peer_connected", this, nameof(_GodotSignalPeerConnected));
            }
            return this;
        }
        private void _GodotSignalPeerConnected(int id) {
            if (_onPeerConnectedAction == null || _onPeerConnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerConnectedAction.Count; i++) _onPeerConnectedAction[i].Invoke(id);
        }
        

        private List<Action<int>>? _onPeerDisconnectedAction; 
        public WebRTCMultiplayerAction OnPeerDisconnected(Action<int> action, bool oneShot = false, bool deferred = false) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) {
                _onPeerDisconnectedAction ??= new List<Action<int>>(); 
                GetParent().Connect("peer_disconnected", this, nameof(_GodotSignalPeerDisconnected));
            }
            _onPeerDisconnectedAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return this;
            _onPeerDisconnectedAction.Remove(action); 
            if (_onPeerDisconnectedAction.Count == 0) {
                GetParent().Disconnect("peer_disconnected", this, nameof(_GodotSignalPeerDisconnected));
            }
            return this;
        }
        private void _GodotSignalPeerDisconnected(int id) {
            if (_onPeerDisconnectedAction == null || _onPeerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onPeerDisconnectedAction.Count; i++) _onPeerDisconnectedAction[i].Invoke(id);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public WebRTCMultiplayerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onServerDisconnectedAction; 
        public WebRTCMultiplayerAction OnServerDisconnected(Action action, bool oneShot = false, bool deferred = false) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) {
                _onServerDisconnectedAction ??= new List<Action>(); 
                GetParent().Connect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            _onServerDisconnectedAction.Add(action);
            return this;
        }
        public WebRTCMultiplayerAction RemoveOnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return this;
            _onServerDisconnectedAction.Remove(action); 
            if (_onServerDisconnectedAction.Count == 0) {
                GetParent().Disconnect("server_disconnected", this, nameof(_GodotSignalServerDisconnected));
            }
            return this;
        }
        private void _GodotSignalServerDisconnected() {
            if (_onServerDisconnectedAction == null || _onServerDisconnectedAction.Count == 0) return;
            for (var i = 0; i < _onServerDisconnectedAction.Count; i++) _onServerDisconnectedAction[i].Invoke();
        }
        
    }
}