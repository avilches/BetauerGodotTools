using System;
using System.Collections.Concurrent;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    public abstract class DisposableObject : IDisposable {
        public static bool ShowShutdownWarning = false;

        protected bool Disposed { get; private set; } = false;

        ~DisposableObject() {
            // DoDispose(false);
        }

        public void Dispose() {
            // DoDispose(true);
            // GC.SuppressFinalize(this);
        }

        private void DoDispose(bool disposing) {
            if (Disposed) return;
            try {
                if (ShowShutdownWarning && !disposing) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    GD.Print($"Disposing on shutdown: {GetType()} " + ToString());
                    Console.ResetColor();
                }
            } catch (Exception e) {
                Console.Write(e);
            } finally {
                Dispose(disposing);
                Disposed = true;
            }
        }

        protected abstract void Dispose(bool disposing);
    }

    public class DisposableGodotObject : Object {
        public static bool ShowShutdownWarning = true;

        // public DisposableGodotObject() {
        // GD.Print("Creating: "+GetType().Name+":"+base.ToString());
        // }

        protected bool Disposed { get; private set; } = false;

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            return;
            if (Disposed) return;
            try {
                if (ShowShutdownWarning && !disposing) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    GD.Print($"Disposing on shutdown: {GetType()} " + ToString());
                    Console.ResetColor();
                }
            } catch (Exception e) {
                Console.Write(e);
            } finally {
                base.Dispose(disposing);
                Disposed = true;
            }
        }
    }

    public class Disposer : DisposableObject {
        private readonly ConcurrentDictionary<int, DisposableGodotObject> _activeObjects =
            new ConcurrentDictionary<int, DisposableGodotObject>();

        public void Add(DisposableGodotObject o) {
            // GD.Print("DisposableTween.AddToDisposeQueue: " + o.GetType().Name + ":" + o);
            _activeObjects[o.GetHashCode()] = o;
        }

        public void Remove(DisposableGodotObject o) {
            // GD.Print("DisposableTween.RemovedFromDisposeQueue: " + o.GetType().Name + ":" + o);
            _activeObjects.TryRemove(o.GetHashCode(), out var x);
        }

        protected override void Dispose(bool disposing) {
            foreach (var pendingValue in _activeObjects.Values) {
                if (!Object.IsInstanceValid(pendingValue)) continue;
                // GD.Print("DisposableTween.Dispose " + disposing + " " + pendingValue.GetType() + " " + pendingValue);
                pendingValue.Dispose();
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