using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class StyleBoxTextureAction : StyleBoxTexture {


        private Action? _onChangedAction; 
        public StyleBoxTextureAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public StyleBoxTextureAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public StyleBoxTextureAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public StyleBoxTextureAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTextureChangedAction; 
        public StyleBoxTextureAction OnTextureChanged(Action action) {
            if (_onTextureChangedAction == null) 
                Connect("texture_changed", this, nameof(ExecuteTextureChanged));
            _onTextureChangedAction = action;
            return this;
        }
        public StyleBoxTextureAction RemoveOnTextureChanged() {
            if (_onTextureChangedAction == null) return this; 
            Disconnect("texture_changed", this, nameof(ExecuteTextureChanged));
            _onTextureChangedAction = null;
            return this;
        }
        private void ExecuteTextureChanged() =>
            _onTextureChangedAction?.Invoke();
        
    }
}