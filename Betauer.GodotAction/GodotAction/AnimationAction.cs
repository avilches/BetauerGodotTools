using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationAction : Animation {


        private Action? _onChangedAction; 
        public AnimationAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AnimationAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTracksChangedAction; 
        public AnimationAction OnTracksChanged(Action action) {
            if (_onTracksChangedAction == null) 
                Connect("tracks_changed", this, nameof(ExecuteTracksChanged));
            _onTracksChangedAction = action;
            return this;
        }
        public AnimationAction RemoveOnTracksChanged() {
            if (_onTracksChangedAction == null) return this; 
            Disconnect("tracks_changed", this, nameof(ExecuteTracksChanged));
            _onTracksChangedAction = null;
            return this;
        }
        private void ExecuteTracksChanged() =>
            _onTracksChangedAction?.Invoke();
        
    }
}