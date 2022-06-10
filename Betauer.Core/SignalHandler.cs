using System;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    public static partial class SignalExtensions {
        public static SignalAwaiter AwaitPhysicsFrame(this Node node) {
            return AwaitPhysicsFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitIdleFrame(this Node node) {
            return AwaitIdleFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitPhysicsFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, SceneTree_PhysicsFrameSignal);
        }

        public static SignalAwaiter AwaitIdleFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, SceneTree_IdleFrameSignal);
        }
    }

    // TODO: this is working, but some units tests will be appreciated
    public abstract class BaseSignalHandler<T> : GodotObject {
        private readonly Object _target;
        private readonly string _signal;
        protected readonly T Action;

        public BaseSignalHandler(Object target, string signal, T action) {
            _target = target;
            _signal = signal;
            Action = action;
            Connect();
        }
        
        public void Connect() {
            if (!IsInstanceValid(_target)) {
                throw new Exception($"Can't connect '{_signal}' to a freed object");
            }
            Error err = _target.Connect(_signal, this, nameof(SignalHandler.Call));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{_signal}' to ${_target} failed: '{err}'");
            }
        }

        public bool IsConnected() {
            return IsInstanceValid(_target) && _target.IsConnected(_signal, this, nameof(SignalHandler.Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(SignalHandler.Call));
            }
        }
    }

    public class SignalHandler : BaseSignalHandler<Action> {
        public SignalHandler(Object target, string signal, Action action) : base(target, signal, action) {
        }

        public void Call() {
            Action();
        }
    }

    public class SignalHandler<T1> : BaseSignalHandler<Action<T1>> {
        public SignalHandler(Object target, string signal, Action<T1> action) : base(target, signal, action) {
        }

        public void Call(T1 v1) {
            Action(v1);
        }
    }

    public class SignalHandler<T1, T2> : BaseSignalHandler<Action<T1, T2>> {
        public SignalHandler(Object target, string signal, Action<T1, T2> action) : base(target, signal, action) {
        }

        public void Call(T1 v1, T2 v2) {
            Action(v1, v2);
        }
    }

    public class SignalHandler<T1, T2, T3> : BaseSignalHandler<Action<T1, T2, T3>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3> action) : base(target, signal, action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3) {
            Action(v1, v2, v3);
        }
    }

    public class SignalHandler<T1, T2, T3, T4> : BaseSignalHandler<Action<T1, T2, T3, T4>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4> action) :
            base(target, signal, action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4) {
            Action(v1, v2, v3, v4);
        }
    }

    public class SignalHandler<T1, T2, T3, T4, T5> : BaseSignalHandler<Action<T1, T2, T3, T4, T5>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5> action) : base(target, signal,
            action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            Action(v1, v2, v3, v4, v5);
        }
    }

    public class SignalHandler<T1, T2, T3, T4, T5, T6> : BaseSignalHandler<Action<T1, T2, T3, T4, T5, T6>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5, T6> action) : base(target, signal,
            action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            Action(v1, v2, v3, v4, v5, v6);
        }
    }

    public class SignalHandler<T1, T2, T3, T4, T5, T6, T7> : BaseSignalHandler<Action<T1, T2, T3, T4, T5, T6, T7>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action) : base(target,
            signal, action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            Action(v1, v2, v3, v4, v5, v6, v7);
        }
    }

    public class
        SignalHandler<T1, T2, T3, T4, T5, T6, T7, T8> : BaseSignalHandler<Action<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action) : base(target,
            signal, action) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
            Action(v1, v2, v3, v4, v5, v6, v7, v8);
        }
    }
}