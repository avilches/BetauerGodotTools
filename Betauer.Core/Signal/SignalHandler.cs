using System;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer.Signal {

    public static class SignalFactory {
        public static SignalHandler Create(Object origin, string signal, Action action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);
            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T>(Object origin, string signal, Action<T> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2>(Object origin, string signal, Action<T1, T2> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3>(Object origin, string signal, Action<T1, T2, T3> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4>(Object origin, string signal, Action<T1, T2, T3, T4> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3, T4>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5>(Object origin, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3, T4, T5>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
    }
    
    public static class SignalTools {
        public static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)Object.ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)Object.ConnectFlags.Deferred : 0) +
            0;
    }

    public class SignalHandler {
        public readonly Object Origin;
        public readonly string Signal;
        public readonly Object Target;
        public readonly string TargetMethodName;
        public readonly bool OneShot;
        public readonly bool Deferred;
        private string? _targetName;
        public SignalHandler(Object origin, string signal, Object target, string targetMethodName, bool oneShot = false, bool deferred = false) {
            Origin = origin;
            Signal = signal;
            Target = target;
            TargetMethodName = targetMethodName;
            OneShot = oneShot;
            Deferred = deferred;
            _targetName = Target is Node node ? node.Name : null;
            Connect();
        }

        public bool IsValid() => Object.IsInstanceValid(Target) && Object.IsInstanceValid(Origin);

        public void Connect() {
            if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
            Error err = Origin.Connect(Signal, Target, TargetMethodName, null, SignalTools.SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
        }

        public bool IsConnected() {
            return IsValid() && Origin.IsConnected(Signal, Target, TargetMethodName);
        }

        public void Disconnect() {
            if (IsConnected()) Origin.Disconnect(Signal, Target, TargetMethodName);
        }

        public void Dispose() {
            if (Target is SignalObjectTarget a) a.Dispose();
        }

        public void Unwatch() {
            if (Target is SignalObjectTarget a) a.Unwatch();
        }
    }

    internal abstract class SignalObjectTarget : DisposableGodotObject, IObjectWatched {
        public readonly Object Origin;
        public readonly bool OneShot;

        protected SignalObjectTarget(Object origin, bool oneShot) {
            Origin = origin;
            OneShot = oneShot;
            DefaultObjectWatcher.Instance.Watch(this);
        }

        protected void AfterCall() {
            if (OneShot) CallDeferred("free");
        }

        public bool MustBeFreed() => !IsInstanceValid(Origin);

        public Object Object => this;

        public void Unwatch() {
            DefaultObjectWatcher.Instance.Unwatch(this);
        }
    }

    internal abstract class SignalObjectTarget<T> : SignalObjectTarget {
        protected readonly T Action;

        protected SignalObjectTarget(Object origin, T action, bool oneShot): base(origin, oneShot) {
            Action = action;
        }
    }

    internal class SignalObjectTargetAction : SignalObjectTarget<Action> {
        public SignalObjectTargetAction(Object origin, Action action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall() {
            try {
                Action();
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1> : SignalObjectTarget<Action<T1>> {
        public SignalObjectTargetAction(Object origin, Action<T1> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1) {
            try {
                Action(v1);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2> : SignalObjectTarget<Action<T1, T2>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2) {
            try {
                Action(v1, v2);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3> : SignalObjectTarget<Action<T1, T2, T3>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3) {
            try {
                Action(v1, v2, v3);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4> : SignalObjectTarget<Action<T1, T2, T3, T4>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4) {
            try {
                Action(v1, v2, v3, v4);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5> : SignalObjectTarget<Action<T1, T2, T3, T4, T5>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            try {
                Action(v1, v2, v3, v4, v5);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            try {
                Action(v1, v2, v3, v4, v5, v6);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6, T7>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTarget<T1, T2, T3, T4, T5, T6, T7, T8> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public SignalObjectTarget(Object origin, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
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