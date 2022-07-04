using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class StyleBoxTextureAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public StyleBoxTextureAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public StyleBoxTextureAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private StyleBoxTextureAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public StyleBoxTextureAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public StyleBoxTextureAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private StyleBoxTextureAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTextureChangedAction; 
        public StyleBoxTextureAction OnTextureChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTextureChangedAction, "texture_changed", nameof(_GodotSignalTextureChanged), action, oneShot, deferred);
            return this;
        }

        public StyleBoxTextureAction RemoveOnTextureChanged(Action action) {
            RemoveSignal(_onTextureChangedAction, "texture_changed", nameof(_GodotSignalTextureChanged), action);
            return this;
        }

        private StyleBoxTextureAction _GodotSignalTextureChanged() {
            ExecuteSignal(_onTextureChangedAction);
            return this;
        }
    }
}