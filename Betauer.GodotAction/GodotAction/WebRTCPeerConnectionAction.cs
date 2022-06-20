using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebRTCPeerConnectionAction : WebRTCPeerConnection {


        private List<Action<Object>>? _onDataChannelReceivedAction; 
        public WebRTCPeerConnectionAction OnDataChannelReceived(Action<Object> action) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) {
                _onDataChannelReceivedAction ??= new List<Action<Object>>(); 
                Connect("data_channel_received", this, nameof(ExecuteDataChannelReceived));
            }
            _onDataChannelReceivedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionAction RemoveOnDataChannelReceived(Action<Object> action) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) return this;
            _onDataChannelReceivedAction.Remove(action); 
            if (_onDataChannelReceivedAction.Count == 0) {
                Disconnect("data_channel_received", this, nameof(ExecuteDataChannelReceived));
            }
            return this;
        }
        private void ExecuteDataChannelReceived(Object channel) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) return;
            for (var i = 0; i < _onDataChannelReceivedAction.Count; i++) _onDataChannelReceivedAction[i].Invoke(channel);
        }
        

        private List<Action<int, string, string>>? _onIceCandidateCreatedAction; 
        public WebRTCPeerConnectionAction OnIceCandidateCreated(Action<int, string, string> action) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) {
                _onIceCandidateCreatedAction ??= new List<Action<int, string, string>>(); 
                Connect("ice_candidate_created", this, nameof(ExecuteIceCandidateCreated));
            }
            _onIceCandidateCreatedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionAction RemoveOnIceCandidateCreated(Action<int, string, string> action) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) return this;
            _onIceCandidateCreatedAction.Remove(action); 
            if (_onIceCandidateCreatedAction.Count == 0) {
                Disconnect("ice_candidate_created", this, nameof(ExecuteIceCandidateCreated));
            }
            return this;
        }
        private void ExecuteIceCandidateCreated(int index, string media, string name) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) return;
            for (var i = 0; i < _onIceCandidateCreatedAction.Count; i++) _onIceCandidateCreatedAction[i].Invoke(index, media, name);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public WebRTCPeerConnectionAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionAction RemoveOnScriptChanged(Action action) {
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
        

        private List<Action<string, string>>? _onSessionDescriptionCreatedAction; 
        public WebRTCPeerConnectionAction OnSessionDescriptionCreated(Action<string, string> action) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) {
                _onSessionDescriptionCreatedAction ??= new List<Action<string, string>>(); 
                Connect("session_description_created", this, nameof(ExecuteSessionDescriptionCreated));
            }
            _onSessionDescriptionCreatedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionAction RemoveOnSessionDescriptionCreated(Action<string, string> action) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) return this;
            _onSessionDescriptionCreatedAction.Remove(action); 
            if (_onSessionDescriptionCreatedAction.Count == 0) {
                Disconnect("session_description_created", this, nameof(ExecuteSessionDescriptionCreated));
            }
            return this;
        }
        private void ExecuteSessionDescriptionCreated(string sdp, string type) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) return;
            for (var i = 0; i < _onSessionDescriptionCreatedAction.Count; i++) _onSessionDescriptionCreatedAction[i].Invoke(sdp, type);
        }
        
    }
}