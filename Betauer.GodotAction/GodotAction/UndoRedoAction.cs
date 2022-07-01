using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class UndoRedoAction : ProxyNode {

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onVersionChangedAction; 
        public void OnVersionChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onVersionChangedAction, "version_changed", nameof(_GodotSignalVersionChanged), action, oneShot, deferred);

        public void RemoveOnVersionChanged(Action action) =>
            RemoveSignal(_onVersionChangedAction, "version_changed", nameof(_GodotSignalVersionChanged), action);

        private void _GodotSignalVersionChanged() =>
            ExecuteSignal(_onVersionChangedAction);
        
    }
}