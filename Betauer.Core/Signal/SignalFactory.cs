using System;
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
                methodName = nameof(SignalObjectTargetAction<T>.GodotCall);

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
                methodName = nameof(SignalObjectTargetAction<T1, T2>.GodotCall);

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
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3>.GodotCall);

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
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3, T4>.GodotCall);

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
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3, T4, T5>.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3, T4, T5, T6>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3, T4, T5, T6>.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7>.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
        
        public static SignalHandler Create<T1, T2, T3, T4, T5, T6, T7, T8>(Object origin, string signal, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot, bool deferred) {
            Object target;
            string methodName;
            if (action.Target is Object obj) {
                target = obj;
                methodName = action.Method.Name;
            } else {
                target = new SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7, T8>(origin, action, oneShot);
                methodName = nameof(SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7, T8>.GodotCall);

            } 
            return new SignalHandler(origin, signal, target, methodName, oneShot, deferred);
        }
    }
}