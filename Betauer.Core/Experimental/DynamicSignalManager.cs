using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Experimental {

    public static class DynamicSignalExtensions {
        public static DynamicSignalManager Manager = new DynamicSignalManager(); 
        public static DynamicSignal OnPressed(this Button button, Action action, bool oneShot = false) =>
            Manager.ConnectSignalToAction(button, "pressed", action, oneShot);
        public static DynamicSignal OnToggled(this Button button, Action<bool> action, bool oneShot = false) =>
            Manager.ConnectSignalToAction(button, "toggled", action, oneShot);
    }
    
    /*
     * Signals can't be disconnected because all signals (of the same number of parameters) share the same method name
     * DynamicSignal objets can be removed
     */
    public class DynamicSignalManager : Godot.Object {
        public readonly Dictionary<string, DynamicSignal0P> Signals0Ps =
            new Dictionary<string, DynamicSignal0P>();

        public readonly Dictionary<string, DynamicSignal1P> Signals1Ps =
            new Dictionary<string, DynamicSignal1P>();
        
        // TODO: add Signals2Ps and so on to allow signals with more parameters ...

        public readonly List<DynamicSignal> SignalsList = new List<DynamicSignal>();

        private void Add(DynamicSignal dynamicSignal) {
            lock (SignalsList) SignalsList.Add(dynamicSignal);
            switch (dynamicSignal) {
                case DynamicSignal0P dynamicSignal0P:
                    lock (Signals0Ps) Signals0Ps.Add(dynamicSignal0P.GetKey(), dynamicSignal0P);
                    break;
                case DynamicSignal1P dynamicSignal1P:
                    lock (Signals1Ps) Signals1Ps.Add(dynamicSignal1P.GetKey(), dynamicSignal1P);
                    break;
            }
        }

        public void Clean() {
            LinkedList<DynamicSignal>? toDelete = null;
            lock (SignalsList) {
                // loop backward allows to delete a List safely
                for (var i = SignalsList.Count - 1; i >= 0; i--) {
                    var signal = SignalsList[i];
                    if (!signal.IsValid()) {
                        SignalsList.RemoveAt(i);
                        toDelete ??= new LinkedList<DynamicSignal>();
                        toDelete.AddLast(signal);
                    }
                }                
            }
            if (toDelete != null) foreach (var dynamicSignal in toDelete) RemoveFromMaps(dynamicSignal);
        }

        public void Remove(DynamicSignal dynamicSignal) {
            lock (SignalsList) SignalsList.Remove(dynamicSignal);
            RemoveFromMaps(dynamicSignal);
        }

        private void RemoveFromMaps(DynamicSignal dynamicSignal) {
            switch (dynamicSignal) {
                case DynamicSignal0P dynamicSignal0P:
                    lock (Signals0Ps) Signals0Ps.Remove(dynamicSignal0P.GetKey());
                    break;
                case DynamicSignal1P dynamicSignal1P:
                    lock (Signals1Ps) Signals1Ps.Remove(dynamicSignal1P.GetKey());
                    break;
            }
        }

        public DynamicSignal ConnectSignalToAction(Node node, string signal, Action action, bool oneShot = false) {
            if (!IsInstanceValid(node))
                throw new Exception("Can't connect node signal " + signal + " from a invalid node: " + node);
            var dynamicSignal = new DynamicSignal0P(node, signal, action, oneShot);
            Add(dynamicSignal);
            var binds = new Godot.Collections.Array { node.GetInstanceId(), signal, action.GetHashCode() };
            node.Connect(signal, this, nameof(_GodotSignal0P), binds, SignalFlags(oneShot));
            return dynamicSignal;
        }

        public DynamicSignal ConnectSignalToAction<T>(Node node, string signal, Action<T> action, bool oneShot = false) {
            if (!IsInstanceValid(node))
                throw new Exception("Can't connect node signal " + signal + " from a invalid node: " + node);
            Action<object> wrapper = (p1) => action((T)p1);
            var dynamicSignal = new DynamicSignal1P(node, signal, wrapper, oneShot);
            Add(dynamicSignal);
            var binds = new Godot.Collections.Array { node.GetInstanceId(), signal, wrapper.GetHashCode() };
            node.Connect(signal, this, nameof(_GodotSignal1P), binds, SignalFlags(oneShot));
            return dynamicSignal;
        }

        private static uint SignalFlags(bool oneShot, bool deferred = false) {
            return (oneShot ? (uint)ConnectFlags.Oneshot : 0) + 
                   (deferred ? (uint)ConnectFlags.Deferred : 0) + // TODO: test deferred
                   (uint)ConnectFlags.ReferenceCounted; // reference counted is needed to allow multiple connections to the same object + method name
        }

        public void _GodotSignal0P(ulong instanceId, string signal, int actionHashCode) {
            var key = DynamicSignal.CreateKey(instanceId, signal, actionHashCode);
            var dynamicSignal = Signals0Ps[key];
            dynamicSignal.Action();
            dynamicSignal.Consume(); // If oneShot, mark as invalid
        }

        // public void _GodotSignal1P(ulong instanceId, string signal, int actionHashCode, object p1) {
        public void _GodotSignal1P(object p1, ulong instanceId, string signal, int actionHashCode) {
            var key = DynamicSignal.CreateKey(instanceId, signal, actionHashCode);
            var dynamicSignal = Signals1Ps[key];
            dynamicSignal.Action(p1);
            dynamicSignal.Consume(); // If oneShot, mark as invalid

        }
    }
}