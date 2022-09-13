using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Betauer.Memory;
using Object = Godot.Object;

namespace Betauer.Signal {
    public class DefaultSignalManager {
        public static SignalManager Instance = new SignalManager();
    }

    public class SignalManager : Object {
        
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SignalManager));

        public readonly Dictionary<int, ObjectSignals> SignalsByObject = new Dictionary<int, ObjectSignals>();

        public class ObjectSignals {
            internal readonly Object Origin;
            private readonly List<SignalHandler> _signals = new List<SignalHandler>();

            public ObjectSignals(SignalManager signalManager, Object origin) {
                Origin = origin;
                new WatchObjectSignalHandler(signalManager)
                    .Watch(origin)
                    .AddToDefaultObjectWatcher();
            }

            public int Count {
                get {
                    lock (_signals) {
                        return _signals.Count;
                    }
                }

            }

            public void Add(SignalHandler signalHandler) {
                lock (_signals) _signals.Add(signalHandler);
            }

            public bool Contains(SignalHandler signalHandler) {
                lock (_signals) {
                    return _signals.Contains(signalHandler);
                }
            }

            public int RemoveSignalAndGetSimilarAlive(SignalHandler signalHandler) {
                var alive = 0;
                lock (_signals) {
                    _signals.RemoveAll(sh => {
                        if (sh == signalHandler) return true; // the specific handler has been found, true = delete it
                        if (sh.Signal == signalHandler.Signal) alive++; // a signal of the same type 
                        return false; // ignore different type signals
                    });
                }
                return alive;
            }

            public int ExecuteAllSignalsAndGetAlive<T>(string signal, Action<T> execute) {
                var alive = 0;
                lock (_signals) {
                    _signals.RemoveAll(sh => {
                        if (sh is T signalHandler && sh.Signal == signal) {
                            execute(signalHandler);
                            if (sh.OneShot) return true; // delete all OneShot signals
                            alive++;
                        }
                        return false;
                    });
                }
                return alive;
            }

            public IEnumerable<IGrouping<string, SignalHandler>> GroupBySignalName() {
                lock (_signals) {
                    return _signals.GroupBy(sh => sh.Signal);

                }
            }
        }
        
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

        private bool TryGetObjectSignals(int objectId, out ObjectSignals objectSignals) {
            lock (SignalsByObject) return SignalsByObject.TryGetValue(objectId, out objectSignals);
        }

        internal SignalHandler Connect(SignalHandler signalHandler) {
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
                lock (SignalsByObject) SignalsByObject[originHash] = objectSignals;
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

        internal SignalHandler Disconnect(SignalHandler signalHandler) {
            if (!IsConnected(signalHandler) ||
                !TryGetObjectSignals(signalHandler.Origin.GetHashCode(), out var objectSignals)) return signalHandler;
            var alive = objectSignals.RemoveSignalAndGetSimilarAlive(signalHandler);
            if (alive == 0) {
                #if DEBUG
                Logger.Debug($"Specific disconnection {objectSignals.Origin.ToStringSafe()} signal: \"{signalHandler.Signal}\"");
                #endif
                objectSignals.Origin.Disconnect(signalHandler.Signal, this, GetMethod(signalHandler));
            }
            return signalHandler;
        }

        public int GetSignalCount(Object origin) {
            return TryGetObjectSignals(origin.GetHashCode(), out var objectSignals) ? objectSignals.Count : 0;
        }

        private void OnExecuteSignals<T>(int originHash, string signal, Action<T> execute,
            [CallerMemberName] string signalMethodName = "") where T : SignalHandler {
            
            if (!TryGetObjectSignals(originHash, out var objectSignals))
                throw new KeyNotFoundException($"Signal {signal} not found for {originHash}/{signal} when executing");
            
            var alive = objectSignals.ExecuteAllSignalsAndGetAlive(signal, execute);
            if (alive == 0) {
                #if DEBUG
                Logger.Debug(
                    $"Executing signal. Disconnecting {objectSignals.Origin.ToStringSafe()} signal: \"{signal}\". Because this execution was the last OneShot and 0 alive");
                #endif
                objectSignals.Origin.Disconnect(signal, this, signalMethodName);
            }
        }

        public void RemoveAndDisconnectAll(Object origin) {
            if (IsInstanceValid(origin) && 
                TryGetObjectSignals(origin.GetHashCode(), out var objectSignals)) {
                
                objectSignals.GroupBySignalName().ForEach(group => {
                    #if DEBUG
                    Logger.Debug($"RemoveAll. Disconnecting {objectSignals.Origin.ToStringSafe()} signal: \"{group.Key}\"");
                    #endif
                    objectSignals.Origin.Disconnect(group.Key, this, GetMethod(group.First()));
                });
            }
            lock (SignalsByObject) SignalsByObject.Remove(origin.GetHashCode());
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

        public bool CheckOriginConnection(SignalHandler signalHandler) {
            return IsInstanceValid(signalHandler.Origin) &&
                   signalHandler.Origin.IsConnected(signalHandler.Signal, this, GetMethod(signalHandler));
        }

        public bool IsConnected(SignalHandler signalHandler) {
            return CheckOriginConnection(signalHandler) &&
                   TryGetObjectSignals(signalHandler.Origin.GetHashCode(), out var objectSignals) &&
                   objectSignals.Contains(signalHandler);
        }

        private static uint SignalFlags(SignalHandler signalHandler) {
            return (signalHandler.Deferred ? (uint)ConnectFlags.Deferred : 0) +
                   (uint)ConnectFlags
                       .ReferenceCounted; // reference counted is needed to allow multiple connections to the same object + method name
        }

        private void _GodotSignal0P(int originHash, string signal) {
            OnExecuteSignals<SignalHandler0P>(originHash, signal, sh => sh.Action());
        }

        private void _GodotSignal1P(object p1, int originHash, string signal) {
            OnExecuteSignals<SignalHandler1P>(originHash, signal, sh => sh.Action(p1));
        }

        private void _GodotSignal2P(object p1, object p2, int originHash, string signal) {
            OnExecuteSignals<SignalHandler2P>(originHash, signal, sh => sh.Action(p1, p2));
        }

        private void _GodotSignal3P(object p1, object p2, object p3, int originHash, string signal) {
            OnExecuteSignals<SignalHandler3P>(originHash, signal, sh => sh.Action(p1, p2, p3));
        }

        private void _GodotSignal4P(object p1, object p2, object p3, object p4, int originHash, string signal) {
            OnExecuteSignals<SignalHandler4P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4));
        }

        private void _GodotSignal5P(object p1, object p2, object p3, object p4, object p5, int originHash, string signal) {
            OnExecuteSignals<SignalHandler5P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4, p5));
        }

        private void _GodotSignal6P(object p1, object p2, object p3, object p4, object p5, object p6, int originHash, string signal) {
            OnExecuteSignals<SignalHandler6P>(originHash, signal, sh => sh.Action(p1, p2, p3, p4, p5, p6));
        }

        private class WatchObjectSignalHandler : IObjectConsumer {
            private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Consumer));
            private readonly SignalManager _signalManager;
            private Object _watching;

            public WatchObjectSignalHandler(SignalManager signalManager) {
                _signalManager = signalManager;
            }

            public WatchObjectSignalHandler Watch(Object sceneTreeTweenWatching) {
                _watching = sceneTreeTweenWatching;
                return this;
            }

            public bool Consume(bool force) {
                if (force || !IsInstanceValid(_watching)) {
                    #if DEBUG
                    Logger.Debug($"Consumed: {_watching.ToStringSafe()} {_signalManager.GetSignalCount(_watching).ToString()} signal actions");
                    #endif
                    _signalManager.RemoveAndDisconnectAll(_watching);
                    return true;
                }
                return false;
            }

            public WatchObjectSignalHandler AddToDefaultObjectWatcher() {
                DefaultObjectWatcherRunner.Instance.Add(this);
                return this;
            }

            public override string ToString() {
                return $"Watching: {_watching.ToStringSafe()}";
            }
        }
    }
}