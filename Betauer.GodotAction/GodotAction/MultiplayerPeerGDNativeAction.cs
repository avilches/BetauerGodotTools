using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MultiplayerPeerGDNativeAction : MultiplayerPeerGDNative {


        private Action? _onConnectionFailedAction; 
        public MultiplayerPeerGDNativeAction OnConnectionFailed(Action action) {
            if (_onConnectionFailedAction == null) 
                Connect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnConnectionFailed() {
            if (_onConnectionFailedAction == null) return this; 
            Disconnect("connection_failed", this, nameof(ExecuteConnectionFailed));
            _onConnectionFailedAction = null;
            return this;
        }
        private void ExecuteConnectionFailed() =>
            _onConnectionFailedAction?.Invoke();
        

        private Action? _onConnectionSucceededAction; 
        public MultiplayerPeerGDNativeAction OnConnectionSucceeded(Action action) {
            if (_onConnectionSucceededAction == null) 
                Connect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnConnectionSucceeded() {
            if (_onConnectionSucceededAction == null) return this; 
            Disconnect("connection_succeeded", this, nameof(ExecuteConnectionSucceeded));
            _onConnectionSucceededAction = null;
            return this;
        }
        private void ExecuteConnectionSucceeded() =>
            _onConnectionSucceededAction?.Invoke();
        

        private Action<int>? _onPeerConnectedAction; 
        public MultiplayerPeerGDNativeAction OnPeerConnected(Action<int> action) {
            if (_onPeerConnectedAction == null) 
                Connect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnPeerConnected() {
            if (_onPeerConnectedAction == null) return this; 
            Disconnect("peer_connected", this, nameof(ExecutePeerConnected));
            _onPeerConnectedAction = null;
            return this;
        }
        private void ExecutePeerConnected(int id) =>
            _onPeerConnectedAction?.Invoke(id);
        

        private Action<int>? _onPeerDisconnectedAction; 
        public MultiplayerPeerGDNativeAction OnPeerDisconnected(Action<int> action) {
            if (_onPeerDisconnectedAction == null) 
                Connect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnPeerDisconnected() {
            if (_onPeerDisconnectedAction == null) return this; 
            Disconnect("peer_disconnected", this, nameof(ExecutePeerDisconnected));
            _onPeerDisconnectedAction = null;
            return this;
        }
        private void ExecutePeerDisconnected(int id) =>
            _onPeerDisconnectedAction?.Invoke(id);
        

        private Action? _onScriptChangedAction; 
        public MultiplayerPeerGDNativeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onServerDisconnectedAction; 
        public MultiplayerPeerGDNativeAction OnServerDisconnected(Action action) {
            if (_onServerDisconnectedAction == null) 
                Connect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = action;
            return this;
        }
        public MultiplayerPeerGDNativeAction RemoveOnServerDisconnected() {
            if (_onServerDisconnectedAction == null) return this; 
            Disconnect("server_disconnected", this, nameof(ExecuteServerDisconnected));
            _onServerDisconnectedAction = null;
            return this;
        }
        private void ExecuteServerDisconnected() =>
            _onServerDisconnectedAction?.Invoke();
        
    }
}