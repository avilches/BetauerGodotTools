using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Core.Memory {
    /// <summary>
    /// 1: It shows a warning if the object is disposed in shutdown. It could mean you forget to dispose it
    /// 2: It offers you a safe OnDispose method to implement without worrying to call base.Dispose(disposing) and
    /// try/catch your code. The OnDispose() method is already wrapped and the base.Dispose(disposing) is always called.
    /// 3: It has a public Disposed bool flag you can check. No need to use Object.IsInstanceValid() anymore.
    /// 4: Optionally, it can show a message when the instance is created and disposed 
    /// </summary>
    public abstract partial class DisposableGodotObject : Object {
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
}