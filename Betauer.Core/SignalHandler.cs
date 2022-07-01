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
            return sceneTree.ToSignal(sceneTree, SignalConstants.SceneTree_PhysicsFrameSignal);
        }

        public static SignalAwaiter AwaitIdleFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, SignalConstants.SceneTree_IdleFrameSignal);
        }
    }

    public abstract class SignalHandler : GodotObject, IObjectLifeCycle {
        public readonly Object Target;
        public readonly string Signal;
        public readonly bool OneShot;
        public readonly bool Deferred;
        public int Calls { get; private set; } = 0;

        public SignalHandler(Object target, string signal, bool oneShot = false, bool deferred = false) {
            Target = target;
            Signal = signal;
            OneShot = oneShot;
            Deferred = deferred;
            Connect();
            ObjectLifeCycleManager.Singleton.Add(this);
        }
        
        public void Connect() {
            if (!IsValid()) {
                throw new Exception($"Can't connect '{Signal}' to a freed object");
            }
            Error err = Target.Connect(Signal, this, nameof(SignalHandlerAction.Call), null, SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
        }

        protected void Consumed() {
            Calls++;
            if (OneShot) ObjectLifeCycleManager.Singleton.Free(this);
        }
        
        private static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)ConnectFlags.Deferred : 0) +
            0;


        public bool IsValid() => IsInstanceValid(this) && IsInstanceValid(Target);

        public void Destroy() {
            Disconnect();
            Free();
        }

        public bool IsConnected() {
            return IsValid() && Target.IsConnected(Signal, this, nameof(SignalHandlerAction.Call));
        }

        public void Disconnect() {
            if (IsConnected()) Target.Disconnect(Signal, this, nameof(SignalHandlerAction.Call));
        }
    }
    
    public abstract class SignalHandler<T> : SignalHandler {
        protected readonly T Action;
        public SignalHandler(Object target, string signal, T action, bool oneShot = false, bool deferred = false): base(target, signal, oneShot, deferred) {
            Action = action;
        }
    }

    public class SignalHandlerAction : SignalHandler<Action> {
        public SignalHandlerAction(Object target, string signal, Action action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call() {
            try {
                Action();
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1> : SignalHandler<Action<T1>> {
        public SignalHandlerAction(Object target, string signal, Action<T1> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1) {
            try {
                Action(v1);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2> : SignalHandler<Action<T1, T2>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2) {
            try {
                Action(v1, v2);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3> : SignalHandler<Action<T1, T2, T3>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3) {
            try {
                Action(v1, v2, v3);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4> : SignalHandler<Action<T1, T2, T3, T4>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4) {
            try {
                Action(v1, v2, v3, v4);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5> : SignalHandler<Action<T1, T2, T3, T4, T5>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            try {
                Action(v1, v2, v3, v4, v5);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5, T6> : SignalHandler<Action<T1, T2, T3, T4, T5, T6>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            try {
                Action(v1, v2, v3, v4, v5, v6);
            } finally {
                Consumed();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5, T6, T7> : SignalHandler<Action<T1, T2, T3, T4, T5, T6, T7>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7);
            } finally {
                Consumed();
            }
        }
    }

    public class
        SignalHandler<T1, T2, T3, T4, T5, T6, T7, T8> : SignalHandler<Action<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7, v8);
            } finally {
                Consumed();
            }
        }
    }
}