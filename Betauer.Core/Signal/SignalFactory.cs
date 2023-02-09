using System;
using Godot;

namespace Betauer.Core.Signal; 

public static class SignalFactory {
    public static SignalHandler Create(GodotObject origin, string signal, Action action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T>(GodotObject origin, string signal, Action<T> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2>(GodotObject origin, string signal, Action<T1, T2> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3>(GodotObject origin, string signal, Action<T1, T2, T3> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3, T4>(GodotObject origin, string signal, Action<T1, T2, T3, T4> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3, T4, T5>(GodotObject origin, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3, T4, T5, T6>(GodotObject origin, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7>(GodotObject origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
        
    public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7, T8>(GodotObject origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot, bool deferred) {
        return new SignalHandler(origin, signal, Callable.From(action), oneShot, deferred);
    }
}