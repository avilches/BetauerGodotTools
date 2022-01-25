using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using static Godot.Mathf;
using Object = Godot.Object;

namespace Betauer {

    public class DisposableGodotObject : Object {
        public static bool ShowShutdownWarning = true;

        // public DisposableGodotObject() {
            // GD.Print("Creating: "+GetType().Name+":"+base.ToString());
        // }

        private bool _warningDisabled = false;
        public void DisableNoDisposedOnShutdownWarning() {
            _warningDisabled = true;
        }

        protected bool Disposed { get; private set; }= false;
        protected override void Dispose(bool disposing) {
            if (Disposed) return;
            try {
                if (!disposing && ShowShutdownWarning && !_warningDisabled) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    GD.Print($"Disposing on shutdown: {GetType()} "+ToString());
                    Console.ResetColor();
                }
            } finally {
                base.Dispose(disposing);
                Disposed = true;
            }
        }
    }

    public class Disposer : DisposableGodotObject {
        private readonly ConcurrentDictionary<int, DisposableGodotObject> _activeObjects = new ConcurrentDictionary<int, DisposableGodotObject>();

        public void Add(DisposableGodotObject o) {
            // GD.Print("DisposableTween.AddToDisposeQueue: " + o.GetType().Name + ":" + o);
            _activeObjects[o.GetHashCode()] = o;
        }

        public void Remove(DisposableGodotObject o) {
            // GD.Print("DisposableTween.RemovedFromDisposeQueue: " + o.GetType().Name + ":" + o);
            _activeObjects.TryRemove(o.GetHashCode(), out var x);
        }

        protected override void Dispose(bool disposing) {
            if (Disposed) return;
            try {
                foreach (var pendingValue in _activeObjects.Values) {
                    if (!IsInstanceValid(pendingValue)) continue;
                    // GD.Print("DisposableTween.Dispose " + disposing + " " + pendingValue.GetType() + " " + pendingValue);
                    pendingValue.Dispose();
                }
            } finally {
                base.Dispose(disposing);
            }
        }
    }

    public class DisposableTween : Tween {
        private readonly Disposer _disposer = new Disposer();
        internal void AddToDisposeQueue(DisposableGodotObject o) => _disposer.Add(o);
        internal void RemovedFromDisposeQueue(DisposableGodotObject o) => _disposer.Remove(o);
        protected override void Dispose(bool disposing) {
            _disposer.Dispose();
        }
    }

}