using System;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer.Signal {

    public interface ISignalHandler {
        public bool IsValid();
        public void Connect();
        public bool IsConnected();
        public void Disconnect();
    }

    public static class SignalTools {
        public static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)Object.ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)Object.ConnectFlags.Deferred : 0) +
            0;
    }

    public class SignalHandler : ISignalHandler {
        public readonly Object Origin;
        public readonly Object Target;
        public readonly string Signal;
        public readonly string MethodName;
        public readonly bool OneShot;
        public readonly bool Deferred;
        private string? _targetName;
        public SignalHandler(Object origin, string signal, Object target, string methodName, bool oneShot = false, bool deferred = false) {
            Origin = origin;
            Signal = signal;
            Target = target;
            MethodName = methodName;
            OneShot = oneShot;
            Deferred = deferred;
            _targetName = Target is Node node ? node.Name : null;
            Connect();
        }

        public bool IsValid() => Object.IsInstanceValid(Target) && Object.IsInstanceValid(Origin);

        public void Connect() {
            if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
            Error err = Target.Connect(Signal, Target, MethodName, null, SignalTools.SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
        }

        public bool IsConnected() {
            return IsValid() && Target.IsConnected(Signal, Target, MethodName);
        }

        public void Disconnect() {
            if (IsConnected()) Target.Disconnect(Signal, Target, MethodName);
        }
    }

    public abstract class SignalHandlerBase : DisposableGodotObject, ISignalHandler, IObjectWatched {
        public readonly Object Target;
        public readonly string Signal;
        public readonly bool OneShot;
        public readonly bool Deferred;
        public string? TargetName;
        public Object? Bound { get; private set; } = null;
        private bool _valid = true;

        protected SignalHandlerBase(Object target, string signal, bool oneShot = false, bool deferred = false) {
            Target = target;
            Signal = signal;
            OneShot = oneShot;
            Deferred = deferred;
            Connect();
            Watch();
        }

        public void Unwatch() => DefaultObjectWatcher.Instance.Unwatch(this);

        public void Watch() => DefaultObjectWatcher.Instance.Watch(this);

        public SignalHandlerBase Bind(Object o) {
            Bound = o;
            return this;
        }

        public void Connect() {
            if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
            Error err = Target.Connect(Signal, this, nameof(SignalHandlerAction.Call), null, SignalTools.SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
            TargetName = Target is Node node ? node.Name : null;
        }

        protected void AfterCall() {
            if (OneShot) {
                CallDeferred("free");
                _valid = false;
            }
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

        public bool MustBeFreed() {
            return !_valid || !IsValid() || (Bound != null && !IsInstanceValid(Bound));
        }

        public Object Object => this;

        public override string ToString() {
            // string txt = null;
            // if (IsInstanceValid(this)) {
                // txt += "Target:" + Target.GetType();                
            // } else {
                // txt += "Disposed. Target:" + Target.GetType();                
            // }
            // if (_targetName != null) {
                // txt += " \"" + _targetName + "\"";
            // }
            // if (!IsInstanceValid(Target)) {
                // txt += " (Disposed)";
            // }
            // txt += ". Signal:" + Signal;
            // return txt;
            return GetType().ToString();
        }

    }
    
    public abstract class SignalHandlerBase<T> : SignalHandlerBase {
        protected readonly T Action;

        protected SignalHandlerBase(Object target, string signal, T action, bool oneShot = false, bool deferred = false): base(target, signal, oneShot, deferred) {
            Action = action;
        }
    }

    public class SignalHandlerAction : SignalHandlerBase<Action> {
        public SignalHandlerAction(Object target, string signal, Action action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call() {
            try {
                Action();
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1> : SignalHandlerBase<Action<T1>> {
        public SignalHandlerAction(Object target, string signal, Action<T1> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1) {
            try {
                Action(v1);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2> : SignalHandlerBase<Action<T1, T2>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2) {
            try {
                Action(v1, v2);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3> : SignalHandlerBase<Action<T1, T2, T3>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3) {
            try {
                Action(v1, v2, v3);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4> : SignalHandlerBase<Action<T1, T2, T3, T4>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4) {
            try {
                Action(v1, v2, v3, v4);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5> : SignalHandlerBase<Action<T1, T2, T3, T4, T5>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            try {
                Action(v1, v2, v3, v4, v5);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5, T6> : SignalHandlerBase<Action<T1, T2, T3, T4, T5, T6>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            try {
                Action(v1, v2, v3, v4, v5, v6);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class SignalHandlerAction<T1, T2, T3, T4, T5, T6, T7> : SignalHandlerBase<Action<T1, T2, T3, T4, T5, T6, T7>> {
        public SignalHandlerAction(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    public class
        SignalHandlerBase<T1, T2, T3, T4, T5, T6, T7, T8> : SignalHandlerBase<Action<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public SignalHandlerBase(Object target, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot = false, bool deferred = false) : base(target, signal, action, oneShot, deferred) {
        }

        public void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7, v8);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }
}