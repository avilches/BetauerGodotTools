using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationAction : Animation {


        private List<Action>? _onChangedAction; 
        public AnimationAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public AnimationAction RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                Disconnect("changed", this, nameof(_GodotSignalChanged));
            }
            return this;
        }
        private void _GodotSignalChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTracksChangedAction; 
        public AnimationAction OnTracksChanged(Action action) {
            if (_onTracksChangedAction == null || _onTracksChangedAction.Count == 0) {
                _onTracksChangedAction ??= new List<Action>(); 
                Connect("tracks_changed", this, nameof(_GodotSignalTracksChanged));
            }
            _onTracksChangedAction.Add(action);
            return this;
        }
        public AnimationAction RemoveOnTracksChanged(Action action) {
            if (_onTracksChangedAction == null || _onTracksChangedAction.Count == 0) return this;
            _onTracksChangedAction.Remove(action); 
            if (_onTracksChangedAction.Count == 0) {
                Disconnect("tracks_changed", this, nameof(_GodotSignalTracksChanged));
            }
            return this;
        }
        private void _GodotSignalTracksChanged() {
            if (_onTracksChangedAction == null || _onTracksChangedAction.Count == 0) return;
            for (var i = 0; i < _onTracksChangedAction.Count; i++) _onTracksChangedAction[i].Invoke();
        }
        
    }
}