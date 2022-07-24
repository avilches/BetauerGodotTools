using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class WebRTCPeerConnectionGDNativeAction : ProxyNode {

        private List<Action<Object>>? _onDataChannelReceivedAction; 
        public WebRTCPeerConnectionGDNativeAction OnDataChannelReceived(Action<Object> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDataChannelReceivedAction, "data_channel_received", nameof(_GodotSignalDataChannelReceived), action, oneShot, deferred);
            return this;
        }

        public WebRTCPeerConnectionGDNativeAction RemoveOnDataChannelReceived(Action<Object> action) {
            RemoveSignal(_onDataChannelReceivedAction, "data_channel_received", nameof(_GodotSignalDataChannelReceived), action);
            return this;
        }

        private WebRTCPeerConnectionGDNativeAction _GodotSignalDataChannelReceived(Object channel) {
            ExecuteSignal(_onDataChannelReceivedAction, channel);
            return this;
        }

        private List<Action<int, string, string>>? _onIceCandidateCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnIceCandidateCreated(Action<int, string, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onIceCandidateCreatedAction, "ice_candidate_created", nameof(_GodotSignalIceCandidateCreated), action, oneShot, deferred);
            return this;
        }

        public WebRTCPeerConnectionGDNativeAction RemoveOnIceCandidateCreated(Action<int, string, string> action) {
            RemoveSignal(_onIceCandidateCreatedAction, "ice_candidate_created", nameof(_GodotSignalIceCandidateCreated), action);
            return this;
        }

        private WebRTCPeerConnectionGDNativeAction _GodotSignalIceCandidateCreated(int index, string media, string name) {
            ExecuteSignal(_onIceCandidateCreatedAction, index, media, name);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public WebRTCPeerConnectionGDNativeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public WebRTCPeerConnectionGDNativeAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private WebRTCPeerConnectionGDNativeAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action<string, string>>? _onSessionDescriptionCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnSessionDescriptionCreated(Action<string, string> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSessionDescriptionCreatedAction, "session_description_created", nameof(_GodotSignalSessionDescriptionCreated), action, oneShot, deferred);
            return this;
        }

        public WebRTCPeerConnectionGDNativeAction RemoveOnSessionDescriptionCreated(Action<string, string> action) {
            RemoveSignal(_onSessionDescriptionCreatedAction, "session_description_created", nameof(_GodotSignalSessionDescriptionCreated), action);
            return this;
        }

        private WebRTCPeerConnectionGDNativeAction _GodotSignalSessionDescriptionCreated(string sdp, string type) {
            ExecuteSignal(_onSessionDescriptionCreatedAction, sdp, type);
            return this;
        }
    }
}