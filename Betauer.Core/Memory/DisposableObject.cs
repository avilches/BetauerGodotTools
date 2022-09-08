using System;
using Godot;

namespace Betauer.Memory {
    /// <summary>
    /// 1: It shows a warning if the object is disposed in shutdown. It could mean you forget to dispose it
    /// 2: It offers you a safe OnDispose method to implement without worrying to call base.Dispose(disposing) and
    /// try/catch your code. The OnDispose() method is already wrapped and the base.Dispose(disposing) is always called.
    /// 3: It has a public Disposed bool flag you can check. No need to use Object.IsInstanceValid() anymore.
    /// 4: Optionally, it can show a message when the instance is created and disposed 
    /// </summary>
    public abstract class DisposableObject : IDisposable {
        public bool Disposed { get; private set; } = false;

#if DEBUG
        protected DisposableObject() {
            DisposeTools.LogNewInstance(this);
        }
#endif
        
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
#if DEBUG
                GD.PushError(e.ToString());
#endif
            }
        }

        protected virtual void OnDispose(bool disposing) {
        }
        
        ~DisposableObject() {
            Dispose(false);
        }
    }
}