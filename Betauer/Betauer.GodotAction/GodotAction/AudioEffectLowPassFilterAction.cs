using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AudioEffectLowPassFilterAction : AudioEffectLowPassFilter {


        private Action? _onChangedAction; 
        public AudioEffectLowPassFilterAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AudioEffectLowPassFilterAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AudioEffectLowPassFilterAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AudioEffectLowPassFilterAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}