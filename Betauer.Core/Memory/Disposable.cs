using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Memory {
    /*
     * 1: It shows a warning if the object is disposed in shutdown. It could mean you forget to dispose it
     * 2: It offers you a safe OnDispose method to implement without worrying to call base.Dispose(disposing) and
     * try/catch your code. The OnDispose() method is already wrapped and the base.Dispose(disposing) is always called.
     * 3: Optionally, it can show a message when the instance is created
     */
    public abstract class DisposableObject : IDisposable {
        public bool Disposed { get; private set; } = false;

        protected DisposableObject() {
            DisposeTools.LogNewInstance(this);
        }

        ~DisposableObject() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (Disposed) return;
            Disposed = true;
            try {
                DisposeTools.LogDispose(disposing, this);
                OnDispose(disposing);
            } catch (Exception e) {
                GD.PushError(e.ToString());
            }
        }

        protected virtual void OnDispose(bool disposing) {
        }
    }

    /*
     * 1: It shows a warning if the object is disposed in shutdown. It could mean you forget to dispose it
     * 2: It offers you a safe OnDispose method to implement without worrying to call base.Dispose(disposing) and
     * try/catch your code. The OnDispose() method is already wrapped and the base.Dispose(disposing) is always called.
     * 3: Optionally, it can show a message when the instance is created
     */
     public abstract class GodotObject : Object {
        protected bool Disposed { get; private set; } = false;

        protected GodotObject() {
            DisposeTools.LogNewInstance(this);
        }

        protected sealed override void Dispose(bool disposing) {
            if (Disposed) return;
            Disposed = true;
            try {
                DisposeTools.LogDispose(disposing, this);
                OnDispose(disposing);
            } catch (Exception e) {
                GD.PushError(e.ToString());
            } finally {
                base.Dispose(disposing);
            }
        }

        protected virtual void OnDispose(bool disposing) {
        }

     }

    public static class DisposeTools {
        public static bool ShowMessageOnNewInstance = false;
        public static bool ShowWarningOnShutdownDispose = false;
        public static bool ShowMessageOnDispose = false;

        public static void LogNewInstance(object o) {
            if (ShowMessageOnNewInstance) 
                GD.Print($"New instance: {o.GetType().Name}: {o}");
        }

        public static void LogDispose(bool disposing, object o) {
            if (disposing) {
                if (ShowMessageOnDispose) GD.Print($"Dispose(): {o.GetType()}: {o}");
            } else {
                if (ShowWarningOnShutdownDispose) GD.PushWarning($"Disposing on shutdown: {o.GetType()}: {o}");
            }
        }
    }
}