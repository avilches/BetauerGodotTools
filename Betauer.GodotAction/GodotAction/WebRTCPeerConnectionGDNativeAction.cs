using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebRTCPeerConnectionGDNativeAction : ProxyNode {

        private List<Action<Object>>? _onDataChannelReceivedAction; 
        public void OnDataChannelReceived(Action<Object> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDataChannelReceivedAction, "data_channel_received", nameof(_GodotSignalDataChannelReceived), action, oneShot, deferred);

        public void RemoveOnDataChannelReceived(Action<Object> action) =>
            RemoveSignal(_onDataChannelReceivedAction, "data_channel_received", nameof(_GodotSignalDataChannelReceived), action);

        private void _GodotSignalDataChannelReceived(Object channel) =>
            ExecuteSignal(_onDataChannelReceivedAction, channel);
        

        private List<Action<int, string, string>>? _onIceCandidateCreatedAction; 
        public void OnIceCandidateCreated(Action<int, string, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onIceCandidateCreatedAction, "ice_candidate_created", nameof(_GodotSignalIceCandidateCreated), action, oneShot, deferred);

        public void RemoveOnIceCandidateCreated(Action<int, string, string> action) =>
            RemoveSignal(_onIceCandidateCreatedAction, "ice_candidate_created", nameof(_GodotSignalIceCandidateCreated), action);

        private void _GodotSignalIceCandidateCreated(int index, string media, string name) =>
            ExecuteSignal(_onIceCandidateCreatedAction, index, media, name);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action<string, string>>? _onSessionDescriptionCreatedAction; 
        public void OnSessionDescriptionCreated(Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onSessionDescriptionCreatedAction, "session_description_created", nameof(_GodotSignalSessionDescriptionCreated), action, oneShot, deferred);

        public void RemoveOnSessionDescriptionCreated(Action<string, string> action) =>
            RemoveSignal(_onSessionDescriptionCreatedAction, "session_description_created", nameof(_GodotSignalSessionDescriptionCreated), action);

        private void _GodotSignalSessionDescriptionCreated(string sdp, string type) =>
            ExecuteSignal(_onSessionDescriptionCreatedAction, sdp, type);
        
    }
}