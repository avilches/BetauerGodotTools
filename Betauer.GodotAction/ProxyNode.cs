using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    public abstract class ProxyNode : Node {

        protected ProxyNode() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        protected void AddSignal<T>(ref List<T>? list, string signal, string methodName, T action, bool oneShot = false, bool deferred = false) {
            if (list == null || list.Count == 0) {
                list ??= new List<T>(); 
                GetParent().Connect(signal, this, methodName);
            }
            list.Add(action);
        }

        protected void RemoveSignal<T>(List<T>? list, string signal, string methodName, T action) {
            if (list == null || list.Count == 0) return;
            list.Remove(action); 
            if (list.Count == 0) {
                GetParent().Disconnect(signal, this, methodName);
            }
        }

        protected void ExecuteSignal(List<Action>? list) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke();
        }

        protected void ExecuteSignal<T1>(List<Action<T1>>? list, T1 p1) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke(p1);
        }

        protected void ExecuteSignal<T1, T2>(List<Action<T1, T2>>? list, T1 p1, T2 p2) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke(p1, p2);
        }

        protected void ExecuteSignal<T1, T2, T3>(List<Action<T1, T2, T3>>? list, T1 p1, T2 p2, T3 p3) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke(p1, p2, p3);
        }

        protected void ExecuteSignal<T1, T2, T3, T4>(List<Action<T1, T2, T3, T4>>? list, T1 p1, T2 p2, T3 p3, T4 p4) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke(p1, p2, p3, p4);
        }

        protected void ExecuteSignal<T1, T2, T3, T4, T5>(List<Action<T1, T2, T3, T4, T5>>? list, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) {
            if (list == null || list.Count == 0) return;
            foreach (var t in list) t.Invoke(p1, p2, p3, p4, p5);
        }
        
        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public void OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
        }
        public void OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
        }

        public void OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
        }

        public void OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
        }

        public void OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
        }

        public void RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
        }

        public void RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
        }

        public void RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
        }

        public void RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
        }

        public void RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions?.Remove(action);
        }

        public override void _Process(float delta) {
            if (_onProcessActions == null || _onProcessActions.Count == 0) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null || _onPhysicsProcessActions.Count == 0) {
                SetPhysicsProcess(false);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null || _onInputActions.Count == 0) {
                SetProcessInput(false);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null || _onUnhandledInputActions.Count == 0) {
                SetProcessUnhandledInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null || _onUnhandledKeyInputActions.Count == 0) {
                SetProcessUnhandledKeyInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

    }
}