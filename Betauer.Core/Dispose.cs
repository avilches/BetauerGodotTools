using System;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    /*
     * 1: It shows a warning if the object is disposed in shutdown. It could mean you forget to dispose it
     * 2: It offers you a safe OnDispose method to implement without worrying to call base.Dispose(disposing) and
     * try/catch your code. The OnDispose() method is already wrapped and the base.Dispose(disposing) is always called.
     * 3: Optionally, it can show a message when the instance is created
     */
    public abstract class DisposableObject : IDisposable {
        protected bool Disposed { get; private set; } = false;

        public DisposableObject() {
            if (DisposeTools.ShowMessageOnCreate) {
                GD.Print("New DisposableObject instance: " + GetType().Name + ":" + this);
            }
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
            if (!disposing) DisposeTools.ShowDisposeOnShutdownWarning(this);
            else if (DisposeTools.ShowMessageOnDispose) GD.Print("Dispose(): " + GetType() + " " + this);
            try {
                OnDispose(disposing);
            } catch (Exception e) {
                DisposeTools.ShowDisposeException(e);
            } finally {
                Disposed = true;
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
     public class GodotObject : Object {
        protected bool Disposed { get; private set; } = false;
        public GodotObject() {
            if (DisposeTools.ShowMessageOnCreate) {
                GD.Print("New GodotObject instance: " + GetType().Name + ":" + this);
            }
        }

        // private bool _warningDisabled = false;
        // public void DisableNoDisposedOnShutdownWarning() {
        // _warningDisabled = true;
        // }

        protected sealed override void Dispose(bool disposing) {
            if (Disposed) return;
            Disposed = true;
            if (!disposing) DisposeTools.ShowDisposeOnShutdownWarning(this);
            else if (DisposeTools.ShowMessageOnDispose) GD.Print("Dispose(): " + GetType() + " " + this);
            try {
                OnDispose(disposing);
            } catch (Exception e) {
                DisposeTools.ShowDisposeException(e);
            } finally {
                base.Dispose(disposing);
                Disposed = true;
                // GD.Print("Dispose(" + disposing + "): " + GetType() + " " + ToString());
            }
        }

        protected virtual void OnDispose(bool disposing) {
        }

    }

    public static class DisposeTools {
        public static bool ShowShutdownWarning = true;
        public static bool ShowMessageOnCreate = true;
        public static bool ShowMessageOnDispose = true;

        public static void ShowDisposeException(Exception e) {
            if (ShowShutdownWarning) {
                GD.Print(e);
            }
        }

        public static void ShowDisposeOnShutdownWarning(object o) {
            if (ShowShutdownWarning) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                GD.Print($"Disposing on shutdown: {o.GetType()} " + o.ToString());
                Console.ResetColor();
            }
        }
    }
}