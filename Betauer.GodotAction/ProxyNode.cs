using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.GodotAction {
    public abstract class ProxyNode : Node {
        protected void AddSignal<T>(ref List<T>? list, string signal, string methodName, T action, bool oneShot = false, bool deferred = false) {
            if (!IsInstanceValid(this)) return;
            if (list == null || list.Count == 0) {
                var parent = GetParent();
                if (parent == null) throw new InvalidOperationException("Can't add signal to a Proxy without parent");
                if (!IsInstanceValid(parent)) return;
                list ??= new List<T>(); 
                parent.Connect(signal, this, methodName);
            }
            list.Add(action);
        }

        protected void RemoveSignal<T>(List<T>? list, string signal, string methodName, T action) {
            if (!IsInstanceValid(this)) return;
            if (list == null || list.Count == 0) return;
            list.Remove(action); 
            if (list.Count == 0) {
                var parent = GetParent();
                if (parent == null) throw new InvalidOperationException("Can't remove a signal from a Proxy without parent");
                if (!IsInstanceValid(parent)) return;
                parent.Disconnect(signal, this, methodName);
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
        
        private event Action<float>? OnProcessActions; 
        private event Action<float>? OnPhysicsProcessActions; 
        private event Action<InputEvent>? OnInputActions; 
        private event Action<InputEvent>? OnUnhandledInputActions; 
        private event Action<InputEventKey>? OnUnhandledKeyInputActions;

        public void OnProcess(Action<float> action) {
            OnProcessActions += action;
            SetProcess(true);
        }
        
        public void OnPhysicsProcess(Action<float> action) {
            OnPhysicsProcessActions += action;
            SetPhysicsProcess(true);
        }

        public void OnInput(Action<InputEvent> action) {
            OnInputActions += action;
            SetProcessInput(true);
        }

        public void OnUnhandledInput(Action<InputEvent> action) {
            OnUnhandledInputActions += action;
            SetProcessUnhandledInput(true);
        }

        public void OnUnhandledKeyInput(Action<InputEventKey> action) {
            OnUnhandledKeyInputActions += action;
            SetProcessUnhandledKeyInput(true);
        }

        public void RemoveOnProcess(Action<float> action) {
            OnProcessActions -= action;
        }

        public void RemoveOnPhysicsProcess(Action<float> action) {
            OnPhysicsProcessActions -= action;
        }

        public void RemoveOnInput(Action<InputEvent> action) {
            OnInputActions -= action;
        }

        public void RemoveOnUnhandledInput(Action<InputEvent> action) {
            OnUnhandledInputActions -= action;
        }

        public void RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            OnUnhandledKeyInputActions -= action;
        }

        public override void _Process(float delta) {
            if (OnProcessActions == null) {
                SetProcess(false);
                return;
            }
            OnProcessActions?.Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (OnPhysicsProcessActions == null) {
                SetPhysicsProcess(false);
                return;
            }
            OnPhysicsProcessActions?.Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (OnInputActions == null) {
                SetProcessInput(false);
                return;
            }
            OnInputActions?.Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (OnUnhandledInputActions == null) {
                SetProcessUnhandledInput(false);
                return;
            }
            OnUnhandledInputActions?.Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (OnUnhandledKeyInputActions == null) {
                SetProcessUnhandledKeyInput(false);
                return;
            }
            OnUnhandledKeyInputActions?.Invoke(@event);
        }

    }
}