using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebRTCPeerConnectionGDNativeAction : WebRTCPeerConnectionGDNative {


        private Action<Object>? _onDataChannelReceivedAction; 
        public WebRTCPeerConnectionGDNativeAction OnDataChannelReceived(Action<Object> action) {
            if (_onDataChannelReceivedAction == null) 
                Connect("data_channel_received", this, nameof(ExecuteDataChannelReceived));
            _onDataChannelReceivedAction = action;
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnDataChannelReceived() {
            if (_onDataChannelReceivedAction == null) return this; 
            Disconnect("data_channel_received", this, nameof(ExecuteDataChannelReceived));
            _onDataChannelReceivedAction = null;
            return this;
        }
        private void ExecuteDataChannelReceived(Object channel) =>
            _onDataChannelReceivedAction?.Invoke(channel);
        

        private Action<int, string, string>? _onIceCandidateCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnIceCandidateCreated(Action<int, string, string> action) {
            if (_onIceCandidateCreatedAction == null) 
                Connect("ice_candidate_created", this, nameof(ExecuteIceCandidateCreated));
            _onIceCandidateCreatedAction = action;
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnIceCandidateCreated() {
            if (_onIceCandidateCreatedAction == null) return this; 
            Disconnect("ice_candidate_created", this, nameof(ExecuteIceCandidateCreated));
            _onIceCandidateCreatedAction = null;
            return this;
        }
        private void ExecuteIceCandidateCreated(int index, string media, string name) =>
            _onIceCandidateCreatedAction?.Invoke(index, media, name);
        

        private Action? _onScriptChangedAction; 
        public WebRTCPeerConnectionGDNativeAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action<string, string>? _onSessionDescriptionCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnSessionDescriptionCreated(Action<string, string> action) {
            if (_onSessionDescriptionCreatedAction == null) 
                Connect("session_description_created", this, nameof(ExecuteSessionDescriptionCreated));
            _onSessionDescriptionCreatedAction = action;
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnSessionDescriptionCreated() {
            if (_onSessionDescriptionCreatedAction == null) return this; 
            Disconnect("session_description_created", this, nameof(ExecuteSessionDescriptionCreated));
            _onSessionDescriptionCreatedAction = null;
            return this;
        }
        private void ExecuteSessionDescriptionCreated(string sdp, string type) =>
            _onSessionDescriptionCreatedAction?.Invoke(sdp, type);
        
    }
}