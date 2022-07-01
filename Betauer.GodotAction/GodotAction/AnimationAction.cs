using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public void OnChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);

        public void RemoveOnChanged(Action action) =>
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);

        private void _GodotSignalChanged() =>
            ExecuteSignal(_onChangedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onTracksChangedAction; 
        public void OnTracksChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTracksChangedAction, "tracks_changed", nameof(_GodotSignalTracksChanged), action, oneShot, deferred);

        public void RemoveOnTracksChanged(Action action) =>
            RemoveSignal(_onTracksChangedAction, "tracks_changed", nameof(_GodotSignalTracksChanged), action);

        private void _GodotSignalTracksChanged() =>
            ExecuteSignal(_onTracksChangedAction);
        
    }
}