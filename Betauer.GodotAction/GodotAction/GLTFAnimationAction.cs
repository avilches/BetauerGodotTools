using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class GLTFAnimationAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public GLTFAnimationAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public GLTFAnimationAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private GLTFAnimationAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public GLTFAnimationAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public GLTFAnimationAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private GLTFAnimationAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}