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

#if DEBUG
        protected DisposableObject() {
            DisposeTools.LogNewInstance(this);
        }
#endif
        
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
#if DEBUG
                DisposeTools.LogDispose(disposing, this);
#endif
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
     public abstract class DisposableGodotObject : Object {
        public bool Disposed { get; private set; } = false;

#if DEBUG
        protected DisposableGodotObject() {
            DisposeTools.LogNewInstance(this);
        }
#endif
        protected sealed override void Dispose(bool disposing) {
            if (Disposed) return;
            Disposed = true;
            try {
#if DEBUG
                DisposeTools.LogDispose(disposing, this);
#endif
                OnDispose(disposing);
            } catch (Exception e) {
#if DEBUG
                GD.PushError(e.ToString());
#endif
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

        internal static void LogNewInstance(object o) {
#if DEBUG
            if (ShowMessageOnNewInstance) 
                GD.Print($"New instance: {o.GetType().Name}: {o}");
#endif
        }

        internal static void LogDispose(bool disposing, object o) {
#if DEBUG
            if (disposing) {
                if (ShowMessageOnDispose) GD.Print($"{o.GetType().FullName}.Dispose(false): \"{o}\" ({o.GetHashCode():x8})");
            } else {
                if (ShowWarningOnShutdownDispose) GD.PushWarning($"Shutdown. {o.GetType().FullName}.Dispose(true): \"{o}\" ({o.GetHashCode():x8})");
            }
#endif
        }
    }
}