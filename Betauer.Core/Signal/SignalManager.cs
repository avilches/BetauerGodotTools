using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Betauer.Memory;
using Object = Godot.Object;

namespace Betauer.Signal {
    public class DefaultSignalManager {
        public static SignalManager Instance = new();
    }

    public class SignalManager : Object {
        
        public readonly Dictionary<int, ObjectSignals> SignalsByObject = new();
        public int Size => SignalsByObject.Select(o => o.Value.Count).Sum();
        public int ObjectsSize => SignalsByObject.Count;

        public class ObjectSignals {
            public readonly Object Emitter;
            public readonly List<SignalHandler> Signals = new();
            public readonly Watcher Watcher;

            public ObjectSignals(SignalManager signalManager, Object emitter) {
                Emitter = emitter;
                Watcher = Watcher.IfInvalidInstance(emitter)
                    .Do(() => signalManager.DisconnectAll(emitter), "Disconnect all signals");
            }

            public int Count => Signals.Count;

            public void Add(SignalHandler signalHandler) {
                Signals.Add(signalHandler);
            }

            public bool Contains(SignalHandler signalHandler) {
                return Signals.Contains(signalHandler);
            }

            public int RemoveSignalAndGetSimilarAlive(SignalHandler signalHandler) {
                var alive = 0;
                Signals.RemoveAll(sh => {
                    if (sh == signalHandler) return true; // the specific handler has been found, true = delete it
                    if (sh.Signal == signalHandler.Signal) alive++; // a signal of the same type 
                    return false; // ignore different type signals
                });
                return alive;
            }

            public int ExecuteAllSignalsAndGetAlive<T>(string signal, Action<T> execute) {
                var alive = 0;
                Signals.RemoveAll(sh => {
                    if (sh is T signalHandler && sh.Signal == signal) {
                        execute(signalHandler);
                        if (sh.OneShot) return true; // delete all OneShot signals
                        alive++;
                    }
                    return false;
                });
                return alive;
            }

            public IEnumerable<IGrouping<string, SignalHandler>> GroupBySignalName() {
                return Signals.GroupBy(sh => sh.Signal);
            }
        }

        public int GetSignalCount(Object origin) =>
            TryGetObjectSignals(origin.GetHashCode(), out var objectSignals) ? objectSignals.Count : 0;

        public SignalHandler Create(Object origin, string signal, Action action, bool oneShot = false, bool deferred = false) {
            return Connect(new SignalHandler0P(this, Validate(origin, signal), signal, action, oneShot, deferred));
        }

        public SignalHandler Create<T>(Object origin, string signal, Action<T> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1) => action((T)p1);
            return Connect(new SignalHandler1P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        public SignalHandler Create<T1, T2>(Object origin, string signal, Action<T1, T2> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1, object p2) => action((T1)p1, (T2)p2);
            return Connect(new SignalHandler2P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        public SignalHandler Create<T1, T2, T3>(Object origin, string signal, Action<T1, T2, T3> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1, object p2, object p3) => action((T1)p1, (T2)p2, (T3)p3);
            return Connect(new SignalHandler3P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        public SignalHandler Create<T1, T2, T3, T4>(Object origin, string signal, Action<T1, T2, T3, T4> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1, object p2, object p3, object p4) => action((T1)p1, (T2)p2, (T3)p3, (T4)p4);
            return Connect(new SignalHandler4P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        public SignalHandler Create<T1, T2, T3, T4, T5>(Object origin, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1, object p2, object p3, object p4, object p5) => action((T1)p1, (T2)p2, (T3)p3, (T4)p4, (T5)p5);
            return Connect(new SignalHandler5P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        public SignalHandler Create<T1, T2, T3, T4, T5, T6>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot = false, bool deferred = false) {
            void Wrapper(object p1, object p2, object p3, object p4, object p5, object p6) => action((T1)p1, (T2)p2, (T3)p3, (T4)p4, (T5)p5, (T6)p6);
            return Connect(new SignalHandler6P(this, Validate(origin, signal), signal, Wrapper, oneShot, deferred));
        }

        private static Object Validate(Object origin, string signal) {
            if (!IsInstanceValid(origin)) 
                throw new Exception($"Can't connect signal \"{signal}\" from a freed object instance");
            return origin;
        }

        private bool TryGetObjectSignals(int objectHash, out ObjectSignals objectSignals) {
            return SignalsByObject.TryGetValue(objectHash, out objectSignals);
        }

        public SignalHandler Connect(SignalHandler signalHandler) {
            if (!IsInstanceValid(signalHandler.Origin)) return signalHandler;
            AddSignalHandler(signalHandler);
            ConnectIfNotConnected(signalHandler);
            return signalHandler;
        }

        private void AddSignalHandler(SignalHandler signalHandler) {
            var origin = signalHandler.Origin;
            var originHash = origin.GetHashCode();
            if (!TryGetObjectSignals(originHash, out var objectSignals)) {
                objectSignals = new ObjectSignals(this, origin);
                SignalsByObject[originHash] = objectSignals;
            }
            if (!objectSignals.Contains(signalHandler)) objectSignals.Add(signalHandler);
        }

        private void ConnectIfNotConnected(SignalHandler signalHandler) {
            var method = GetMethod(signalHandler);
            if (!signalHandler.Origin.IsConnected(signalHandler.Signal, this, method)) {
                var binds = new Godot.Collections.Array { signalHandler.Origin.GetHashCode(), signalHandler.Signal };
                signalHandler.Origin.Connect(signalHandler.Signal, this, method, binds, SignalFlags(signalHandler));
            }
        }

        public SignalHandler Disconnect(SignalHandler signalHandler) {
            if (!IsConnected(signalHandler) ||
                !TryGetObjectSignals(signalHandler.Origin.GetHashCode(), out var objectSignals)) return signalHandler;
            var alive = objectSignals.RemoveSignalAndGetSimilarAlive(signalHandler);
            if (alive == 0) {
                objectSignals.Emitter.Disconnect(signalHandler.Signal, this, GetMethod(signalHandler));
            }
            return signalHandler;
        }

        public bool CheckOriginConnection(SignalHandler signalHandler) =>
            IsInstanceValid(signalHandler.Origin) &&
            signalHandler.Origin.IsConnected(signalHandler.Signal, this, GetMethod(signalHandler));

        public bool IsConnected(SignalHandler signalHandler) =>
            CheckOriginConnection(signalHandler) &&
            TryGetObjectSignals(signalHandler.Origin.GetHashCode(), out var objectSignals) &&
            objectSignals.Contains(signalHandler);

        public void DisconnectAll(Object origin) {
            if (IsInstanceValid(origin) && 
                TryGetObjectSignals(origin.GetHashCode(), out var objectSignals)) {
                
                objectSignals.GroupBySignalName().ForEach(group => {
                    objectSignals.Emitter.Disconnect(group.Key, this, GetMethod(group.First()));
                });
            }
            SignalsByObject.Remove(origin.GetHashCode());
        }

        private static string GetMethod(SignalHandler signalHandler) =>
            GetMethod(signalHandler.GetType());
        
        private static string GetMethod(Type type) {
            if (type == typeof(SignalHandler0P)) return nameof(_GodotSignal0P);
            if (type == typeof(SignalHandler1P)) return nameof(_GodotSignal1P);
            if (type == typeof(SignalHandler2P)) return nameof(_GodotSignal2P);
            if (type == typeof(SignalHandler3P)) return nameof(_GodotSignal3P);
            if (type == typeof(SignalHandler4P)) return nameof(_GodotSignal4P);
            if (type == typeof(SignalHandler5P)) return nameof(_GodotSignal5P);
            if (type == typeof(SignalHandler6P)) return nameof(_GodotSignal6P);
            throw new Exception("Unknown SignalHandler type ");
        }

        private static uint SignalFlags(SignalHandler signalHandler) =>
            (signalHandler.Deferred ? (uint)ConnectFlags.Deferred : 0) +
            // reference counted is needed to allow multiple connections to the same object + method name
            (uint)ConnectFlags.ReferenceCounted; 

        private void _GodotSignal0P(int originHash, string signal) =>
            _GodotSignal<SignalHandler0P>(originHash, signal, sh => sh.Action());

        private void _GodotSignal1P(object p1, int originHash, string signal) =>
            _GodotSignal<SignalHandler1P>(originHash, signal, sh => sh.Action(p1));

        private void _GodotSignal2P(object p1, object p2, int originHash, string signal) =>
            _GodotSignal<SignalHandler2P>(originHash, signal, sh => sh.Action(p1, p2));

        private void _GodotSignal3P(object p1, object p2, object p3, int originHash, string signal) =>
            _GodotSignal<SignalHandler3P>(originHash, signal, sh => sh.Action(p1, p2, p3));

        private void _GodotSignal4P(object p1, object p2, object p3, object p4, int originHash, string signal) =>
            _GodotSignal<SignalHandler4P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4));

        private void _GodotSignal5P(object p1, object p2, object p3, object p4, object p5, int originHash, string signal) =>
            _GodotSignal<SignalHandler5P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4, p5));

        private void _GodotSignal6P(object p1, object p2, object p3, object p4, object p5, object p6, int originHash, string signal) =>
            _GodotSignal<SignalHandler6P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4, p5, p6));
        
        private void _GodotSignal<T>(int originHash, string signal, Action<T> execute,
            [CallerMemberName] string signalMethodName = "") where T : SignalHandler {
            
            if (!TryGetObjectSignals(originHash, out var objectSignals))
                throw new KeyNotFoundException($"Signal {signal} not found for {originHash}/{signal} when executing");
            
            var alive = objectSignals.ExecuteAllSignalsAndGetAlive(signal, execute);
            if (alive == 0) {
                objectSignals.Emitter.Disconnect(signal, this, signalMethodName);
            }
        }

        public string GetStateAsString() {
            var txt = new StringBuilder();
            foreach (var objectSignals in SignalsByObject.Values) {
                txt.Append(objectSignals.Emitter.ToStringSafe());
                txt.Append(": ");
                txt.AppendLine(string.Join(", ", objectSignals.Signals.Select(s => s.Signal)));
            }
            return txt.ToString();
        }
    }
}