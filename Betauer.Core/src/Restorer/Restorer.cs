using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Restorer; 

public abstract class Restorer {
    public bool HasSavedState { get; private set; } = false;

    public void Save() {
        DoSave();
        HasSavedState = true;
    }

    /// <summary>
    /// Warning: always call to Restore() in a process_frame, not in a tween/signal callback
    /// <code>
    /// await this.AwaitProcessFrame();
    /// restorer.Restore();
    /// </code>
    /// </summary>
    /// <returns></returns>
    public void Restore() {
        if (!HasSavedState) {
#if DEBUG                
            GD.PushWarning("Restoring without saving before");
#endif                
        } else DoRestore();
    }

    protected abstract void DoSave();
    protected abstract void DoRestore();
}