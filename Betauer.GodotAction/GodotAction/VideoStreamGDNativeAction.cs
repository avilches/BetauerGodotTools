using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VideoStreamGDNativeAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VideoStreamGDNativeAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VideoStreamGDNativeAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VideoStreamGDNativeAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VideoStreamGDNativeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VideoStreamGDNativeAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VideoStreamGDNativeAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}