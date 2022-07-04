using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public AnimationAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTracksChangedAction; 
        public AnimationAction OnTracksChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTracksChangedAction, "tracks_changed", nameof(_GodotSignalTracksChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationAction RemoveOnTracksChanged(Action action) {
            RemoveSignal(_onTracksChangedAction, "tracks_changed", nameof(_GodotSignalTracksChanged), action);
            return this;
        }

        private AnimationAction _GodotSignalTracksChanged() {
            ExecuteSignal(_onTracksChangedAction);
            return this;
        }
    }
}