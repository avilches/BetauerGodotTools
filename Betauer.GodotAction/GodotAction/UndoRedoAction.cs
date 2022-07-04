using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class UndoRedoAction : ProxyNode {

        private List<Action>? _onScriptChangedAction; 
        public UndoRedoAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public UndoRedoAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private UndoRedoAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onVersionChangedAction; 
        public UndoRedoAction OnVersionChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVersionChangedAction, "version_changed", nameof(_GodotSignalVersionChanged), action, oneShot, deferred);
            return this;
        }

        public UndoRedoAction RemoveOnVersionChanged(Action action) {
            RemoveSignal(_onVersionChangedAction, "version_changed", nameof(_GodotSignalVersionChanged), action);
            return this;
        }

        private UndoRedoAction _GodotSignalVersionChanged() {
            ExecuteSignal(_onVersionChangedAction);
            return this;
        }
    }
}