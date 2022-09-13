using System;
using Object = Godot.Object;

namespace Betauer.Signal {
    public abstract class SignalHandler {
        public readonly SignalManager SignalManager;
        public readonly Object Origin;
        public readonly string Signal;
        public readonly bool OneShot = false;
        public readonly bool Deferred = false;

        protected SignalHandler(SignalManager signalManager, Object origin, string signal, bool oneShot, bool deferred) {
            SignalManager = signalManager;
            Origin = origin;
            Signal = signal;
            OneShot = oneShot;
            Deferred = deferred;
        }

        public bool IsConnected() => SignalManager.IsConnected(this);
        public bool CheckOriginConnection() => SignalManager.CheckOriginConnection(this);
        public SignalHandler Disconnect() => SignalManager.Disconnect(this);
        public SignalHandler Connect() => SignalManager.Connect(this);
    }

    public abstract class SignalHandler<T> : SignalHandler {
        internal readonly T Action;

        protected SignalHandler(SignalManager signalManager, Object origin, string signal, T action, bool oneShot, bool deferred) : 
            base(signalManager, origin, signal, oneShot, deferred) {
            Action = action;
        }

    }

    public class SignalHandler0P : SignalHandler<Action> {
        public SignalHandler0P(SignalManager signalManager, Object origin, string signal, Action action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler1P : SignalHandler<Action<object>> {
        public SignalHandler1P(SignalManager signalManager, Object origin, string signal, Action<object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler2P : SignalHandler<Action<object, object>> {
        public SignalHandler2P(SignalManager signalManager, Object origin, string signal, Action<object, object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler3P : SignalHandler<Action<object, object, object>> {
        public SignalHandler3P(SignalManager signalManager, Object origin, string signal, Action<object, object, object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler4P : SignalHandler<Action<object, object, object, object>> {
        public SignalHandler4P(SignalManager signalManager, Object origin, string signal, Action<object, object, object, object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler5P : SignalHandler<Action<object, object, object, object, object>> {
        public SignalHandler5P(SignalManager signalManager, Object origin, string signal, Action<object, object, object, object, object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }

    public class SignalHandler6P : SignalHandler<Action<object, object, object, object, object, object>> {
        public SignalHandler6P(SignalManager signalManager, Object origin, string signal, Action<object, object, object, object, object, object> action, bool oneShot, bool deferred) :
            base(signalManager, origin, signal, action, oneShot, deferred) {
        }
    }
}