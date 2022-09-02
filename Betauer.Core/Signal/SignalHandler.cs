using System;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer.Signal {
    public static class SignalTools {
        public static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)Object.ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)Object.ConnectFlags.Deferred : 0) +
            0;
    }

    public abstract class SignalHandler : Object {
        public readonly Object Origin;
        public readonly string Signal;
        public readonly bool OneShot;
        public readonly bool Deferred;
        private bool _deferredWatchCall = true;
        private bool _watched = false;
        private WatchAndFree? _signalHandlerWatched;
        protected readonly Delegate Delegate; 

        protected SignalHandler(Object origin, string signal, Delegate @delegate, bool oneShot = false, bool deferred = false) {
            Origin = origin;
            Signal = signal;
            Delegate = @delegate;
            OneShot = oneShot;
            Deferred = deferred;
            Connect();
            Watch();
        }
        
        public SignalHandler Watch() {
            if (_signalHandlerWatched == null) {
                var watching = Delegate.Target is Object o ? new [] { o, Origin } : new[] { Origin }; 
                _signalHandlerWatched = new WatchAndFree(this, watching, true);
                DefaultObjectWatcher.Instance.Watch(_signalHandlerWatched);
            }
            return this;
        }

        public SignalHandler Unwatch() {
            if (_signalHandlerWatched != null) {
                DefaultObjectWatcher.Instance.Unwatch(_signalHandlerWatched);
                _signalHandlerWatched = null;
            }
            return this;
        }


        public void Connect() {
            if (!IsInstanceValid(Origin)) 
                throw new Exception($"Can't connect '{Signal}' from a freed {Origin.GetType().Name}");
            
            var flags = SignalTools.SignalFlags(OneShot, Deferred);
            Error err = Origin.Connect(Signal, this, nameof(SignalObjectTargetAction.GodotCall), null, flags);
            
            if (err != Error.Ok) {
                var name = $"{Origin.GetType().Name} '{(Origin is Node node ? node.Name : null)}'";
                throw new Exception($"Connecting signal '{Signal}' from {name} failed: {err}");
            }
        }

        public bool IsConnected() {
            return IsInstanceValid(Origin) && Origin.IsConnected(Signal, this, nameof(SignalObjectTargetAction.GodotCall));
        }

        public void Disconnect() {
            if (IsConnected()) Origin.Disconnect(Signal, this, nameof(SignalObjectTargetAction.GodotCall));
        }

        protected void AfterCall() {
            if (OneShot) CallDeferred("free");
        }

        public bool IsValid() {
            return IsInstanceValid(Origin);
        }
    }

    internal class SignalObjectTargetAction : SignalHandler {
        private readonly Action _action;

        public SignalObjectTargetAction(Object origin, string signal, Action action, bool oneShot = false,
            bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall() {
            try {
                _action();
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1> : SignalHandler {
        private readonly Action<T1> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1> action, bool oneShot = false,
            bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1) {
            try {
                _action(v1);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2> : SignalHandler {
        private readonly Action<T1, T2> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2> action, bool oneShot = false,
            bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2) {
            try {
                _action(v1, v2);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3> : SignalHandler {
        private readonly Action<T1, T2, T3> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2, T3> action, bool oneShot = false,
            bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3) {
            try {
                _action(v1, v2, v3);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4> : SignalHandler {
        private readonly Action<T1, T2, T3, T4> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2, T3, T4> action, 
            bool oneShot = false, bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4) {
            try {
                _action(v1, v2, v3, v4);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5> : SignalHandler {
        private readonly Action<T1, T2, T3, T4, T5> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2, T3, T4, T5> action,
            bool oneShot = false, bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            try {
                _action(v1, v2, v3, v4, v5);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6> : SignalHandler {
        private readonly Action<T1, T2, T3, T4, T5, T6> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6> action,
            bool oneShot = false, bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            try {
                _action(v1, v2, v3, v4, v5, v6);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7> : SignalHandler {
        private readonly Action<T1, T2, T3, T4, T5, T6, T7> _action;

        public SignalObjectTargetAction(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action,
            bool oneShot = false, bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            try {
                _action(v1, v2, v3, v4, v5, v6, v7);
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTarget<T1, T2, T3, T4, T5, T6, T7, T8> : SignalHandler {
        private readonly Action<T1, T2, T3, T4, T5, T6, T7, T8> _action;

        public SignalObjectTarget(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, 
            bool oneShot = false, bool deferred = false) : base(origin, signal, action, oneShot, deferred) {
            _action = action;
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
            try {
                _action(v1, v2, v3, v4, v5, v6, v7, v8);
            } finally {
                AfterCall();
            }
        }
    }
}