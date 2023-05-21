using System.Collections.Generic;

namespace Betauer.Application.Lifecycle.Pool;

public interface IManagedPool {
    public int BusyCount();
    public int AvailableCount();
    public int InvalidCount();
    public void Purge();
    public IEnumerable<object> GetAvailable();
    public IEnumerable<object> GetBusy();
    void Clear();
    void PreInject(string name);
}