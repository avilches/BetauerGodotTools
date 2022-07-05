using System;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer.Signal {

    public abstract class SignalHandler : DisposableGodotObject, IObjectLifeCycle {
        public readonly Object Target;
        public readonly string Signal;
        public readonly bool OneShot;
        public readonly bool Deferred;
        private Object? _bound = null;
        private bool _valid = true;
        private string? _targetName;

        protected SignalHandler(Object target, string signal, bool oneShot = false, bool deferred = false) {
            Target = target;
            Signal = signal;
            OneShot = oneShot;
            Deferred = deferred;
            Connect();
            Watch();
        }

        public void Unwatch() => ObjectLifeCycleManager.Singleton.Unwatch(this);

        public void Watch() => ObjectLifeCycleManager.Singleton.Watch(this);

        public SignalHandler Bind(Object o) {
            _bound = o;
            return this;
        }

        public void Connect() {
            if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
            Error err = Target.Connect(Signal, this, nameof(SignalHandlerAction.Call), null, SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
            _targetName = Target is Node node ? node.Name : null;
        }

        protected void AfterCall() {
            if (OneShot) _valid = false;
        }

        public bool IsValid() => IsInstanceValid(Target) && IsInstanceValid(this);

        public bool IsConnected() {
            return IsValid() && Target.IsConnected(Signal, this, nameof(SignalHandlerAction.Call));
        }

        public void Disconnect() {
            if (IsConnected()) Target.Disconnect(Signal, this, nameof(SignalHandlerAction.Call));
        }

        protected override void OnDispose(bool disposing) {
            _valid = false;
            if (disposing) Disconnect();
        }

        public bool MustBeDisposed() {
            return !_valid || !IsValid() || (_bound != null && !IsInstanceValid(_bound));
        }

        public override string ToString() {
            string txt = null;
            if (IsInstanceValid(this)) {
                txt += "Target:" + Target.GetType();                
            } else {
                txt += "Disposed. Target:" + Target.GetType();                
            }
            if (_targetName != null) {
                txt += " \"" + _targetName + "\"";
            }
            if (!IsInstanceValid(Target)) {
                txt += " (Disposed)";
            }
            txt += ". Signal:" + Signal;
            return txt;
        }

        private static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)ConnectFlags.Deferred : 0) +
            0;
    }
    
    public abstract class SignalHandler<T> : SignalHandler {
        protected readonly T Action;

        protected SignalHandler(Object target, string signal, T action, bool oneShot = false, bool deferred = false): base(target, signal, oneShot, deferred) {
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
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
                AfterCall();
            }
        }
    }
}