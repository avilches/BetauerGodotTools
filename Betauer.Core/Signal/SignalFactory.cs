using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Core.Signal {
    public static class SignalFactory {
        public static SignalHandler Create(Object origin, string signal, Action action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T>(Object origin, string signal, Action<T> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2>(Object origin, string signal, Action<T1, T2> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3>(Object origin, string signal, Action<T1, T2, T3> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4>(Object origin, string signal, Action<T1, T2, T3, T4> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5>(Object origin, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7, T8>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot, bool deferred) {
            var callable = action.Target is Object obj ? new Callable(obj, action.Method.Name) : Callable.From(action);
            return new SignalHandler(origin, signal, callable, oneShot, deferred);
        }
    }
}