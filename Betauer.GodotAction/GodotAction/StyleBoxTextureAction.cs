using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class StyleBoxTextureAction : StyleBoxTexture {


        private List<Action>? _onChangedAction; 
        public StyleBoxTextureAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(ExecuteChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public StyleBoxTextureAction RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                Disconnect("changed", this, nameof(ExecuteChanged));
            }
            return this;
        }
        private void ExecuteChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public StyleBoxTextureAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public StyleBoxTextureAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTextureChangedAction; 
        public StyleBoxTextureAction OnTextureChanged(Action action) {
            if (_onTextureChangedAction == null || _onTextureChangedAction.Count == 0) {
                _onTextureChangedAction ??= new List<Action>(); 
                Connect("texture_changed", this, nameof(ExecuteTextureChanged));
            }
            _onTextureChangedAction.Add(action);
            return this;
        }
        public StyleBoxTextureAction RemoveOnTextureChanged(Action action) {
            if (_onTextureChangedAction == null || _onTextureChangedAction.Count == 0) return this;
            _onTextureChangedAction.Remove(action); 
            if (_onTextureChangedAction.Count == 0) {
                Disconnect("texture_changed", this, nameof(ExecuteTextureChanged));
            }
            return this;
        }
        private void ExecuteTextureChanged() {
            if (_onTextureChangedAction == null || _onTextureChangedAction.Count == 0) return;
            for (var i = 0; i < _onTextureChangedAction.Count; i++) _onTextureChangedAction[i].Invoke();
        }
        
    }
}